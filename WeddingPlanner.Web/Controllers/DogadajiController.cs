using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WeddingPlanner.Core.Models;
using WeddingPlanner.Infrastructure.Data;
using WeddingPlanner.Infrastructure.Services;

namespace WeddingPlanner.Web.Controllers;

public class DogadajiController : Controller
{
    private readonly AppDbContext _db;
    private readonly BendService _bendService;

    public DogadajiController(AppDbContext db, BendService bendService)
    {
        _db = db;
        _bendService = bendService;
    }

    public async Task<IActionResult> Index(string sort = "datum", int page = 1, int pageSize = 10)
    {
        var query = _db.Dogadaji.Include(d => d.TipVjencanja).AsQueryable();

        query = sort switch
        {
            "par" => query.OrderBy(d => d.NazivPara),
            "par_desc" => query.OrderByDescending(d => d.NazivPara),
            "status" => query.OrderBy(d => d.Status),
            "datum_desc" => query.OrderByDescending(d => d.DatumDogadaja),
            _ => query.OrderBy(d => d.DatumDogadaja)
        };

        ViewBag.CurrentSort = sort;
        ViewBag.CurrentPage = page;
        var ukupno = await query.CountAsync();
        ViewBag.TotalPages = (int)Math.Ceiling(ukupno / (double)pageSize);

        var dogadaji = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return View(dogadaji);
    }

    public async Task<IActionResult> Detalji(int id)
    {
        var dogadaj = await _db.Dogadaji
            .Include(d => d.TipVjencanja)
            .Include(d => d.Rezervacije)
                .ThenInclude(r => r.Bend)
                .ThenInclude(b => b.Partner)
            .Include(d => d.RacunStavke)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (dogadaj == null) return NotFound();

        ViewBag.Ukupno = dogadaj.RacunStavke
            .Sum(s => s.IznosOsnovni + s.IznosOsnovni * s.Provizija / 100);

        return View(dogadaj);
    }

    public async Task<IActionResult> Dodaj()
    {
        await PopuniViewBag();
        return View(new Dogadaj { DatumDogadaja = DateTime.Today.AddMonths(1) });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Dodaj(
        Dogadaj dogadaj,
        int? odabraniiBendId,
        int? odabranSlasticarnicaId,
        int? odabranRestaurantId)
    {
        ModelState.Remove("TipVjencanja");
        ModelState.Remove("Rezervacije");
        ModelState.Remove("RacunStavke");

        // Provjera kolizije benda PRIJE nego što išta dodamo
        if (odabraniiBendId.HasValue)
        {
            bool zauzet = await _bendService.JeAngaziran(
                odabraniiBendId.Value, dogadaj.DatumDogadaja);

            if (zauzet)
            {
                ModelState.AddModelError("",
                    "Odabrani bend je već rezerviran za taj datum. Molimo odaberite drugi bend ili drugi datum.");
                await PopuniViewBag();
                return View(dogadaj);
            }
        }

        if (!ModelState.IsValid)
        {
            await PopuniViewBag();
            return View(dogadaj);
        }

        // Spremi događaj
        _db.Dogadaji.Add(dogadaj);
        await _db.SaveChangesAsync();

        // Rezervacija benda
        if (odabraniiBendId.HasValue)
        {
            _db.Rezervacije.Add(new Rezervacija
            {
                DogadajId = dogadaj.Id,
                BendId = odabraniiBendId.Value,
                Datum = dogadaj.DatumDogadaja,
                Potvrdena = true
            });

            var bend = await _db.Bendovi.FindAsync(odabraniiBendId.Value);
            if (bend != null) bend.Status = StatusBenda.Zauzet;
        }

        // Stavka za slastičarnicu
        if (odabranSlasticarnicaId.HasValue)
        {
            var sl = await _db.Slasticarnice
                .Include(s => s.Partner)
                .FirstOrDefaultAsync(s => s.Id == odabranSlasticarnicaId.Value);

            if (sl != null)
            {
                _db.RacunStavke.Add(new RacunStavka
                {
                    DogadajId = dogadaj.Id,
                    Opis = $"Slastičarnica: {sl.Partner.Naziv}",
                    Tip = TipStavke.Slastice,
                    IznosOsnovni = 0,
                    Provizija = sl.Partner.Provizija
                });
            }
        }

        // Stavka za salon/restoran
        if (odabranRestaurantId.HasValue)
        {
            var rest = await _db.RestaurantSaloni
                .Include(r => r.Partner)
                .FirstOrDefaultAsync(r => r.Id == odabranRestaurantId.Value);

            if (rest != null)
            {
                _db.RacunStavke.Add(new RacunStavka
                {
                    DogadajId = dogadaj.Id,
                    Opis = $"Salon/Restoran: {rest.Partner.Naziv}",
                    Tip = TipStavke.Sala,
                    IznosOsnovni = rest.CijenaSale ?? rest.CijenaPoOsobi,
                    Provizija = rest.Partner.Provizija
                });
            }
        }

        await _db.SaveChangesAsync();

        TempData["Uspjeh"] = $"Događaj za '{dogadaj.NazivPara}' je uspješno kreiran!";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Uredi(int id)
    {
        var dogadaj = await _db.Dogadaji.FindAsync(id);
        if (dogadaj == null) return NotFound();

        ViewBag.TipoviVjencanja = new SelectList(
            await _db.TipoviVjencanja.ToListAsync(), "Id", "Naziv",
            dogadaj.TipVjencanjaId);

        return View(dogadaj);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Uredi(int id, Dogadaj dogadaj)
    {
        if (id != dogadaj.Id) return BadRequest();

        ModelState.Remove("TipVjencanja");
        ModelState.Remove("Rezervacije");
        ModelState.Remove("RacunStavke");

        if (!ModelState.IsValid)
        {
            ViewBag.TipoviVjencanja = new SelectList(
                await _db.TipoviVjencanja.ToListAsync(), "Id", "Naziv");
            return View(dogadaj);
        }

        _db.Dogadaji.Update(dogadaj);
        await _db.SaveChangesAsync();
        TempData["Uspjeh"] = "Događaj je ažuriran.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Obrisi(int id)
    {
        var dogadaj = await _db.Dogadaji
            .Include(d => d.TipVjencanja)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (dogadaj == null) return NotFound();
        return View(dogadaj);
    }

    [HttpPost, ActionName("Obrisi")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ObrisiPotvrdeno(int id)
    {
        var dogadaj = await _db.Dogadaji.FindAsync(id);
        if (dogadaj != null)
        {
            _db.Dogadaji.Remove(dogadaj);
            await _db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    private async Task PopuniViewBag()
    {
        ViewBag.TipoviVjencanja = new SelectList(
            await _db.TipoviVjencanja.ToListAsync(), "Id", "Naziv");

        ViewBag.SlobodniBendovi = new SelectList(
            await _db.Bendovi
                .Include(b => b.Partner)
                .Where(b => b.Status == StatusBenda.Slobodan)
                .Select(b => new { b.Id, Naziv = b.Partner.Naziv })
                .ToListAsync(), "Id", "Naziv");

        ViewBag.Slasticarnice = new SelectList(
            await _db.Slasticarnice
                .Include(s => s.Partner)
                .Select(s => new { s.Id, Naziv = s.Partner.Naziv })
                .ToListAsync(), "Id", "Naziv");

        ViewBag.RestaurantSaloni = new SelectList(
            await _db.RestaurantSaloni
                .Include(r => r.Partner)
                .Select(r => new { r.Id, Naziv = r.Partner.Naziv })
                .ToListAsync(), "Id", "Naziv");
    }
}
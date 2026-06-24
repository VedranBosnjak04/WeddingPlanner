using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WeddingPlanner.Core.Models;
using WeddingPlanner.Infrastructure.Data;

namespace WeddingPlanner.Web.Controllers;

public class DogadajiController : Controller
{
    private readonly AppDbContext _db;
    public DogadajiController(AppDbContext db) => _db = db;

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

        var dogadaji = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return View(dogadaji);
    }

    public async Task<IActionResult> Detalji(int id)
    {
        var dogadaj = await _db.Dogadaji
            .Include(d => d.TipVjencanja)
            .Include(d => d.Rezervacije).ThenInclude(r => r.Bend).ThenInclude(b => b.Partner)
            .Include(d => d.RacunStavke)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (dogadaj == null) return NotFound();

        ViewBag.Ukupno = dogadaj.RacunStavke.Sum(s => s.IznosOsnovni + s.IznosOsnovni * s.Provizija / 100);

        return View(dogadaj);
    }

    public async Task<IActionResult> Dodaj()
    {
        ViewBag.TipoviVjencanja = new SelectList(await _db.TipoviVjencanja.ToListAsync(), "Id", "Naziv");
        return View(new Dogadaj { DatumDogadaja = DateTime.Today.AddMonths(1) });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Dodaj(Dogadaj dogadaj)
    {
        ModelState.Remove("TipVjencanja");
        ModelState.Remove("Rezervacije");
        ModelState.Remove("RacunStavke");

        if (!ModelState.IsValid)
        {
            ViewBag.TipoviVjencanja = new SelectList(await _db.TipoviVjencanja.ToListAsync(), "Id", "Naziv");
            return View(dogadaj);
        }

        _db.Dogadaji.Add(dogadaj);
        await _db.SaveChangesAsync();
        TempData["Uspjeh"] = $"Događaj za '{dogadaj.NazivPara}' je kreiran.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Uredi(int id)
    {
        var dogadaj = await _db.Dogadaji.FindAsync(id);
        if (dogadaj == null) return NotFound();
        ViewBag.TipoviVjencanja = new SelectList(await _db.TipoviVjencanja.ToListAsync(), "Id", "Naziv", dogadaj.TipVjencanjaId);
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
            ViewBag.TipoviVjencanja = new SelectList(await _db.TipoviVjencanja.ToListAsync(), "Id", "Naziv");
            return View(dogadaj);
        }

        _db.Dogadaji.Update(dogadaj);
        await _db.SaveChangesAsync();
        TempData["Uspjeh"] = "Događaj je ažuriran.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Obrisi(int id)
    {
        var dogadaj = await _db.Dogadaji.Include(d => d.TipVjencanja).FirstOrDefaultAsync(d => d.Id == id);
        if (dogadaj == null) return NotFound();
        return View(dogadaj);
    }

    [HttpPost, ActionName("Obrisi")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ObrisiPotvrdeno(int id)
    {
        var dogadaj = await _db.Dogadaji.FindAsync(id);
        if (dogadaj != null) { _db.Dogadaji.Remove(dogadaj); await _db.SaveChangesAsync(); }
        return RedirectToAction(nameof(Index));
    }
}
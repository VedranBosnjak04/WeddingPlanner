using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WeddingPlanner.Core.Models;
using WeddingPlanner.Infrastructure.Data;

namespace WeddingPlanner.Web.Controllers;

public class BendoviController : Controller
{
    private readonly AppDbContext _db;
    public BendoviController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Index(string sort = "naziv", int page = 1, int pageSize = 10)
    {
        var query = _db.Bendovi.Include(b => b.Partner).AsQueryable();

        query = sort switch
        {
            "status" => query.OrderBy(b => b.Status),
            "status_desc" => query.OrderByDescending(b => b.Status),
            "naziv_desc" => query.OrderByDescending(b => b.Partner.Naziv),
            _ => query.OrderBy(b => b.Partner.Naziv)
        };

        ViewBag.CurrentSort = sort;
        ViewBag.CurrentPage = page;
        var ukupno = await query.CountAsync();
        ViewBag.TotalPages = (int)Math.Ceiling(ukupno / (double)pageSize);

        var bendovi = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return View(bendovi);
    }

    public async Task<IActionResult> Detalji(int id)
    {
        var bend = await _db.Bendovi
            .Include(b => b.Partner)
            .Include(b => b.Cijene)
            .Include(b => b.Playliste)
            .Include(b => b.Rezervacije).ThenInclude(r => r.Dogadaj)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (bend == null) return NotFound();
        return View(bend);
    }

    public async Task<IActionResult> Dodaj()
    {
        var dostupniPartneri = await _db.Partneri
            .Where(p => (p.Tip == TipPartnera.Bend || p.Tip == TipPartnera.DJ) && p.Bend == null)
            .ToListAsync();

        ViewBag.Partneri = new SelectList(dostupniPartneri, "Id", "Naziv");
        return View(new Bend());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Dodaj(Bend bend)
    {
        ModelState.Remove("Partner");
        ModelState.Remove("Playliste");
        ModelState.Remove("Cijene");
        ModelState.Remove("Rezervacije");

        if (!ModelState.IsValid)
        {
            ViewBag.Partneri = new SelectList(
                await _db.Partneri.Where(p => p.Bend == null).ToListAsync(), "Id", "Naziv");
            return View(bend);
        }

        _db.Bendovi.Add(bend);
        await _db.SaveChangesAsync();
        TempData["Uspjeh"] = "Bend/DJ je dodan.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Uredi(int id)
    {
        var bend = await _db.Bendovi.Include(b => b.Partner).FirstOrDefaultAsync(b => b.Id == id);
        if (bend == null) return NotFound();
        return View(bend);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Uredi(int id, Bend bend)
    {
        if (id != bend.Id) return BadRequest();
        ModelState.Remove("Partner");
        ModelState.Remove("Playliste");
        ModelState.Remove("Cijene");
        ModelState.Remove("Rezervacije");

        if (!ModelState.IsValid) return View(bend);

        _db.Bendovi.Update(bend);
        await _db.SaveChangesAsync();
        TempData["Uspjeh"] = "Bend je ažuriran.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> PromijeniStatus(int id, StatusBenda status)
    {
        var bend = await _db.Bendovi.FindAsync(id);
        if (bend == null) return NotFound();
        bend.Status = status;
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Detalji), new { id });
    }

    public async Task<IActionResult> Obrisi(int id)
    {
        var bend = await _db.Bendovi.Include(b => b.Partner).FirstOrDefaultAsync(b => b.Id == id);
        if (bend == null) return NotFound();
        return View(bend);
    }

    [HttpPost, ActionName("Obrisi")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ObrisiPotvrdeno(int id)
    {
        var bend = await _db.Bendovi.FindAsync(id);
        if (bend != null) { _db.Bendovi.Remove(bend); await _db.SaveChangesAsync(); }
        TempData["Uspjeh"] = "Bend je obrisan.";
        return RedirectToAction(nameof(Index));
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeddingPlanner.Core.Models;
using WeddingPlanner.Infrastructure.Data;

namespace WeddingPlanner.Web.Controllers;

public class RacunController : Controller
{
    private readonly AppDbContext _db;
    public RacunController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Index(int dogadajId)
    {
        var dogadaj = await _db.Dogadaji
            .Include(d => d.RacunStavke)
            .Include(d => d.TipVjencanja)
            .FirstOrDefaultAsync(d => d.Id == dogadajId);

        if (dogadaj == null) return NotFound();

        ViewBag.Ukupno = dogadaj.RacunStavke.Sum(s => s.IznosOsnovni + s.IznosOsnovni * s.Provizija / 100);

        return View(dogadaj);
    }

    public IActionResult DodajStavku(int dogadajId)
    {
        ViewBag.DogadajId = dogadajId;
        return View(new RacunStavka { DogadajId = dogadajId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DodajStavku(RacunStavka stavka)
    {
        ModelState.Remove("Dogadaj");
        if (!ModelState.IsValid)
        {
            ViewBag.DogadajId = stavka.DogadajId;
            return View(stavka);
        }

        _db.RacunStavke.Add(stavka);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index), new { dogadajId = stavka.DogadajId });
    }

    public async Task<IActionResult> UrediStavku(int id)
    {
        var stavka = await _db.RacunStavke.FindAsync(id);
        if (stavka == null) return NotFound();
        return View(stavka);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UrediStavku(RacunStavka stavka)
    {
        ModelState.Remove("Dogadaj");
        if (!ModelState.IsValid) return View(stavka);

        _db.RacunStavke.Update(stavka);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index), new { dogadajId = stavka.DogadajId });
    }

    [HttpPost]
    public async Task<IActionResult> ObrisiStavku(int id)
    {
        var stavka = await _db.RacunStavke.FindAsync(id);
        int dogadajId = stavka?.DogadajId ?? 0;
        if (stavka != null) { _db.RacunStavke.Remove(stavka); await _db.SaveChangesAsync(); }
        return RedirectToAction(nameof(Index), new { dogadajId });
    }
}
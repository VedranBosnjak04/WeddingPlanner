using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WeddingPlanner.Core.Models;
using WeddingPlanner.Infrastructure.Data;
using WeddingPlanner.Infrastructure.Services;

namespace WeddingPlanner.Web.Controllers;

public class RezervacijeController : Controller
{
    private readonly AppDbContext _db;
    private readonly BendService _bendService;

    public RezervacijeController(AppDbContext db, BendService bendService)
    {
        _db = db;
        _bendService = bendService;
    }

    public async Task<IActionResult> Dodaj(int dogadajId)
    {
        var dogadaj = await _db.Dogadaji.Include(d => d.TipVjencanja).FirstOrDefaultAsync(d => d.Id == dogadajId);
        if (dogadaj == null) return NotFound();

        var slobodniBendovi = await _bendService.GetSlobodniZaDatum(dogadaj.DatumDogadaja);

        ViewBag.Dogadaj = dogadaj;
        ViewBag.SlobodniBendovi = new SelectList(
            slobodniBendovi.Select(b => new { b.Id, Naziv = b.Partner.Naziv }), "Id", "Naziv");

        return View(new Rezervacija { DogadajId = dogadajId, Datum = dogadaj.DatumDogadaja });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Dodaj(Rezervacija rezervacija)
    {
        ModelState.Remove("Dogadaj");
        ModelState.Remove("Bend");

        bool zauzet = await _bendService.JeAngaziran(rezervacija.BendId, rezervacija.Datum);

        if (zauzet)
        {
            ModelState.AddModelError("BendId", "Bend je već zauzet za odabrani datum.");
            var slobodni = await _bendService.GetSlobodniZaDatum(rezervacija.Datum);
            ViewBag.SlobodniBendovi = new SelectList(
                slobodni.Select(b => new { b.Id, Naziv = b.Partner.Naziv }), "Id", "Naziv");
            return View(rezervacija);
        }

        rezervacija.Potvrdena = true;
        _db.Rezervacije.Add(rezervacija);

        var bend = await _db.Bendovi.FindAsync(rezervacija.BendId);
        if (bend != null) bend.Status = StatusBenda.Zauzet;

        await _db.SaveChangesAsync();
        TempData["Uspjeh"] = "Bend je rezerviran.";
        return RedirectToAction("Detalji", "Dogadaji", new { id = rezervacija.DogadajId });
    }
}
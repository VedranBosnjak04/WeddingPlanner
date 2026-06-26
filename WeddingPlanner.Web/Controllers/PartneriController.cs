using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WeddingPlanner.Core.Models;
using WeddingPlanner.Infrastructure.Data;

namespace WeddingPlanner.Web.Controllers;

public class PartneriController : Controller
{
    private readonly AppDbContext _db;

    public PartneriController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Index(
        string sort = "naziv",
        string? filter = null,
        int page = 1,
        int pageSize = 10)
    {
        var query = _db.Partneri.AsQueryable();

        if (!string.IsNullOrEmpty(filter))
        {
            // Provjeri je li filter tip partnera (npr. Cvjecara, Slasticarnica...)
            if (Enum.TryParse<TipPartnera>(filter, out var tipFilter))
            {
                query = query.Where(p => p.Tip == tipFilter);
            }
            else
            {
                // Inače pretraži po nazivu ili kontaktu
                query = query.Where(p => p.Naziv.Contains(filter) || p.Kontakt.Contains(filter));
            }
        }

        ViewBag.CurrentSort = sort;
        ViewBag.CurrentFilter = filter;
        ViewBag.CurrentPage = page;

        query = sort switch
        {
            "tip" => query.OrderBy(p => p.Tip),
            "tip_desc" => query.OrderByDescending(p => p.Tip),
            "provizija" => query.OrderBy(p => p.Provizija),
            "provizija_desc" => query.OrderByDescending(p => p.Provizija),
            "naziv_desc" => query.OrderByDescending(p => p.Naziv),
            _ => query.OrderBy(p => p.Naziv)
        };

        var ukupno = await query.CountAsync();
        ViewBag.TotalPages = (int)Math.Ceiling(ukupno / (double)pageSize);

        var partneri = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return View(partneri);
    }

    public async Task<IActionResult> Detalji(int id)
    {
        var partner = await _db.Partneri
            .Include(p => p.Bend).ThenInclude(b => b!.Cijene)
            .Include(p => p.Bend).ThenInclude(b => b!.Playliste)
            .Include(p => p.Cvjecara).ThenInclude(c => c!.Aranzmani)
            .Include(p => p.Slasticarnica).ThenInclude(s => s!.Stavke)
            .Include(p => p.RestaurantSalon)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (partner == null) return NotFound();
        return View(partner);
    }

    public IActionResult Dodaj() => View(new Partner());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Dodaj(Partner partner)
    {
        if (!ModelState.IsValid) return View(partner);

        _db.Partneri.Add(partner);
        await _db.SaveChangesAsync();
        TempData["Uspjeh"] = $"Partner '{partner.Naziv}' je uspješno dodan.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Uredi(int id)
    {
        var partner = await _db.Partneri.FindAsync(id);
        if (partner == null) return NotFound();
        return View(partner);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Uredi(int id, Partner partner)
    {
        if (id != partner.Id) return BadRequest();
        if (!ModelState.IsValid) return View(partner);

        _db.Partneri.Update(partner);
        await _db.SaveChangesAsync();
        TempData["Uspjeh"] = "Partner je uspješno ažuriran.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Obrisi(int id)
    {
        var partner = await _db.Partneri
            .Include(p => p.Bend)
            .Include(p => p.Cvjecara)
            .Include(p => p.Slasticarnica)
            .Include(p => p.RestaurantSalon)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (partner == null) return NotFound();
        return View(partner);
    }

    [HttpPost, ActionName("Obrisi")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ObrisiPotvrdeno(int id)
    {
        var partner = await _db.Partneri.FindAsync(id);
        if (partner != null)
        {
            _db.Partneri.Remove(partner);
            await _db.SaveChangesAsync();
        }
        TempData["Uspjeh"] = "Partner je obrisan.";
        return RedirectToAction(nameof(Index));
    }
}
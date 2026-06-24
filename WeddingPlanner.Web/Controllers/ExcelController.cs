using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeddingPlanner.Infrastructure.Data;
using WeddingPlanner.Infrastructure.Services;

namespace WeddingPlanner.Web.Controllers;

public class ExcelController : Controller
{
    private readonly AppDbContext _db;
    private readonly ExcelService _excel;

    public ExcelController(AppDbContext db, ExcelService excel)
    {
        _db = db;
        _excel = excel;
    }

    [HttpGet]
    public async Task<IActionResult> ExportPartnere()
    {
        var partneri = await _db.Partneri.OrderBy(p => p.Naziv).ToListAsync();
        var bytes = _excel.ExportPartnere(partneri);
        return File(bytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"partneri_{DateTime.Now:yyyyMMdd}.xlsx");
    }

    [HttpGet]
    public async Task<IActionResult> ExportRacun(int dogadajId)
    {
        var dogadaj = await _db.Dogadaji.FindAsync(dogadajId);
        if (dogadaj == null) return NotFound();

        var stavke = await _db.RacunStavke
            .Where(s => s.DogadajId == dogadajId)
            .ToListAsync();

        var bytes = _excel.ExportRacun(dogadaj, stavke);
        return File(bytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"racun_{dogadaj.NazivPara.Replace(" ", "_")}_{DateTime.Now:yyyyMMdd}.xlsx");
    }

    [HttpGet]
    public IActionResult ImportPartnere() => View();

    [HttpPost]
    public async Task<IActionResult> ImportPartnere(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            ModelState.AddModelError("", "Odaberite Excel datoteku.");
            return View();
        }

        var partneri = _excel.ImportPartnere(file);
        _db.Partneri.AddRange(partneri);
        await _db.SaveChangesAsync();

        TempData["Uspjeh"] = $"Uvezeno {partneri.Count} partnera.";
        return RedirectToAction("Index", "Partneri");
    }
}
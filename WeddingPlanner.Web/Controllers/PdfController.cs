using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeddingPlanner.Infrastructure.Data;
using WeddingPlanner.Infrastructure.Services;

namespace WeddingPlanner.Web.Controllers;

public class PdfController : Controller
{
    private readonly AppDbContext _db;
    private readonly PdfService _pdf;

    public PdfController(AppDbContext db, PdfService pdf)
    {
        _db = db;
        _pdf = pdf;
    }

    public async Task<IActionResult> PonudaSlasticarnice(int slasticarnicaId)
    {
        var sl = await _db.Slasticarnice
            .Include(s => s.Partner)
            .Include(s => s.Stavke)
            .FirstOrDefaultAsync(s => s.Id == slasticarnicaId);

        if (sl == null) return NotFound();

        var bytes = _pdf.GenerirајPonuduSlasticarnice(sl.Partner, sl.Stavke);
        return File(bytes, "application/pdf",
            $"ponuda_{sl.Partner.Naziv.Replace(" ", "_")}.pdf");
    }

    public async Task<IActionResult> RacunPdf(int dogadajId)
    {
        var dogadaj = await _db.Dogadaji.FindAsync(dogadajId);
        if (dogadaj == null) return NotFound();

        var stavke = await _db.RacunStavke
            .Where(s => s.DogadajId == dogadajId)
            .ToListAsync();

        var bytes = _pdf.GeneriraqRacunPdf(dogadaj, stavke);
        return File(bytes, "application/pdf",
            $"racun_{dogadaj.NazivPara.Replace(" ", "_")}.pdf");
    }
}
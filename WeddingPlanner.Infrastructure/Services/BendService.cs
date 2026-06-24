using Microsoft.EntityFrameworkCore;
using WeddingPlanner.Core.Models;
using WeddingPlanner.Infrastructure.Data;

namespace WeddingPlanner.Infrastructure.Services;

public class BendService
{
    private readonly AppDbContext _db;
    public BendService(AppDbContext db) => _db = db;

    public async Task<bool> JeAngaziran(int bendId, DateTime datum)
    {
        return await _db.Rezervacije
            .AnyAsync(r => r.BendId == bendId
                        && r.Datum.Date == datum.Date
                        && r.Potvrdena);
    }

    public async Task<IEnumerable<Bend>> GetSlobodniZaDatum(DateTime datum)
    {
        var zauzetiBendIds = await _db.Rezervacije
            .Where(r => r.Datum.Date == datum.Date && r.Potvrdena)
            .Select(r => r.BendId)
            .ToListAsync();

        return await _db.Bendovi
            .Include(b => b.Partner)
            .Include(b => b.Cijene)
            .Where(b => !zauzetiBendIds.Contains(b.Id))
            .ToListAsync();
    }

    public async Task<decimal?> GetCijenu(
        int bendId, KategorijaVremena kategorija, int trajanjeH)
    {
        var cijena = await _db.CijeneBendova
            .FirstOrDefaultAsync(c => c.BendId == bendId
                                   && c.Kategorija == kategorija
                                   && c.TrajanjeH == trajanjeH);
        return cijena?.Iznos;
    }

    public async Task OznaciKaoZauzet(int bendId)
    {
        var bend = await _db.Bendovi.FindAsync(bendId);
        if (bend != null)
        {
            bend.Status = StatusBenda.Zauzet;
            await _db.SaveChangesAsync();
        }
    }

    public async Task OznaciKaoSlobodan(int bendId)
    {
        bool imaBuduce = await _db.Rezervacije
            .AnyAsync(r => r.BendId == bendId
                        && r.Datum > DateTime.Now
                        && r.Potvrdena);

        if (!imaBuduce)
        {
            var bend = await _db.Bendovi.FindAsync(bendId);
            if (bend != null)
            {
                bend.Status = StatusBenda.Slobodan;
                await _db.SaveChangesAsync();
            }
        }
    }
}
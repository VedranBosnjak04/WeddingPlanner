using HotChocolate;
using HotChocolate.Data;
using Microsoft.EntityFrameworkCore;
using WeddingPlanner.Core.Models;
using WeddingPlanner.Infrastructure.Data;

namespace WeddingPlanner.API.GraphQL;

public class Query
{
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Partner> GetPartneri([Service] AppDbContext context)
        => context.Partneri;

    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Bend> GetBendovi([Service] AppDbContext context)
        => context.Bendovi.Include(b => b.Partner);

    public async Task<Bend?> GetBend(int id, [Service] AppDbContext context)
        => await context.Bendovi
            .Include(b => b.Partner)
            .Include(b => b.Cijene)
            .Include(b => b.Playliste)
            .FirstOrDefaultAsync(b => b.Id == id);

    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Dogadaj> GetDogadaji([Service] AppDbContext context)
        => context.Dogadaji.Include(d => d.TipVjencanja);

    public async Task<Dogadaj?> GetDogadaj(int id, [Service] AppDbContext context)
        => await context.Dogadaji
            .Include(d => d.TipVjencanja)
            .Include(d => d.Rezervacije).ThenInclude(r => r.Bend)
            .Include(d => d.RacunStavke)
            .FirstOrDefaultAsync(d => d.Id == id);

    [UseProjection]
    public IQueryable<TipVjencanja> GetTipoviVjencanja([Service] AppDbContext context)
        => context.TipoviVjencanja;

    public async Task<IEnumerable<Bend>> GetSlobodniBendoviZaDatum(
        DateTime datum,
        [Service] AppDbContext context)
    {
        var zauzeti = await context.Rezervacije
            .Where(r => r.Datum.Date == datum.Date && r.Potvrdena)
            .Select(r => r.BendId)
            .ToListAsync();

        return await context.Bendovi
            .Include(b => b.Partner)
            .Where(b => !zauzeti.Contains(b.Id))
            .ToListAsync();
    }
}
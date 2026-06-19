using HotChocolate;
using HotChocolate.Data;
using Microsoft.EntityFrameworkCore;
using WeddingPlanner.Core.Models;
using WeddingPlanner.Infrastructure.Data;

namespace WeddingPlanner.API.GraphQL;

public class Query
{
    [UseDbContext(typeof(AppDbContext))]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Partner> GetPartneri([ScopedService] AppDbContext context)
        => context.Partneri;

    [UseDbContext(typeof(AppDbContext))]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Bend> GetBendovi([ScopedService] AppDbContext context)
        => context.Bendovi.Include(b => b.Partner);

    [UseDbContext(typeof(AppDbContext))]
    public async Task<Bend?> GetBend(int id, [ScopedService] AppDbContext context)
        => await context.Bendovi
            .Include(b => b.Partner)
            .Include(b => b.Cijene)
            .Include(b => b.Playliste)
            .FirstOrDefaultAsync(b => b.Id == id);

    [UseDbContext(typeof(AppDbContext))]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Dogadaj> GetDogadaji([ScopedService] AppDbContext context)
        => context.Dogadaji.Include(d => d.TipVjencanja);

    [UseDbContext(typeof(AppDbContext))]
    public async Task<Dogadaj?> GetDogadaj(int id, [ScopedService] AppDbContext context)
        => await context.Dogadaji
            .Include(d => d.TipVjencanja)
            .Include(d => d.Rezervacije).ThenInclude(r => r.Bend)
            .Include(d => d.RacunStavke)
            .FirstOrDefaultAsync(d => d.Id == id);

    [UseDbContext(typeof(AppDbContext))]
    [UseProjection]
    public IQueryable<TipVjencanja> GetTipoviVjencanja([ScopedService] AppDbContext context)
        => context.TipoviVjencanja;

    [UseDbContext(typeof(AppDbContext))]
    public async Task<IEnumerable<Bend>> GetSlobodniBendoviZaDatum(
        DateTime datum,
        [ScopedService] AppDbContext context)
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
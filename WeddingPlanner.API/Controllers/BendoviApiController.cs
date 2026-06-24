using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeddingPlanner.Core.Models;
using WeddingPlanner.Infrastructure.Data;

namespace WeddingPlanner.API.Controllers;

[ApiController]
[Route("api/bendovi")]
[Produces("application/json")]
public class BendoviApiController : ControllerBase
{
    private readonly AppDbContext _db;
    public BendoviApiController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetAll([FromQuery] bool? samoDJ = null)
    {
        var query = _db.Bendovi.Include(b => b.Partner).AsQueryable();
        if (samoDJ.HasValue) query = query.Where(b => b.JeDJ == samoDJ.Value);

        var rezultat = await query.Select(b => new
        {
            b.Id,
            NazivPartnera = b.Partner.Naziv,
            b.JeDJ,
            Status = b.Status.ToString(),
            BrojPlaylisti = b.Playliste.Count,
            BrojCijena = b.Cijene.Count
        }).ToListAsync();

        return Ok(rezultat);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<object>> GetById(int id)
    {
        var bend = await _db.Bendovi
            .Include(b => b.Partner)
            .Include(b => b.Cijene)
            .Include(b => b.Playliste)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (bend == null) return NotFound(new { poruka = $"Bend s ID={id} nije pronađen." });

        return Ok(new
        {
            bend.Id,
            NazivPartnera = bend.Partner.Naziv,
            Kontakt = bend.Partner.Kontakt,
            bend.JeDJ,
            Status = bend.Status.ToString(),
            Cijene = bend.Cijene.Select(c => new
            {
                Kategorija = c.Kategorija.ToString(),
                c.TrajanjeH,
                c.Iznos
            }),
            Playliste = bend.Playliste.Select(p => new { p.Naziv, p.Zanr })
        });
    }

    [HttpGet("slobodni")]
    public async Task<ActionResult<IEnumerable<object>>> GetSlobodniZaDatum([FromQuery] DateTime datum)
    {
        var zauzeti = await _db.Rezervacije
            .Where(r => r.Datum.Date == datum.Date && r.Potvrdena)
            .Select(r => r.BendId)
            .ToListAsync();

        var slobodni = await _db.Bendovi
            .Include(b => b.Partner)
            .Where(b => !zauzeti.Contains(b.Id))
            .Select(b => new { b.Id, NazivPartnera = b.Partner.Naziv, b.JeDJ })
            .ToListAsync();

        return Ok(slobodni);
    }

    [HttpPost]
    public async Task<ActionResult<Bend>> Create([FromBody] Bend bend)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        _db.Bendovi.Add(bend);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = bend.Id }, bend);
    }

    [HttpPut("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] StatusBenda status)
    {
        var bend = await _db.Bendovi.FindAsync(id);
        if (bend == null) return NotFound();
        bend.Status = status;
        await _db.SaveChangesAsync();
        return Ok(new { bend.Id, Status = bend.Status.ToString() });
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var bend = await _db.Bendovi.FindAsync(id);
        if (bend == null) return NotFound();
        _db.Bendovi.Remove(bend);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
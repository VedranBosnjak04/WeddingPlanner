using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeddingPlanner.Core.Models;
using WeddingPlanner.Infrastructure.Data;

namespace WeddingPlanner.API.Controllers;

[ApiController]
[Route("api/dogadaji")]
[Produces("application/json")]
public class DogadajiApiController : ControllerBase
{
    private readonly AppDbContext _db;
    public DogadajiApiController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var dogadaji = await _db.Dogadaji
            .Include(d => d.TipVjencanja)
            .Select(d => new
            {
                d.Id,
                d.NazivPara,
                d.DatumDogadaja,
                TipVjencanja = d.TipVjencanja.Naziv,
                Status = d.Status.ToString()
            })
            .ToListAsync();

        return Ok(dogadaji);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var dogadaj = await _db.Dogadaji
            .Include(d => d.TipVjencanja)
            .Include(d => d.Rezervacije).ThenInclude(r => r.Bend).ThenInclude(b => b.Partner)
            .Include(d => d.RacunStavke)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (dogadaj == null) return NotFound(new { poruka = $"Događaj ID={id} nije pronađen." });

        decimal ukupno = dogadaj.RacunStavke.Sum(s => s.IznosOsnovni + s.IznosOsnovni * s.Provizija / 100);

        return Ok(new
        {
            dogadaj.Id,
            dogadaj.NazivPara,
            dogadaj.DatumDogadaja,
            TipVjencanja = dogadaj.TipVjencanja.Naziv,
            Status = dogadaj.Status.ToString(),
            Rezervacije = dogadaj.Rezervacije.Select(r => new
            {
                r.Id,
                NazivBenda = r.Bend.Partner.Naziv,
                r.Datum,
                r.Potvrdena
            }),
            UkupniIznos = ukupno
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Dogadaj dogadaj)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        _db.Dogadaji.Add(dogadaj);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = dogadaj.Id }, dogadaj);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Dogadaj dogadaj)
    {
        if (id != dogadaj.Id) return BadRequest();
        var exists = await _db.Dogadaji.AnyAsync(d => d.Id == id);
        if (!exists) return NotFound();
        _db.Dogadaji.Update(dogadaj);
        await _db.SaveChangesAsync();
        return Ok(dogadaj);
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var dogadaj = await _db.Dogadaji.FindAsync(id);
        if (dogadaj == null) return NotFound();
        _db.Dogadaji.Remove(dogadaj);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("{id:int}/racun")]
    public async Task<IActionResult> GetRacun(int id)
    {
        var stavke = await _db.RacunStavke.Where(s => s.DogadajId == id).ToListAsync();
        decimal ukupno = stavke.Sum(s => s.IznosOsnovni + s.IznosOsnovni * s.Provizija / 100);

        return Ok(new
        {
            DogadajId = id,
            Stavke = stavke.Select(s => new
            {
                s.Id,
                s.Opis,
                Tip = s.Tip.ToString(),
                s.IznosOsnovni,
                s.Provizija,
                UkupnoSProvizijom = s.IznosOsnovni + s.IznosOsnovni * s.Provizija / 100
            }),
            UkupnoSvega = ukupno
        });
    }
}
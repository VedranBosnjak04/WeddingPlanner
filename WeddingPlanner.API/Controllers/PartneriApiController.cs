using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeddingPlanner.Core.Models;
using WeddingPlanner.Infrastructure.Data;

namespace WeddingPlanner.API.Controllers;

[ApiController]
[Route("api/partneri")]
[Produces("application/json")]
public class PartneriApiController : ControllerBase
{
    private readonly AppDbContext _db;
    public PartneriApiController(AppDbContext db) => _db = db;

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Partner>>> GetAll([FromQuery] TipPartnera? tip = null)
    {
        var query = _db.Partneri.AsQueryable();
        if (tip.HasValue) query = query.Where(p => p.Tip == tip.Value);
        return Ok(await query.OrderBy(p => p.Naziv).ToListAsync());
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Partner>> GetById(int id)
    {
        var partner = await _db.Partneri
            .Include(p => p.Bend)
            .Include(p => p.Cvjecara)
            .Include(p => p.Slasticarnica)
            .Include(p => p.RestaurantSalon)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (partner == null) return NotFound(new { poruka = $"Partner s ID={id} nije pronađen." });
        return Ok(partner);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Partner>> Create([FromBody] Partner partner)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        if (string.IsNullOrEmpty(partner.Naziv))
            return BadRequest(new { poruka = "Naziv je obavezan." });

        _db.Partneri.Add(partner);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = partner.Id }, partner);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Partner>> Update(int id, [FromBody] Partner partner)
    {
        if (id != partner.Id)
            return BadRequest(new { poruka = "ID u URL-u ne odgovara ID-u u tijelu zahtjeva." });

        var exists = await _db.Partneri.AnyAsync(p => p.Id == id);
        if (!exists) return NotFound(new { poruka = $"Partner s ID={id} nije pronađen." });

        _db.Partneri.Update(partner);
        await _db.SaveChangesAsync();
        return Ok(partner);
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var partner = await _db.Partneri.FindAsync(id);
        if (partner == null) return NotFound(new { poruka = $"Partner s ID={id} nije pronađen." });

        _db.Partneri.Remove(partner);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
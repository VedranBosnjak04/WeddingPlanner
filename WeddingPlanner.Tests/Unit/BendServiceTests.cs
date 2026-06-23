using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using WeddingPlanner.Core.Models;
using WeddingPlanner.Infrastructure.Data;
using WeddingPlanner.Infrastructure.Services;

namespace WeddingPlanner.Tests.Unit;

public class BendServiceTests
{
    private AppDbContext KreirajDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task JeAngaziran_VracaTrue_KadBendImaPotvrdenuRezervaciju()
    {
        // Arrange
        var db = KreirajDb();
        var datum = DateTime.Today.AddDays(10);

        db.Rezervacije.Add(new Rezervacija
        {
            BendId = 1,
            DogadajId = 1,
            Datum = datum,
            Potvrdena = true
        });
        await db.SaveChangesAsync();

        var service = new BendService(db);

        // Act
        var rezultat = await service.JeAngaziran(1, datum);

        // Assert
        rezultat.Should().BeTrue();
    }

    [Fact]
    public async Task JeAngaziran_VracaFalse_KadBendNemaRezervaciju()
    {
        // Arrange
        var db = KreirajDb();
        var service = new BendService(db);

        // Act
        var rezultat = await service.JeAngaziran(99, DateTime.Today);

        // Assert
        rezultat.Should().BeFalse();
    }

    [Fact]
    public async Task JeAngaziran_VracaFalse_KadRezervacijaNijePotvrdena()
    {
        // Arrange
        var db = KreirajDb();
        var datum = DateTime.Today.AddDays(5);

        db.Rezervacije.Add(new Rezervacija
        {
            BendId = 2,
            DogadajId = 1,
            Datum = datum,
            Potvrdena = false
        });
        await db.SaveChangesAsync();

        var service = new BendService(db);

        // Act
        var rezultat = await service.JeAngaziran(2, datum);

        // Assert
        rezultat.Should().BeFalse();
    }

    [Fact]
    public async Task GetSlobodniZaDatum_VracaSamoBendoveBezRezervacije()
    {
        // Arrange
        var db = KreirajDb();
        var datum = DateTime.Today.AddDays(20);

        db.Partneri.Add(new Partner
        {
            Id = 1,
            Naziv = "Bend A",
            Kontakt = "tel",
            Adresa = "adr"
        });
        db.Bendovi.Add(new Bend { Id = 1, PartnerId = 1 });
        db.Rezervacije.Add(new Rezervacija
        {
            BendId = 1,
            DogadajId = 1,
            Datum = datum,
            Potvrdena = true
        });

        db.Partneri.Add(new Partner
        {
            Id = 2,
            Naziv = "Bend B",
            Kontakt = "tel",
            Adresa = "adr"
        });
        db.Bendovi.Add(new Bend { Id = 2, PartnerId = 2 });

        await db.SaveChangesAsync();

        var service = new BendService(db);

        // Act
        var slobodni = (await service.GetSlobodniZaDatum(datum)).ToList();

        // Assert
        slobodni.Should().HaveCount(1);
        slobodni.First().Id.Should().Be(2);
    }

    [Fact]
    public async Task OznaciKaoZauzet_MijenjaStatusBenda()
    {
        // Arrange
        var db = KreirajDb();
        db.Partneri.Add(new Partner
        {
            Id = 1,
            Naziv = "Test",
            Kontakt = "t",
            Adresa = "a"
        });
        db.Bendovi.Add(new Bend
        {
            Id = 1,
            PartnerId = 1,
            Status = StatusBenda.Slobodan
        });
        await db.SaveChangesAsync();

        var service = new BendService(db);

        // Act
        await service.OznaciKaoZauzet(1);

        // Assert
        var bend = await db.Bendovi.FindAsync(1);
        bend!.Status.Should().Be(StatusBenda.Zauzet);
    }
}
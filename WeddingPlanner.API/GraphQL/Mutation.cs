using HotChocolate;
using WeddingPlanner.Core.Models;
using WeddingPlanner.Infrastructure.Data;

namespace WeddingPlanner.API.GraphQL;

public class Mutation
{
    public async Task<Partner> DodajPartnera(
        string naziv,
        string adresa,
        string kontakt,
        TipPartnera tip,
        decimal provizija,
        [Service] AppDbContext context)
    {
        var partner = new Partner
        {
            Naziv = naziv,
            Adresa = adresa,
            Kontakt = kontakt,
            Tip = tip,
            Provizija = provizija
        };
        context.Partneri.Add(partner);
        await context.SaveChangesAsync();
        return partner;
    }

    public async Task<Partner> AzurirajProviziju(
        int partnerId,
        decimal novaProvizija,
        [Service] AppDbContext context)
    {
        var partner = await context.Partneri.FindAsync(partnerId)
            ?? throw new Exception($"Partner ID={partnerId} nije pronađen.");
        partner.Provizija = novaProvizija;
        await context.SaveChangesAsync();
        return partner;
    }

    public async Task<Bend> PostaviStatusBenda(
        int bendId,
        StatusBenda status,
        [Service] AppDbContext context)
    {
        var bend = await context.Bendovi.FindAsync(bendId)
            ?? throw new Exception($"Bend ID={bendId} nije pronađen.");
        bend.Status = status;
        await context.SaveChangesAsync();
        return bend;
    }

    public async Task<Dogadaj> DodajDogadaj(
        string nazivPara,
        DateTime datumDogadaja,
        int tipVjencanjaId,
        [Service] AppDbContext context)
    {
        var dogadaj = new Dogadaj
        {
            NazivPara = nazivPara,
            DatumDogadaja = datumDogadaja,
            TipVjencanjaId = tipVjencanjaId,
            Status = StatusDogadaja.Planiranje
        };
        context.Dogadaji.Add(dogadaj);
        await context.SaveChangesAsync();
        return dogadaj;
    }

    public async Task<RacunStavka> DodajRacunStavku(
        int dogadajId,
        string opis,
        TipStavke tip,
        decimal iznosOsnovni,
        decimal provizija,
        [Service] AppDbContext context)
    {
        var stavka = new RacunStavka
        {
            DogadajId = dogadajId,
            Opis = opis,
            Tip = tip,
            IznosOsnovni = iznosOsnovni,
            Provizija = provizija
        };
        context.RacunStavke.Add(stavka);
        await context.SaveChangesAsync();
        return stavka;
    }
}
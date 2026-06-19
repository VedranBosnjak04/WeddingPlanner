using Bogus;
using WeddingPlanner.Core.Models;

namespace WeddingPlanner.Infrastructure.Data;

public static class DataSeeder
{
    public static void Seed(AppDbContext context)
    {
        if (context.TipoviVjencanja.Any()) return;

        var tipovi = new List<TipVjencanja>
        {
            new() { Naziv = "Boho", Opis = "Rustikalni stil s prirodnim materijalima" },
            new() { Naziv = "Vanjsko", Opis = "Vjenčanje u vrtu ili prirodi" },
            new() { Naziv = "Klasično", Opis = "Elegantno unutarnje vjenčanje" },
            new() { Naziv = "Plaža", Opis = "Romantično vjenčanje uz more" }
        };
        context.TipoviVjencanja.AddRange(tipovi);
        context.SaveChanges();

        var faker = new Faker("hr");

        var imenaOrkestara = new[] { "Ritam Srca", "Glazbeni Val", "Zvjezdana Noć", "Melodija Ljeta", "Harmonija Plus" };
        for (int i = 0; i < 5; i++)
        {
            var partner = new Partner
            {
                Naziv = imenaOrkestara[i],
                Adresa = faker.Address.FullAddress(),
                Kontakt = faker.Phone.PhoneNumber("+387 ## ### ###"),
                Tip = TipPartnera.Bend,
                Provizija = faker.Random.Decimal(10, 30),
                DodatniPodaci = $"{{\"uobicajenaProvizija\": {faker.Random.Int(15, 25)}}}"
            };
            context.Partneri.Add(partner);
            context.SaveChanges();

            var bend = new Bend { PartnerId = partner.Id, JeDJ = false, Status = StatusBenda.Slobodan };
            context.Bendovi.Add(bend);
            context.SaveChanges();

            foreach (var kat in Enum.GetValues<KategorijaVremena>())
                foreach (var trajanje in new[] { 1, 2, 3, 4 })
                {
                    decimal baza = kat == KategorijaVremena.Vikend ? 1200m : kat == KategorijaVremena.Praznik ? 1800m : 800m;
                    context.CijeneBendova.Add(new CijenaBenda
                    {
                        BendId = bend.Id,
                        Kategorija = kat,
                        TrajanjeH = trajanje,
                        Iznos = baza * trajanje * faker.Random.Decimal(0.9m, 1.1m)
                    });
                }

            var zanrovi = new[] { "Pop", "Rock", "Jazz", "Sevdalinka", "Narodna" };
            for (int j = 0; j < faker.Random.Int(2, 4); j++)
                context.Playliste.Add(new Playlist
                {
                    BendId = bend.Id,
                    Naziv = $"Playlist {j + 1}",
                    Zanr = zanrovi[faker.Random.Int(0, zanrovi.Length - 1)]
                });
        }

        context.SaveChanges();

        var dogadaji = new[]
        {
            ("Ana & Marko", DateTime.Now.AddMonths(2), 1),
            ("Sara & Ivan", DateTime.Now.AddMonths(3), 3),
            ("Lejla & Damir", DateTime.Now.AddMonths(5), 2)
        };
        foreach (var (par, datum, tipId) in dogadaji)
            context.Dogadaji.Add(new Dogadaj
            {
                NazivPara = par,
                DatumDogadaja = datum,
                TipVjencanjaId = tipId,
                Status = StatusDogadaja.Planiranje
            });

        context.SaveChanges();
    }
}
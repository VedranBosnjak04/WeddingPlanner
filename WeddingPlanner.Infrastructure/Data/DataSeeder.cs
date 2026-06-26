using Bogus;
using WeddingPlanner.Core.Models;

namespace WeddingPlanner.Infrastructure.Data;

public static class DataSeeder
{
    public static void Seed(AppDbContext context)
    {
        if (context.TipoviVjencanja.Any()) return;

        var faker = new Faker("hr");

        // --- Tipovi vjenčanja ---
        var tipovi = new List<TipVjencanja>
        {
            new() { Naziv = "Boho", Opis = "Rustikalni stil s prirodnim materijalima" },
            new() { Naziv = "Vanjsko", Opis = "Vjenčanje u vrtu ili prirodi" },
            new() { Naziv = "Klasično", Opis = "Elegantno unutarnje vjenčanje" },
            new() { Naziv = "Plaža", Opis = "Romantično vjenčanje uz more" }
        };
        context.TipoviVjencanja.AddRange(tipovi);
        context.SaveChanges();

        // --- Bendovi ---
        var imenaOrkestara = new[]
        {
            "Ritam Srca", "Glazbeni Val", "Zvjezdana Noć",
            "Melodija Ljeta", "Harmonija Plus"
        };

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

            var bend = new Bend
            {
                PartnerId = partner.Id,
                JeDJ = false,
                Status = StatusBenda.Slobodan
            };
            context.Bendovi.Add(bend);
            context.SaveChanges();

            foreach (var kat in Enum.GetValues<KategorijaVremena>())
                foreach (var trajanje in new[] { 1, 2, 3, 4 })
                {
                    decimal baza = kat == KategorijaVremena.Vikend ? 1200m :
                                   kat == KategorijaVremena.Praznik ? 1800m : 800m;
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

        // --- DJ-ovi ---
        var imenaD = new[] { "DJ Maestro", "DJ Sunce" };
        for (int i = 0; i < 2; i++)
        {
            var partner = new Partner
            {
                Naziv = imenaD[i],
                Adresa = faker.Address.FullAddress(),
                Kontakt = faker.Internet.Email(),
                Tip = TipPartnera.DJ,
                Provizija = faker.Random.Decimal(10, 20),
                DodatniPodaci = $"{{\"oprema\": \"profesionalna\", \"iskustvo\": {faker.Random.Int(5, 20)}}}"
            };
            context.Partneri.Add(partner);
            context.SaveChanges();

            var dj = new Bend
            {
                PartnerId = partner.Id,
                JeDJ = true,
                Status = StatusBenda.Slobodan
            };
            context.Bendovi.Add(dj);
            context.SaveChanges();

            foreach (var kat in Enum.GetValues<KategorijaVremena>())
                foreach (var trajanje in new[] { 2, 4, 6 })
                    context.CijeneBendova.Add(new CijenaBenda
                    {
                        BendId = dj.Id,
                        Kategorija = kat,
                        TrajanjeH = trajanje,
                        Iznos = faker.Random.Decimal(300, 1200) * trajanje
                    });
        }

        context.SaveChanges();

        // --- Cvjećare ---
        var cvjecare = new[]
        {
            ("Cvjetni Raj", "Kralja Tomislava 12, Mostar", "cvjetniraj@gmail.com"),
            ("Rosa i Lala", "Španskih boraca 5, Sarajevo", "rosailala@gmail.com"),
            ("Zeleni Vrt", "Bulevar 22, Banja Luka", "zelenivrt@gmail.com")
        };

        foreach (var (naziv, adresa, kontakt) in cvjecare)
        {
            var partner = new Partner
            {
                Naziv = naziv,
                Adresa = adresa,
                Kontakt = kontakt,
                Tip = TipPartnera.Cvjecara,
                Provizija = faker.Random.Decimal(8, 18),
                DodatniPodaci = "{\"dostava\": true, \"radnoVrijeme\": \"08-20h\"}"
            };
            context.Partneri.Add(partner);
            context.SaveChanges();

            var cvj = new Cvjecara { PartnerId = partner.Id };
            context.Cvjecare.Add(cvj);
            context.SaveChanges();

            var aranzmani = new[]
            {
                ("Mali buket", TipAranzmana.Buket, 50m),
                ("Veliki buket", TipAranzmana.Buket, 120m),
                ("Dekoracija stola", TipAranzmana.Stol, 80m),
                ("Dekoracija auta", TipAranzmana.Auto, 150m),
                ("Vjenčana dekoracija", TipAranzmana.Dekoracija, 500m)
            };

            foreach (var (naz, tip, cij) in aranzmani)
                context.CvjecarskiAranzmani.Add(new CvjecarskiAranzman
                {
                    CvjecaraId = cvj.Id,
                    Naziv = naz,
                    Tip = tip,
                    Cijena = cij * faker.Random.Decimal(0.9m, 1.2m),
                    Opis = faker.Lorem.Sentence()
                });

            context.SaveChanges();
        }

        // --- Slastičarnice ---
        var slasticarnice = new[]
        {
            ("Slatki Kutak", "Ferhadija 8, Sarajevo", "slatki.kutak@gmail.com"),
            ("Torte Maje", "Kralja Petra 15, Mostar", "tortemaje@gmail.com"),
            ("Petit Four", "Bulevar 33, Banja Luka", "petitfour@gmail.com")
        };

        foreach (var (naziv, adresa, kontakt) in slasticarnice)
        {
            var partner = new Partner
            {
                Naziv = naziv,
                Adresa = adresa,
                Kontakt = kontakt,
                Tip = TipPartnera.Slasticarnica,
                Provizija = faker.Random.Decimal(10, 20)
            };
            context.Partneri.Add(partner);
            context.SaveChanges();

            var sl = new Slasticarnica { PartnerId = partner.Id };
            context.Slasticarnice.Add(sl);
            context.SaveChanges();

            var stavke = new[]
            {
                ("Svadbena torta 1kg", KategorijaSlastica.Torta, 40m),
                ("Svadbena torta 2kg", KategorijaSlastica.Torta, 75m),
                ("Svadbena torta 3kg", KategorijaSlastica.Torta, 110m),
                ("Kolač sa kremom", KategorijaSlastica.Kolac, 5m),
                ("Baklava", KategorijaSlastica.Deserti, 3m),
                ("Cheesecake", KategorijaSlastica.Deserti, 6m),
                ("Macarons 12 kom", KategorijaSlastica.Deserti, 18m)
            };

            foreach (var (naz, kat, cij) in stavke)
                context.SlasticarskeStavke.Add(new SlasticarskaStavka
                {
                    SlasticarnicaId = sl.Id,
                    Naziv = naz,
                    Kategorija = kat,
                    Cijena = cij * faker.Random.Decimal(0.95m, 1.1m),
                    Opis = faker.Commerce.ProductDescription()
                });

            context.SaveChanges();
        }

        // --- Catering ---
        var catering = new[]
        {
            ("Catering Deluxe", "Zmaja od Bosne 10, Sarajevo", "deluxe@catering.ba"),
            ("Svadbeni Stol", "Mostarskog bataljona 3, Mostar", "svadbenistol@gmail.com")
        };

        foreach (var (naziv, adresa, kontakt) in catering)
        {
            context.Partneri.Add(new Partner
            {
                Naziv = naziv,
                Adresa = adresa,
                Kontakt = kontakt,
                Tip = TipPartnera.Catering,
                Provizija = faker.Random.Decimal(12, 22),
                DodatniPodaci = "{\"minOsoba\": 50, \"maxOsoba\": 500}"
            });
        }
        context.SaveChanges();

        // --- Restorani i Saloni ---
        var restorani = new[]
        {
            ("Svadbeni Dvorac", "Ilidža bb, Sarajevo", "dvorac@gmail.com",
             TipUsluge.HranaISala, 20, 8, 45m, (decimal?)2000m),
            ("Salon Kristal", "Bulevar 44, Banja Luka", "kristal@gmail.com",
             TipUsluge.SamoSala, 15, 10, 0m, (decimal?)3000m),
            ("Villa Rosa", "Obala 12, Mostar", "villarosa@gmail.com",
             TipUsluge.HranaISala, 25, 10, 55m, (decimal?)3500m),
            ("Catering Premium", "Adema Buće 10, Sarajevo", "premium@catering.ba",
             TipUsluge.SamoHrana, 0, 0, 35m, (decimal?)null)
        };

        foreach (var (naziv, adresa, kontakt, tip, stolovi, mjesta, cijOsoba, cijSala)
                 in restorani)
        {
            var partner = new Partner
            {
                Naziv = naziv,
                Adresa = adresa,
                Kontakt = kontakt,
                Tip = TipPartnera.RestaurantSalon,
                Provizija = faker.Random.Decimal(10, 25)
            };
            context.Partneri.Add(partner);
            context.SaveChanges();

            context.RestaurantSaloni.Add(new RestaurantSalon
            {
                PartnerId = partner.Id,
                TipUsluge = tip,
                BrojStolova = stolovi,
                MjestaPoBrojStolu = mjesta,
                CijenaPoOsobi = cijOsoba,
                CijenaSale = cijSala
            });
            context.SaveChanges();
        }

        // --- Primjeri događaja ---
        var tipoviIzBaze = context.TipoviVjencanja.ToList();
        if (tipoviIzBaze.Count >= 3)
        {
            var dogadaji = new[]
            {
                ("Ana & Marko", DateTime.Now.AddMonths(2), tipoviIzBaze[0].Id),
                ("Sara & Ivan", DateTime.Now.AddMonths(3), tipoviIzBaze[2].Id),
                ("Lejla & Damir", DateTime.Now.AddMonths(5), tipoviIzBaze[1].Id)
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
}
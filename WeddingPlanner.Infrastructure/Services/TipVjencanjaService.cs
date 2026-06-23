using WeddingPlanner.Core.Models;
using WeddingPlanner.Infrastructure.Data;

namespace WeddingPlanner.Infrastructure.Services;

public class TipVjencanjaService
{
    private readonly AppDbContext _db;

    private static readonly Dictionary<string, string[]> PreporuceniMjeseci = new()
    {
        ["Boho"] = new[] { "Maj", "Juni", "Septembar", "Oktobar" },
        ["Vanjsko"] = new[] { "Juni", "Juli", "August", "Septembar" },
        ["Klasično"] = new[] { "Oktobar", "Novembar", "Decembar", "Mart" },
        ["Plaža"] = new[] { "Juli", "August" }
    };

    private static readonly Dictionary<string, string[]> PreporucenaGlazba = new()
    {
        ["Boho"] = new[] { "Akustični bend", "Folk", "Jazz trio" },
        ["Vanjsko"] = new[] { "Pop bend", "DJ", "Mješoviti repertoar" },
        ["Klasično"] = new[] { "Klasični orkestar", "Klavir", "Sevdalinka" },
        ["Plaža"] = new[] { "DJ", "Latin bend", "Acoustic pop" }
    };

    private static readonly Dictionary<string, string[]> PreporuceniCatering = new()
    {
        ["Boho"] = new[] { "Rustikalni bifé", "BBQ catering", "Organska hrana" },
        ["Vanjsko"] = new[] { "Catering u prirodi", "Roštilj", "Koktel recepcija" },
        ["Klasično"] = new[] { "Višekratna večera", "Elegantan bifé", "Svadbena sala" },
        ["Plaža"] = new[] { "Morska hrana", "Koktel recepcija", "Lagani bifé" }
    };

    public TipVjencanjaService(AppDbContext db) => _db = db;

    public async Task<PreporukaVjencanja?> GetPreporuke(int tipVjencanjaId)
    {
        var tip = await _db.TipoviVjencanja.FindAsync(tipVjencanjaId);
        if (tip == null) return null;

        PreporuceniMjeseci.TryGetValue(tip.Naziv, out var datumi);
        PreporucenaGlazba.TryGetValue(tip.Naziv, out var glazba);
        PreporuceniCatering.TryGetValue(tip.Naziv, out var catering);

        return new PreporukaVjencanja
        {
            TipNaziv = tip.Naziv,
            TipOpis = tip.Opis,
            PreporuceniMjeseci = datumi ?? Array.Empty<string>(),
            PreporucenaGlazba = glazba ?? Array.Empty<string>(),
            PreporuceniCatering = catering ?? Array.Empty<string>()
        };
    }
}

public class PreporukaVjencanja
{
    public string TipNaziv { get; set; } = string.Empty;
    public string TipOpis { get; set; } = string.Empty;
    public string[] PreporuceniMjeseci { get; set; } = Array.Empty<string>();
    public string[] PreporucenaGlazba { get; set; } = Array.Empty<string>();
    public string[] PreporuceniCatering { get; set; } = Array.Empty<string>();
}
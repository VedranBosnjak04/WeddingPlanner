namespace WeddingPlanner.Core.Models;

public class Dogadaj
{
    public int Id { get; set; }
    public string NazivPara { get; set; } = string.Empty;
    public DateTime DatumDogadaja { get; set; }
    public int TipVjencanjaId { get; set; }
    public TipVjencanja TipVjencanja { get; set; } = null!;
    public StatusDogadaja Status { get; set; } = StatusDogadaja.Planiranje;
    public string? Napomena { get; set; }

    public ICollection<Rezervacija> Rezervacije { get; set; } = new List<Rezervacija>();
    public ICollection<RacunStavka> RacunStavke { get; set; } = new List<RacunStavka>();
}
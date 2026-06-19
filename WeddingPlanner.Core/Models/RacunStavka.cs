namespace WeddingPlanner.Core.Models;

public class RacunStavka
{
    public int Id { get; set; }
    public int DogadajId { get; set; }
    public Dogadaj Dogadaj { get; set; } = null!;
    public string Opis { get; set; } = string.Empty;
    public TipStavke Tip { get; set; }
    public decimal IznosOsnovni { get; set; }
    public decimal Provizija { get; set; }
}
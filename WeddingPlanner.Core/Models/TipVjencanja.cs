namespace WeddingPlanner.Core.Models;

public class TipVjencanja
{
    public int Id { get; set; }
    public string Naziv { get; set; } = string.Empty;
    public string Opis { get; set; } = string.Empty;

    public ICollection<Dogadaj> Dogadaji { get; set; } = new List<Dogadaj>();
}
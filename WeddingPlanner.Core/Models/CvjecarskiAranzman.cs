namespace WeddingPlanner.Core.Models;

public class CvjecarskiAranzman
{
    public int Id { get; set; }
    public int CvjecaraId { get; set; }
    public Cvjecara Cvjecara { get; set; } = null!;
    public string Naziv { get; set; } = string.Empty;
    public TipAranzmana Tip { get; set; }
    public decimal Cijena { get; set; }
    public string? Opis { get; set; }
}
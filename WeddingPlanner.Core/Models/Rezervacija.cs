namespace WeddingPlanner.Core.Models;

public class Rezervacija
{
    public int Id { get; set; }
    public int DogadajId { get; set; }
    public Dogadaj Dogadaj { get; set; } = null!;
    public int BendId { get; set; }
    public Bend Bend { get; set; } = null!;
    public DateTime Datum { get; set; }
    public bool Potvrdena { get; set; }
}
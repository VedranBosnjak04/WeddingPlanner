namespace WeddingPlanner.Core.Models;

public class Cvjecara
{
    public int Id { get; set; }
    public int PartnerId { get; set; }
    public Partner Partner { get; set; } = null!;

    public ICollection<CvjecarskiAranzman> Aranzmani { get; set; } = new List<CvjecarskiAranzman>();
}
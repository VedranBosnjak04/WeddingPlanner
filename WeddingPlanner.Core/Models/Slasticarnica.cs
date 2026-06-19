namespace WeddingPlanner.Core.Models;

public class Slasticarnica
{
    public int Id { get; set; }
    public int PartnerId { get; set; }
    public Partner Partner { get; set; } = null!;

    public ICollection<SlasticarskaStavka> Stavke { get; set; } = new List<SlasticarskaStavka>();
}
namespace WeddingPlanner.Core.Models;

public class CijenaBenda
{
    public int Id { get; set; }
    public int BendId { get; set; }
    public Bend Bend { get; set; } = null!;
    public KategorijaVremena Kategorija { get; set; }
    public int TrajanjeH { get; set; }
    public decimal Iznos { get; set; }
}

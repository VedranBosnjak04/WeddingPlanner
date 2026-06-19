namespace WeddingPlanner.Core.Models;

public class SlasticarskaStavka
{
    public int Id { get; set; }
    public int SlasticarnicaId { get; set; }
    public Slasticarnica Slasticarnica { get; set; } = null!;
    public string Naziv { get; set; } = string.Empty;
    public KategorijaSlastica Kategorija { get; set; }
    public decimal Cijena { get; set; }
    public string? Opis { get; set; }
}
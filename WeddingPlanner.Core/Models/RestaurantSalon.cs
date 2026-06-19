namespace WeddingPlanner.Core.Models;

public class RestaurantSalon
{
    public int Id { get; set; }
    public int PartnerId { get; set; }
    public Partner Partner { get; set; } = null!;
    public TipUsluge TipUsluge { get; set; }
    public int BrojStolova { get; set; }
    public int MjestaPoBrojStolu { get; set; }
    public decimal CijenaPoOsobi { get; set; }
    public decimal? CijenaSale { get; set; }
}
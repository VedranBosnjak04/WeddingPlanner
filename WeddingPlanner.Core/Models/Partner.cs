namespace WeddingPlanner.Core.Models;

public class Partner
{
    public int Id { get; set; }
    public string Naziv { get; set; } = string.Empty;
    public string Adresa { get; set; } = string.Empty;
    public string Kontakt { get; set; } = string.Empty;
    public TipPartnera Tip { get; set; }
    public decimal Provizija { get; set; }
    public string? DodatniPodaci { get; set; }

    public Bend? Bend { get; set; }
    public Cvjecara? Cvjecara { get; set; }
    public Slasticarnica? Slasticarnica { get; set; }
    public RestaurantSalon? RestaurantSalon { get; set; }
}
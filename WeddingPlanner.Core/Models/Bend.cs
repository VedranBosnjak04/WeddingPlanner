namespace WeddingPlanner.Core.Models;

public class Bend
{
    public int Id { get; set; }
    public int PartnerId { get; set; }
    public Partner Partner { get; set; } = null!;
    public bool JeDJ { get; set; }
    public StatusBenda Status { get; set; } = StatusBenda.Slobodan;

    public ICollection<Playlist> Playliste { get; set; } = new List<Playlist>();
    public ICollection<CijenaBenda> Cijene { get; set; } = new List<CijenaBenda>();
    public ICollection<Rezervacija> Rezervacije { get; set; } = new List<Rezervacija>();
}
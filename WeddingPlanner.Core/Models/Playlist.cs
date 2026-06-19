namespace WeddingPlanner.Core.Models;

public class Playlist
{
    public int Id { get; set; }
    public int BendId { get; set; }
    public Bend Bend { get; set; } = null!;
    public string Naziv { get; set; } = string.Empty;
    public string Zanr { get; set; } = string.Empty;
}
using Microsoft.EntityFrameworkCore;
using WeddingPlanner.Core.Models;

namespace WeddingPlanner.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Partner> Partneri => Set<Partner>();
    public DbSet<Bend> Bendovi => Set<Bend>();
    public DbSet<CijenaBenda> CijeneBendova => Set<CijenaBenda>();
    public DbSet<Playlist> Playliste => Set<Playlist>();
    public DbSet<Cvjecara> Cvjecare => Set<Cvjecara>();
    public DbSet<CvjecarskiAranzman> CvjecarskiAranzmani => Set<CvjecarskiAranzman>();
    public DbSet<Slasticarnica> Slasticarnice => Set<Slasticarnica>();
    public DbSet<SlasticarskaStavka> SlasticarskeStavke => Set<SlasticarskaStavka>();
    public DbSet<RestaurantSalon> RestaurantSaloni => Set<RestaurantSalon>();
    public DbSet<TipVjencanja> TipoviVjencanja => Set<TipVjencanja>();
    public DbSet<Dogadaj> Dogadaji => Set<Dogadaj>();
    public DbSet<Rezervacija> Rezervacije => Set<Rezervacija>();
    public DbSet<RacunStavka> RacunStavke => Set<RacunStavka>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Partner>(e =>
        {
            e.Property(p => p.Provizija).HasColumnType("decimal(5,2)");
            e.Property(p => p.DodatniPodaci).HasColumnType("nvarchar(max)");
        });

        modelBuilder.Entity<CijenaBenda>()
            .Property(c => c.Iznos).HasColumnType("decimal(10,2)");

        modelBuilder.Entity<CvjecarskiAranzman>()
            .Property(c => c.Cijena).HasColumnType("decimal(10,2)");

        modelBuilder.Entity<SlasticarskaStavka>()
            .Property(s => s.Cijena).HasColumnType("decimal(10,2)");

        modelBuilder.Entity<RestaurantSalon>(e =>
        {
            e.Property(r => r.CijenaPoOsobi).HasColumnType("decimal(10,2)");
            e.Property(r => r.CijenaSale).HasColumnType("decimal(10,2)");
        });

        modelBuilder.Entity<RacunStavka>(e =>
        {
            e.Property(r => r.IznosOsnovni).HasColumnType("decimal(10,2)");
            e.Property(r => r.Provizija).HasColumnType("decimal(5,2)");
        });

        modelBuilder.Entity<Rezervacija>()
            .HasOne(r => r.Bend)
            .WithMany(b => b.Rezervacije)
            .HasForeignKey(r => r.BendId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Rezervacija>()
            .HasOne(r => r.Dogadaj)
            .WithMany(d => d.Rezervacije)
            .HasForeignKey(r => r.DogadajId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

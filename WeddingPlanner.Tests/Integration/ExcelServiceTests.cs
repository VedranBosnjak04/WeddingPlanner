using FluentAssertions;
using WeddingPlanner.Core.Models;
using WeddingPlanner.Infrastructure.Services;

namespace WeddingPlanner.Tests.Integration;

public class ExcelServiceTests
{
    [Fact]
    public void ExportPartnere_VracaNePrazanNizBajtova()
    {
        // Arrange
        var service = new ExcelService();
        var partneri = new List<Partner>
        {
            new() { Id = 1, Naziv = "Test Bend", Tip = TipPartnera.Bend,
                    Adresa = "Mostar", Kontakt = "061 111 111", Provizija = 20 },
            new() { Id = 2, Naziv = "Test Cvjecara", Tip = TipPartnera.Cvjecara,
                    Adresa = "Mostar", Kontakt = "061 222 222", Provizija = 15 }
        };

        // Act
        var bytes = service.ExportPartnere(partneri);

        // Assert
        bytes.Should().NotBeNullOrEmpty();
        bytes.Length.Should().BeGreaterThan(1000);
    }

    [Fact]
    public void ExportPartnere_SaPraznomListom_VracaValidanExcel()
    {
        // Arrange
        var service = new ExcelService();

        // Act
        var bytes = service.ExportPartnere(new List<Partner>());

        // Assert
        bytes.Should().NotBeNullOrEmpty();
    }
}
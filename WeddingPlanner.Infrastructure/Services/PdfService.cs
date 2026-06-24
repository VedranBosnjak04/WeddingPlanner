using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using WeddingPlanner.Core.Models;

namespace WeddingPlanner.Infrastructure.Services;

public class PdfService
{
    public PdfService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public byte[] GenerirајPonuduSlasticarnice(
        Partner partner,
        IEnumerable<SlasticarskaStavka> stavke)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header().Column(col =>
                {
                    col.Item().Text($"PONUDA — {partner.Naziv.ToUpper()}")
                        .FontSize(18).Bold().FontColor(Colors.Purple.Medium);
                    col.Item().Text($"Kontakt: {partner.Kontakt}")
                        .FontSize(10).FontColor(Colors.Grey.Medium);
                    col.Item().Text($"Datum ponude: {DateTime.Now:dd.MM.yyyy}")
                        .FontSize(10).FontColor(Colors.Grey.Medium);
                    col.Item().LineHorizontal(1).LineColor(Colors.Purple.Medium);
                    col.Item().Height(10);
                });

                page.Content().Column(col =>
                {
                    col.Item().PaddingBottom(10).Text("Ponuda slastičarskih proizvoda")
                        .FontSize(14).Bold();

                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(c =>
                        {
                            c.RelativeColumn(3);
                            c.RelativeColumn(2);
                            c.RelativeColumn(2);
                        });

                        table.Header(h =>
                        {
                            h.Cell().Background(Colors.Purple.Medium)
                                .Padding(5).Text("Proizvod").FontColor(Colors.White).Bold();
                            h.Cell().Background(Colors.Purple.Medium)
                                .Padding(5).Text("Kategorija").FontColor(Colors.White).Bold();
                            h.Cell().Background(Colors.Purple.Medium)
                                .Padding(5).Text("Cijena (KM)").FontColor(Colors.White).Bold();
                        });

                        bool parni = true;
                        foreach (var s in stavke)
                        {
                            var boja = parni ? Colors.Purple.Lighten5 : Colors.White;
                            table.Cell().Background(boja).Padding(4).Text(s.Naziv);
                            table.Cell().Background(boja).Padding(4).Text(s.Kategorija.ToString());
                            table.Cell().Background(boja).Padding(4)
                                .Text($"{s.Cijena:F2} KM").AlignRight();
                            parni = !parni;
                        }
                    });

                    col.Item().Height(20);

                    col.Item().Background(Colors.Grey.Lighten3).Padding(10).Column(info =>
                    {
                        info.Item().Text($"Provizija agencije: {partner.Provizija:F1}%")
                            .FontSize(11).Bold();
                        info.Item().Text("Konačna cijena se dogovara pri narudžbi.")
                            .FontSize(9).FontColor(Colors.Grey.Medium);
                    });
                });

                page.Footer().Row(row =>
                {
                    row.RelativeItem().Text("Wedding Planner Agency")
                        .FontColor(Colors.Grey.Medium);
                    row.ConstantItem(80).AlignRight().Text(x =>
                    {
                        x.Span("Stranica ").FontColor(Colors.Grey.Medium);
                        x.CurrentPageNumber().FontColor(Colors.Grey.Medium);
                    });
                });
            });
        }).GeneratePdf();
    }

    public byte[] GeneriraqRacunPdf(Dogadaj dogadaj, List<RacunStavka> stavke)
    {
        decimal sveukupno = stavke.Sum(s =>
            s.IznosOsnovni + s.IznosOsnovni * s.Provizija / 100);

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);

                page.Header().Column(col =>
                {
                    col.Item().Text($"RAČUN — {dogadaj.NazivPara.ToUpper()}")
                        .FontSize(18).Bold().FontColor(Colors.Blue.Medium);
                    col.Item().Text($"Datum vjenčanja: {dogadaj.DatumDogadaja:dd.MM.yyyy}")
                        .FontSize(11);
                    col.Item().LineHorizontal(1).LineColor(Colors.Blue.Medium);
                    col.Item().Height(10);
                });

                page.Content().Column(col =>
                {
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(c =>
                        {
                            c.RelativeColumn(4);
                            c.RelativeColumn(1);
                            c.RelativeColumn(2);
                            c.RelativeColumn(1);
                            c.RelativeColumn(2);
                        });

                        table.Header(h =>
                        {
                            foreach (var hdr in new[] { "Opis", "Tip", "Iznos", "Prov.%", "Ukupno" })
                            {
                                h.Cell().Background(Colors.Blue.Medium)
                                    .Padding(5).Text(hdr).FontColor(Colors.White).Bold();
                            }
                        });

                        foreach (var s in stavke)
                        {
                            decimal ukupno = s.IznosOsnovni + s.IznosOsnovni * s.Provizija / 100;
                            table.Cell().Padding(4).Text(s.Opis);
                            table.Cell().Padding(4).Text(s.Tip.ToString());
                            table.Cell().Padding(4).Text($"{s.IznosOsnovni:F2}").AlignRight();
                            table.Cell().Padding(4).Text($"{s.Provizija:F0}%").AlignCenter();
                            table.Cell().Padding(4).Text($"{ukupno:F2} KM").AlignRight();
                        }
                    });

                    col.Item().Height(15);
                    col.Item().AlignRight().Text($"UKUPNO: {sveukupno:F2} KM")
                        .FontSize(14).Bold().FontColor(Colors.Blue.Darken1);
                });

                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Stranica ").FontSize(9).FontColor(Colors.Grey.Medium);
                    x.CurrentPageNumber().FontSize(9).FontColor(Colors.Grey.Medium);
                });
            });
        }).GeneratePdf();
    }
}
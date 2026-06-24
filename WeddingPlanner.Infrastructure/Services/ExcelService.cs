using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using WeddingPlanner.Core.Models;

namespace WeddingPlanner.Infrastructure.Services;

public class ExcelService
{
    public byte[] ExportPartnere(IEnumerable<Partner> partneri)
    {
        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Partneri");

        var zaglavlja = new[] { "ID", "Naziv", "Tip", "Adresa", "Kontakt", "Provizija %" };
        for (int i = 0; i < zaglavlja.Length; i++)
        {
            ws.Cell(1, i + 1).Value = zaglavlja[i];
            ws.Cell(1, i + 1).Style.Font.Bold = true;
            ws.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.DarkSlateBlue;
            ws.Cell(1, i + 1).Style.Font.FontColor = XLColor.White;
        }

        int red = 2;
        foreach (var p in partneri)
        {
            ws.Cell(red, 1).Value = p.Id;
            ws.Cell(red, 2).Value = p.Naziv;
            ws.Cell(red, 3).Value = p.Tip.ToString();
            ws.Cell(red, 4).Value = p.Adresa;
            ws.Cell(red, 5).Value = p.Kontakt;
            ws.Cell(red, 6).Value = (double)p.Provizija;
            red++;
        }

        ws.Columns().AdjustToContents();

        using var ms = new MemoryStream();
        wb.SaveAs(ms);
        return ms.ToArray();
    }

    public byte[] ExportRacun(Dogadaj dogadaj, IEnumerable<RacunStavka> stavke)
    {
        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Račun");

        ws.Cell(1, 1).Value = $"Račun za: {dogadaj.NazivPara}";
        ws.Cell(1, 1).Style.Font.Bold = true;
        ws.Cell(1, 1).Style.Font.FontSize = 14;
        ws.Range(1, 1, 1, 6).Merge();

        ws.Cell(2, 1).Value = $"Datum: {dogadaj.DatumDogadaja:dd.MM.yyyy}";
        ws.Range(2, 1, 2, 6).Merge();

        var cols = new[] { "Opis", "Tip", "Iznos (KM)", "Provizija %", "Iznos provizije", "Ukupno" };
        for (int i = 0; i < cols.Length; i++)
        {
            ws.Cell(4, i + 1).Value = cols[i];
            ws.Cell(4, i + 1).Style.Font.Bold = true;
            ws.Cell(4, i + 1).Style.Fill.BackgroundColor = XLColor.Gray;
            ws.Cell(4, i + 1).Style.Font.FontColor = XLColor.White;
        }

        int red = 5;
        decimal sveukupno = 0;

        foreach (var s in stavke)
        {
            decimal iznosProvizije = s.IznosOsnovni * s.Provizija / 100;
            decimal ukupno = s.IznosOsnovni + iznosProvizije;
            sveukupno += ukupno;

            ws.Cell(red, 1).Value = s.Opis;
            ws.Cell(red, 2).Value = s.Tip.ToString();
            ws.Cell(red, 3).Value = (double)s.IznosOsnovni;
            ws.Cell(red, 4).Value = (double)s.Provizija;
            ws.Cell(red, 5).Value = (double)iznosProvizije;
            ws.Cell(red, 6).Value = (double)ukupno;
            red++;
        }

        ws.Cell(red, 5).Value = "SVEUKUPNO:";
        ws.Cell(red, 5).Style.Font.Bold = true;
        ws.Cell(red, 6).Value = (double)sveukupno;
        ws.Cell(red, 6).Style.Font.Bold = true;
        ws.Cell(red, 6).Style.Fill.BackgroundColor = XLColor.LightGreen;

        ws.Columns().AdjustToContents();

        using var ms = new MemoryStream();
        wb.SaveAs(ms);
        return ms.ToArray();
    }

    public List<Partner> ImportPartnere(IFormFile file)
    {
        var partneri = new List<Partner>();
        using var wb = new XLWorkbook(file.OpenReadStream());
        var ws = wb.Worksheet(1);

        foreach (var row in ws.RowsUsed().Skip(1))
        {
            if (string.IsNullOrEmpty(row.Cell(2).GetString())) continue;

            if (!Enum.TryParse<TipPartnera>(row.Cell(3).GetString(), out var tip))
                tip = TipPartnera.Bend;

            partneri.Add(new Partner
            {
                Naziv = row.Cell(2).GetString(),
                Tip = tip,
                Adresa = row.Cell(4).GetString(),
                Kontakt = row.Cell(5).GetString(),
                Provizija = row.Cell(6).TryGetValue<decimal>(out var prov) ? prov : 0
            });
        }

        return partneri;
    }
}
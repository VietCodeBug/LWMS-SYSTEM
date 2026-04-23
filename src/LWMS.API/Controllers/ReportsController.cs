using ClosedXML.Excel;
using LWMS.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LWMS.API.Controllers;

[Route("api/v1/reports")]
[Authorize(Roles = "Admin,HubManager")]
public class ReportsController : ApiControllerBase
{
    private readonly IApplicationDbContext _context;

    public ReportsController(IApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("delivery-performance")]
    public async Task<IActionResult> GetDeliveryReport()
    {
        var parcels = await _context.Parcels
            .Select(p => new { p.TrackingCode, p.Status, p.CreatedAt, p.ReceiverName })
            .ToListAsync();

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Delivery Report");

        // Header
        worksheet.Cell(1, 1).Value = "Tracking Code";
        worksheet.Cell(1, 2).Value = "Status";
        worksheet.Cell(1, 3).Value = "Created At";
        worksheet.Cell(1, 4).Value = "Receiver";

        var headerRow = worksheet.Row(1);
        headerRow.Style.Font.Bold = true;
        headerRow.Style.Fill.BackgroundColor = XLColor.AirForceBlue;
        headerRow.Style.Font.FontColor = XLColor.White;

        // Data
        for (int i = 0; i < parcels.Count; i++)
        {
            worksheet.Cell(i + 2, 1).Value = parcels[i].TrackingCode;
            worksheet.Cell(i + 2, 2).Value = parcels[i].Status.ToString();
            worksheet.Cell(i + 2, 3).Value = parcels[i].CreatedAt.ToString("yyyy-MM-dd HH:mm");
            worksheet.Cell(i + 2, 4).Value = parcels[i].ReceiverName;
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        var content = stream.ToArray();

        return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Delivery_{DateTime.Now:yyyyMMdd}.xlsx");
    }

    [HttpGet("cod-settlement")]
    public async Task<IActionResult> GetCodReport()
    {
        var cods = await _context.CodRecords
            .Include(c => c.Parcel)
            .ToListAsync();

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("COD Settlement");

        worksheet.Cell(1, 1).Value = "Tracking Code";
        worksheet.Cell(1, 2).Value = "Amount";
        worksheet.Cell(1, 3).Value = "Status";
        worksheet.Cell(1, 4).Value = "Settled Date";

        worksheet.Row(1).Style.Font.Bold = true;

        for (int i = 0; i < cods.Count; i++)
        {
            worksheet.Cell(i + 2, 1).Value = cods[i].Parcel.TrackingCode;
            worksheet.Cell(i + 2, 2).Value = cods[i].Amount;
            worksheet.Cell(i + 2, 3).Value = cods[i].Status;
            worksheet.Cell(i + 2, 4).Value = cods[i].SettledAt?.ToString("yyyy-MM-dd") ?? "N/A";
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        var content = stream.ToArray();

        return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"COD_{DateTime.Now:yyyyMMdd}.xlsx");
    }
}

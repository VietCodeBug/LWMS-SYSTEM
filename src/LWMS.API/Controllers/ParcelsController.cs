using LWMS.Application.Common.Models;
using LWMS.Application.Parcels.Commands.BulkUpload;
using LWMS.Application.Parcels.Commands.Create;
using LWMS.Application.Parcels.Queries.GetParcelByTracking;
using LWMS.Application.Parcels.Queries.GetParcelList;
using LWMS.Application.Parcels.Queries.GetSlaAlert;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace LWMS.API.Controllers;

[Authorize]
[EnableRateLimiting("Merchant")]
public class ParcelsController : ApiControllerBase
{
    [HttpGet("{trackingCode}/label")]
    public async Task<IActionResult> GetLabel(string trackingCode)
    {
        var result = await Mediator.Send(new GetParcelByTrackingQuery { TrackingCode = trackingCode });
        if (result == null) return NotFound();

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(new PageSize(283, 425)); // 100x150mm in points
                page.Margin(10);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Content().Column(col =>
                {
                    col.Item().Text("LWMS LOGISTICS").FontSize(16).Bold().AlignCenter();
                    col.Item().PaddingVertical(5).LineHorizontal(1);

                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Column(c =>
                        {
                            c.Item().Text("SENDER:").Bold().FontSize(8);
                            c.Item().Text(result.SenderName);
                        });
                        row.RelativeItem().Column(c =>
                        {
                            c.Item().Text("RECEIVER:").Bold().FontSize(8);
                            c.Item().Text(result.ReceiverName);
                            c.Item().Text(result.ReceiverPhone);
                        });
                    });

                    col.Item().PaddingVertical(15).AlignCenter().Text(result.TrackingCode).FontSize(24).Bold();
                    
                    col.Item().AlignBottom().Row(row => {
                        row.RelativeItem().Text($"Date: {DateTime.Now:dd/MM/yyyy}");
                        row.RelativeItem().AlignRight().Text($"COD: {result.CodAmount:N0} VND").Bold();
                    });
                });
            });
        });

        var pdf = document.GeneratePdf();
        return File(pdf, "application/pdf", $"Label_{result.TrackingCode}.pdf");
    }

    [HttpPost("bulk-upload")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<BulkUploadResponse>> BulkUpload(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("File không hợp lệ");

        var command = new BulkUploadCommand 
        { 
            Stream = file.OpenReadStream(),
            MerchantId = CurrentUserService.MerchantId ?? Guid.Empty
        };

        return await Mediator.Send(command);
    }

    [HttpPost]
    public async Task<ActionResult<CreateParcelResponse>> Create(CreateParcelCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedList<ParcelBriefDto>>> GetList([FromQuery] GetParcelListQuery query)
    {
        return await Mediator.Send(query);
    }

    [HttpGet("{trackingCode}")]
    public async Task<ActionResult<ParcelDto>> GetByTracking(string trackingCode)
    {
        var result = await Mediator.Send(new GetParcelByTrackingQuery { TrackingCode = trackingCode });
        if (result == null) return NotFound();
        return result;
    }

    [HttpGet("sla-alerts")]
    [Authorize(Roles = "Admin,HubManager")]
    public async Task<ActionResult<List<SlaAlertDto>>> GetSlaAlerts([FromQuery] int warningHours = 24)
    {
        return await Mediator.Send(new GetSlaAlertQuery { WarningHours = warningHours });
    }
}

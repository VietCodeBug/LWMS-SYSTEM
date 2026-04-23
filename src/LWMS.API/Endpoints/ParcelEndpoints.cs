using LWMS.Application.Parcels.Commands.Create;
using LWMS.Application.Parcels.Commands.ScanInbound;
using LWMS.Application.Parcels.Commands.Sort;
using LWMS.Application.Parcels.Queries.GetParcelByTracking;
using LWMS.Application.Parcels.Queries.GetParcelList;
using LWMS.Application.Parcels.Queries.GetSlaAlert;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LWMS.API.Endpoints;

public static class ParcelEndpoints
{
    public static WebApplication MapParcelEndpoints(this WebApplication app)
    {
        app.MapPost("/api/parcels", async ([FromBody] CreateParcelCommand command, IMediator mediator) =>
        {
            var id = await mediator.Send(command);
            return Results.Ok(new { id, status = "Created" });
        })
        .RequireAuthorization()
        .WithTags("Parcels");

        app.MapPost("/api/parcels/scan-inbound", async ([FromBody] ScanInboundCommand command, IMediator mediator) =>
        {
            await mediator.Send(command);
            return Results.Ok(new { message = "Scan Inbound thành công!" });
        })
        .RequireAuthorization()
        .WithTags("Parcels");

        app.MapPost("/api/parcels/sort", async ([FromBody] SortParcelCommand command, IMediator mediator) =>
        {
            await mediator.Send(command);
            return Results.Ok(new { message = "Phân loại (Sort) thành công!" });
        })
        .RequireAuthorization()
        .WithTags("Parcels");

        app.MapGet("/api/parcels/{trackingCode}", async (string trackingCode, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetParcelByTrackingQuery { TrackingCode = trackingCode });
            return result != null ? Results.Ok(result) : Results.NotFound();
        })
        .WithTags("Parcels");

        app.MapGet("/api/parcels", async ([FromQuery] int pageNumber, [FromQuery] int pageSize, [FromQuery] string? status, IMediator mediator) =>
        {
            var query = new GetParcelListQuery { PageNumber = pageNumber > 0 ? pageNumber : 1, PageSize = pageSize > 0 ? pageSize : 10 };
            if (!string.IsNullOrEmpty(status) && Enum.TryParse<LWMS.Domain.Enums.ParcelStatus>(status, true, out var statusEnum))
            {
                query.Status = statusEnum;
            }
            var result = await mediator.Send(query);
            return Results.Ok(result);
        })
        .RequireAuthorization()
        .WithTags("Parcels");

        app.MapGet("/api/parcels/sla-alerts", async ([FromQuery] int warningHours, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetSlaAlertQuery { WarningHours = warningHours > 0 ? warningHours : 24 });
            return Results.Ok(result);
        })
        .RequireAuthorization()
        .WithTags("Parcels");

        return app;
    }
}

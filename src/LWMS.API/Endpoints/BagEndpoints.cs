using LWMS.Application.Bags.Commands.AssignShipper;
using LWMS.Application.Bags.Commands.DeliveryFailed;
using LWMS.Application.Bags.Commands.DeliverySuccess;
using LWMS.Application.Bags.Commands.Create;
using LWMS.Application.Bags.Commands.AddParcel;
using LWMS.Application.Bags.Commands.Seal;
using LWMS.Application.Bags.Commands.Receive;
using LWMS.Application.Bags.Commands.ReAssignShipper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LWMS.API.Endpoints;

public static class BagEndpoints
{
    public static WebApplication MapBagEndpoints(this WebApplication app)
    {
        app.MapPost("/api/bags/create", async ([FromBody] CreateBagCommand command, IMediator mediator) =>
        {
            var id = await mediator.Send(command);
            return Results.Ok(new { id, message = "Tạo Bag thành công!" });
        })
        .RequireAuthorization()
        .WithTags("Bags");

        app.MapPost("/api/bags/add-parcel", async ([FromBody] AddParcelToBagCommand command, IMediator mediator) =>
        {
            await mediator.Send(command);
            return Results.Ok(new { message = "Thêm Parcel vào Bag thành công!" });
        })
        .RequireAuthorization()
        .WithTags("Bags");

        app.MapPost("/api/bags/seal", async ([FromBody] SealBagCommand command, IMediator mediator) =>
        {
            await mediator.Send(command);
            return Results.Ok(new { message = "Seal Bag thành công!" });
        })
        .RequireAuthorization()
        .WithTags("Bags");

        app.MapPost("/api/bags/receive", async ([FromBody] ReceiveBagCommand command, IMediator mediator) =>
        {
            await mediator.Send(command);
            return Results.Ok(new { message = "Nhận Bag tại kho đích thành công!" });
        })
        .RequireAuthorization()
        .WithTags("Bags");

        app.MapPost("/assign-shipper", async ([FromBody] AssignShipperCommand command, IMediator mediator) =>
        {
            await mediator.Send(command);
            return Results.Ok(new { message = "Gán Shipper thành công!" });
        })
        .RequireAuthorization()
        .WithTags("Bags");

        app.MapPost("/re-assign-shipper", async ([FromBody] ReAssignShipperCommand command, IMediator mediator) =>
        {
            await mediator.Send(command);
            return Results.Ok(new { message = "Giao lại thành công!" });
        })
        .RequireAuthorization()
        .WithTags("Bags");

        app.MapPost("/delivery-failed", async ([FromBody] DeliveryFailedCommand command, IMediator mediator) =>
        {
            await mediator.Send(command);
            return Results.Ok(new { message = "Ghi nhận giao hàng thất bại thành công!" });
        })
        .RequireAuthorization()
        .WithTags("Bags");

        app.MapPost("/delivery-success", async ([FromBody] DeliverySuccessCommand command, IMediator mediator) =>
        {
            await mediator.Send(command);
            return Results.Ok(new { message = "Ghi nhận giao thành công!" });
        })
        .RequireAuthorization()
        .WithTags("Bags");

        return app;
    }
}

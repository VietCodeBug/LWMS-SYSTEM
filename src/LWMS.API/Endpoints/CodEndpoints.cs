using LWMS.Application.Cod.Commands.Submit;
using LWMS.Application.Cod.Commands.Settle;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LWMS.API.Endpoints;

public static class CodEndpoints
{
    public static WebApplication MapCodEndpoints(this WebApplication app)
    {
        app.MapPost("/submit-cod", async ([FromBody] SubmitCodCommand command, IMediator mediator) =>
        {
            await mediator.Send(command);
            return Results.Ok(new { message = "Nộp tiền COD thành công!" });
        })
        .RequireAuthorization()
        .WithTags("COD");

        app.MapPost("/settle-cod", async ([FromBody] SettleCodCommand command, IMediator mediator) =>
        {
            await mediator.Send(command);
            return Results.Ok(new { message = "Đối soát COD thành công!" });
        })
        .RequireAuthorization()
        .WithTags("COD");

        return app;
    }
}

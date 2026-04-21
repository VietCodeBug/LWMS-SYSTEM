using LWMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LWMS.API.Endpoints;

public static class DiagnosticEndpoints
{
    public static void MapDiagnosticEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/debug").WithTags("🛠 DIAGNOSTIC");

        group.MapGet("/preview-data", async (AppDbContext db) =>
        {
            var summary = new
            {
                ServerTime = DateTime.UtcNow,
                DatabaseConnection = "OK",
                Stats = new
                {
                    TotalHubs = await db.Hubs.CountAsync(),
                    TotalUsers = await db.Users.CountAsync(),
                    TotalMerchants = await db.Merchants.CountAsync(),
                    TotalServiceTypes = await db.ServiceTypes.CountAsync()
                },
                Hubs = await db.Hubs.Select(h => new { h.HubCode, h.Name, h.Address }).ToListAsync(),
                Merchants = await db.Merchants.Select(m => new { m.MerchantCode, m.Name }).ToListAsync(),
                ActiveUsers = await db.Users.Select(u => new { u.EmployeeCode, u.FullName, u.Role }).ToListAsync(),
                ServiceTypes = await db.ServiceTypes.Select(s => new { s.Code, s.Name, s.BaseFee }).ToListAsync()
            };

            return Results.Ok(summary);
        })
        .AllowAnonymous(); // Cho phép xem nhanh không cần login để test
    }
}

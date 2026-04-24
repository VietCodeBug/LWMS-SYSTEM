using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LWMS.Infrastructure.Data;
using Microsoft.Extensions.Configuration;

namespace LWMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CloudDiagnosticsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public CloudDiagnosticsController(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpGet("check-db")]
    public async Task<IActionResult> CheckDatabase()
    {
        var provider = _context.Database.ProviderName;
        var connectionString = _configuration.GetConnectionString("TiDbConnection");
        
        try
        {
            var canConnect = await _context.Database.CanConnectAsync();
            var hubCount = await _context.Hubs.CountAsync();
            
            return Ok(new
            {
                Status = "Success",
                CurrentProvider = provider,
                IsCloud = provider?.Contains("MySql") ?? false,
                ConnectionOk = canConnect,
                DataFound = new {
                    Hubs = hubCount,
                    Message = "Database đã sẵn sàng và được seed dữ liệu mẫu."
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new {
                Status = "Error",
                CurrentProvider = provider,
                ErrorMessage = ex.Message,
                Help = "Kiểm tra lại IP Whitelist trên TiDB Cloud và Connection String trong appsettings.json"
            });
        }
    }
}

using LWMS.Application;
using LWMS.Infrastructure;
using LWMS.API.Middlewares;
using LWMS.Application.Common.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using LWMS.Domain.Services;
using LWMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using LWMS.Application.Common.Interfaces;
using LWMS.API.Services;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using QuestPDF.Infrastructure;
using LWMS.API.Configurations;

// ──────────────────────────────────────────────
// 0. CONFIG LIBRARIES
// ──────────────────────────────────────────────
QuestPDF.Settings.License = LicenseType.Community;

// ──────────────────────────────────────────────
// 1. BOOTSTRAP LOGGER (khởi động sớm để log lỗi startup)
// ──────────────────────────────────────────────
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);

// ──────────────────────────────────────────────
// 2. SERILOG (đọc cấu hình từ appsettings)
// ──────────────────────────────────────────────
builder.Host.UseSerilog((ctx, lc) =>
    lc.ReadFrom.Configuration(ctx.Configuration));

// ──────────────────────────────────────────────
// 3. JWT AUTHENTICATION
// ──────────────────────────────────────────────
var jwtSettings = builder.Configuration
    .GetSection("JwtSettings")
    .Get<JwtSettings>()!;

builder.Services.AddSingleton(jwtSettings);

builder.Services
    .AddAuthentication(opt =>
    {
        opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                                           Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// ──────────────────────────────────────────────
// 4. APPLICATION & INFRASTRUCTURE SERVICES
// ──────────────────────────────────────────────
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHealthChecks().AddDbContextCheck<AppDbContext>();

// Auth Services
builder.Services.AddHttpContextAccessor();
builder.Services.AddAppAuthorization();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

builder.Services.AddControllers();

// ──────────────────────────────────────────────
// 5.5 RATE LIMITING (.NET 7+)
// ──────────────────────────────────────────────
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("Merchant", opt =>
    {
        opt.Window = TimeSpan.FromMinutes(1);
        opt.PermitLimit = 1000;
        opt.QueueLimit = 10;
        opt.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
    });

    options.AddFixedWindowLimiter("Public", opt =>
    {
        options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
        opt.Window = TimeSpan.FromMinutes(1);
        opt.PermitLimit = 100;
        opt.QueueLimit = 0;
    });
});

// ──────────────────────────────────────────────
// 5. SWAGGER / OPENAPI
// ──────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "LWMS Logistics API", 
        Version = "v1",
        Description = "Hệ thống quản lý vận hành Logistics - Phase 4 Production Ready"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Chỉ cần dán JWT token vào đây (không cần gõ 'Bearer')"
    });

    c.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("Bearer", doc),
            new List<string>()
        }
    });
});

// ──────────────────────────────────────────────
// 6. BUILD APP
// ──────────────────────────────────────────────
var app = builder.Build();

// ──────────────────────────────────────────────
// 7. MIDDLEWARE PIPELINE (thứ tự quan trọng!)
// ──────────────────────────────────────────────
app.UseMiddleware<ExceptionMiddleware>();    // 1. Bắt lỗi toàn cục
app.UseMiddleware<CorrelationIdMiddleware>(); // 2. Gán mã truy vết
app.UseMiddleware<RequestLoggingMiddleware>(); // 3. Ghi log Request
app.UseMiddleware<ApiKeyMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter(); // 4. Giới hạn tần suất

// ──────────────────────────────────────────────
// 8. MAP CONTROLLERS
// ──────────────────────────────────────────────
app.MapControllers();
app.MapHealthChecks("/health");

// ──────────────────────────────────────────────
// 9. AUTO DATABASE INITIALIZATION & SEEDING (For TiDB Cloud)
// ──────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        var passwordService = services.GetRequiredService<IPasswordService>();
        // Tự động tạo bảng nếu sài TiDB Cloud mới
        await context.Database.EnsureCreatedAsync();
        // Đổ dữ liệu mẫu ban đầu để sài luôn
        await InitialSeedData.SeedAsync(context, passwordService);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Lỗi khi tự động khởi tạo Database trên Cloud.");
    }
}

app.Run();

public partial class Program { }

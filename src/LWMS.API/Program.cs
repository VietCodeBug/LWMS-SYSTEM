using LWMS.Application;
using LWMS.Infrastructure;
using LWMS.API.Endpoints;
using LWMS.API.Middlewares;
using LWMS.Application.Common.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Serilog;
using System.Text;

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
        opt.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidateAudience         = true,
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer              = jwtSettings.Issuer,
            ValidAudience            = jwtSettings.Audience,
            IssuerSigningKey         = new SymmetricSecurityKey(
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

// ──────────────────────────────────────────────
// 5. SWAGGER / OPENAPI
// ──────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name        = "Authorization",
        Type        = SecuritySchemeType.Http,
        Scheme      = "bearer",
        BearerFormat = "JWT",
        In          = ParameterLocation.Header,
        Description = "Chỉ cần dán JWT token vào đây (không cần gõ 'Bearer')"
    });

    c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
    });
});

// ──────────────────────────────────────────────
// 6. BUILD APP
// ──────────────────────────────────────────────
var app = builder.Build();

// ──────────────────────────────────────────────
// 7. MIDDLEWARE PIPELINE (thứ tự quan trọng!)
// ──────────────────────────────────────────────
app.UseMiddleware<ExceptionMiddleware>(); // ← bắt exception toàn cục

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

// ──────────────────────────────────────────────
// 8. MAP ENDPOINTS (tách ra từng file)
// ──────────────────────────────────────────────
app.MapAuthEndpoints();
app.MapProductEndpoints();

app.Run();
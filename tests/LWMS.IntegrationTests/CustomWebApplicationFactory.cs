using Microsoft.Extensions.Hosting;
using LWMS.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Authentication;
using LWMS.Application.Common.Interfaces;
using LWMS.Domain.Entities;
using System.Net.Http.Headers;
using System.Linq;

namespace LWMS.IntegrationTests;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    public static Guid TestHubId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    public static Guid TestDestHubId = Guid.Parse("99999999-9999-9999-9999-999999999999");
    public static Guid TestServiceId = Guid.Parse("00000000-0000-0000-0000-000000000002");
    public static Guid TestMerchantId = Guid.Parse("00000000-0000-0000-0000-000000000003");
    public static Guid TestUserId = Guid.Parse("00000000-0000-0000-0000-000000000004");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<AppDbContext>));
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase("LWMS_Test_DB_" + Guid.NewGuid().ToString());
            });

            services.RemoveAll(typeof(ICurrentUserService));
            services.AddScoped<ICurrentUserService, MockCurrentUserService>();

            services.AddAuthentication("Test")
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", null);
        });
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);
        using (var scope = host.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.EnsureCreated();

            db.Hubs.Add(new Hub { Id = TestHubId, HubCode = "O", Name = "O", Address = "A" });
            db.Hubs.Add(new Hub { Id = TestDestHubId, HubCode = "D", Name = "D", Address = "B" });
            db.ServiceTypes.Add(new ServiceType { Id = TestServiceId, Code = "S", Name = "S", BaseFee = 0 });
            db.Merchants.Add(new Merchant { Id = TestMerchantId, MerchantCode = "M", Name = "M", IsActive = true, ApiKey = "K" });
            db.SaveChanges();
        }
        return host;
    }
}

public class MockCurrentUserService : ICurrentUserService
{
    public Guid? UserId => CustomWebApplicationFactory<Program>.TestUserId;
    public Guid? MerchantId => CustomWebApplicationFactory<Program>.TestMerchantId;
    public string? Role => "Admin";
}

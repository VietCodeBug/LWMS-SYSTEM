using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using LWMS.Infrastructure.Data;
using LWMS.Application.Common.Interfaces;
using LWMS.Infrastructure.Services;
using LWMS.Infrastructure.Repositories;

namespace LWMS.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            services.AddDbContext<AppDbContext>(options =>
            {
                // ☁️ Chốt hạ sử dụng TiDB Cloud (MySQL) cho toàn bộ hệ thống
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            });

            services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<AppDbContext>());
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IPasswordService, PasswordService>();

            // Repositories
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IParcelRepository, ParcelRepository>();
            services.AddScoped<IHubRepository, HubRepository>();
            services.AddScoped<IMerchantRepository, MerchantRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IBagRepository, BagRepository>();

            // Interceptors
            services.AddScoped<Data.Interceptors.AuditInterceptor>();

            return services;
        }
    }
}

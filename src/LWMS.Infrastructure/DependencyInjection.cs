using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using LWMS.Infrastructure.Data;
using LWMS.Application.Common.Interfaces;
using LWMS.Infrastructure.Services;


namespace LWMS.Infrastructure
    {   
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(config.GetConnectionString("DefaultConnection")));
            
            services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<AppDbContext>());
             services.AddScoped<JwtService>();
            return services;
        }
    }
}   

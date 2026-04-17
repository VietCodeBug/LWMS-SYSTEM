using Microsoft.EntityFrameworkCore;
using LWMS.Domain.Entities;
using LWMS.Application.Common.Interfaces;

namespace LWMS.Infrastructure.Data
{
    public class AppDbContext : DbContext, IApplicationDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
    }
}
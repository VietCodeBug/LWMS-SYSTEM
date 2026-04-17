using MediatR;
using LWMS.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using LWMS.Domain.Entities;

namespace LWMS.Application.Products.Queries
{
    public class GetProductsHandler : IRequestHandler<GetProductsQuery,List<Product>>
    {
        private readonly IApplicationDbContext _db;

        public GetProductsHandler(IApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<Product>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            return await _db.Products.ToListAsync(cancellationToken);
        }
    }
}
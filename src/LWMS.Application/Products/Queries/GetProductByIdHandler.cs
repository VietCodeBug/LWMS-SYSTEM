using MediatR;
using Microsoft.EntityFrameworkCore;
using LWMS.Application.Common.Interfaces;
using LWMS.Domain.Entities;

namespace LWMS.Application.Products.Queries
{
    public class GetProductByIdHandler : IRequestHandler<GetProductByIdQuery, Product?>
    {
        private readonly IApplicationDbContext _db;

        public GetProductByIdHandler(IApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<Product?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            return await _db.Products.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
        }
    }
}
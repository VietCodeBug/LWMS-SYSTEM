using MediatR;
using Microsoft.EntityFrameworkCore;
using LWMS.Application.Common.Interfaces;
using LWMS.Domain.Entities;
namespace LWMS.Application.Products.Commands.Update
{
    public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, bool>
    {
        private readonly IApplicationDbContext _db;

        public UpdateProductHandler(IApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<bool> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
            if (product == null)
                return false;

            product.Name = request.Name;
            product.Quantity = request.Quantity;
            product.Price = request.Price;

            await _db.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
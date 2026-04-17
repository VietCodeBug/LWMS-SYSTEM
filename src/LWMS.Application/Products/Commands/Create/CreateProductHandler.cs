using MediatR;
using LWMS.Domain.Entities;
using LWMS.Application.Common.Interfaces;

namespace LWMS.Application.Products.Commands.Create
{
    public class CreateProductHandler : IRequestHandler<CreateProductCommand, int>
    {
        private readonly IApplicationDbContext _db;

        public CreateProductHandler(IApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<int> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var product = new Product
            {
                Name = request.Name,
                Quantity = request.Quantity,
                Price = request.Price
            };

            _db.Products.Add(product);
            await _db.SaveChangesAsync(cancellationToken);

            return product.Id;
        }
    }
}
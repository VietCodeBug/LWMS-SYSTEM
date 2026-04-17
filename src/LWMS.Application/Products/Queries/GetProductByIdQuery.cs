using MediatR;
using LWMS.Domain.Entities;

namespace LWMS.Application.Products.Queries
{
    public class GetProductByIdQuery : IRequest<Product?>
    {
        public int Id { get; set; }
    }
}
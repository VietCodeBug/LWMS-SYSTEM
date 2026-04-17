using MediatR;
using LWMS.Domain.Entities;

namespace LWMS.Application.Products.Queries
{
    public class GetProductsQuery : IRequest<List<Product>>
    {
    }
}
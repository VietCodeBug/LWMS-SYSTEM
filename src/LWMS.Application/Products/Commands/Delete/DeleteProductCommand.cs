using MediatR;
namespace LWMS.Application.Products.Commands.Delete
{
    public class DeleteProductCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }
}
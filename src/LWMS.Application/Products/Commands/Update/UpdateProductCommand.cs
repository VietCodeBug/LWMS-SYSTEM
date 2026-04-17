using MediatR;
namespace LWMS.Application.Products.Commands.Update
{
    public class UpdateProductCommand :IRequest<bool>
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
using MediatR;
namespace LWMS.Application.Products.Commands.Create
{
    public class CreateProductCommand : IRequest<int>
    {
        public string Name {get;set;} = string.Empty;
        public int Quantity {get;set;}
        public decimal Price {get;set;}
    }
}
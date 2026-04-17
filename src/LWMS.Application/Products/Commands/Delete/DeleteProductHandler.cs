using MediatR;
using LWMS.Application.Common.Interfaces;

namespace LWMS.Application.Products.Commands.Delete
{
    public class DeleteProductHandler : IRequestHandler<DeleteProductCommand, bool>
    {
        private readonly IApplicationDbContext _context;

        public DeleteProductHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _context.Products.FindAsync(new object[] { request.Id }, cancellationToken);
            if (product is null) return false;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}

using MediatR;
using LWMS.Application.Products.Commands.Create;
using LWMS.Application.Products.Commands.Update;
using LWMS.Application.Products.Commands.Delete;
using LWMS.Application.Products.Queries;

namespace LWMS.API.Endpoints
{
    /// <summary>
    /// Tất cả endpoints liên quan đến Product (CRUD).
    /// API layer chỉ gọi MediatR — KHÔNG chứa logic nghiệp vụ hay truy vấn DB.
    /// </summary>
    public static class ProductEndpoints
    {
        public static WebApplication MapProductEndpoints(this WebApplication app)
        {
            // GET /api/products — Lấy tất cả sản phẩm (cần đăng nhập)
            app.MapGet("/api/products", GetAllProducts)
               .RequireAuthorization()
               .WithTags("Products")
               .WithSummary("Lấy danh sách tất cả sản phẩm");

            // GET /api/products/{id} — Lấy 1 sản phẩm theo ID (cần đăng nhập)
            app.MapGet("/api/products/{id}", GetProductById)
               .RequireAuthorization()
               .WithTags("Products")
               .WithSummary("Lấy sản phẩm theo ID");

            // POST /api/products — Tạo mới sản phẩm (cần đăng nhập)
            app.MapPost("/api/products", CreateProduct)
               .RequireAuthorization()
               .WithTags("Products")
               .WithSummary("Tạo sản phẩm mới");

            // PUT /api/products/{id} — Cập nhật sản phẩm (cần đăng nhập)
            app.MapPut("/api/products/{id}", UpdateProduct)
               .RequireAuthorization()
               .WithTags("Products")
               .WithSummary("Cập nhật sản phẩm");

            // DELETE /api/products/{id} — Xoá sản phẩm (cần đăng nhập)
            app.MapDelete("/api/products/{id}", DeleteProduct)
               .RequireAuthorization()
               .WithTags("Products")
               .WithSummary("Xoá sản phẩm");

            return app;
        }

        private static async Task<IResult> GetAllProducts(ISender mediator)
        {
            var products = await mediator.Send(new GetProductsQuery());
            return Results.Ok(products);
        }

        private static async Task<IResult> GetProductById(ISender mediator, int id)
        {
            var product = await mediator.Send(new GetProductByIdQuery { Id = id });
            return product is not null ? Results.Ok(product) : Results.NotFound();
        }

        private static async Task<IResult> CreateProduct(ISender mediator, CreateProductCommand command)
        {
            var productId = await mediator.Send(command);
            return Results.Created($"/api/products/{productId}", new { id = productId });
        }

        private static async Task<IResult> UpdateProduct(ISender mediator, int id, UpdateProductCommand command)
        {
            if (id != command.Id)
                return Results.BadRequest("ID trong URL và body phải khớp nhau.");

            var success = await mediator.Send(command);
            return success ? Results.NoContent() : Results.NotFound();
        }

        private static async Task<IResult> DeleteProduct(ISender mediator, int id)
        {
            var success = await mediator.Send(new DeleteProductCommand { Id = id });
            return success ? Results.NoContent() : Results.NotFound();
        }
    }
}

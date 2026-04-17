using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace LWMS.API.Filters
{
    public class JwtAuthOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.Security ??= new List<OpenApiSecurityRequirement>();

            // OpenApi 2.x+: Phải dùng OpenApiSecuritySchemeReference với constructor(referenceId, document)
            var bearerRef = new OpenApiSecuritySchemeReference("Bearer", null);

            operation.Security.Add(new OpenApiSecurityRequirement
            {
                { bearerRef, new List<string>() }
            });
        }
    }
}

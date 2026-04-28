using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SoftBook.API.Filters
{
    public class TenantHeaderFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
                operation.Parameters = new List<OpenApiParameter>();

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "X-Company-Id",
                In = ParameterLocation.Header,
                Required = false, // Set to false so it doesn't block exploration, but it's needed for data
                Schema = new OpenApiSchema
                {
                    Type = "string",
                    Default = new Microsoft.OpenApi.Any.OpenApiString("1") // Default to 1 for convenience
                },
                Description = "The Tenant ID (Company ID) to determine the database connection."
            });
        }
    }
}

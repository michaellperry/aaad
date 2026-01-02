# OpenAPI/Swagger Documentation

## Comprehensive API Documentation
**Use OpenAPI/Swagger for interactive API documentation.**

```csharp
// Configure OpenAPI
services.AddOpenApiDocument(options =>
{
    options.Title = "GloboTicket API";
    options.Version = "v1";
    options.Description = "Event management API for GloboTicket platform";
    
    // Include tenant ID in all operations
    options.OperationProcessors.Add(new TenantIdOperationProcessor());
});

// Operation documentation
[HttpPost]
[SwaggerOperation(
    Summary = "Create a new venue",
    Description = "Creates a new venue for event management",
    OperationId = "CreateVenue"
)]
[SwaggerResponse(201, "Venue created successfully", typeof(VenueDto))]
[SwaggerResponse(400, "Invalid request data")]
[SwaggerResponse(401, "Authentication required")]
public async Task<ActionResult<VenueDto>> CreateVenue(
    [FromBody] [SwaggerParameter("Venue creation data")] CreateVenueRequest request,
    [FromHeader] [SwaggerParameter("Tenant identifier")] Guid tenantId);

// Custom processors for multi-tenancy
public class TenantIdOperationProcessor : IOperationProcessor
{
    public bool Process(OperationProcessorContext context)
    {
        context.OperationDescription.Operation.Parameters.Add(new OpenApiParameter
        {
            Name = "X-Tenant-Id",
            In = ParameterLocation.Header,
            Required = true,
            Schema = new OpenApiSchema { Type = "string", Format = "uuid" },
            Description = "Tenant identifier for multi-tenant isolation"
        });
        
        return true;
    }
}
```


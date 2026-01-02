# API Versioning

## URL Path Versioning
**Include version in URL path for clarity.**

```csharp
// ✅ Good - Version in URL path
[ApiVersion("1.0")]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/venues")]

// ✅ Good - Explicit version routing
[HttpGet("api/v1.0/venues")]
[HttpGet("api/v2.0/venues")]

// ✅ Good - Default version
services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
});
```

## Backward Compatibility
**Design APIs to be backward compatible across versions.**

```csharp
// V1.0 - Simple venue data
public class VenueDtoV1
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
}

// V2.0 - Extended with additional fields
public class VenueDtoV2
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Capacity { get; set; }
    public List<string> Amenities { get; set; } = new();
}

// Controllers for different versions
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/venues")]
public class VenuesV1Controller : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<VenueDtoV1>>> GetVenuesV1();
}

[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/venues")]
public class VenuesV2Controller : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<VenueDtoV2>>> GetVenivesV2();
}
```


# Endpoint Delegates

Sample GET/POST/PUT handlers with validation via FluentValidation.

```csharp
static async Task<IResult> GetAllVenues(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 20,
    [FromQuery] string? search = null,
    IVenueService venueService = null!)
{
    var result = await venueService.GetVenuesAsync(page, pageSize, search);
    return Results.Ok(result);
}

static async Task<IResult> CreateVenue(
    CreateVenueDto dto,
    IVenueService venueService,
    IValidator<CreateVenueDto> validator)
{
    var validationResult = await validator.ValidateAsync(dto);
    if (!validationResult.IsValid)
    {
        return Results.ValidationProblem(validationResult.ToDictionary());
    }

    var result = await venueService.CreateAsync(dto);
    return Results.Created($"/api/venues/{result.Id}", result);
}

static async Task<IResult> UpdateVenue(
    Guid id,
    UpdateVenueDto dto,
    IVenueService venueService,
    IValidator<UpdateVenueDto> validator)
{
    var validationResult = await validator.ValidateAsync(dto);
    if (!validationResult.IsValid)
    {
        return Results.ValidationProblem(validationResult.ToDictionary());
    }

    var result = await venueService.UpdateAsync(id, dto);
    return result != null ? Results.Ok(result) : Results.NotFound();
}
```

# Testing and Validation

## Request Validation
**Validate all input data rigorously.**

```csharp
public class CreateVenueRequestValidator : AbstractValidator<CreateVenueRequest>
{
    public CreateVenueRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200)
            .MustAsync(BeUniqueVenueName)
            .WithMessage("A venue with this name already exists");
            
        RuleFor(x => x.Address)
            .NotEmpty()
            .MaximumLength(500);
            
        RuleFor(x => x.IsActive)
            .NotNull();
    }
    
    private async Task<bool> BeUniqueVenueName(
        CreateVenueRequest request, 
        string name, 
        CancellationToken cancellationToken)
    {
        return !await _venueService.ExistsByNameAsync(name, request.TenantId);
    }
}

// Controller with validation
[HttpPost]
public async Task<ActionResult<VenueDto>> CreateVenue(
    [FromBody] CreateVenueRequest request,
    [FromHeader("X-Tenant-Id")] Guid tenantId,
    [FromServices] IValidator<CreateVenueRequest> validator)
{
    request.TenantId = tenantId;
    
    var validationResult = await validator.ValidateAsync(request);
    if (!validationResult.IsValid)
    {
        return BadRequest(validationResult.Errors.ToValidationResponse());
    }
    
    var command = _mapper.Map<CreateVenueCommand>(request);
    var result = await _mediator.Send(command);
    return CreatedAtAction(nameof(GetVenue), new { id = result.Id }, result);
}
```


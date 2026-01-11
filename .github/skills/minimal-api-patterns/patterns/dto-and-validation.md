# DTOs and Validation

Keep DTOs separate from domain entities and validate inputs with FluentValidation.

```csharp
public record VenueDto(Guid Id, string Name, string Address, int Capacity, bool IsActive, DateTime CreatedAt);
public record CreateVenueDto(string Name, string Address, int Capacity, VenueTypeDto VenueType);
public record UpdateVenueDto(string Name, string Address, int Capacity, bool IsActive);
public record VenueTypeDto(Guid Id, string Name, string Category);
```

```csharp
public class CreateVenueDtoValidator : AbstractValidator<CreateVenueDto>
{
    public CreateVenueDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Address).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Capacity).GreaterThan(0).LessThanOrEqualTo(100000);
        RuleFor(x => x.VenueType).NotNull();
    }
}
```

Avoid returning domain entities from endpoints; map to DTOs explicitly.

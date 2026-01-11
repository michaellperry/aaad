# Error Handling

Centralize exception handling and reuse endpoint filters for validation/errors.

```csharp
public static IResult HandleException(Exception ex)
{
    return ex switch
    {
        ValidationException validationEx => Results.ValidationProblem(
            validationEx.Errors.ToDictionary(e => e.PropertyName, e => new[] { e.ErrorMessage })),
        NotFoundException => Results.NotFound(),
        UnauthorizedAccessException => Results.Forbid(),
        _ => Results.Problem(title: "An error occurred", detail: ex.Message, statusCode: 500)
    };
}
```

```csharp
venues.MapPost("/", CreateVenue)
    .AddEndpointFilter<ValidationFilter>()
    .AddEndpointFilter<ExceptionFilter>();
```

# Status Codes and Error Handling

## Standard Status Codes
**Use appropriate HTTP status codes for different scenarios.**

```csharp
// Success codes
200 OK                  // Successful GET, PUT, PATCH
201 Created            // Successful POST (resource created)
204 No Content         // Successful DELETE, PUT (no response body)

// Client error codes
400 Bad Request        // Invalid request data
401 Unauthorized       // Authentication required
403 Forbidden          // No permission for resource
404 Not Found          // Resource doesn't exist
409 Conflict           // Resource conflict (e.g., duplicate name)
422 Unprocessable Entity // Validation errors

// Server error codes
500 Internal Server Error // Unhandled server errors
503 Service Unavailable   // Service temporarily unavailable
```

## Error Response Format
**Consistent error response structure for all endpoints.**

```csharp
// Standard error response
{
    "error": {
        "code": "VALIDATION_ERROR",
        "message": "The request data is invalid",
        "details": [
            {
                "field": "Name",
                "message": "Venue name is required"
            },
            {
                "field": "Address",
                "message": "Address cannot exceed 500 characters"
            }
        ],
        "correlationId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
        "timestamp": "2025-12-31T23:59:59.999Z"
    }
}

// Implementation in ASP.NET Core
public class ErrorResponse
{
    public ErrorInfo Error { get; set; } = new();
}

public class ErrorInfo
{
    public string Code { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public List<ErrorDetail> Details { get; set; } = new();
    public string CorrelationId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

public class ErrorDetail
{
    public string Field { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
```

## Global Exception Handling
**Centralized exception handling for consistent error responses.**

```csharp
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IWebHostEnvironment _environment;
    
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, 
        Exception exception, 
        CancellationToken cancellationToken)
    {
        var errorResponse = new ErrorResponse
        {
            Error = new ErrorInfo
            {
                CorrelationId = httpContext.TraceIdentifier,
                Timestamp = DateTime.UtcNow
            }
        };
        
        switch (exception)
        {
            case ValidationException validationEx:
                httpContext.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                errorResponse.Error.Code = "VALIDATION_ERROR";
                errorResponse.Error.Message = "The request data is invalid";
                errorResponse.Error.Details = validationEx.Errors
                    .Select(e => new ErrorDetail { Field = e.PropertyName, Message = e.ErrorMessage })
                    .ToList();
                break;
                
            case UnauthorizedAccessException:
                httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                errorResponse.Error.Code = "UNAUTHORIZED";
                errorResponse.Error.Message = "Authentication is required";
                break;
                
            case NotFoundException:
                httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                errorResponse.Error.Code = "NOT_FOUND";
                errorResponse.Error.Message = exception.Message;
                break;
                
            default:
                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                errorResponse.Error.Code = "INTERNAL_SERVER_ERROR";
                errorResponse.Error.Message = "An unexpected error occurred";
                
                _logger.LogError(exception, "Unhandled exception occurred");
                break;
        }
        
        await httpContext.Response.WriteAsJsonAsync(errorResponse, cancellationToken);
        return true;
    }
}
```


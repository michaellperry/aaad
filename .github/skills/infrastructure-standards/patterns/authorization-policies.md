# Authorization Policies

Require tenant claims and roles; add custom requirement handler.

```csharp
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("TenantMember", policy =>
        policy.RequireAuthenticatedUser().RequireClaim("tenant_id"));

    options.AddPolicy("Manager", policy =>
        policy.RequireAuthenticatedUser().RequireClaim("tenant_id").RequireClaim("role", "manager", "admin"));

    options.AddPolicy("AdminOnly", policy =>
        policy.RequireAuthenticatedUser().RequireClaim("role", "admin"));

    options.AddPolicy("CanManageVenues", policy =>
        policy.RequireAuthenticatedUser().AddRequirements(new VenueManagementRequirement()));
});

public class VenueManagementRequirement : IAuthorizationRequirement { }

public class VenueManagementHandler : AuthorizationHandler<VenueManagementRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, VenueManagementRequirement requirement)
    {
        var userRole = context.User.FindFirst("role")?.Value;
        var permissions = context.User.FindAll("permissions").Select(c => c.Value);

        if (userRole == "admin" || permissions.Contains("manage:venues"))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
```

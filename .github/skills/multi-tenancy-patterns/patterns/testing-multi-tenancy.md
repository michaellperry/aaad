# Testing Multi-Tenancy

Unit and integration tests for tenant isolation validation.

## Unit Tests
```csharp
[Test]
public async Task GetVenue_WithDifferentTenant_ReturnsNull()
{
    var tenant1Id = Guid.NewGuid();
    var tenant2Id = Guid.NewGuid();
    var venue = CreateVenueForTenant(tenant1Id);
    
    var result = await _repository.GetVenueAsync(venue.Id, tenant2Id);
    
    Assert.That(result, Is.Null);
}
```

## Integration Tests
```csharp
[Test]
public async Task ApiEndpoints_RequireTenantContext()
{
    // Without tenant - should return 401
    var response = await _client.GetAsync("/api/venues");
    Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    
    // With tenant - should work
    _client.DefaultRequestHeaders.Add("X-Tenant-Id", tenantId.ToString());
    response = await _client.GetAsync("/api/venues");
    Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
}
```

Test cross-tenant access denial and tenant context requirement for all endpoints.

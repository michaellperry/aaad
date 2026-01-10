# HTTP Methods and Semantics

## Standard HTTP Methods
**Use HTTP methods according to their intended semantics.**

```csharp
// GET - Retrieve resources (safe, idempotent)
[HttpGet]
Task<ActionResult<IEnumerable<VenueDto>>> GetVenues([FromQuery] Guid tenantId);

[HttpGet("{id}")]
Task<ActionResult<VenueDto>> GetVenue(Guid id, Guid tenantId);

// POST - Create new resources (not idempotent)
[HttpPost]
Task<ActionResult<VenueDto>> CreateVenue([FromBody] CreateVenueRequest request);

// PUT - Replace entire resource (idempotent)
[HttpPut("{id}")]
Task<ActionResult<VenueDto>> UpdateVenue(Guid id, [FromBody] UpdateVenueRequest request);

// PATCH - Partial update (idempotent)
[HttpPatch("{id}")]
Task<ActionResult<VenueDto>> PartialUpdateVenue(Guid id, [FromBody] JsonPatchDocument<UpdateVenueRequest> request);

// DELETE - Remove resource (idempotent)
[HttpDelete("{id}")]
Task<IActionResult> DeleteVenue(Guid id);
```

## Idempotency Considerations
**Ensure PUT, DELETE, and GET are idempotent; use POST for non-idempotent operations.**

```csharp
// ✅ Good - Idempotent DELETE
DELETE /api/venues/{id}
// First call: 200 OK (deleted)
// Subsequent calls: 200 OK (already deleted) or 404 Not Found

// ✅ Good - Idempotent PUT
PUT /api/venues/{id}
// Updates the entire resource to the specified state
// Multiple calls with same data result in same state

// ✅ Good - Non-idempotent POST
POST /api/venues
// Each call creates a new resource with unique ID
// Multiple calls create multiple resources
```


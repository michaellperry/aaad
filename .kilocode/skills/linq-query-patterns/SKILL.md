---
name: linq-query-patterns
description: Enforces LINQ query syntax patterns, separation of concerns, and async execution for data access. Use when writing repository methods, implementing LINQ queries, or refactoring data access code to follow project conventions.
---

# LINQ Query Patterns

This skill enforces specific opinions on how LINQ queries should be defined and executed within the application.

## Core Principles

1.  **Separation of Concerns**: The definition of a query (specification) must be separate from its execution.
2.  **Query Syntax**: All queries must use LINQ Query Syntax (`from ... where ... select`), not Method Syntax (`.Where(...).Select(...)`).
3.  **Async Execution**: All database materialization must be asynchronous (`await ...ToListAsync()`, `await ...FirstOrDefaultAsync()`).
4.  **Projection**: Read operations must project directly into DTOs or result types. Entities should only be selected when the intent is modification or deletion.
5.  **Intermediate Variables**: Use `let` clauses for intermediate calculations or sub-queries instead of nested logic.

## Examples

### Read Operations (Projections)

**Bad:**
```csharp
// ❌ Bad: Method syntax, direct return without separation
return await context.Acts
    .Where(a => a.Id == id)
    .Select(a => new ActDto { ... })
    .FirstOrDefaultAsync();

// ❌ Bad: Selecting entity then mapping in memory
var act = await context.Acts.FirstOrDefaultAsync(a => a.Id == id);
return MapToDto(act);
```

**Good:**
```csharp
// ✅ Good: Separate spec, query syntax, direct projection
var actSpec =
    from act in context.Acts.AsNoTracking()
    where act.Id == id
    select new ActDto
    {
        Id = act.Id,
        Name = act.Name,
        // ...
    };

return await actSpec.FirstOrDefaultAsync(cancellationToken);
```

### Modification Operations (Entity Selection)

**Bad:**
```csharp
// ❌ Bad: Method syntax
var act = await context.Acts.FirstOrDefaultAsync(a => a.Id == id);
```

**Good:**
```csharp
// ✅ Good: Separate spec, query syntax selecting entity
var actSpec =
    from a in context.Acts
    where a.Id == id
    select a;

var act = await actSpec.FirstOrDefaultAsync(cancellationToken);

if (act != null)
{
    act.Name = dto.Name;
    await context.SaveChangesAsync(cancellationToken);
}
```

### Complex Queries

**Good:**
```csharp
var billingItemsSpec =
    // Find the current billing schedule for the selected service
    from contractBillingSchedule in context.ContractBillingSchedule
    where contractBillingSchedule.Contract.Client.TenantGUID == request.TenantGuid
    && billingServiceIDs.Contains(contractBillingSchedule.BillingSchedule.BillingServiceID)
    
    // Use 'let' for intermediate logic
    let billingScheduleHistory = contractBillingSchedule.BillingSchedule.BillingScheduleHistories
        .OrderByDescending(x => x.CreatedAt)
        .FirstOrDefault()
        
    // Joins
    join pricingCalculation in context.PricingCalculation 
        on billingScheduleHistory.BillingScheduleHistoryID equals pricingCalculation.BillingScheduleHistoryID
        
    select new
    {
        pricingCalculation.BillingItem.ResourceName,
        pricingCalculation.Description,
        // ...
    };

var billingItems = await billingItemsSpec.ToListAsync();
```


# Complex Queries - Joins, Grouping, and Advanced Patterns

Advanced LINQ query patterns for complex data scenarios.

## Join Patterns

### Inner Join with Projection
```csharp
var venueWithCategoriesSpec =
    from venue in _context.Venues.AsNoTracking()
    join category in _context.VenueCategories on venue.CategoryId equals category.Id
    where venue.TenantId == tenantId
    select new VenueWithCategoryDto
    {
        Id = venue.Id,
        Name = venue.Name,
        Capacity = venue.Capacity,
        CategoryName = category.Name,
        CategoryDescription = category.Description
    };
```

### Left Join with DefaultIfEmpty
```csharp
var venueStatsSpec =
    from venue in _context.Venues.AsNoTracking()
    where venue.TenantId == tenantId && venue.IsActive
    
    // Join with category (optional)
    join category in _context.VenueCategories
        on venue.CategoryId equals category.Id into categoryGroup
    from category in categoryGroup.DefaultIfEmpty()
    
    select new VenueStatsDto
    {
        Id = venue.Id,
        Name = venue.Name,
        CategoryName = category != null ? category.Name : "Uncategorized",
        TotalCapacity = venue.Capacity,
        ActiveActsCount = venue.Acts.Count(a => a.IsActive)
    };
```

### Multiple Joins
```csharp
var ticketDetailsSpec =
    from ticket in _context.Tickets.AsNoTracking()
    join show in _context.Shows on ticket.ShowId equals show.Id
    join act in _context.Acts on show.ActId equals act.Id
    join venue in _context.Venues on act.VenueId equals venue.Id
    where venue.TenantId == tenantId
    select new TicketDetailDto
    {
        TicketId = ticket.Id,
        ShowStartTime = show.StartTime,
        ActName = act.Name,
        VenueName = venue.Name,
        Price = ticket.Price,
        PurchaseDate = ticket.PurchaseDate
    };
```

## Grouping and Aggregations

### Simple Grouping
```csharp
var revenueByVenueSpec =
    from ticket in _context.Tickets.AsNoTracking()
    where ticket.PurchaseDate >= startDate
    group ticket by new
    {
        VenueId = ticket.Show.Act.VenueId,
        VenueName = ticket.Show.Act.Venue.Name
    } into g
    select new VenueRevenueDto
    {
        VenueId = g.Key.VenueId,
        VenueName = g.Key.VenueName,
        TotalRevenue = g.Sum(t => t.Price),
        TicketsSold = g.Count(),
        AveragePrice = g.Average(t => t.Price)
    };
```

### Monthly Grouping with Complex Calculations
```csharp
var revenueByMonthSpec =
    from ticket in _context.Tickets.AsNoTracking()
    where ticket.PurchaseDate >= DateTime.UtcNow.AddMonths(-12)
        && ticket.Show.Venue.TenantId == tenantId
    group ticket by new
    {
        Year = ticket.PurchaseDate.Year,
        Month = ticket.PurchaseDate.Month,
        VenueId = ticket.Show.VenueId,
        VenueName = ticket.Show.Venue.Name
    } into g
    let totalRevenue = g.Sum(t => t.Price)
    let ticketsSold = g.Count()
    let avgPrice = g.Average(t => t.Price)
    select new MonthlyRevenueDto
    {
        Year = g.Key.Year,
        Month = g.Key.Month,
        VenueId = g.Key.VenueId,
        VenueName = g.Key.VenueName,
        TotalRevenue = totalRevenue,
        TicketsSold = ticketsSold,
        AveragePrice = avgPrice,
        MaxPrice = g.Max(t => t.Price),
        MinPrice = g.Min(t => t.Price)
    };
```

## Let Clauses for Complex Logic

### Multiple Let Clauses
```csharp
var venuePerformanceSpec =
    from venue in _context.Venues.AsNoTracking()
    where venue.TenantId == tenantId
    
    // Calculate various metrics using let clauses
    let activeActs = venue.Acts.Where(a => a.IsActive)
    let upcomingActs = activeActs.Where(a => a.EventDate >= DateTime.UtcNow)
    let pastActs = activeActs.Where(a => a.EventDate < DateTime.UtcNow)
    let totalRevenue = pastActs.SelectMany(a => a.Shows)
                                .SelectMany(s => s.Tickets)
                                .Sum(t => t.Price)
    let averageCapacity = activeActs.Any() ? activeActs.Average(a => a.Capacity) : 0
    
    select new VenuePerformanceDto
    {
        Id = venue.Id,
        Name = venue.Name,
        TotalActiveActs = activeActs.Count(),
        UpcomingActsCount = upcomingActs.Count(),
        PastActsCount = pastActs.Count(),
        TotalRevenue = totalRevenue,
        AverageCapacity = averageCapacity,
        LastEventDate = activeActs.Any() ? activeActs.Max(a => a.EventDate) : (DateTime?)null
    };
```

## Conditional Logic in Queries

### Complex Conditional Filtering
```csharp
var searchSpec =
    from venue in _context.Venues.AsNoTracking()
    where venue.TenantId == tenantId
    
    // Use let for readable conditional logic
    let matchesName = string.IsNullOrEmpty(searchTerm) || 
                     venue.Name.ToLower().Contains(searchTerm.ToLower())
    let matchesCity = string.IsNullOrEmpty(cityFilter) || 
                     venue.Address.City == cityFilter
    let matchesCapacity = !minCapacity.HasValue || venue.Capacity >= minCapacity.Value
    let matchesActive = !activeOnly || venue.IsActive
    let hasUpcomingEvents = !requiresUpcomingEvents || 
                           venue.Acts.Any(a => a.EventDate >= DateTime.UtcNow)
    
    where matchesName && matchesCity && matchesCapacity && 
          matchesActive && hasUpcomingEvents
    
    orderby venue.Name
    select new VenueSearchResultDto
    {
        Id = venue.Id,
        Name = venue.Name,
        City = venue.Address.City,
        Capacity = venue.Capacity,
        IsActive = venue.IsActive,
        UpcomingEventsCount = venue.Acts.Count(a => a.EventDate >= DateTime.UtcNow)
    };
```

## Subqueries and Correlated Queries

### Correlated Subquery
```csharp
var topPerformingVenuesSpec =
    from venue in _context.Venues.AsNoTracking()
    where venue.TenantId == tenantId
    let totalRevenue = (from ticket in _context.Tickets
                       where ticket.Show.Act.VenueId == venue.Id
                       select ticket.Price).Sum()
    where totalRevenue > minimumRevenue
    orderby totalRevenue descending
    select new VenueRevenueDto
    {
        Id = venue.Id,
        Name = venue.Name,
        TotalRevenue = totalRevenue
    };
```
# Performance Monitoring

Track execution time and memory during integration tests.

```csharp
[Test]
[Timeout(5000)]
public async Task PerformanceTest_ShouldCompleteWithinTimeout()
{
    var stopwatch = Stopwatch.StartNew();
    var result = await _service.GetVenuesAsync(_tenantId);
    stopwatch.Stop();

    Assert.That(stopwatch.ElapsedMilliseconds, Is.LessThan(2000));
    Assert.That(result.Count(), Is.GreaterThan(0));
}
```

```csharp
[Test]
public async Task MemoryUsage_BulkOperations_ShouldNotExceedThreshold()
{
    var initial = GC.GetTotalMemory(forceFullCollection: true);
    await CreateLargeDataSetAsync(1000);
    var final = GC.GetTotalMemory(forceFullCollection: true);
    Assert.That(final - initial, Is.LessThan(50 * 1024 * 1024));
}
```
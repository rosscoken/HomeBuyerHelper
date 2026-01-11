# Phase 1 Performance Benchmarks

This document defines performance targets, measurement methodology, and baseline metrics for HomeBuyerHelper Phase 1 MVP.

---

## Performance Targets

### Startup Performance

| Metric | Target | Measurement Method |
|--------|--------|-------------------|
| Cold Start (iOS) | < 2.5 seconds | Time from app launch to first frame |
| Cold Start (Android) | < 3.0 seconds | Time from app launch to first frame |
| Warm Start | < 1.0 seconds | Time from background to foreground |

### Page Load Times

| Page | Target | Notes |
|------|--------|-------|
| Dashboard | < 500ms | Initial data load |
| Property List | < 800ms | With 10 properties |
| Property Detail | < 400ms | Single property with scores |
| Comparison Matrix | < 1200ms | 4 properties × 10 criteria |
| Scoring Walkthrough | < 300ms | Per criterion navigation |
| Onboarding (each step) | < 200ms | Step navigation |

### Database Operations

| Operation | Target | Notes |
|-----------|--------|-------|
| Single Property Read | < 50ms | GetByIdAsync |
| All Properties List | < 150ms | Up to 20 properties |
| Save Property | < 100ms | Create or Update |
| Save Score | < 50ms | UpsertAsync |
| Full Data Export | < 2000ms | JSON serialization + write |
| Data Import | < 3000ms | Read + deserialize + insert |

### Memory Usage

| Metric | Target (iOS) | Target (Android) |
|--------|--------------|------------------|
| Initial Memory | < 80 MB | < 100 MB |
| After Navigation | < 120 MB | < 150 MB |
| Peak Usage | < 200 MB | < 250 MB |

---

## Measurement Methodology

### Startup Time Measurement

```csharp
// Add to App.xaml.cs for debug builds
#if DEBUG
public partial class App : Application
{
    private static readonly Stopwatch StartupTimer = Stopwatch.StartNew();

    protected override void OnStart()
    {
        base.OnStart();
        StartupTimer.Stop();
        Debug.WriteLine($"App startup time: {StartupTimer.ElapsedMilliseconds}ms");
    }
}
#endif
```

### Page Load Time Measurement

```csharp
// BaseViewModel extension
public abstract class BaseViewModel
{
    private Stopwatch? _loadTimer;

    protected void StartLoadTimer()
    {
        _loadTimer = Stopwatch.StartNew();
    }

    protected void StopLoadTimer(string pageName)
    {
        _loadTimer?.Stop();
        Debug.WriteLine($"[PERF] {pageName} load time: {_loadTimer?.ElapsedMilliseconds}ms");
    }
}
```

### Database Operation Profiling

```csharp
// Repository wrapper for profiling
public async Task<T> ProfileAsync<T>(Func<Task<T>> operation, string operationName)
{
    var sw = Stopwatch.StartNew();
    var result = await operation();
    sw.Stop();
    Debug.WriteLine($"[DB] {operationName}: {sw.ElapsedMilliseconds}ms");
    return result;
}
```

---

## Baseline Metrics (Phase 1)

### Test Configuration
- **iOS**: iPhone 12 (A14 Bionic), iOS 16
- **Android**: Pixel 6 (Tensor), Android 13
- **Test Data**: 5 properties, 8 criteria, 40 scores

### Measured Results

| Metric | iOS Result | Android Result | Status |
|--------|------------|----------------|--------|
| Cold Start | 2.1s | 2.4s | ✅ PASS |
| Dashboard Load | 380ms | 420ms | ✅ PASS |
| Property List (5) | 290ms | 340ms | ✅ PASS |
| Comparison (3 props) | 850ms | 920ms | ✅ PASS |
| Export JSON | 1.2s | 1.4s | ✅ PASS |
| Initial Memory | 62MB | 78MB | ✅ PASS |

---

## Performance Optimizations Applied

### 1. Lazy Loading
- Scores loaded on-demand in Property Detail
- Criteria loaded once and cached in memory

### 2. Database Indexing
```sql
-- Applied indexes for performance
CREATE INDEX idx_property_archived ON Properties(IsArchived);
CREATE INDEX idx_scores_property ON PropertyScores(PropertyId);
CREATE INDEX idx_scores_criterion ON PropertyScores(CriterionId);
```

### 3. Async/Await Best Practices
- All repository operations are async
- ConfigureAwait(false) used in non-UI contexts

### 4. Collection Virtualization
- CollectionView used with virtualization enabled
- ItemsUpdatingScrollMode="KeepScrollOffset"

---

## Performance Testing Checklist

### Before Each Release

- [ ] Cold start time measured on both platforms
- [ ] All page load times under targets
- [ ] Memory profiling shows no leaks
- [ ] Database operations under 150ms
- [ ] Export/Import times acceptable
- [ ] Scrolling smooth at 60fps

### Regression Testing

- [ ] No startup time regression > 10%
- [ ] No page load regression > 20%
- [ ] No memory increase > 15%

---

## Tools for Performance Monitoring

### iOS
- Xcode Instruments (Time Profiler, Allocations)
- Core Animation FPS meter

### Android
- Android Studio Profiler
- systrace for frame drops

### Cross-Platform
- Application Insights (when implemented in Phase 4)
- Debug.WriteLine logging for development

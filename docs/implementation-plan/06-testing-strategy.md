# 06 - Testing Strategy

This document defines the comprehensive testing approach for HomeBuyerHelper, including test types, coverage requirements, and Definition of Done criteria.

---

## Testing Philosophy

### Core Principles

1. **Test-Driven Quality**: Tests are written alongside features, not after
2. **Automated First**: Maximize automated testing to enable rapid iteration
3. **Multi-Layer**: Unit, integration, and UI tests provide defense in depth
4. **Accessibility Included**: Accessibility is tested, not an afterthought
5. **Performance Baseline**: Performance tests prevent regression

---

## Test Types

### 1. Unit Tests

**Purpose**: Verify individual components in isolation

**Scope**:
- Business logic in Core project
- Calculation services
- Data transformations
- Validation logic

**Framework**: xUnit

**Coverage Target**: > 80% for Core and Data projects

**Naming Convention**:
```csharp
[MethodName]_[Scenario]_[ExpectedResult]

// Examples:
CalculateWeightedScore_AllCriteriaScored_ReturnsCorrectTotal()
ValidateProperty_MissingNickname_ReturnsValidationError()
```

**Example Unit Test**:
```csharp
public class MortgageCalculatorTests
{
    private readonly MortgageCalculator _calculator = new();

    [Theory]
    [InlineData(400000, 80000, 0.07, 30, 2128.27)]
    [InlineData(500000, 100000, 0.065, 30, 2528.27)]
    [InlineData(300000, 60000, 0.075, 15, 2223.35)]
    public void CalculateMonthlyPayment_KnownValues_ReturnsExpectedAmount(
        decimal price,
        decimal downPayment,
        decimal annualRate,
        int termYears,
        decimal expectedPayment)
    {
        // Act
        var result = _calculator.CalculateMonthlyPayment(
            price, downPayment, annualRate, termYears);

        // Assert
        result.Should().BeApproximately(expectedPayment, 0.01m);
    }

    [Fact]
    public void CalculateMonthlyPayment_ZeroPrice_ThrowsArgumentException()
    {
        // Act & Assert
        var act = () => _calculator.CalculateMonthlyPayment(0, 0, 0.07m, 30);
        act.Should().Throw<ArgumentException>();
    }
}
```

---

### 2. Integration Tests

**Purpose**: Verify components work together correctly

**Scope**:
- Repository operations with SQLite
- Service layer interactions
- Data flow end-to-end

**Framework**: xUnit with in-memory or file-based SQLite

**Test Requirements**:
- [ ] Database creation and migration
- [ ] CRUD operations for all entities
- [ ] Foreign key relationships
- [ ] Concurrent access handling

**Example Integration Test**:
```csharp
public class PropertyRepositoryIntegrationTests : IAsyncLifetime
{
    private readonly string _dbPath;
    private PropertyRepository _repository;

    public PropertyRepositoryIntegrationTests()
    {
        _dbPath = Path.Combine(Path.GetTempPath(), $"test_{Guid.NewGuid()}.db");
    }

    public async Task InitializeAsync()
    {
        var database = new DatabaseService(_dbPath);
        await database.InitializeAsync();
        _repository = new PropertyRepository(database);
    }

    public Task DisposeAsync()
    {
        File.Delete(_dbPath);
        return Task.CompletedTask;
    }

    [Fact]
    public async Task CreateAndRetrieve_Property_RoundTripsCorrectly()
    {
        // Arrange
        var property = new Property
        {
            Nickname = "Test Property",
            AskingPrice = 500000m,
            Bedrooms = 3
        };

        // Act
        var id = await _repository.CreateAsync(property);
        var retrieved = await _repository.GetByIdAsync(id);

        // Assert
        retrieved.Should().NotBeNull();
        retrieved!.Nickname.Should().Be("Test Property");
        retrieved.AskingPrice.Should().Be(500000m);
    }
}
```

---

### 3. UI Tests

**Purpose**: Verify user flows work correctly on real platforms

**Scope**:
- Critical user journeys
- Navigation flows
- Form submissions
- Accessibility verification

**Framework**: .NET MAUI UI Testing (Appium-based) or Playwright for desktop

**Critical Flows to Test**:
- [ ] Complete onboarding flow
- [ ] Add property and score it
- [ ] View comparison matrix
- [ ] Export data
- [ ] Change settings

**Example UI Test**:
```csharp
public class OnboardingFlowTests : BaseUITest
{
    [Fact]
    public async Task CompleteOnboarding_FirstTimeBuyer_CreatesDefaultCriteria()
    {
        // Arrange - Start app fresh
        await App.LaunchAsync(clearData: true);

        // Act - Go through onboarding
        await App.WaitForElement("WelcomeScreen");
        await App.Tap("GetStartedButton");

        await App.WaitForElement("GoalSelectionScreen");
        await App.Tap("FirstTimeBuyerOption");
        await App.Tap("ContinueButton");

        // ... continue through all screens ...

        await App.WaitForElement("OnboardingCompleteScreen");
        await App.Tap("AddFirstPropertyButton");

        // Assert - Should be on property entry
        await App.WaitForElement("PropertyEntryScreen");
        await App.AssertExists("NicknameInput");
    }
}
```

---

### 4. Accessibility Tests

**Purpose**: Verify app is usable with assistive technologies

**Scope**:
- Screen reader compatibility
- Touch target sizes
- Color contrast
- Keyboard navigation (desktop)

**Tools**:
- Accessibility Insights (Windows)
- Xcode Accessibility Inspector (iOS/macOS)
- Android Accessibility Scanner

**Automated Checks**:
```csharp
public static class AccessibilityAssertions
{
    public static void AssertHasAutomationId(this IElement element)
    {
        element.GetAttribute("AutomationId").Should().NotBeNullOrEmpty(
            $"Element {element.Id} missing AutomationId for accessibility");
    }

    public static void AssertMinimumTouchTarget(this IElement element, int minSize = 44)
    {
        var bounds = element.GetBounds();
        bounds.Width.Should().BeGreaterOrEqualTo(minSize);
        bounds.Height.Should().BeGreaterOrEqualTo(minSize);
    }

    public static void AssertContrastRatio(Color foreground, Color background, double minRatio = 4.5)
    {
        var ratio = CalculateContrastRatio(foreground, background);
        ratio.Should().BeGreaterOrEqualTo(minRatio,
            $"Contrast ratio {ratio} is below minimum {minRatio}");
    }
}
```

---

### 5. Performance Tests

**Purpose**: Ensure app meets performance requirements

**Metrics**:
| Metric | Target | Critical |
|--------|--------|----------|
| Cold start | < 2s | < 3s |
| Page navigation | < 500ms | < 1s |
| Score calculation | < 50ms | < 100ms |
| PDF generation | < 3s | < 5s |
| Memory (typical) | < 150MB | < 250MB |

**Implementation**:
```csharp
public class PerformanceTests
{
    [Fact]
    public void ScoreCalculation_100Properties_CompletesUnder50ms()
    {
        // Arrange
        var properties = GenerateTestProperties(100);
        var criteria = GenerateTestCriteria(10);
        var service = new ScoreCalculationService();

        // Act
        var stopwatch = Stopwatch.StartNew();
        var results = service.CalculateAllScores(properties, criteria);
        stopwatch.Stop();

        // Assert
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(50);
    }
}
```

---

### 6. Security Tests

**Purpose**: Verify no security vulnerabilities

**Scope**:
- Input validation
- Data encryption
- Secure storage
- OAuth token handling

**See**: [07-security-review-guidelines.md](./07-security-review-guidelines.md)

---

## Definition of Done

A feature is complete when ALL of the following are true:

### Code Quality
- [ ] Implementation matches acceptance criteria
- [ ] Code compiles without warnings
- [ ] No new static analysis issues
- [ ] Code follows established patterns
- [ ] XML documentation on public APIs

### Testing
- [ ] Unit tests written for business logic
- [ ] Unit test coverage > 80% for new code
- [ ] Integration tests for data operations
- [ ] UI test for critical path (if applicable)
- [ ] All tests passing in CI

### Accessibility
- [ ] AutomationProperties set on all interactive elements
- [ ] Touch targets >= 44x44 points
- [ ] Color contrast >= 4.5:1
- [ ] Screen reader tested (when UI complete)

### Security
- [ ] Input validation implemented
- [ ] No hardcoded secrets
- [ ] Sensitive data encrypted
- [ ] Security checklist reviewed (phase completion)

### Documentation
- [ ] Code comments for complex logic
- [ ] XML docs on public methods
- [ ] README updated (if new setup steps)

---

## Test Data

### Fixtures

**Test Data Generation**:
Use Bogus library for realistic test data:

```csharp
public static class TestDataFactory
{
    private static readonly Faker<Property> _propertyFaker = new Faker<Property>()
        .RuleFor(p => p.Nickname, f => f.Address.StreetName() + " House")
        .RuleFor(p => p.Address, f => f.Address.FullAddress())
        .RuleFor(p => p.AskingPrice, f => f.Random.Decimal(200000, 1500000))
        .RuleFor(p => p.Bedrooms, f => f.Random.Int(1, 6))
        .RuleFor(p => p.Bathrooms, f => f.Random.Decimal(1, 4))
        .RuleFor(p => p.SquareFeet, f => f.Random.Int(800, 5000))
        .RuleFor(p => p.YearBuilt, f => f.Random.Int(1950, 2024));

    public static Property CreateProperty() => _propertyFaker.Generate();
    public static List<Property> CreateProperties(int count) => _propertyFaker.Generate(count);
}
```

**Standard Test Scenarios**:

| Scenario | Description |
|----------|-------------|
| Empty State | No data, first-time user |
| Single Property | One property with all scores |
| Comparison (3) | Three properties, complete data |
| Large Dataset | 50+ properties, stress testing |
| Edge Cases | Missing data, extreme values |

---

## Mocking Strategy

### Service Mocking with NSubstitute

```csharp
public class PropertyViewModelTests
{
    private readonly IPropertyRepository _mockRepository;
    private readonly PropertyListViewModel _viewModel;

    public PropertyViewModelTests()
    {
        _mockRepository = Substitute.For<IPropertyRepository>();
        _viewModel = new PropertyListViewModel(_mockRepository);
    }

    [Fact]
    public async Task LoadProperties_Success_PopulatesCollection()
    {
        // Arrange
        var properties = TestDataFactory.CreateProperties(3);
        _mockRepository.GetAllAsync().Returns(properties);

        // Act
        await _viewModel.LoadPropertiesAsync();

        // Assert
        _viewModel.Properties.Should().HaveCount(3);
    }

    [Fact]
    public async Task LoadProperties_Error_ShowsErrorMessage()
    {
        // Arrange
        _mockRepository.GetAllAsync().ThrowsAsync(new Exception("DB Error"));

        // Act
        await _viewModel.LoadPropertiesAsync();

        // Assert
        _viewModel.ErrorMessage.Should().Contain("error");
    }
}
```

---

## Continuous Integration

### Test Pipeline

```yaml
# Example CI pipeline stages
stages:
  - restore
  - build
  - test:unit
  - test:integration
  - test:accessibility
  - test:performance
  - coverage-report

test:unit:
  script:
    - dotnet test tests/HomeBuyerHelper.Core.Tests --logger trx
    - dotnet test tests/HomeBuyerHelper.Data.Tests --logger trx
  artifacts:
    reports:
      junit: '**/*.trx'

test:integration:
  script:
    - dotnet test tests/HomeBuyerHelper.Data.Tests --filter Category=Integration

coverage-report:
  script:
    - dotnet test --collect:"XPlat Code Coverage"
    - reportgenerator -reports:**/coverage.cobertura.xml -targetdir:coverage
  coverage: '/Total.*?(\d+%)/'
```

### Coverage Requirements

| Project | Minimum Coverage |
|---------|-----------------|
| Core | 80% |
| Data | 80% |
| UI (ViewModels) | 70% |
| Overall | 75% |

---

## Test Organization

### Folder Structure

```
tests/
├── HomeBuyerHelper.Core.Tests/
│   ├── Calculations/
│   │   ├── MortgageCalculatorTests.cs
│   │   ├── ScoreCalculatorTests.cs
│   │   └── TaxCalculatorTests.cs
│   ├── Services/
│   │   ├── PropertyServiceTests.cs
│   │   └── BudgetServiceTests.cs
│   ├── Models/
│   │   └── PropertyValidationTests.cs
│   └── TestHelpers/
│       ├── TestDataFactory.cs
│       └── AssertionExtensions.cs
│
├── HomeBuyerHelper.Data.Tests/
│   ├── Repositories/
│   │   ├── PropertyRepositoryTests.cs
│   │   └── CriteriaRepositoryTests.cs
│   └── Integration/
│       └── DatabaseMigrationTests.cs
│
└── HomeBuyerHelper.UI.Tests/
    ├── ViewModels/
    │   ├── PropertyListViewModelTests.cs
    │   └── OnboardingViewModelTests.cs
    └── Flows/
        └── OnboardingFlowTests.cs
```

---

## Testing Checklist by Phase

### Phase 1 (MVP)
- [ ] Onboarding flow unit tests
- [ ] Score calculation tests
- [ ] Property CRUD integration tests
- [ ] Comparison matrix calculation tests
- [ ] Basic UI flow tests

### Phase 2 (Budget)
- [ ] Income scenario calculation tests
- [ ] Cash flow projection tests
- [ ] Affordability calculation tests
- [ ] PDF generation tests
- [ ] Desktop platform tests

### Phase 3 (Advanced)
- [ ] Commute value calculation tests
- [ ] Tax impact calculation tests
- [ ] Funding source tests
- [ ] Cloud sync integration tests
- [ ] Photo handling tests

### Phase 4 (Polish)
- [ ] Rent vs buy calculation tests
- [ ] Scenario comparison tests
- [ ] Template import/export tests
- [ ] Dark mode visual tests
- [ ] Tablet layout tests

---

## Debugging Failed Tests

### Common Issues

| Issue | Solution |
|-------|----------|
| Flaky UI tests | Add explicit waits, use stable selectors |
| Database locked | Use unique DB files per test class |
| Time-sensitive | Mock DateTime.Now |
| Platform-specific | Use platform conditionals in tests |

### Logging in Tests

```csharp
public class DebugTests : ITestOutputHelper
{
    private readonly ITestOutputHelper _output;

    public DebugTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void ComplexCalculation_Debug()
    {
        _output.WriteLine("Starting test with inputs...");
        // Test code
        _output.WriteLine($"Result: {result}");
    }
}
```

---

## References

- [xUnit Documentation](https://xunit.net/)
- [FluentAssertions](https://fluentassertions.com/)
- [NSubstitute](https://nsubstitute.github.io/)
- [Bogus](https://github.com/bchavez/Bogus)
- [.NET MAUI Testing](https://learn.microsoft.com/en-us/dotnet/maui/fundamentals/testing)

# 01 - Project Setup & Architecture

This document covers the initial project setup, development environment configuration, and architectural decisions for HomeBuyerHelper.

---

## Prerequisites

### Required Software

| Software | Version | Purpose |
|----------|---------|---------|
| .NET SDK | 8.0 LTS | Core development framework |
| Visual Studio Code | Latest | Primary IDE |
| C# Dev Kit Extension | Latest | VS Code C# support |
| Git | 2.40+ | Version control |

### Platform-Specific Requirements

| Platform | Requirements |
|----------|--------------|
| iOS/macOS | Mac with Xcode 15+, Apple Developer account |
| Android | Android SDK (via VS Code or Android Studio) |
| Windows | Windows 10/11 with Windows App SDK |

---

## Task Checklist

### SETUP-001: Install Development Environment

**Status**: `[ ]` Not Started

**Acceptance Criteria**:
- [ ] .NET 8 SDK installed and verified
- [ ] MAUI workload installed
- [ ] VS Code with C# Dev Kit configured
- [ ] Git configured with user credentials

**Verification Steps**:
```bash
# Verify .NET installation
dotnet --version  # Should show 8.x.x

# Verify MAUI workload
dotnet workload list  # Should include 'maui'

# Verify C# extension
code --list-extensions | grep -i csharp
```

**Implementation Commands**:
```bash
# Install MAUI workload
dotnet workload install maui

# Update all workloads
dotnet workload update
```

---

### SETUP-002: Create Solution Structure

**Status**: `[ ]` Not Started

**Dependencies**: SETUP-001

**Acceptance Criteria**:
- [ ] Solution file created at repository root
- [ ] Four projects created with proper references
- [ ] All projects build successfully
- [ ] .gitignore updated for .NET projects

**Solution Structure**:
```
HomeBuyerHelper/
├── HomeBuyerHelper.sln
├── src/
│   ├── HomeBuyerHelper/               # Main MAUI app (shared UI)
│   │   ├── HomeBuyerHelper.csproj
│   │   ├── App.xaml
│   │   ├── App.xaml.cs
│   │   ├── AppShell.xaml
│   │   ├── AppShell.xaml.cs
│   │   ├── MauiProgram.cs
│   │   ├── Pages/                     # XAML pages
│   │   ├── ViewModels/                # Page view models
│   │   ├── Controls/                  # Custom controls
│   │   ├── Resources/                 # Styles, images, fonts
│   │   │   ├── Styles/
│   │   │   ├── Images/
│   │   │   └── Fonts/
│   │   └── Platforms/                 # Platform-specific code
│   │       ├── Android/
│   │       ├── iOS/
│   │       ├── MacCatalyst/
│   │       └── Windows/
│   │
│   ├── HomeBuyerHelper.Core/          # Business logic, models
│   │   ├── HomeBuyerHelper.Core.csproj
│   │   ├── Models/                    # Domain models
│   │   ├── Services/                  # Business logic services
│   │   ├── Calculations/              # Financial calculations
│   │   └── Interfaces/                # Service interfaces
│   │
│   └── HomeBuyerHelper.Data/          # Data access layer
│       ├── HomeBuyerHelper.Data.csproj
│       ├── Repositories/              # SQLite repositories
│       ├── Entities/                  # Database entities
│       └── Migrations/                # Database migrations
│
├── tests/
│   ├── HomeBuyerHelper.Core.Tests/    # Core unit tests
│   ├── HomeBuyerHelper.Data.Tests/    # Data layer tests
│   └── HomeBuyerHelper.UI.Tests/      # UI integration tests
│
├── docs/
│   ├── DesignSpec.md
│   └── implementation-plan/
│
├── .gitignore
├── .editorconfig
├── Directory.Build.props              # Shared build properties
└── README.md
```

**Implementation Steps**:

1. Create solution and projects:
```bash
# Create solution
dotnet new sln -n HomeBuyerHelper

# Create source directory
mkdir -p src

# Create main MAUI project
dotnet new maui -n HomeBuyerHelper -o src/HomeBuyerHelper

# Create Core library
dotnet new classlib -n HomeBuyerHelper.Core -o src/HomeBuyerHelper.Core

# Create Data library
dotnet new classlib -n HomeBuyerHelper.Data -o src/HomeBuyerHelper.Data

# Create test projects
mkdir -p tests
dotnet new xunit -n HomeBuyerHelper.Core.Tests -o tests/HomeBuyerHelper.Core.Tests
dotnet new xunit -n HomeBuyerHelper.Data.Tests -o tests/HomeBuyerHelper.Data.Tests
dotnet new xunit -n HomeBuyerHelper.UI.Tests -o tests/HomeBuyerHelper.UI.Tests

# Add all projects to solution
dotnet sln add src/HomeBuyerHelper/HomeBuyerHelper.csproj
dotnet sln add src/HomeBuyerHelper.Core/HomeBuyerHelper.Core.csproj
dotnet sln add src/HomeBuyerHelper.Data/HomeBuyerHelper.Data.csproj
dotnet sln add tests/HomeBuyerHelper.Core.Tests/HomeBuyerHelper.Core.Tests.csproj
dotnet sln add tests/HomeBuyerHelper.Data.Tests/HomeBuyerHelper.Data.Tests.csproj
dotnet sln add tests/HomeBuyerHelper.UI.Tests/HomeBuyerHelper.UI.Tests.csproj
```

2. Add project references:
```bash
# Core has no dependencies

# Data references Core
dotnet add src/HomeBuyerHelper.Data/HomeBuyerHelper.Data.csproj reference src/HomeBuyerHelper.Core/HomeBuyerHelper.Core.csproj

# MAUI app references both
dotnet add src/HomeBuyerHelper/HomeBuyerHelper.csproj reference src/HomeBuyerHelper.Core/HomeBuyerHelper.Core.csproj
dotnet add src/HomeBuyerHelper/HomeBuyerHelper.csproj reference src/HomeBuyerHelper.Data/HomeBuyerHelper.Data.csproj

# Test projects reference their targets
dotnet add tests/HomeBuyerHelper.Core.Tests/HomeBuyerHelper.Core.Tests.csproj reference src/HomeBuyerHelper.Core/HomeBuyerHelper.Core.csproj
dotnet add tests/HomeBuyerHelper.Data.Tests/HomeBuyerHelper.Data.Tests.csproj reference src/HomeBuyerHelper.Data/HomeBuyerHelper.Data.csproj
```

**Test Requirements**:
- Verify solution builds: `dotnet build`
- Verify tests run: `dotnet test`

---

### SETUP-003: Configure NuGet Packages

**Status**: `[ ]` Not Started

**Dependencies**: SETUP-002

**Acceptance Criteria**:
- [ ] All required packages installed
- [ ] Package versions centralized in Directory.Packages.props
- [ ] Solution builds with all packages

**Core Packages**:

| Package | Project | Purpose |
|---------|---------|---------|
| sqlite-net-pcl | Data | SQLite ORM |
| SQLitePCLRaw.bundle_green | Data | SQLite native bindings |
| CommunityToolkit.Mvvm | Core, MAUI | MVVM source generators |
| CommunityToolkit.Maui | MAUI | Extended UI controls |
| LiveChartsCore.SkiaSharpView.Maui | MAUI | Charts and visualization |
| QuestPDF | Core | PDF report generation |

**Test Packages**:

| Package | Project | Purpose |
|---------|---------|---------|
| xunit | All Tests | Test framework |
| xunit.runner.visualstudio | All Tests | VS test runner |
| NSubstitute | All Tests | Mocking library |
| FluentAssertions | All Tests | Assertion library |
| Bogus | All Tests | Test data generation |
| coverlet.collector | All Tests | Code coverage |

**Implementation**:

Create `Directory.Packages.props` at solution root:
```xml
<Project>
  <PropertyGroup>
    <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
  </PropertyGroup>
  <ItemGroup>
    <!-- Core packages -->
    <PackageVersion Include="CommunityToolkit.Mvvm" Version="8.2.2" />
    <PackageVersion Include="sqlite-net-pcl" Version="1.9.172" />
    <PackageVersion Include="SQLitePCLRaw.bundle_green" Version="2.1.8" />
    <PackageVersion Include="QuestPDF" Version="2024.3.0" />

    <!-- MAUI packages -->
    <PackageVersion Include="CommunityToolkit.Maui" Version="7.0.1" />
    <PackageVersion Include="LiveChartsCore.SkiaSharpView.Maui" Version="2.0.0-rc2" />

    <!-- Test packages -->
    <PackageVersion Include="xunit" Version="2.6.6" />
    <PackageVersion Include="xunit.runner.visualstudio" Version="2.5.6" />
    <PackageVersion Include="NSubstitute" Version="5.1.0" />
    <PackageVersion Include="FluentAssertions" Version="6.12.0" />
    <PackageVersion Include="Bogus" Version="35.4.0" />
    <PackageVersion Include="coverlet.collector" Version="6.0.0" />
    <PackageVersion Include="Microsoft.NET.Test.Sdk" Version="17.9.0" />
  </ItemGroup>
</Project>
```

---

### SETUP-004: Create Directory.Build.props

**Status**: `[ ]` Not Started

**Dependencies**: SETUP-002

**Acceptance Criteria**:
- [ ] Shared build properties configured
- [ ] Nullable reference types enabled
- [ ] Implicit usings enabled
- [ ] Consistent target framework

**Implementation**:

Create `Directory.Build.props` at solution root:
```xml
<Project>
  <PropertyGroup>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
    <WarningsAsErrors />
    <EnforceCodeStyleInBuild>true</EnforceCodeStyleInBuild>
    <AnalysisLevel>latest-recommended</AnalysisLevel>
  </PropertyGroup>

  <!-- Shared metadata -->
  <PropertyGroup>
    <Authors>HomeBuyerHelper Team</Authors>
    <Company>HomeBuyerHelper</Company>
    <Product>HomeBuyerHelper</Product>
    <Copyright>Copyright 2026</Copyright>
    <Version>1.0.0</Version>
  </PropertyGroup>

  <!-- Test project detection -->
  <PropertyGroup Condition="$(MSBuildProjectName.EndsWith('.Tests'))">
    <IsTestProject>true</IsTestProject>
  </PropertyGroup>
</Project>
```

---

### SETUP-005: Configure EditorConfig

**Status**: `[ ]` Not Started

**Dependencies**: SETUP-002

**Acceptance Criteria**:
- [ ] .editorconfig created with C# coding standards
- [ ] Formatting consistent across all files
- [ ] Accessibility naming conventions included

**Implementation**:

Create `.editorconfig` at solution root:
```ini
# EditorConfig for HomeBuyerHelper
root = true

[*]
indent_style = space
indent_size = 4
end_of_line = lf
charset = utf-8
trim_trailing_whitespace = true
insert_final_newline = true

[*.{csproj,props,targets}]
indent_size = 2

[*.{json,yml,yaml}]
indent_size = 2

[*.md]
trim_trailing_whitespace = false

[*.cs]
# Naming conventions
dotnet_naming_rule.private_fields_with_underscore.symbols = private_fields
dotnet_naming_rule.private_fields_with_underscore.style = prefix_underscore
dotnet_naming_rule.private_fields_with_underscore.severity = suggestion

dotnet_naming_symbols.private_fields.applicable_kinds = field
dotnet_naming_symbols.private_fields.applicable_accessibilities = private

dotnet_naming_style.prefix_underscore.capitalization = camel_case
dotnet_naming_style.prefix_underscore.required_prefix = _

# Code style
csharp_style_var_for_built_in_types = true:suggestion
csharp_style_var_when_type_is_apparent = true:suggestion
csharp_style_var_elsewhere = true:suggestion

csharp_prefer_braces = true:warning
csharp_style_expression_bodied_methods = when_on_single_line:suggestion
csharp_style_expression_bodied_properties = true:suggestion

# Accessibility - require AutomationProperties descriptions
dotnet_diagnostic.MA0001.severity = warning
```

---

### SETUP-006: Update .gitignore

**Status**: `[ ]` Not Started

**Dependencies**: SETUP-002

**Acceptance Criteria**:
- [ ] .NET build outputs ignored
- [ ] IDE-specific files ignored
- [ ] User secrets and local settings ignored
- [ ] Platform-specific build artifacts ignored

**Implementation**:

Update `.gitignore`:
```gitignore
# Build outputs
bin/
obj/
out/

# IDE
.vs/
.vscode/
*.user
*.suo
*.userprefs

# .NET
*.dll
*.exe
*.pdb
*.cache
project.lock.json

# MAUI / Mobile
*.ipa
*.apk
*.aab
*.dSYM/
*.xcarchive/

# macOS
.DS_Store
Thumbs.db

# Test results
TestResults/
coverage/
*.coverage
*.coveragexml

# Secrets (never commit)
appsettings.*.json
secrets.json

# Local database files
*.db
*.sqlite
*.sqlite3

# Package caches
packages/
*.nupkg
```

---

### SETUP-007: Create Core Domain Models

**Status**: `[ ]` Not Started

**Dependencies**: SETUP-003

**Acceptance Criteria**:
- [ ] All core domain models created
- [ ] Models use proper C# conventions
- [ ] XML documentation on all public members
- [ ] Unit tests for model validation

**Models to Create**:

| Model | Location | Purpose |
|-------|----------|---------|
| Property | Core/Models/ | Property being evaluated |
| EvaluationCriterion | Core/Models/ | User-defined evaluation criteria |
| PropertyScore | Core/Models/ | Score for property on criterion |
| IncomeSource | Core/Models/ | User income configuration |
| Expense | Core/Models/ | User expense tracking |
| FundingSource | Core/Models/ | Down payment funding source |
| UserPreferences | Core/Models/ | App settings and preferences |
| BudgetMonth | Core/Models/ | Monthly budget projection |

**Example Implementation** (Property.cs):
```csharp
namespace HomeBuyerHelper.Core.Models;

/// <summary>
/// Represents a property being evaluated for purchase.
/// </summary>
public class Property
{
    public int Id { get; set; }

    /// <summary>
    /// User-friendly name for the property (e.g., "Blue house on Oak St").
    /// </summary>
    public required string Nickname { get; set; }

    /// <summary>
    /// Full street address of the property.
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Listing price in USD.
    /// </summary>
    public decimal AskingPrice { get; set; }

    /// <summary>
    /// Number of bedrooms.
    /// </summary>
    public int Bedrooms { get; set; }

    /// <summary>
    /// Number of bathrooms (supports half baths like 2.5).
    /// </summary>
    public decimal Bathrooms { get; set; }

    /// <summary>
    /// Total square footage of living space.
    /// </summary>
    public int SquareFeet { get; set; }

    /// <summary>
    /// Year the property was built.
    /// </summary>
    public int? YearBuilt { get; set; }

    /// <summary>
    /// Monthly HOA fee, if applicable.
    /// </summary>
    public decimal MonthlyHOA { get; set; }

    /// <summary>
    /// Optional URL to the property listing.
    /// </summary>
    public string? ListingUrl { get; set; }

    /// <summary>
    /// User notes about the property.
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// When the property was added to the app.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When the property was last modified.
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
```

**Test Requirements**:
- Verify default values are set correctly
- Test required property validation
- Test date handling

---

### SETUP-008: Create Data Layer Entities and Repository Pattern

**Status**: `[ ]` Not Started

**Dependencies**: SETUP-007

**Acceptance Criteria**:
- [ ] SQLite database context created
- [ ] Repository interfaces defined in Core
- [ ] Repository implementations in Data
- [ ] Database initialization on app start
- [ ] Unit tests for repository operations

**Repository Interface Pattern**:
```csharp
// In Core/Interfaces/IPropertyRepository.cs
namespace HomeBuyerHelper.Core.Interfaces;

public interface IPropertyRepository
{
    Task<Property?> GetByIdAsync(int id);
    Task<IReadOnlyList<Property>> GetAllAsync();
    Task<int> CreateAsync(Property property);
    Task UpdateAsync(Property property);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
```

**SQLite Entity Pattern**:
```csharp
// In Data/Entities/PropertyEntity.cs
using SQLite;

namespace HomeBuyerHelper.Data.Entities;

[Table("Properties")]
public class PropertyEntity
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [MaxLength(200)]
    public string Nickname { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Address { get; set; }

    public decimal AskingPrice { get; set; }
    public int Bedrooms { get; set; }
    public decimal Bathrooms { get; set; }
    public int SquareFeet { get; set; }
    public int? YearBuilt { get; set; }
    public decimal MonthlyHOA { get; set; }
    public string? ListingUrl { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

**Test Requirements**:
- Test CRUD operations
- Test concurrent access handling
- Test database file creation and path

---

### SETUP-009: Configure Dependency Injection

**Status**: `[ ]` Not Started

**Dependencies**: SETUP-008

**Acceptance Criteria**:
- [ ] All services registered in MauiProgram.cs
- [ ] Proper lifetimes (singleton for database, transient for view models)
- [ ] Service provider accessible throughout app

**Implementation** (MauiProgram.cs pattern):
```csharp
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Register database
        builder.Services.AddSingleton<IDatabaseService, DatabaseService>();

        // Register repositories
        builder.Services.AddSingleton<IPropertyRepository, PropertyRepository>();
        builder.Services.AddSingleton<ICriteriaRepository, CriteriaRepository>();
        builder.Services.AddSingleton<IScoreRepository, ScoreRepository>();

        // Register services
        builder.Services.AddSingleton<IPropertyService, PropertyService>();
        builder.Services.AddSingleton<ICalculationService, CalculationService>();
        builder.Services.AddSingleton<IExportService, ExportService>();

        // Register view models (transient - new instance per page)
        builder.Services.AddTransient<DashboardViewModel>();
        builder.Services.AddTransient<PropertyListViewModel>();
        builder.Services.AddTransient<PropertyDetailViewModel>();
        builder.Services.AddTransient<OnboardingViewModel>();

        // Register pages
        builder.Services.AddTransient<DashboardPage>();
        builder.Services.AddTransient<PropertyListPage>();
        builder.Services.AddTransient<PropertyDetailPage>();
        builder.Services.AddTransient<OnboardingPage>();

        return builder.Build();
    }
}
```

---

### SETUP-010: Create Base ViewModel and Page Infrastructure

**Status**: `[ ]` Not Started

**Dependencies**: SETUP-009

**Acceptance Criteria**:
- [ ] BaseViewModel with common functionality
- [ ] IsBusy, Title, and navigation properties
- [ ] MVVM patterns established using CommunityToolkit.Mvvm
- [ ] Page-ViewModel binding convention established

**Base ViewModel Pattern**:
```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace HomeBuyerHelper.ViewModels;

public abstract partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    private bool _isBusy;

    [ObservableProperty]
    private string _title = string.Empty;

    public bool IsNotBusy => !IsBusy;

    /// <summary>
    /// Called when the page appears.
    /// </summary>
    public virtual Task OnAppearingAsync() => Task.CompletedTask;

    /// <summary>
    /// Called when the page disappears.
    /// </summary>
    public virtual Task OnDisappearingAsync() => Task.CompletedTask;
}
```

---

## Architecture Decisions

### ADR-001: Use .NET MAUI over Other Frameworks

**Decision**: Use .NET MAUI for cross-platform development.

**Rationale**:
- Single C# codebase for iOS, Android, Windows, macOS
- Native UI controls (not web views)
- Strong SQLite support for offline-first
- Mature ecosystem with long-term Microsoft support
- Familiar patterns for C#/.NET developers

**Alternatives Considered**:
- React Native: Would require JavaScript, less native feel
- Flutter: Good option but Dart is less familiar, smaller package ecosystem
- Xamarin.Forms: Deprecated in favor of MAUI

---

### ADR-002: SQLite for Local Storage

**Decision**: Use SQLite with sqlite-net-pcl for all local data storage.

**Rationale**:
- Battle-tested embedded database
- Works on all target platforms
- Single file - easy backup and restore
- No server process required
- Excellent .NET support via sqlite-net

**Alternatives Considered**:
- LiteDB: Smaller community, less mature
- Realm: Vendor lock-in concerns
- JSON files: Poor query performance at scale

---

### ADR-003: MVVM Pattern with CommunityToolkit

**Decision**: Use MVVM pattern with CommunityToolkit.Mvvm source generators.

**Rationale**:
- Clean separation of UI and business logic
- Testable view models without UI dependencies
- Source generators reduce boilerplate
- Community-maintained, Microsoft-endorsed

---

### ADR-004: Layered Architecture

**Decision**: Separate solution into Core, Data, and UI projects.

**Rationale**:
- Core logic can be unit tested without platform dependencies
- Data layer is swappable (could change from SQLite)
- Clear boundaries prevent accidental coupling
- Parallel development possible

---

## Verification Checklist

Before proceeding to Phase 1, verify:

- [ ] `dotnet build` succeeds with no warnings
- [ ] `dotnet test` runs (even if no tests yet)
- [ ] Solution opens in VS Code without errors
- [ ] All projects reference correct dependencies
- [ ] Database can be created on app launch (test with Android emulator or Windows)

---

## Next Steps

Once project setup is complete, proceed to [02-phase1-mvp.md](./02-phase1-mvp.md) to begin implementing the Property Evaluation Engine.

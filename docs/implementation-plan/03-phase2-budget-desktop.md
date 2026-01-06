# 03 - Phase 2: Budget Planning + Desktop

This phase adds the Annual Budget Planner module and expands the application to Windows and macOS platforms.

---

## Phase Overview

| Aspect | Details |
|--------|---------|
| **Goal** | Comprehensive financial planning with desktop platform support |
| **Platforms** | + Windows 10/11, macOS 12+ (in addition to iOS/Android) |
| **Core Features** | Income configuration, cash flow projection, emergency fund tracking, PDF reports |
| **Quality Gates** | All tests passing, security review complete, accessibility audit passed |

---

## Task Sections

1. [Income Configuration](#1-income-configuration)
2. [Expense Management](#2-expense-management)
3. [Cash Flow Projection](#3-cash-flow-projection)
4. [Affordability Analysis](#4-affordability-analysis)
5. [Emergency Fund Tracking](#5-emergency-fund-tracking)
6. [PDF Report Generation](#6-pdf-report-generation)
7. [Desktop Platform Support](#7-desktop-platform-support)
8. [Phase 2 Quality Gates](#8-phase-2-quality-gates)

---

## 1. Income Configuration

### P2-INC-001: Income Setup Page

**Status**: `[ ]` Not Started

**Dependencies**: Phase 1 complete

**Acceptance Criteria**:
- [ ] Page to configure all income sources
- [ ] Add/Edit/Delete income sources
- [ ] Visual summary of total monthly income by scenario
- [ ] Navigation from main menu/settings

**Files to Create**:
- `src/HomeBuyerHelper/Pages/Budget/IncomeSetupPage.xaml`
- `src/HomeBuyerHelper/ViewModels/Budget/IncomeSetupViewModel.cs`
- `src/HomeBuyerHelper.Core/Models/IncomeSource.cs`

---

### P2-INC-002: Salary Income Configuration

**Status**: `[ ]` Not Started

**Dependencies**: P2-INC-001

**Acceptance Criteria**:
- [ ] Form for salary income entry
- [ ] Fields: Annual base salary, Pay frequency, 401(k) contribution, HSA, Other deductions
- [ ] Calculate net per-paycheck amount
- [ ] Support multiple salary sources (household)

**Form Fields**:
| Field | Type | Validation |
|-------|------|------------|
| Source Name | Text | Required, e.g., "My Salary" |
| Annual Base | Currency | > 0 |
| Pay Frequency | Dropdown | Weekly, Bi-weekly, Semi-monthly, Monthly |
| 401(k) % | Percentage | 0-100% |
| 401(k) Match % | Percentage | 0-100% (up to limit) |
| HSA Annual | Currency | 0-$8,300 (family limit 2024) |
| Other Deductions | Currency | >= 0 |

**Test Requirements**:
- [ ] Unit test: Net pay calculation accuracy
- [ ] Unit test: Pay frequency distribution

---

### P2-INC-003: Bonus Income Configuration

**Status**: `[ ]` Not Started

**Dependencies**: P2-INC-002

**Acceptance Criteria**:
- [ ] Form for bonus income entry
- [ ] Fields: Expected amount, Bonus month, Probability
- [ ] Support multiple bonuses per year

**Form Fields**:
| Field | Type | Validation |
|-------|------|------------|
| Bonus Name | Text | Required |
| Expected Amount | Currency | > 0 |
| Payment Month | Month Picker | Jan-Dec |
| Probability | Percentage | 0-100% |

**Scenario Handling**:
- Conservative: 0% of bonus
- Realistic: Probability × Amount
- Expected: 100% of amount

---

### P2-INC-004: RSU/Stock Vesting Configuration

**Status**: `[ ]` Not Started

**Dependencies**: P2-INC-002

**Acceptance Criteria**:
- [ ] Form for RSU vesting schedule
- [ ] Fields: Vest schedule type, Shares per vest, Estimated price, Tax withholding
- [ ] Calculate net value per vest
- [ ] Support quarterly, annual, or custom vest dates

**Form Fields**:
| Field | Type | Validation |
|-------|------|------------|
| Grant Name | Text | Required |
| Vest Schedule | Dropdown | Quarterly, Annual, Custom |
| Custom Vest Dates | Multi-date picker | If Custom selected |
| Shares per Vest | Number | > 0 |
| Estimated Share Price | Currency | > 0 |
| Tax Withholding % | Percentage | 0-50% (default 37%) |

**Test Requirements**:
- [ ] Unit test: RSU value calculation
- [ ] Unit test: Custom vest date handling

---

### P2-INC-005: Other Income Sources

**Status**: `[ ]` Not Started

**Dependencies**: P2-INC-002

**Acceptance Criteria**:
- [ ] Generic income source form
- [ ] Types: Rental, Side Business, Investment, Partner Contribution, Other
- [ ] Fields: Amount, Frequency, Start Date, End Date (optional)
- [ ] Partner contribution can have future start date

**Form Fields**:
| Field | Type | Notes |
|-------|------|-------|
| Source Name | Text | Required |
| Type | Dropdown | Rental, Side Business, etc. |
| Amount | Currency | Per occurrence |
| Frequency | Dropdown | Weekly, Bi-weekly, Monthly, Quarterly, Annual, One-time |
| Start Date | Date | When income begins |
| End Date | Date | Optional, when income ends |

---

### P2-INC-006: Income Scenarios Service

**Status**: `[ ]` Not Started

**Dependencies**: P2-INC-002, P2-INC-003, P2-INC-004

**Acceptance Criteria**:
- [ ] Calculate three income scenarios per month
- [ ] Conservative Floor: Salary + guaranteed only
- [ ] Realistic Planning: Salary + 70% of variable
- [ ] Expected Outcome: All income at expected values

**Files to Create**:
- `src/HomeBuyerHelper.Core/Services/IncomeScenarioService.cs`

**Test Requirements**:
- [ ] Unit test: Scenario calculation for each type
- [ ] Unit test: Multi-source aggregation
- [ ] Unit test: Variable income weighting

---

## 2. Expense Management

### P2-EXP-001: Expense Setup Page

**Status**: `[ ]` Not Started

**Dependencies**: Phase 1 complete

**Acceptance Criteria**:
- [ ] Page to manage all expense categories
- [ ] Separate sections for Fixed and Variable expenses
- [ ] Add/Edit/Delete expenses
- [ ] Total monthly expenses displayed

**Files to Create**:
- `src/HomeBuyerHelper/Pages/Budget/ExpenseSetupPage.xaml`
- `src/HomeBuyerHelper/ViewModels/Budget/ExpenseSetupViewModel.cs`
- `src/HomeBuyerHelper.Core/Models/Expense.cs`

---

### P2-EXP-002: Fixed Expense Configuration

**Status**: `[ ]` Not Started

**Dependencies**: P2-EXP-001

**Acceptance Criteria**:
- [ ] Form for recurring fixed expenses
- [ ] Categories: Housing, Transportation, Insurance, Subscriptions, Loans
- [ ] Fields: Name, Amount, Frequency, Category

**Common Fixed Expenses** (pre-populated suggestions):
- Mortgage/Rent
- Car Payment
- Car Insurance
- Life Insurance
- Health Insurance
- Internet
- Phone
- Streaming Services
- Gym Membership
- Student Loans

---

### P2-EXP-003: Variable Expense Configuration

**Status**: `[ ]` Not Started

**Dependencies**: P2-EXP-001

**Acceptance Criteria**:
- [ ] Form for variable monthly expenses
- [ ] Categories: Groceries, Gas, Utilities, Entertainment, Dining, Shopping
- [ ] Fields: Name, Average Monthly Amount, Category

**Common Variable Expenses**:
- Groceries
- Gas/Transportation
- Electric
- Gas (utility)
- Water
- Entertainment
- Dining Out
- Clothing
- Personal Care

---

### P2-EXP-004: One-Time Events Calendar

**Status**: `[ ]` Not Started

**Dependencies**: P2-EXP-001

**Acceptance Criteria**:
- [ ] Calendar view or list of one-time expenses
- [ ] Add events with: Name, Amount, Date, Category
- [ ] Categories: Moving, Travel, Purchase, Medical, Other
- [ ] Events appear in cash flow for their month

**Files to Create**:
- `src/HomeBuyerHelper/Pages/Budget/OneTimeEventsPage.xaml`
- `src/HomeBuyerHelper.Core/Models/OneTimeEvent.cs`

**Test Requirements**:
- [ ] Unit test: Event included in correct month's calculation

---

## 3. Cash Flow Projection

### P3-CFP-001: Monthly Projection Service

**Status**: `[ ]` Not Started

**Dependencies**: P2-INC-006, P2-EXP-003, P2-EXP-004

**Acceptance Criteria**:
- [ ] Calculate month-by-month projection for 24 months
- [ ] For each month: Income, Fixed Expenses, Variable Expenses, One-Time Events
- [ ] Calculate Surplus/Deficit per month
- [ ] Calculate Cumulative running total
- [ ] Support all three income scenarios

**Output Structure**:
```csharp
public class MonthlyProjection
{
    public DateTime Month { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal FixedExpenses { get; set; }
    public decimal VariableExpenses { get; set; }
    public decimal OneTimeExpenses { get; set; }
    public decimal TotalExpenses => FixedExpenses + VariableExpenses + OneTimeExpenses;
    public decimal Surplus => TotalIncome - TotalExpenses;
    public decimal CumulativeSurplus { get; set; }
    public bool IsCrunchMonth => Surplus < 0;
}
```

**Files to Create**:
- `src/HomeBuyerHelper.Core/Services/CashFlowProjectionService.cs`

**Test Requirements**:
- [ ] Unit test: Projection accuracy over multiple months
- [ ] Unit test: Cumulative calculation
- [ ] Unit test: Crunch month detection

---

### P2-CFP-002: Cash Flow Timeline Page

**Status**: `[ ]` Not Started

**Dependencies**: P2-CFP-001

**Acceptance Criteria**:
- [ ] Interactive month-by-month view
- [ ] Table with columns: Month, Income, Expenses (expandable), Surplus, Cumulative
- [ ] Color coding: Green (surplus), Red (deficit)
- [ ] Tap month to expand details
- [ ] Scenario switcher (Conservative/Realistic/Expected)

**Files to Create**:
- `src/HomeBuyerHelper/Pages/Budget/CashFlowTimelinePage.xaml`
- `src/HomeBuyerHelper/ViewModels/Budget/CashFlowTimelineViewModel.cs`

**Accessibility Requirements**:
- [ ] Table semantics for screen readers
- [ ] Color not sole indicator (also icons/text)

---

### P2-CFP-003: Cash Flow Chart

**Status**: `[ ]` Not Started

**Dependencies**: P2-CFP-002

**Acceptance Criteria**:
- [ ] Bar chart showing monthly surplus/deficit
- [ ] Line overlay showing cumulative
- [ ] Toggle between scenarios
- [ ] Tap bar to see month details

**Implementation Notes**:
- Use LiveCharts2 ComboChart
- Green bars for surplus, red for deficit

---

## 4. Affordability Analysis

### P2-AFF-001: Housing Affordability Calculator

**Status**: `[ ]` Not Started

**Dependencies**: P2-INC-006, Phase 1 Cost Calculator

**Acceptance Criteria**:
- [ ] Calculate Housing Percentage = (Housing Cost / Gross Income) × 100
- [ ] Apply to each income scenario
- [ ] Color-coded zones: Green (<28%), Yellow (28-36%), Orange (36-43%), Red (>43%)

**Files to Create**:
- `src/HomeBuyerHelper.Core/Services/AffordabilityService.cs`

**Test Requirements**:
- [ ] Unit test: Percentage calculation accuracy
- [ ] Unit test: Zone thresholds

---

### P2-AFF-002: Affordability Display on Property

**Status**: `[ ]` Not Started

**Dependencies**: P2-AFF-001

**Acceptance Criteria**:
- [ ] Property detail shows affordability indicator
- [ ] Shows percentage and zone for each scenario
- [ ] Warning message if in Orange/Red zone
- [ ] Link to adjust income/expenses if needed

---

### P2-AFF-003: Affordability Comparison

**Status**: `[ ]` Not Started

**Dependencies**: P2-AFF-001

**Acceptance Criteria**:
- [ ] Add affordability column to comparison matrix
- [ ] Show zone indicator per property
- [ ] Allow filtering by affordable properties only

---

## 5. Emergency Fund Tracking

### P2-EMF-001: Emergency Fund Configuration

**Status**: `[ ]` Not Started

**Dependencies**: P2-EXP-003

**Acceptance Criteria**:
- [ ] Configure target emergency fund (default: 6 months expenses)
- [ ] Enter current emergency fund balance
- [ ] Calculate target based on monthly expenses

**Files to Create**:
- `src/HomeBuyerHelper/Pages/Budget/EmergencyFundPage.xaml`
- `src/HomeBuyerHelper.Core/Models/EmergencyFundConfig.cs`

---

### P2-EMF-002: Emergency Fund Projection

**Status**: `[ ]` Not Started

**Dependencies**: P2-EMF-001, P2-CFP-001

**Acceptance Criteria**:
- [ ] Project emergency fund balance month-by-month
- [ ] Automatic draws during deficit months
- [ ] Calculate recovery timeline after draws
- [ ] Warning when fund drops below 3 months

**Logic**:
```csharp
if (MonthSurplus < 0)
{
    EmergencyFundBalance += MonthSurplus; // Negative, so subtraction
}
else if (EmergencyFundBalance < TargetBalance)
{
    var contribution = Math.Min(MonthSurplus, TargetBalance - EmergencyFundBalance);
    EmergencyFundBalance += contribution;
}
```

**Test Requirements**:
- [ ] Unit test: Fund draw during deficit
- [ ] Unit test: Fund recovery calculation
- [ ] Unit test: Warning threshold

---

### P2-EMF-003: Emergency Fund Visualization

**Status**: `[ ]` Not Started

**Dependencies**: P2-EMF-002

**Acceptance Criteria**:
- [ ] Line chart showing fund balance over time
- [ ] Target line displayed for reference
- [ ] Red zone indicator below 3 months
- [ ] Annotations for draw and recovery events

---

## 6. PDF Report Generation

### P2-PDF-001: Property Comparison Report

**Status**: `[ ]` Not Started

**Dependencies**: Phase 1 Comparison Matrix

**Acceptance Criteria**:
- [ ] Generate PDF with comparison matrix
- [ ] Include: Executive summary, property details, scoring matrix, financial comparison
- [ ] Professional formatting with logo
- [ ] Save to device or share

**Report Sections**:
1. Cover Page: Title, Date, User name (optional)
2. Executive Summary: Top recommendation, key findings
3. Properties Overview: Details of each property
4. Scoring Matrix: Full weighted comparison
5. Financial Comparison: Monthly costs, 30-year costs
6. Appendix: Criteria definitions and weights

**Files to Create**:
- `src/HomeBuyerHelper.Core/Reports/PropertyComparisonReport.cs`
- `src/HomeBuyerHelper.Core/Reports/ReportGenerator.cs`

**Implementation Notes**:
- Use QuestPDF library
- Fluent API for document construction

---

### P2-PDF-002: Budget Plan Report

**Status**: `[ ]` Not Started

**Dependencies**: P2-CFP-002

**Acceptance Criteria**:
- [ ] Generate PDF with budget projection
- [ ] Include: Income summary, expense breakdown, 24-month projection, emergency fund status

**Report Sections**:
1. Cover Page
2. Income Structure Summary
3. Expense Breakdown (Fixed, Variable)
4. Affordability Analysis per Property
5. 24-Month Cash Flow Table
6. Emergency Fund Projection
7. Risk Scenarios

---

### P2-PDF-003: Combined Report

**Status**: `[ ]` Not Started

**Dependencies**: P2-PDF-001, P2-PDF-002

**Acceptance Criteria**:
- [ ] Single comprehensive report combining both
- [ ] Option to generate from dashboard
- [ ] Table of contents with navigation

---

## 7. Desktop Platform Support

### P2-DSK-001: Windows Application Build

**Status**: `[ ]` Not Started

**Dependencies**: Phase 1 complete

**Acceptance Criteria**:
- [ ] Windows 10/11 build configuration
- [ ] MSIX packaging for distribution
- [ ] App runs correctly on Windows
- [ ] Window resizing handled gracefully

**Configuration Updates**:
- Update `.csproj` for Windows target
- Configure Windows App SDK
- Set up code signing

**Files to Update**:
- `src/HomeBuyerHelper/HomeBuyerHelper.csproj`
- `src/HomeBuyerHelper/Platforms/Windows/`

---

### P2-DSK-002: macOS Application Build

**Status**: `[ ]` Not Started

**Dependencies**: Phase 1 complete

**Acceptance Criteria**:
- [ ] macOS 12+ build configuration
- [ ] Mac Catalyst configuration
- [ ] App runs correctly on Mac
- [ ] Menu bar integration

**Configuration Updates**:
- Configure Mac Catalyst in project
- Set up Apple Developer signing
- Notarization configuration

---

### P2-DSK-003: Desktop-Optimized Layouts

**Status**: `[ ]` Not Started

**Dependencies**: P2-DSK-001, P2-DSK-002

**Acceptance Criteria**:
- [ ] Responsive layouts for larger screens
- [ ] Side-by-side views where appropriate
- [ ] Hover states for interactive elements
- [ ] Keyboard navigation support

**Layout Adaptations**:
| Page | Mobile | Desktop |
|------|--------|---------|
| Property List | Single column cards | Multi-column grid |
| Comparison Matrix | Horizontal scroll | Full table visible |
| Cash Flow | Vertical list | Side-by-side chart and table |
| Settings | Stacked sections | Two-column layout |

---

### P2-DSK-004: Keyboard Shortcuts

**Status**: `[ ]` Not Started

**Dependencies**: P2-DSK-003

**Acceptance Criteria**:
- [ ] Common shortcuts implemented
- [ ] Shortcuts displayed in menus
- [ ] Keyboard navigation for all interactive elements

**Shortcuts**:
| Shortcut | Action |
|----------|--------|
| Cmd/Ctrl+N | New Property |
| Cmd/Ctrl+E | Export Data |
| Cmd/Ctrl+I | Import Data |
| Cmd/Ctrl+P | Generate PDF |
| Cmd/Ctrl+, | Settings |
| Cmd/Ctrl+1-4 | Switch tabs |

---

### P2-DSK-005: Windows Store Submission

**Status**: `[ ]` Not Started

**Dependencies**: P2-DSK-001, Phase 2 Quality Gates

**Acceptance Criteria**:
- [ ] Microsoft Store listing created
- [ ] Screenshots for Windows
- [ ] MSIX signed and uploaded
- [ ] Certification passed

---

### P2-DSK-006: Mac App Store Submission

**Status**: `[ ]` Not Started

**Dependencies**: P2-DSK-002, Phase 2 Quality Gates

**Acceptance Criteria**:
- [ ] App Store Connect listing created
- [ ] Screenshots for Mac
- [ ] Notarized build uploaded
- [ ] App Review passed

---

## 8. Phase 2 Quality Gates

### P2-QG-001: Test Coverage

**Status**: `[ ]` Not Started

**Acceptance Criteria**:
- [ ] Unit test coverage > 80% for new code
- [ ] Integration tests for budget calculations
- [ ] PDF report generation tests
- [ ] Desktop platform tests

---

### P2-QG-002: Security Review

**Status**: `[ ]` Not Started

**Acceptance Criteria**:
- [ ] Complete security checklist for Phase 2 features
- [ ] AI security analysis on new services
- [ ] Financial calculation verification (no data leaks)
- [ ] PDF generation security (no injection)

---

### P2-QG-003: Accessibility Audit

**Status**: `[ ]` Not Started

**Acceptance Criteria**:
- [ ] Desktop accessibility testing (Windows Narrator, macOS VoiceOver)
- [ ] Keyboard navigation complete
- [ ] High contrast mode support
- [ ] PDF accessibility (tagged PDF)

---

### P2-QG-004: Performance Benchmarks

**Status**: `[ ]` Not Started

**Acceptance Criteria**:
- [ ] Cash flow calculation < 100ms for 24 months
- [ ] PDF generation < 3 seconds
- [ ] Desktop memory usage < 200MB
- [ ] Large dataset handling (100+ properties)

---

### P2-QG-005: Cross-Platform Testing

**Status**: `[ ]` Not Started

**Acceptance Criteria**:
- [ ] Feature parity across all 4 platforms
- [ ] Data sync between devices (manual export/import)
- [ ] UI consistency verification
- [ ] Platform-specific bug fixes complete

---

## Phase 2 Summary

| Metric | Target |
|--------|--------|
| Total Tasks | ~35 |
| New Platforms | Windows, macOS |
| Test Coverage | > 80% |
| Accessibility | WCAG 2.1 AA |
| Security Review | Pass |

Upon completion of all quality gates, proceed to [04-phase3-advanced-analysis.md](./04-phase3-advanced-analysis.md).

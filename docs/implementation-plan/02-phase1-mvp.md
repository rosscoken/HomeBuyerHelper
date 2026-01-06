# 02 - Phase 1: MVP (Property Evaluation Engine)

This phase delivers the core Property Evaluation Engine for iOS and Android, establishing the foundation for all future features.

---

## Phase Overview

| Aspect | Details |
|--------|---------|
| **Goal** | Launch iOS and Android apps with property comparison functionality |
| **Platforms** | iOS 15+, Android 10+ |
| **Core Features** | Guided onboarding, property entry, weighted scoring, comparison matrix |
| **Quality Gates** | All tests passing, security review complete, accessibility audit passed |

---

## Task Sections

1. [Onboarding Flow](#1-onboarding-flow)
2. [Criteria Management](#2-criteria-management)
3. [Property Management](#3-property-management)
4. [Scoring System](#4-scoring-system)
5. [Comparison Matrix](#5-comparison-matrix)
6. [Basic Cost Calculator](#6-basic-cost-calculator)
7. [Data Export](#7-data-export)
8. [App Store Preparation](#8-app-store-preparation)
9. [Phase 1 Quality Gates](#9-phase-1-quality-gates)

---

## 1. Onboarding Flow

### P1-ONB-001: Welcome Screen

**Status**: `[ ]` Not Started

**Acceptance Criteria**:
- [ ] App logo and tagline displayed
- [ ] "Get Started" button navigates to first question
- [ ] "I have a backup" link allows import
- [ ] Screen is accessible (VoiceOver/TalkBack compatible)

**Files to Create**:
- `src/HomeBuyerHelper/Pages/Onboarding/WelcomePage.xaml`
- `src/HomeBuyerHelper/Pages/Onboarding/WelcomePage.xaml.cs`
- `src/HomeBuyerHelper/ViewModels/Onboarding/WelcomeViewModel.cs`

**UX Specification**: See [09-ux-specifications.md](./09-ux-specifications.md#welcome-screen)

**Accessibility Requirements**:
- [ ] All buttons have AutomationProperties.Name
- [ ] Minimum touch target 44x44 points
- [ ] Color contrast ratio 4.5:1 minimum

**Test Requirements**:
- [ ] Unit test: ViewModel navigation command
- [ ] UI test: Button tap navigates correctly

---

### P1-ONB-002: Goal Selection Question

**Status**: `[ ]` Not Started

**Dependencies**: P1-ONB-001

**Acceptance Criteria**:
- [ ] Question "What best describes your situation?" displayed
- [ ] Five mutually exclusive options as radio-style buttons
- [ ] Selection stored in onboarding state
- [ ] "Continue" button enabled only when selection made
- [ ] Back navigation available

**Options**:
1. Buying my first home
2. Upgrading to a bigger/better home
3. Downsizing or simplifying
4. Relocating to a new area
5. Buying an investment property

**Files to Create**:
- `src/HomeBuyerHelper/Pages/Onboarding/GoalSelectionPage.xaml`
- `src/HomeBuyerHelper/ViewModels/Onboarding/GoalSelectionViewModel.cs`
- `src/HomeBuyerHelper.Core/Models/OnboardingState.cs`

**Test Requirements**:
- [ ] Unit test: Selection updates state correctly
- [ ] Unit test: Continue button enabled state logic
- [ ] UI test: Selection persists through navigation

---

### P1-ONB-003: Property Count Question

**Status**: `[ ]` Not Started

**Dependencies**: P1-ONB-002

**Acceptance Criteria**:
- [ ] Question "How many properties are you currently considering?"
- [ ] Three options displayed
- [ ] Selection stored in onboarding state

**Options**:
1. Just starting my search (0-1)
2. A few options (2-3)
3. Several to compare (4+)

---

### P1-ONB-004: Household Composition Questions

**Status**: `[ ]` Not Started

**Dependencies**: P1-ONB-003

**Acceptance Criteria**:
- [ ] "Who will be living in this home?" question
- [ ] "How do you and others work?" question
- [ ] "Do you have pets?" question
- [ ] Multi-select support for pets question
- [ ] Selections stored and used for criteria suggestions

**Questions & Options**:

**Q1: Who will be living in this home?**
- Just me
- Me and a partner/spouse
- Family with kids
- Roommates
- Multi-generational household

**Q2: How do you and others work?**
- Everyone commutes to an office daily
- Hybrid (some days office, some days home)
- Fully remote / work from home
- Retired or not working
- Mix of the above

**Q3: Do you have pets?**
- No pets
- Dog(s)
- Cat(s)
- Other pets

---

### P1-ONB-005: Priority Selection (Location)

**Status**: `[ ]` Not Started

**Dependencies**: P1-ONB-004

**Acceptance Criteria**:
- [ ] "What's most important about where you live?" displayed
- [ ] Multi-select (up to 3) with visual feedback
- [ ] Counter shows selections remaining
- [ ] Continue enabled with at least 1 selection

**Options**:
- Short commute to work
- Good schools nearby
- Walkable neighborhood
- Peace and quiet
- Near family/friends
- Access to outdoor activities
- Nightlife and entertainment
- Low cost of living

---

### P1-ONB-006: Priority Selection (Home)

**Status**: `[ ]` Not Started

**Dependencies**: P1-ONB-005

**Acceptance Criteria**:
- [ ] "What matters most about the home itself?" displayed
- [ ] Multi-select (up to 3)
- [ ] Responses combined with location priorities

**Options**:
- Move-in ready condition
- Space for home office(s)
- Outdoor space (yard, patio, balcony)
- Modern kitchen
- Lots of natural light
- Garage or parking
- Storage space
- Guest accommodations
- Low maintenance

---

### P1-ONB-007: Priority Ranking Screen

**Status**: `[ ]` Not Started

**Dependencies**: P1-ONB-006

**Acceptance Criteria**:
- [ ] Personalized criteria list generated from answers
- [ ] Drag-and-drop reordering with haptic feedback
- [ ] Ranking order used to suggest initial weights
- [ ] Accessibility: reorder via long-press menu alternative

**Implementation Notes**:
- Use CommunityToolkit.Maui DragGestureRecognizer
- Store ranking in OnboardingState
- Calculate suggested weights: #1 = 25%, #2 = 20%, descending

**Test Requirements**:
- [ ] Unit test: Weight calculation from ranking
- [ ] UI test: Drag-drop reorders list correctly

---

### P1-ONB-008: Criteria Review Screen

**Status**: `[ ]` Not Started

**Dependencies**: P1-ONB-007

**Acceptance Criteria**:
- [ ] Generated criteria shown with checkboxes
- [ ] Suggested weight and reason displayed
- [ ] Users can uncheck to exclude criteria
- [ ] "Add More Criteria" button available
- [ ] Weight total shown (updates live)

**Columns**: Include (checkbox) | Criterion Name | Suggested Weight | Why Suggested

---

### P1-ONB-009: Common Criteria Browser

**Status**: `[ ]` Not Started

**Dependencies**: P1-ONB-008

**Acceptance Criteria**:
- [ ] Categorized list of common criteria
- [ ] One-tap to add criterion
- [ ] Already-added criteria shown as selected
- [ ] Categories: Location, Home Features, Practical

**Files to Create**:
- `src/HomeBuyerHelper.Core/Data/CommonCriteria.cs` (static data)
- `src/HomeBuyerHelper/Pages/Onboarding/CriteriaBrowserPage.xaml`

---

### P1-ONB-010: Custom Criterion Creation

**Status**: `[ ]` Not Started

**Dependencies**: P1-ONB-009

**Acceptance Criteria**:
- [ ] Form with fields: Name, "What does a 10 look like?", "What does a 1 look like?", Initial Weight
- [ ] Validation: Name required, weight 1-50%
- [ ] Save adds to criteria list

---

### P1-ONB-011: Weight Fine-Tuning Screen

**Status**: `[ ]` Not Started

**Dependencies**: P1-ONB-008

**Acceptance Criteria**:
- [ ] Slider for each criterion weight
- [ ] Real-time pie chart visualization
- [ ] Running total must equal 100%
- [ ] "Lock" toggle to freeze criterion during adjustment
- [ ] "Reset to Suggested" button
- [ ] Auto-balance option for remaining weight

**Implementation Notes**:
- Use LiveCharts2 PieChart
- Implement proportional adjustment when one slider moves (for unlocked items)

**Test Requirements**:
- [ ] Unit test: Weight normalization to 100%
- [ ] Unit test: Lock functionality prevents adjustment
- [ ] UI test: Sliders update chart correctly

---

### P1-ONB-012: Onboarding Complete Screen

**Status**: `[ ]` Not Started

**Dependencies**: P1-ONB-011

**Acceptance Criteria**:
- [ ] Summary card showing: criteria count, top 3 priorities
- [ ] "Add Your First Property" CTA button
- [ ] "Browse Example" secondary option
- [ ] Onboarding state saved to database
- [ ] App navigates to main shell

**Files to Create**:
- `src/HomeBuyerHelper/Pages/Onboarding/OnboardingCompletePage.xaml`

---

### P1-ONB-013: Onboarding State Persistence

**Status**: `[ ]` Not Started

**Dependencies**: P1-ONB-012

**Acceptance Criteria**:
- [ ] All onboarding answers persisted to SQLite
- [ ] Criteria saved to EvaluationCriteria table
- [ ] UserPreferences.OnboardingCompleted flag set
- [ ] App skips onboarding on subsequent launches

**Test Requirements**:
- [ ] Integration test: Full onboarding flow persists data
- [ ] Unit test: State persistence and retrieval

---

## 2. Criteria Management

### P1-CRI-001: Criteria List Page

**Status**: `[ ]` Not Started

**Dependencies**: P1-ONB-013

**Acceptance Criteria**:
- [ ] List view of all user criteria
- [ ] Shows: Name, Weight, Category
- [ ] Tap to edit, swipe to delete
- [ ] "Add Criterion" FAB or header button
- [ ] Total weight shown in header

**Files to Create**:
- `src/HomeBuyerHelper/Pages/Settings/CriteriaListPage.xaml`
- `src/HomeBuyerHelper/ViewModels/Settings/CriteriaListViewModel.cs`

---

### P1-CRI-002: Criterion Edit Page

**Status**: `[ ]` Not Started

**Dependencies**: P1-CRI-001

**Acceptance Criteria**:
- [ ] Edit: Name, Description, Weight, Category, Score Anchors
- [ ] Score anchors: definitions for 1-2, 3-4, 5-6, 7-8, 9-10
- [ ] Save button validates and persists
- [ ] Delete button with confirmation

**Model Fields**:
```csharp
public class EvaluationCriterion
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public decimal Weight { get; set; }  // 0.01 to 1.00
    public string? Category { get; set; }
    public string? ScoreAnchorLow { get; set; }     // 1-2
    public string? ScoreAnchorMidLow { get; set; }  // 3-4
    public string? ScoreAnchorMid { get; set; }     // 5-6
    public string? ScoreAnchorMidHigh { get; set; } // 7-8
    public string? ScoreAnchorHigh { get; set; }    // 9-10
    public bool IsCalculated { get; set; }
    public int SortOrder { get; set; }
}
```

---

### P1-CRI-003: Weight Rebalancing Service

**Status**: `[ ]` Not Started

**Dependencies**: P1-CRI-002

**Acceptance Criteria**:
- [ ] Service to normalize weights to 100%
- [ ] Proportional adjustment when one weight changes
- [ ] Respect "locked" weights during adjustment
- [ ] Warn if single criterion exceeds 40%

**Files to Create**:
- `src/HomeBuyerHelper.Core/Services/WeightBalancingService.cs`

**Test Requirements**:
- [ ] Unit test: Weights sum to 100% after rebalance
- [ ] Unit test: Locked weights unchanged
- [ ] Unit test: Warning threshold detection

---

### P1-CRI-004: Starter Templates

**Status**: `[ ]` Not Started

**Dependencies**: P1-CRI-001

**Acceptance Criteria**:
- [ ] Four templates available: First-Time Buyer, Family-Focused, Investment-Focused, Remote Worker
- [ ] Template selection replaces current criteria (with confirmation)
- [ ] Template data from static C# class

**Files to Create**:
- `src/HomeBuyerHelper.Core/Data/CriteriaTemplates.cs`

---

## 3. Property Management

### P1-PRO-001: Property List Page (Dashboard)

**Status**: `[ ]` Not Started

**Dependencies**: P1-ONB-013

**Acceptance Criteria**:
- [ ] Card-based list of all properties
- [ ] Each card shows: Nickname, Address, Price, Overall Score, Rank
- [ ] Color-coded score indicator (green/yellow/red)
- [ ] Empty state with "Add Property" CTA
- [ ] Pull-to-refresh functionality
- [ ] Sort options: Score (default), Price, Date Added

**Files to Create**:
- `src/HomeBuyerHelper/Pages/Properties/PropertyListPage.xaml`
- `src/HomeBuyerHelper/ViewModels/Properties/PropertyListViewModel.cs`
- `src/HomeBuyerHelper/Controls/PropertyCard.xaml`

**Accessibility Requirements**:
- [ ] Cards announce: "Property [Name], score [X] out of 10, ranked [N]"
- [ ] Sort control accessible via VoiceOver

---

### P1-PRO-002: Property Entry Form

**Status**: `[ ]` Not Started

**Dependencies**: P1-PRO-001

**Acceptance Criteria**:
- [ ] Form fields per design spec (Nickname required, others optional)
- [ ] Input validation with error messages
- [ ] Save creates property and navigates to scoring
- [ ] Cancel discards and returns to list

**Form Fields**:
| Field | Type | Required | Validation |
|-------|------|----------|------------|
| Nickname | Text | Yes | Max 200 chars |
| Address | Text | No | Max 500 chars |
| Asking Price | Currency | No | > 0 |
| Bedrooms | Number | No | 0-20 |
| Bathrooms | Decimal | No | 0-10, 0.5 increments |
| Square Footage | Number | No | > 0 |
| Year Built | Year | No | 1800-current year |
| Property Type | Dropdown | No | Single Family, Townhome, Condo, Multi-family |
| HOA Fee | Currency | No | >= 0 |
| Listing URL | URL | No | Valid URL format |

**Files to Create**:
- `src/HomeBuyerHelper/Pages/Properties/PropertyEntryPage.xaml`
- `src/HomeBuyerHelper/ViewModels/Properties/PropertyEntryViewModel.cs`

**Test Requirements**:
- [ ] Unit test: Validation logic for each field
- [ ] UI test: Form submission creates property

---

### P1-PRO-003: Property Detail Page

**Status**: `[ ]` Not Started

**Dependencies**: P1-PRO-002

**Acceptance Criteria**:
- [ ] Read-only view of property details
- [ ] Summary of all scores with visual indicators
- [ ] Overall weighted score prominently displayed
- [ ] Edit button navigates to edit form
- [ ] "Score This Property" button if incomplete scores

**Sections**:
1. Header: Nickname, Address, Price
2. Details: Beds, Baths, SqFt, Year, Type, HOA
3. Scores: List of criteria with scores (color-coded)
4. Actions: Edit, Delete, Rescore

---

### P1-PRO-004: Property Edit Form

**Status**: `[ ]` Not Started

**Dependencies**: P1-PRO-003

**Acceptance Criteria**:
- [ ] Pre-populated form with existing values
- [ ] Same validation as entry form
- [ ] Save updates property, Cancel discards changes
- [ ] Delete button with confirmation dialog

---

### P1-PRO-005: Property Deletion

**Status**: `[ ]` Not Started

**Dependencies**: P1-PRO-004

**Acceptance Criteria**:
- [ ] Confirmation dialog before deletion
- [ ] Deletes property and all associated scores
- [ ] Toast notification confirms deletion
- [ ] Undo option for 5 seconds

**Test Requirements**:
- [ ] Integration test: Cascade delete removes scores

---

## 4. Scoring System

### P1-SCO-001: Scoring Walkthrough Page

**Status**: `[ ]` Not Started

**Dependencies**: P1-PRO-002, P1-CRI-002

**Acceptance Criteria**:
- [ ] One criterion displayed at a time
- [ ] Large 1-10 scale buttons or slider
- [ ] Score anchors displayed as reference
- [ ] Progress indicator (e.g., "3 of 7")
- [ ] "Skip for now" option
- [ ] Optional notes field
- [ ] Back button to previous criterion

**UX Flow**:
1. Navigate here after property creation or via "Score" button
2. Display first unscored criterion
3. User selects score (1-10)
4. Optionally adds note
5. "Next" advances to next criterion
6. After last criterion, show summary and return to property detail

**Files to Create**:
- `src/HomeBuyerHelper/Pages/Scoring/ScoringWalkthroughPage.xaml`
- `src/HomeBuyerHelper/ViewModels/Scoring/ScoringWalkthroughViewModel.cs`

**Accessibility Requirements**:
- [ ] Score buttons announce value and anchor text
- [ ] Progress announced on each criterion

**Test Requirements**:
- [ ] Unit test: Score persistence
- [ ] Unit test: Progress calculation
- [ ] UI test: Full walkthrough flow

---

### P1-SCO-002: Score Entry Controls

**Status**: `[ ]` Not Started

**Dependencies**: P1-SCO-001

**Acceptance Criteria**:
- [ ] Two input modes: Button Grid (default), Slider
- [ ] User can toggle between modes
- [ ] Both modes support accessibility
- [ ] Haptic feedback on selection (mobile)

**Button Grid**: 10 buttons in 2 rows of 5, selected button highlighted
**Slider**: Continuous slider 1-10 with step snapping

---

### P1-SCO-003: Quick Score Edit

**Status**: `[ ]` Not Started

**Dependencies**: P1-SCO-001

**Acceptance Criteria**:
- [ ] From property detail, tap score row to edit single criterion
- [ ] Modal or inline editor for quick adjustment
- [ ] Save immediately updates comparison

---

### P1-SCO-004: Score Calculation Service

**Status**: `[ ]` Not Started

**Dependencies**: P1-SCO-001

**Acceptance Criteria**:
- [ ] Calculate weighted score for property
- [ ] Handle missing scores (exclude from calculation or use 0)
- [ ] Rank properties by weighted score
- [ ] Recalculate on any score or weight change

**Formula**:
```
WeightedScore = Σ(Score_i × Weight_i) for all scored criteria
```

**Files to Create**:
- `src/HomeBuyerHelper.Core/Services/ScoreCalculationService.cs`

**Test Requirements**:
- [ ] Unit test: Weighted score calculation
- [ ] Unit test: Ranking with ties
- [ ] Unit test: Partial scores handling

---

## 5. Comparison Matrix

### P1-CMP-001: Comparison Matrix Page

**Status**: `[ ]` Not Started

**Dependencies**: P1-SCO-004

**Acceptance Criteria**:
- [ ] Properties as columns, criteria as rows
- [ ] Cells show score (1-10) with color coding
- [ ] Green: 8-10, Yellow: 5-7, Red: 1-4
- [ ] Weighted total row at bottom
- [ ] Rank indicator on header (#1, #2, etc.)
- [ ] Horizontally scrollable for many properties
- [ ] Tap cell to edit score

**Files to Create**:
- `src/HomeBuyerHelper/Pages/Comparison/ComparisonMatrixPage.xaml`
- `src/HomeBuyerHelper/ViewModels/Comparison/ComparisonMatrixViewModel.cs`

**Accessibility Requirements**:
- [ ] Table semantics for screen readers
- [ ] Cell reads: "[Property] scored [X] on [Criterion]"

---

### P1-CMP-002: Matrix Filtering

**Status**: `[ ]` Not Started

**Dependencies**: P1-CMP-001

**Acceptance Criteria**:
- [ ] Filter to show only selected properties
- [ ] Filter to show only selected categories
- [ ] "Show All" quick reset

---

### P1-CMP-003: Winner Highlight

**Status**: `[ ]` Not Started

**Dependencies**: P1-CMP-001

**Acceptance Criteria**:
- [ ] #1 ranked property column highlighted
- [ ] Per-row winner highlighted (highest score in that criterion)
- [ ] Visual distinction (bold, icon, or background)

---

## 6. Basic Cost Calculator

### P1-CST-001: Monthly Cost Calculation Service

**Status**: `[ ]` Not Started

**Dependencies**: P1-PRO-002

**Acceptance Criteria**:
- [ ] Calculate mortgage payment (P&I)
- [ ] Add property tax estimate
- [ ] Add insurance estimate
- [ ] Add HOA fee
- [ ] Add PMI if down payment < 20%
- [ ] Return total monthly cost

**Inputs** (with defaults):
| Input | Default |
|-------|---------|
| Property Price | From property |
| Down Payment % | 20% |
| Interest Rate | User-configured or 7% |
| Loan Term | 30 years |
| Property Tax Rate | 0.96% annually |
| Insurance | $125/month |
| HOA | From property |

**Formula for Mortgage P&I**:
```
M = P × [r(1+r)^n] / [(1+r)^n - 1]

Where:
M = Monthly payment
P = Principal (Price - Down Payment)
r = Monthly interest rate (annual / 12)
n = Total payments (years × 12)
```

**Files to Create**:
- `src/HomeBuyerHelper.Core/Calculations/MortgageCalculator.cs`
- `src/HomeBuyerHelper.Core/Calculations/MonthlyCostCalculator.cs`

**Test Requirements**:
- [ ] Unit test: Mortgage calculation accuracy (known values)
- [ ] Unit test: PMI calculation
- [ ] Unit test: Total monthly cost sum

---

### P1-CST-002: Cost Display on Property Detail

**Status**: `[ ]` Not Started

**Dependencies**: P1-CST-001, P1-PRO-003

**Acceptance Criteria**:
- [ ] "Estimated Monthly Cost" section on property detail
- [ ] Breakdown: Mortgage, Tax, Insurance, HOA, PMI (if any)
- [ ] Total prominently displayed
- [ ] Tap to expand calculation details

---

### P1-CST-003: Loan Configuration Settings

**Status**: `[ ]` Not Started

**Dependencies**: P1-CST-001

**Acceptance Criteria**:
- [ ] Settings page for default loan parameters
- [ ] Down payment percentage (5-100%)
- [ ] Interest rate (0-20%)
- [ ] Loan term (15, 20, 30 years)
- [ ] Property tax rate override
- [ ] Insurance estimate override

**Files to Create**:
- `src/HomeBuyerHelper/Pages/Settings/LoanSettingsPage.xaml`

---

## 7. Data Export

### P1-EXP-001: JSON Export

**Status**: `[ ]` Not Started

**Dependencies**: P1-PRO-001, P1-CRI-001

**Acceptance Criteria**:
- [ ] Export all data to JSON file
- [ ] Include: Properties, Criteria, Scores, Settings
- [ ] Save to device-appropriate location (Documents, Downloads)
- [ ] Share sheet integration for sending file

**Files to Create**:
- `src/HomeBuyerHelper.Core/Services/ExportService.cs`

**Test Requirements**:
- [ ] Integration test: Export and reimport data matches

---

### P1-EXP-002: JSON Import

**Status**: `[ ]` Not Started

**Dependencies**: P1-EXP-001

**Acceptance Criteria**:
- [ ] Import JSON backup file
- [ ] Validate file format before import
- [ ] Option: Replace all data OR Merge with existing
- [ ] Confirmation dialog before destructive replace

---

## 8. App Store Preparation

### P1-APP-001: App Icons and Splash Screen

**Status**: `[ ]` Not Started

**Acceptance Criteria**:
- [ ] App icon for all required sizes (iOS, Android)
- [ ] Splash screen with logo
- [ ] Consistent branding across platforms

**Files to Update**:
- `src/HomeBuyerHelper/Resources/AppIcon/`
- `src/HomeBuyerHelper/Resources/Splash/`

---

### P1-APP-002: App Store Metadata

**Status**: `[ ]` Not Started

**Acceptance Criteria**:
- [ ] App name: "HomeBuyerHelper"
- [ ] Short description (80 chars)
- [ ] Full description (4000 chars)
- [ ] Keywords for ASO
- [ ] Screenshots for all device sizes
- [ ] Privacy policy URL

**Privacy Policy Requirements**:
- No data collection
- No third-party analytics
- All data stored locally
- Optional cloud sync uses user's own accounts

---

### P1-APP-003: iOS Build Configuration

**Status**: `[ ]` Not Started

**Acceptance Criteria**:
- [ ] Bundle ID configured
- [ ] Signing certificates and provisioning profiles
- [ ] Archive builds successfully
- [ ] TestFlight submission prepared

---

### P1-APP-004: Android Build Configuration

**Status**: `[ ]` Not Started

**Acceptance Criteria**:
- [ ] Package name configured
- [ ] Signing keystore created and secured
- [ ] AAB (Android App Bundle) builds successfully
- [ ] Google Play Console listing prepared

---

## 9. Phase 1 Quality Gates

### P1-QG-001: Test Coverage

**Status**: `[ ]` Not Started

**Acceptance Criteria**:
- [ ] Unit test coverage > 80% for Core project
- [ ] Integration tests for all repository operations
- [ ] UI tests for critical user flows (onboarding, scoring)
- [ ] All tests passing in CI

---

### P1-QG-002: Security Review

**Status**: `[ ]` Not Started

**Acceptance Criteria**:
- [ ] Complete security checklist from [07-security-review-guidelines.md](./07-security-review-guidelines.md)
- [ ] AI security analysis run on all source files
- [ ] No high or critical vulnerabilities
- [ ] Data storage encryption verified

---

### P1-QG-003: Accessibility Audit

**Status**: `[ ]` Not Started

**Acceptance Criteria**:
- [ ] All pages pass automated accessibility checks
- [ ] Manual VoiceOver testing on iOS
- [ ] Manual TalkBack testing on Android
- [ ] Color contrast verified (4.5:1 minimum)
- [ ] Touch targets >= 44x44 points

---

### P1-QG-004: Performance Benchmarks

**Status**: `[ ]` Not Started

**Acceptance Criteria**:
- [ ] App startup < 2 seconds
- [ ] Page navigation < 500ms
- [ ] Memory usage < 150MB typical
- [ ] Battery impact negligible (no background activity)

---

### P1-QG-005: Documentation

**Status**: `[ ]` Not Started

**Acceptance Criteria**:
- [ ] All public APIs have XML documentation
- [ ] README updated with build instructions
- [ ] Architecture decisions documented

---

## Phase 1 Summary

| Metric | Target |
|--------|--------|
| Total Tasks | ~40 |
| Test Coverage | > 80% |
| Accessibility | WCAG 2.1 AA |
| Security Review | Pass |
| Platforms | iOS 15+, Android 10+ |

Upon completion of all quality gates, proceed to [03-phase2-budget-desktop.md](./03-phase2-budget-desktop.md).

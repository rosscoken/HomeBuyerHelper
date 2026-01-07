# 04 - Phase 3: Advanced Analysis + Sync

This phase adds advanced analytical features including commute time value analysis, funding strategy calculator with tax implications, and optional cloud sync.

---

## Phase Overview

| Aspect | Details |
|--------|---------|
| **Goal** | Deep analysis tools and cross-device sync capabilities |
| **New Features** | Commute monetization, funding sources, tax impact, cloud backup |
| **Cloud Providers** | iCloud, Google Drive, OneDrive (user's own accounts) |
| **Quality Gates** | All tests passing, security review (critical for sync), accessibility audit |

---

## Task Sections

1. [Commute Time Value Analysis](#1-commute-time-value-analysis)
2. [True Total Cost Calculation](#2-true-total-cost-calculation)
3. [Funding Strategy Calculator](#3-funding-strategy-calculator)
4. [Tax Impact Analysis](#4-tax-impact-analysis)
5. [Cloud Sync (Optional)](#5-cloud-sync-optional)
6. [Photo Attachments](#6-photo-attachments)
7. [Notes and Pros/Cons](#7-notes-and-proscons)
8. [Phase 3 Quality Gates](#8-phase-3-quality-gates)

---

## 1. Commute Time Value Analysis

### P3-COM-001: Commute Configuration

**Status**: `[ ]` Not Started

**Dependencies**: Phase 2 complete

**Acceptance Criteria**:
- [ ] Global settings for commute calculation
- [ ] Fields: Work address, Hourly rate for time value, Workdays per month
- [ ] Per-property commute time entry (round-trip minutes)

**Settings Fields**:
| Field | Default | Description |
|-------|---------|-------------|
| Work Address | (required) | Primary commute destination |
| Time Value Rate | $100/hour | What user values their time at |
| Workdays per Month | 22 | Standard working days |

**Files to Create**:
- `src/HomeBuyerHelper/Pages/Settings/CommuteSettingsPage.xaml`
- `src/HomeBuyerHelper.Core/Models/CommuteConfig.cs`

---

### P3-COM-002: Per-Property Commute Entry

**Status**: `[ ]` Not Started

**Dependencies**: P3-COM-001

**Acceptance Criteria**:
- [ ] Add commute time field to property entry/edit
- [ ] Round-trip minutes input
- [ ] Optional: Different commute for partner
- [ ] Display commute time on property card

**Fields on Property**:
```csharp
public int? CommuteMinutesPrimary { get; set; }
public int? CommuteMinutesSecondary { get; set; }  // Partner's commute
```

---

### P3-COM-003: Commute Value Calculation Service

**Status**: `[ ]` Not Started

**Dependencies**: P3-COM-002

**Acceptance Criteria**:
- [ ] Calculate monthly commute time value
- [ ] Calculate annual and 30-year projections
- [ ] Calculate hours and "full days" spent commuting per year
- [ ] Compare properties by commute cost

**Formula**:
```
Monthly Commute Value = (Round-Trip Minutes × Workdays × Hourly Rate) / 60
Annual = Monthly × 12
30-Year = Annual × 30

Hours per Year = (Round-Trip Minutes × Workdays × 12) / 60
Days per Year = Hours per Year / 8
```

**Files to Create**:
- `src/HomeBuyerHelper.Core/Services/CommuteValueService.cs`

**Test Requirements**:
- [ ] Unit test: Value calculation accuracy
- [ ] Unit test: Comparison between properties
- [ ] Unit test: Edge cases (0 commute, remote work)

---

### P3-COM-004: Commute Analysis Display

**Status**: `[ ]` Not Started

**Dependencies**: P3-COM-003

**Acceptance Criteria**:
- [ ] Property detail shows commute analysis section
- [ ] Displays: Monthly/Annual/30-year time value
- [ ] Displays: Hours and days lost per year
- [ ] Color coding based on commute length

**Commute Zones**:
| Commute (RT) | Zone | Color |
|--------------|------|-------|
| < 30 min | Excellent | Green |
| 30-60 min | Moderate | Yellow |
| 60-90 min | Long | Orange |
| > 90 min | Very Long | Red |

---

### P3-COM-005: Commute Comparison View

**Status**: `[ ]` Not Started

**Dependencies**: P3-COM-004

**Acceptance Criteria**:
- [ ] Side-by-side commute comparison of all properties
- [ ] Show difference from baseline (shortest commute)
- [ ] Monetized difference highlighted
- [ ] Chart visualization of commute costs

---

## 2. True Total Cost Calculation

### P3-TTC-001: True Total Cost Service

**Status**: `[ ]` Not Started

**Dependencies**: P3-COM-003, Phase 1 Cost Calculator

**Acceptance Criteria**:
- [ ] Combine housing costs + commute time value
- [ ] Calculate monthly and 30-year totals
- [ ] Handle optional components gracefully

**Components**:
| Component | Source |
|-----------|--------|
| Mortgage P&I | Phase 1 calculator |
| Property Taxes | Phase 1 calculator |
| Insurance | Phase 1 calculator |
| HOA | Property entry |
| PMI | Phase 1 calculator |
| Utilities | User estimate or default |
| Commute Time Value | P3-COM-003 |

**Files to Create**:
- `src/HomeBuyerHelper.Core/Services/TrueTotalCostService.cs`

**Test Requirements**:
- [ ] Unit test: All component aggregation
- [ ] Unit test: Missing component handling

---

### P3-TTC-002: True Total Cost Display

**Status**: `[ ]` Not Started

**Dependencies**: P3-TTC-001

**Acceptance Criteria**:
- [ ] Property detail shows True Total Cost section
- [ ] Breakdown of all components
- [ ] Monthly and 30-year totals
- [ ] Expandable detail view

---

### P3-TTC-003: True Total Cost Comparison

**Status**: `[ ]` Not Started

**Dependencies**: P3-TTC-002

**Acceptance Criteria**:
- [ ] Add True Total Cost column to comparison matrix
- [ ] Rank properties by true cost (option)
- [ ] Highlight cost differences

---

## 3. Funding Strategy Calculator

### P3-FUN-001: Funding Sources Setup Page

**Status**: `[ ]` Not Started

**Dependencies**: Phase 2 complete

**Acceptance Criteria**:
- [ ] Page to configure down payment funding sources
- [ ] Add/Edit/Delete funding sources
- [ ] Summary of gross vs. net amounts
- [ ] Navigation from budget/settings

**Files to Create**:
- `src/HomeBuyerHelper/Pages/Funding/FundingSetupPage.xaml`
- `src/HomeBuyerHelper/ViewModels/Funding/FundingSetupViewModel.cs`
- `src/HomeBuyerHelper.Core/Models/FundingSource.cs`

---

### P3-FUN-002: Tax Bracket Configuration

**Status**: `[ ]` Not Started

**Dependencies**: P3-FUN-001

**Acceptance Criteria**:
- [ ] Screen to configure user's tax situation
- [ ] Fields: Filing status, Taxable income, State
- [ ] Calculate marginal federal and state brackets
- [ ] Manual override option

**Form Fields**:
| Field | Options |
|-------|---------|
| Filing Status | Single, MFJ, MFS, HoH |
| Estimated Taxable Income | Currency input |
| State | US States dropdown |

---

### P3-FUN-003: Savings Account Source

**Status**: `[ ]` Not Started

**Dependencies**: P3-FUN-002

**Acceptance Criteria**:
- [ ] Form for savings account funding
- [ ] Fields: Amount to use, Account nickname
- [ ] Tax impact: None

**Files to Create**:
- `src/HomeBuyerHelper/Pages/Funding/SavingsSourcePage.xaml`

---

### P3-FUN-004: Brokerage Account Source

**Status**: `[ ]` Not Started

**Dependencies**: P3-FUN-002

**Acceptance Criteria**:
- [ ] Form for brokerage account funding
- [ ] Fields: Amount to sell, Cost basis, Holding period, Nickname
- [ ] Calculate capital gains tax
- [ ] Show short-term vs long-term rates

**Tax Calculation**:
```csharp
Gain = AmountToSell - CostBasis;
if (HoldingPeriod < 12 months)
    Tax = Gain * MarginalIncomeTaxRate;
else
    Tax = Gain * LongTermCapGainsRate;
```

**Test Requirements**:
- [ ] Unit test: Short-term gains calculation
- [ ] Unit test: Long-term gains calculation
- [ ] Unit test: Loss handling

---

### P3-FUN-005: Traditional IRA Source

**Status**: `[ ]` Not Started

**Dependencies**: P3-FUN-002

**Acceptance Criteria**:
- [ ] Form for Traditional IRA withdrawal
- [ ] Fields: Amount, First-time buyer status, Age
- [ ] Calculate income tax + 10% penalty
- [ ] Apply first-time buyer exemption ($10k)

**Tax Calculation**:
```csharp
IncomeTax = Amount * MarginalTaxRate;
if (Age < 59.5 && !FirstTimeBuyer)
    Penalty = Amount * 0.10;
else if (Age < 59.5 && FirstTimeBuyer)
    Penalty = Math.Max(0, Amount - 10000) * 0.10;
```

---

### P3-FUN-006: Inherited IRA Source

**Status**: `[ ]` Not Started

**Dependencies**: P3-FUN-002

**Acceptance Criteria**:
- [ ] Form for Inherited IRA withdrawal
- [ ] Fields: Amount, Year inherited, Remaining balance
- [ ] Calculate income tax (no penalty)
- [ ] Note about 10-year rule

---

### P3-FUN-007: Roth IRA Source

**Status**: `[ ]` Not Started

**Dependencies**: P3-FUN-002

**Acceptance Criteria**:
- [ ] Form for Roth IRA withdrawal
- [ ] Fields: Total contributions, Amount from contributions, Amount from earnings, Account age, First-time buyer
- [ ] Contributions: Tax-free
- [ ] Earnings: May be taxable + penalty depending on rules

**Tax Rules**:
- Contributions always tax-free, no penalty
- Earnings: Tax-free if account 5+ years AND (age 59.5+ OR first-time buyer up to $10k)

---

### P3-FUN-008: Family Gift Source

**Status**: `[ ]` Not Started

**Dependencies**: P3-FUN-002

**Acceptance Criteria**:
- [ ] Form for family gift
- [ ] Fields: Amount, Donor name, Relationship, Expected receipt date
- [ ] Tax impact: None to recipient
- [ ] Note about gift letter and seasoning

---

### P3-FUN-009: 401(k) Source

**Status**: `[ ]` Not Started

**Dependencies**: P3-FUN-002

**Acceptance Criteria**:
- [ ] Form for 401(k) loan or withdrawal
- [ ] Loan fields: Amount, Repayment term, Interest rate
- [ ] Withdrawal fields: Amount
- [ ] Loan: No tax if repaid
- [ ] Withdrawal: Income tax + penalty

---

### P3-FUN-010: Funding Summary Page

**Status**: `[ ]` Not Started

**Dependencies**: P3-FUN-003 through P3-FUN-009

**Acceptance Criteria**:
- [ ] Summary table: Source | Gross | Est. Tax | Net
- [ ] Total row with aggregates
- [ ] Comparison to target down payment + closing costs
- [ ] Surplus/deficit indicator

---

## 4. Tax Impact Analysis

### P3-TAX-001: Tax Impact Service

**Status**: `[ ]` Not Started

**Dependencies**: P3-FUN-002, All funding source forms

**Acceptance Criteria**:
- [ ] Calculate total tax liability from all funding sources
- [ ] Aggregate by tax type (income, capital gains, penalties)
- [ ] Provide optimal ordering recommendation

**Files to Create**:
- `src/HomeBuyerHelper.Core/Services/TaxImpactService.cs`

**Test Requirements**:
- [ ] Unit test: Each source type tax calculation
- [ ] Unit test: Aggregation accuracy
- [ ] Integration test: Full funding scenario

---

### P3-TAX-002: Tax Payment Planning

**Status**: `[ ]` Not Started

**Dependencies**: P3-TAX-001

**Acceptance Criteria**:
- [ ] Display when taxes will be due
- [ ] Capital gains and IRA: April of following year
- [ ] Estimated payment requirement if large amount
- [ ] Suggested action: Set aside amount

---

### P3-TAX-003: Tax Integration with Budget

**Status**: `[ ]` Not Started

**Dependencies**: P3-TAX-002, Phase 2 Budget

**Acceptance Criteria**:
- [ ] Option to add tax set-aside to budget
- [ ] One-time event for estimated tax payment
- [ ] Impact shown in cash flow projection

---

## 5. Cloud Sync (Optional)

### P3-SYN-001: Sync Settings Page

**Status**: `[ ]` Not Started

**Dependencies**: Phase 2 complete

**Acceptance Criteria**:
- [ ] Settings page for optional cloud sync
- [ ] Clear privacy messaging
- [ ] Provider selection: None, iCloud, Google Drive, OneDrive
- [ ] Enable/disable sync toggle

**Files to Create**:
- `src/HomeBuyerHelper/Pages/Settings/SyncSettingsPage.xaml`
- `src/HomeBuyerHelper.Core/Interfaces/ICloudSyncService.cs`

---

### P3-SYN-002: iCloud Sync (Apple Devices)

**Status**: `[ ]` Not Started

**Dependencies**: P3-SYN-001

**Acceptance Criteria**:
- [ ] Sync database to user's iCloud Drive
- [ ] Available on iOS and macOS only
- [ ] Use iCloud Documents container
- [ ] Conflict resolution: Last-write-wins with prompt

**Files to Create**:
- `src/HomeBuyerHelper/Services/ICloudSyncService.cs`

**Security Requirements**:
- [ ] Data encrypted at rest (iCloud default)
- [ ] No data sent to our servers
- [ ] User must sign in with their Apple ID

---

### P3-SYN-003: Google Drive Sync (Android)

**Status**: `[ ]` Not Started

**Dependencies**: P3-SYN-001

**Acceptance Criteria**:
- [ ] Sync database to user's Google Drive
- [ ] Android only
- [ ] Use app-specific folder
- [ ] OAuth authentication

**Security Requirements**:
- [ ] Use Google Sign-In
- [ ] App-specific folder for isolation
- [ ] User controls their own data

---

### P3-SYN-004: OneDrive Sync (Windows)

**Status**: `[ ]` Not Started

**Dependencies**: P3-SYN-001

**Acceptance Criteria**:
- [ ] Sync database to user's OneDrive
- [ ] Windows only
- [ ] Use OneDrive API
- [ ] Microsoft account authentication

---

### P3-SYN-005: Sync Conflict Resolution

**Status**: `[ ]` Not Started

**Dependencies**: P3-SYN-002, P3-SYN-003, P3-SYN-004

**Acceptance Criteria**:
- [ ] Detect conflicting changes
- [ ] Prompt user to choose: Keep Local, Keep Cloud, Merge
- [ ] Merge strategy: Union of properties, latest scores
- [ ] Backup created before resolution

---

### P3-SYN-006: Manual Backup to Cloud

**Status**: `[ ]` Not Started

**Dependencies**: P3-SYN-001

**Acceptance Criteria**:
- [ ] Manual "Backup Now" button
- [ ] Works even without auto-sync enabled
- [ ] Uses platform share sheet for flexibility

---

## 6. Photo Attachments

### P3-PHO-001: Photo Capture and Storage

**Status**: `[ ]` Not Started

**Dependencies**: Phase 2 complete

**Acceptance Criteria**:
- [ ] Add photos to properties
- [ ] Capture from camera (mobile)
- [ ] Select from gallery
- [ ] Store locally, compressed

**Files to Create**:
- `src/HomeBuyerHelper.Core/Services/PhotoService.cs`
- `src/HomeBuyerHelper.Core/Models/PropertyPhoto.cs`

**Storage Considerations**:
- Compress images to ~500KB
- Store in app's local data folder
- Reference by property ID

---

### P3-PHO-002: Photo Gallery View

**Status**: `[ ]` Not Started

**Dependencies**: P3-PHO-001

**Acceptance Criteria**:
- [ ] Property detail shows photo thumbnail gallery
- [ ] Tap to view full-screen
- [ ] Swipe to navigate between photos
- [ ] Delete option per photo

---

### P3-PHO-003: Photo Sync (Optional)

**Status**: `[ ]` Not Started

**Dependencies**: P3-PHO-001, P3-SYN-001

**Acceptance Criteria**:
- [ ] Option to include photos in sync
- [ ] Warning about storage usage
- [ ] Exclude photos option to save space

---

## 7. Notes and Pros/Cons

### P3-NOT-001: Property Notes

**Status**: `[ ]` Not Started

**Dependencies**: Phase 2 complete

**Acceptance Criteria**:
- [ ] Rich text notes per property
- [ ] Save automatically on edit
- [ ] Searchable across all properties
- [ ] Timestamp for last edited

---

### P3-NOT-002: Pros/Cons Lists

**Status**: `[ ]` Not Started

**Dependencies**: P3-NOT-001

**Acceptance Criteria**:
- [ ] Separate pros and cons lists per property
- [ ] Add/Edit/Delete items
- [ ] Display summary on property card (count)
- [ ] Compare pros/cons across properties

**Model**:
```csharp
public class PropertyProCon
{
    public int Id { get; set; }
    public int PropertyId { get; set; }
    public bool IsPro { get; set; }
    public string Description { get; set; }
    public int SortOrder { get; set; }
}
```

---

### P3-NOT-003: Notes in Comparison

**Status**: `[ ]` Not Started

**Dependencies**: P3-NOT-002

**Acceptance Criteria**:
- [ ] View mode in comparison showing pros/cons
- [ ] Side-by-side display
- [ ] Highlight unique pros (only in one property)

---

## 8. Phase 3 Quality Gates

### P3-QG-001: Test Coverage

**Status**: `[ ]` Not Started

**Acceptance Criteria**:
- [ ] Unit tests for all calculation services
- [ ] Integration tests for cloud sync
- [ ] Tax calculation accuracy validation
- [ ] Photo handling tests

---

### P3-QG-002: Security Review (Critical)

**Status**: `[ ]` Not Started

**Acceptance Criteria**:
- [ ] Cloud sync security audit
- [ ] OAuth implementation review
- [ ] Data encryption verification
- [ ] No credentials stored in plaintext
- [ ] No data leakage to our servers
- [ ] AI security analysis on sync services

**Cloud Security Checklist**:
- [ ] Tokens stored securely (Keychain/Keystore)
- [ ] HTTPS only for all cloud communication
- [ ] Token refresh handled properly
- [ ] Logout clears all tokens

---

### P3-QG-003: Accessibility Audit

**Status**: `[ ]` Not Started

**Acceptance Criteria**:
- [ ] Photo gallery accessible
- [ ] Funding forms accessible
- [ ] Sync settings accessible
- [ ] All new pages audited

---

### P3-QG-004: Performance Benchmarks

**Status**: `[ ]` Not Started

**Acceptance Criteria**:
- [ ] Tax calculation < 50ms
- [ ] Photo load < 500ms
- [ ] Sync operation < 30s for typical data
- [ ] No UI blocking during sync

---

### P3-QG-005: Privacy Verification

**Status**: `[ ]` Not Started

**Acceptance Criteria**:
- [ ] No analytics collected
- [ ] Cloud sync uses only user's accounts
- [ ] No server-side storage of user data
- [ ] Privacy policy updated for sync features

---

## Phase 3 Summary

| Metric | Target |
|--------|--------|
| Total Tasks | ~35 |
| New Features | Commute Analysis, Funding, Sync, Photos |
| Test Coverage | > 80% |
| Security Review | Pass (Critical for cloud) |
| Accessibility | WCAG 2.1 AA |

Upon completion of all quality gates, proceed to [05-phase4-polish-sharing.md](./05-phase4-polish-sharing.md).

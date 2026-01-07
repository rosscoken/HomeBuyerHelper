# 05 - Phase 4: Polish + Sharing

This phase focuses on quality-of-life improvements, collaboration features, tablet optimization, and platform polish including dark mode support.

---

## Phase Overview

| Aspect | Details |
|--------|---------|
| **Goal** | Production-ready polish and collaboration features |
| **New Features** | Dropbox sync, sharing, Excel export, rent vs buy, tablet layouts, dark mode |
| **Quality Focus** | Performance optimization, edge case handling, UX refinement |
| **Quality Gates** | All tests passing, security review complete, accessibility audit passed |

---

## Task Sections

1. [Cross-Platform Sync](#1-cross-platform-sync)
2. [Sharing Features](#2-sharing-features)
3. [Enhanced Exports](#3-enhanced-exports)
4. [Rent vs Buy Calculator](#4-rent-vs-buy-calculator)
5. [Scenario Planning](#5-scenario-planning)
6. [Tablet Optimization](#6-tablet-optimization)
7. [Dark Mode](#7-dark-mode)
8. [Template Sharing](#8-template-sharing)
9. [Phase 4 Quality Gates](#9-phase-4-quality-gates)

---

## 1. Cross-Platform Sync

### P4-SYN-001: Dropbox Integration

**Status**: `[ ]` Not Started

**Dependencies**: Phase 3 Sync Foundation

**Acceptance Criteria**:
- [ ] Dropbox SDK integration
- [ ] OAuth authentication flow
- [ ] Sync to user's Dropbox account
- [ ] Works on all four platforms
- [ ] App-specific folder isolation

**Implementation Notes**:
- Use Dropbox .NET SDK
- Store tokens in platform-secure storage
- Implement same conflict resolution as other providers

**Files to Create**:
- `src/HomeBuyerHelper/Services/DropboxSyncService.cs`

**Test Requirements**:
- [ ] Integration test: Auth flow
- [ ] Integration test: Upload/download
- [ ] Unit test: Conflict detection

---

### P4-SYN-002: Cross-Platform Sync Selection

**Status**: `[ ]` Not Started

**Dependencies**: P4-SYN-001, Phase 3 Sync

**Acceptance Criteria**:
- [ ] Any platform can use Dropbox
- [ ] Clear indication of which provider syncs to which platform
- [ ] Warning when switching providers

**Provider Availability Matrix**:
| Provider | iOS | Android | Windows | macOS |
|----------|-----|---------|---------|-------|
| iCloud | Yes | No | No | Yes |
| Google Drive | No | Yes | No | No |
| OneDrive | No | No | Yes | No |
| Dropbox | Yes | Yes | Yes | Yes |

---

### P4-SYN-003: Sync Status Indicator

**Status**: `[ ]` Not Started

**Dependencies**: P4-SYN-002

**Acceptance Criteria**:
- [ ] Visual indicator of sync status in header/toolbar
- [ ] States: Synced, Syncing, Offline, Error
- [ ] Tap for details and manual sync option
- [ ] Last synced timestamp

---

## 2. Sharing Features

### P4-SHR-001: Export for Sharing

**Status**: `[ ]` Not Started

**Dependencies**: Phase 3 PDF Reports

**Acceptance Criteria**:
- [ ] Generate read-only shareable export
- [ ] Options: PDF, Interactive HTML
- [ ] Includes all properties and comparisons
- [ ] Does not include sensitive financial details (configurable)

---

### P4-SHR-002: Share via Platform Sheet

**Status**: `[ ]` Not Started

**Dependencies**: P4-SHR-001

**Acceptance Criteria**:
- [ ] Use native share sheet (iOS, Android, Windows, macOS)
- [ ] Share PDF or HTML file
- [ ] Email, Messages, AirDrop, etc.

---

### P4-SHR-003: Configurable Privacy in Shares

**Status**: `[ ]` Not Started

**Dependencies**: P4-SHR-001

**Acceptance Criteria**:
- [ ] Options for what to include in shared exports
- [ ] Include/exclude: Prices, Scores, Financial Details, Notes
- [ ] Preview before sharing

**Privacy Options**:
| Option | Default | Description |
|--------|---------|-------------|
| Property Prices | Yes | Include asking prices |
| Score Details | Yes | Include individual criterion scores |
| Financial Projections | No | Include budget/cash flow |
| Notes & Pros/Cons | Yes | Include notes |
| Personal Income | No | Never include |

---

### P4-SHR-004: Partner Collaboration (Read-Only)

**Status**: `[ ]` Not Started

**Dependencies**: P4-SHR-002

**Acceptance Criteria**:
- [ ] Generate unique shareable link (local only, not cloud)
- [ ] Recipient can view but not edit
- [ ] Link expires after 7 days
- [ ] Works offline via file sharing

**Implementation Notes**:
- Generate encrypted HTML file
- Embed data in the file itself
- No server required

---

## 3. Enhanced Exports

### P4-EXP-001: Excel/CSV Export

**Status**: `[ ]` Not Started

**Dependencies**: Phase 2 PDF Reports

**Acceptance Criteria**:
- [ ] Export comparison matrix to Excel (.xlsx)
- [ ] Export cash flow projection to Excel
- [ ] Formulas preserved where applicable
- [ ] Alternative: CSV for maximum compatibility

**Sheets in Excel Export**:
1. Properties - All property details
2. Comparison Matrix - Scores and rankings
3. Monthly Costs - Financial breakdown
4. Cash Flow - 24-month projection
5. Criteria - Definitions and weights

**Files to Create**:
- `src/HomeBuyerHelper.Core/Exports/ExcelExporter.cs`

**Library**: Consider ClosedXML or NPOI for Excel generation

---

### P4-EXP-002: Export Scheduling

**Status**: `[ ]` Not Started

**Dependencies**: P4-EXP-001

**Acceptance Criteria**:
- [ ] Option to auto-export weekly backup
- [ ] Save to user-selected location
- [ ] Keep last N backups (configurable, default 4)

---

## 4. Rent vs Buy Calculator

### P4-RVB-001: Rent vs Buy Model

**Status**: `[ ]` Not Started

**Dependencies**: Phase 3 True Total Cost

**Acceptance Criteria**:
- [ ] Compare buying a property to renting
- [ ] Inputs: Current rent, rent increase rate, investment return rate
- [ ] Calculate breakeven point in years
- [ ] Show total wealth comparison over time

**Inputs**:
| Field | Default |
|-------|---------|
| Current Monthly Rent | (required) |
| Annual Rent Increase | 3% |
| Investment Return Rate | 7% |
| Home Appreciation Rate | 3% |
| Planning Horizon | 10 years |

**Outputs**:
- Monthly cost comparison
- Total cost over horizon
- Net wealth comparison (equity vs. invested savings)
- Breakeven year

**Files to Create**:
- `src/HomeBuyerHelper.Core/Calculations/RentVsBuyCalculator.cs`
- `src/HomeBuyerHelper/Pages/Analysis/RentVsBuyPage.xaml`

**Test Requirements**:
- [ ] Unit test: Known calculation scenarios
- [ ] Unit test: Edge cases (negative equity, high rent)

---

### P4-RVB-002: Rent vs Buy Visualization

**Status**: `[ ]` Not Started

**Dependencies**: P4-RVB-001

**Acceptance Criteria**:
- [ ] Line chart showing wealth over time
- [ ] Two lines: Rent scenario, Buy scenario
- [ ] Breakeven point highlighted
- [ ] Interactive slider for horizon

---

## 5. Scenario Planning

### P5-SCN-001: "What If" Scenarios

**Status**: `[ ]` Not Started

**Dependencies**: Phase 3 Budget

**Acceptance Criteria**:
- [ ] Create named scenarios
- [ ] Adjust: Interest rate, Down payment, Price, Closing date
- [ ] Compare multiple scenarios side-by-side
- [ ] Default: "Base Case" from current settings

**Scenario Parameters**:
- Interest Rate
- Down Payment %
- Purchase Price (can adjust from asking)
- Closing Date
- Income Scenario (Conservative/Realistic/Expected)

---

### P5-SCN-002: Scenario Comparison View

**Status**: `[ ]` Not Started

**Dependencies**: P5-SCN-001

**Acceptance Criteria**:
- [ ] Table comparing scenarios
- [ ] Columns: Scenario Name, Monthly Payment, 30-Year Cost, Affordability
- [ ] Highlight best scenario per metric
- [ ] Quick duplicate and modify

---

### P5-SCN-003: Time-Based Scenarios

**Status**: `[ ]` Not Started

**Dependencies**: P5-SCN-001

**Acceptance Criteria**:
- [ ] "What if we wait 6 months?" analysis
- [ ] Adjust for: Continued rent, Savings growth, Market changes
- [ ] Show impact on total cost

---

## 6. Tablet Optimization

### P4-TAB-001: iPad Optimized Layouts

**Status**: `[ ]` Not Started

**Dependencies**: Phase 2 complete

**Acceptance Criteria**:
- [ ] Use full iPad screen width
- [ ] Split-view support (property list + detail)
- [ ] Master-detail pattern for large screens
- [ ] Landscape mode support

**Layout Adaptations**:
| Screen | Phone | Tablet |
|--------|-------|--------|
| Property List | Single column | 2-3 column grid |
| Property Detail | Stacked sections | Side-by-side panels |
| Comparison | Horizontal scroll | Full table visible |
| Settings | Full-width | Sidebar + content |

**Files to Create**:
- Create adaptive layouts using MAUI OnIdiom/OnPlatform

---

### P4-TAB-002: Android Tablet Layouts

**Status**: `[ ]` Not Started

**Dependencies**: P4-TAB-001

**Acceptance Criteria**:
- [ ] Same optimizations for Android tablets
- [ ] Foldable device support
- [ ] Test on Samsung Galaxy Fold

---

### P4-TAB-003: Stylus/Pencil Support

**Status**: `[ ]` Not Started

**Dependencies**: P4-TAB-001

**Acceptance Criteria**:
- [ ] Apple Pencil hover states
- [ ] Quick notes via stylus (draw on property photos)
- [ ] Smooth scrolling and selection

---

## 7. Dark Mode

### P4-DRK-001: Dark Mode Theme

**Status**: `[ ]` Not Started

**Dependencies**: Phase 2 complete

**Acceptance Criteria**:
- [ ] Complete dark color palette
- [ ] All screens support dark mode
- [ ] Charts and visualizations adapt
- [ ] Maintains accessibility contrast ratios

**Color Palette**:
| Element | Light Mode | Dark Mode |
|---------|------------|-----------|
| Background | #FFFFFF | #1C1C1E |
| Surface | #F2F2F7 | #2C2C2E |
| Primary Text | #000000 | #FFFFFF |
| Secondary Text | #6B6B6B | #EBEBF5 |
| Accent | #007AFF | #0A84FF |
| Success | #34C759 | #30D158 |
| Warning | #FF9500 | #FF9F0A |
| Error | #FF3B30 | #FF453A |

**Files to Update**:
- `src/HomeBuyerHelper/Resources/Styles/Colors.xaml`
- `src/HomeBuyerHelper/Resources/Styles/Styles.xaml`

---

### P4-DRK-002: Theme Selection

**Status**: `[ ]` Not Started

**Dependencies**: P4-DRK-001

**Acceptance Criteria**:
- [ ] Settings: System Default, Light, Dark
- [ ] Follow system setting by default
- [ ] Persist user preference
- [ ] Smooth transition on change

---

### P4-DRK-003: Dark Mode Charts

**Status**: `[ ]` Not Started

**Dependencies**: P4-DRK-001

**Acceptance Criteria**:
- [ ] LiveCharts2 themes for dark mode
- [ ] PDF export respects current theme
- [ ] Screenshots capture correct theme

---

## 8. Template Sharing

### P4-TMP-001: Export Criteria Template

**Status**: `[ ]` Not Started

**Dependencies**: Phase 1 Criteria

**Acceptance Criteria**:
- [ ] Export criteria as standalone template file
- [ ] Excludes personal data (properties, scores)
- [ ] Includes: Names, Weights, Categories, Score Anchors
- [ ] JSON format with schema version

**Template Format**:
```json
{
  "version": "1.0",
  "name": "Remote Worker Criteria",
  "description": "For hybrid/remote workers evaluating homes",
  "criteria": [
    {
      "name": "Home Office Space",
      "weight": 0.25,
      "category": "Work",
      "scoreAnchors": {
        "low": "No dedicated space",
        "mid": "Shared room",
        "high": "Dedicated room with good light"
      }
    }
  ]
}
```

**Files to Create**:
- `src/HomeBuyerHelper.Core/Templates/CriteriaTemplate.cs`
- `src/HomeBuyerHelper.Core/Templates/TemplateExporter.cs`

---

### P4-TMP-002: Import Criteria Template

**Status**: `[ ]` Not Started

**Dependencies**: P4-TMP-001

**Acceptance Criteria**:
- [ ] Import template from file
- [ ] Options: Replace current, Merge with current
- [ ] Preview before import
- [ ] Validate schema version

---

### P4-TMP-003: Built-in Template Library

**Status**: `[ ]` Not Started

**Dependencies**: P4-TMP-001

**Acceptance Criteria**:
- [ ] Expanded library of built-in templates
- [ ] Categories: First-Time, Family, Investment, Remote Work, Urban, Suburban, Rural
- [ ] Description and use case for each
- [ ] One-tap apply with confirmation

**New Templates**:
| Template | Focus |
|----------|-------|
| Urban Condo | Walkability, transit, amenities |
| Suburban Family | Schools, yard, safety |
| Rural Retreat | Land, privacy, self-sufficiency |
| Downsizer | Single-level, maintenance, accessibility |
| Multi-Gen | Separate entrances, privacy, dual living |

---

## 9. Phase 4 Quality Gates

### P4-QG-001: Test Coverage

**Status**: `[ ]` Not Started

**Acceptance Criteria**:
- [ ] Unit test coverage > 80% overall
- [ ] Integration tests for all sync providers
- [ ] UI tests for tablet layouts
- [ ] Dark mode visual regression tests

---

### P4-QG-002: Security Review

**Status**: `[ ]` Not Started

**Acceptance Criteria**:
- [ ] Dropbox OAuth security audit
- [ ] Sharing feature security review
- [ ] Template import validation (no code injection)
- [ ] Final AI security scan of entire codebase

---

### P4-QG-003: Accessibility Audit

**Status**: `[ ]` Not Started

**Acceptance Criteria**:
- [ ] Dark mode maintains contrast ratios
- [ ] Tablet layouts fully accessible
- [ ] Share sheet accessible
- [ ] Final WCAG 2.1 AA certification

---

### P4-QG-004: Performance Benchmarks

**Status**: `[ ]` Not Started

**Acceptance Criteria**:
- [ ] App startup < 2s on all platforms
- [ ] Sync operations don't block UI
- [ ] Large data sets (100+ properties) handled smoothly
- [ ] Memory usage stable over time

---

### P4-QG-005: App Store Updates

**Status**: `[ ]` Not Started

**Acceptance Criteria**:
- [ ] All four app stores updated
- [ ] New screenshots including dark mode
- [ ] Release notes for all new features
- [ ] No new review issues

---

### P4-QG-006: Documentation Finalization

**Status**: `[ ]` Not Started

**Acceptance Criteria**:
- [ ] User guide updated for all features
- [ ] FAQ covering common questions
- [ ] Privacy policy current
- [ ] API documentation for templates

---

## Phase 4 Summary

| Metric | Target |
|--------|--------|
| Total Tasks | ~30 |
| New Features | Dropbox, Sharing, Excel, Dark Mode, Tablets |
| Test Coverage | > 80% |
| Security Review | Pass (Full audit) |
| Accessibility | WCAG 2.1 AA Certified |
| Platforms | All 4 stores updated |

---

## Post-Phase 4: Maintenance Mode

After Phase 4 completion, the application enters maintenance mode:

1. **Bug Fixes**: Address user-reported issues
2. **OS Updates**: Support new iOS/Android/Windows/macOS versions
3. **Security Patches**: Address any discovered vulnerabilities
4. **Performance**: Ongoing optimization based on telemetry (opt-in)

---

## Future Considerations (Year 2+)

See Design Spec Section 8.5 for future roadmap items:

- Listing URL parsing for auto-fill
- Mortgage calculator API integration
- Community-contributed templates
- Localization for international markets
- Widget support (iOS, Android, Windows)
- BYOM AI document review

These features would be planned in a future implementation phase.

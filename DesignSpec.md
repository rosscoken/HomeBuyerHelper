# HomeBuyerHelper Design Document

## 1. Executive Summary

HomeBuyerHelper is a privacy-first, offline-capable native application designed to help home buyers make data-driven decisions throughout their home buying journey. The application combines property evaluation, financial planning, and budget management into a single tool that runs entirely on the user's device across iOS, Android, Windows, and macOS.

### 1.1 Problem Statement

Home buyers today face several challenges:

- Comparing properties involves juggling multiple factors with no systematic framework
- True costs of homeownership (including commute time value) are often overlooked
- Complex income structures (salary, bonus, RSU, etc.) make budget planning difficult
- Month-by-month cash flow impacts are hard to visualize during major transitions
- Existing tools require accounts, track user data, or require constant internet
- Home buying is a private decision - users don't want their search monitored

### 1.2 Solution Overview

The application provides three integrated modules, all running locally on the user's device:

1. **Property Evaluation Engine**: User-defined criteria with weighted scoring for objective comparison
2. **Annual Budget Planner**: Month-by-month cash flow projection with multiple income scenarios
3. **Funding Strategy Calculator**: Down payment source tracking with tax impact analysis

### 1.3 Key Differentiators

- **Native App**: True native application for iOS, Android, Windows, and macOS - not a web app
- **Offline-First**: Works without internet - use it at open houses, in basements, anywhere
- **Privacy-First**: No accounts required, no tracking, no data leaves your device
- **Customizable**: Users define their own evaluation criteria, not forced into preset categories
- **Guided Setup**: Quiz-style onboarding makes setup easy, no spreadsheet expertise needed
- **Cross-Platform**: Single codebase, consistent experience across all your devices

---

## 2. Property Evaluation Engine

The Property Evaluation Engine enables users to systematically compare multiple properties using a fully customizable weighted scoring system. Users define their own criteria based on what matters most to them.

### 2.1 Customizable Criteria System

Unlike rigid evaluation tools, users have complete control over what factors they evaluate and how much each factor weighs in their decision.

#### 2.1.1 How It Works

1. Users create their own evaluation criteria (or start from a template)
2. Each criterion gets a user-defined weight (weights must total 100%)
3. Users score each property on each criterion (1-10 scale)
4. The system calculates weighted scores and ranks properties

#### 2.1.2 Criterion Configuration

For each criterion users create, they specify:

| Field | Description |
|-------|-------------|
| **Name** | What the criterion is called (e.g., "Kitchen Quality", "School District") |
| **Weight** | How important this factor is (percentage of total score) |
| **Description** | Optional notes on what to consider when scoring |
| **Score Anchors** | Optional definitions for what 1, 5, and 10 mean for this criterion |
| **Category** | Optional grouping for organization (e.g., "Location", "Interior", "Financial") |

#### 2.1.3 Score Anchors (Optional)

Users can define what different scores mean for each criterion to ensure consistent scoring across properties:

| Score | Example: "Kitchen Quality" | Example: "Commute Time" |
|-------|---------------------------|------------------------|
| 1-2 | Needs complete renovation, outdated appliances | > 60 minutes each way |
| 3-4 | Functional but dated, some updates needed | 45-60 minutes each way |
| 5-6 | Acceptable, minor cosmetic updates desired | 30-45 minutes each way |
| 7-8 | Good quality, modern appliances, well maintained | 15-30 minutes each way |
| 9-10 | Dream kitchen, high-end finishes, everything perfect | < 15 minutes each way |

### 2.2 Starter Templates

To help users get started quickly, the application provides pre-built templates that can be customized:

#### 2.2.1 First-Time Buyer Template

| Criterion | Weight | Focus |
|-----------|--------|-------|
| Overall Feel / Gut Reaction | 20% | First impression, can you see yourself living here? |
| Location & Neighborhood | 20% | Safety, walkability, nearby amenities |
| Commute & Transportation | 15% | Time to work, public transit access |
| Space & Layout | 15% | Room sizes, flow, storage, natural light |
| Condition & Maintenance | 15% | Age, recent updates, anticipated repairs |
| Price & Value | 15% | Affordability, negotiation room, appreciation potential |

#### 2.2.2 Family-Focused Template

| Criterion | Weight | Focus |
|-----------|--------|-------|
| School District Quality | 25% | Ratings, programs, proximity to schools |
| Safety & Neighborhood | 20% | Crime rates, traffic, kid-friendly environment |
| Space for Family | 20% | Bedrooms, yard, play areas, storage |
| Family Amenities | 15% | Parks, activities, community, childcare options |
| Commute & Logistics | 10% | School drop-off, work commute, errands |
| Price & Long-term Value | 10% | Room to grow, resale potential |

#### 2.2.3 Investment-Focused Template

| Criterion | Weight | Focus |
|-----------|--------|-------|
| Cash Flow Potential | 25% | Rent vs. costs, positive cash flow likelihood |
| Appreciation Potential | 20% | Market trends, development plans, comparable sales |
| Rental Demand | 20% | Vacancy rates, tenant pool, location desirability |
| Condition & Capex | 15% | Repair needs, major systems age, turnkey status |
| Price vs. Market | 15% | Below market value, negotiation opportunity |
| Management Ease | 5% | HOA, proximity, tenant type |

#### 2.2.4 Remote Worker Template

| Criterion | Weight | Focus |
|-----------|--------|-------|
| Home Office Space | 25% | Dedicated room, natural light, video call background |
| Internet Infrastructure | 15% | Fiber availability, backup options, reliability |
| Noise & Privacy | 15% | Street noise, neighbor proximity, sound isolation |
| Overall Living Quality | 15% | Since you're home all day, does it feel good? |
| Neighborhood Amenities | 15% | Coffee shops, gyms, walking paths for breaks |
| Price & Value | 15% | Affordability, space per dollar |

### 2.3 Custom Criteria Builder

Users can create entirely custom criteria beyond the templates:

#### 2.3.1 Adding Custom Criteria

- Click "Add Criterion" to create a new evaluation factor
- Name it anything meaningful (e.g., "Dog Park Proximity", "Kitchen Counter Space")
- Assign a weight (system warns if weights don't total 100%)
- Optionally add score anchors to guide consistent scoring

#### 2.3.2 Weight Balancing Assistant

The application helps users balance their weights:

- Visual slider interface for adjusting weights
- Real-time pie chart showing weight distribution
- "Auto-balance" option to distribute remaining weight equally
- Warning if any single criterion exceeds 40% (concentration risk)

#### 2.3.3 Example Custom Criteria

Examples of criteria users might create for their specific needs:

- "Garage Size" - For car enthusiasts or those with multiple vehicles
- "Guest Parking" - For those who entertain frequently
- "Proximity to Parents" - For family caregiving considerations
- "HOA Restrictions" - For those concerned about rules/flexibility
- "Yard for Gardening" - For hobby gardeners
- "Basement Potential" - For future finishing/rental plans
- "EV Charging Ready" - For electric vehicle owners

### 2.4 Calculated vs. Subjective Criteria

The system supports two types of criteria:

#### 2.4.1 Subjective Criteria (User-Scored)

User manually assigns a 1-10 score based on their judgment:

- Overall feel / gut reaction
- Kitchen quality
- Neighborhood vibe
- Layout functionality

#### 2.4.2 Calculated Criteria (System-Generated)

Score automatically derived from data the user enters:

- Commute Time: Score based on minutes (user defines thresholds)
- Monthly Cost: Score based on affordability percentage
- Price per Sqft: Score relative to market average
- Age/Condition: Score based on year built and inspection results

Users choose whether each criterion is subjective or calculated when creating it.

### 2.5 Commute Time Value Analysis

A unique feature of the evaluation engine is the commute time value calculation, which monetizes the true cost of commute differences between properties.

#### 2.5.1 Calculation Formula

**Monthly Commute Time Value** = (Daily Commute Difference in Minutes × Workdays/Month × Hourly Rate) / 60

#### 2.5.2 User Inputs

| Input | Default | Description |
|-------|---------|-------------|
| Work address | (required) | Destination for commute calculation |
| Hourly rate for time value | $100/hour | What user's time is worth |
| Workdays per month | 22 days | Typical working days |
| Commute time per property | (entered per property) | Round-trip minutes |

#### 2.5.3 Output Metrics

- Monthly time cost difference between properties
- Annual time cost
- 30-year time cost (mortgage lifetime)
- Hours lost per year in commute
- "Full days" spent commuting per year

### 2.6 True Total Cost Calculation

Combines all housing costs plus commute time value for complete comparison.

#### 2.6.1 Housing Cost Components

| Component | Source |
|-----------|--------|
| Mortgage principal & interest | Calculated from price, rate, term |
| Property taxes | User input or estimated from price |
| Homeowner's insurance | User input or default estimate |
| HOA fees | User input |
| PMI (if applicable) | Calculated if down payment < 20% |
| Utilities estimate | User input or default |
| Commute time value | Calculated from commute analysis |

**True Total Cost** = Sum of all components

### 2.7 Property Comparison Matrix

Visual side-by-side comparison showing:

- All properties as columns
- All criteria as rows
- Scores color-coded (green 8+, yellow 5-7, red <5)
- Weighted score totals
- Ranking with #1 highlighted

### 2.8 Work-From-Home Analysis

For households with remote workers, the system calculates the value of extra bedrooms for home offices.

#### 2.8.1 Input Parameters

- Number of people working from home
- Days per week each person works from home
- Whether dedicated office space is required

#### 2.8.2 Output Analysis

- Minimum bedrooms needed (sleeping + offices)
- Cost per extra bedroom across properties
- WFH suitability score per property

---

## 3. Annual Budget Planner

The Annual Budget Planner helps users understand their complete financial picture during the home buying transition, with month-by-month cash flow projections.

### 3.1 Income Configuration

#### 3.1.1 Salary Income

| Field | Description |
|-------|-------------|
| Annual base salary | Gross annual salary |
| Pay frequency | Weekly, bi-weekly, semi-monthly, monthly |
| 401(k) contribution | Percentage or fixed amount |
| HSA contribution | Annual amount (if applicable) |
| Other deductions | Insurance, etc. |

#### 3.1.2 Bonus Income

| Field | Description |
|-------|-------------|
| Expected bonus amount | Gross bonus |
| Bonus month | When typically paid |
| Bonus probability | Likelihood (for scenario planning) |

#### 3.1.3 RSU/Stock Vesting

| Field | Description |
|-------|-------------|
| Vest schedule | Quarterly, annual, custom dates |
| Shares per vest | Number of shares |
| Estimated price | For value calculation |
| Tax withholding rate | Typically 22% federal + state |

#### 3.1.4 Other Income

- Rental income
- Side business income
- Partner/roommate contribution (with start date)
- Investment income

### 3.2 Income Scenarios

The system generates three income scenarios:

| Scenario | Calculation | Use Case |
|----------|-------------|----------|
| **Conservative Floor** | Salary + guaranteed income only | Stress testing |
| **Realistic Planning** | Salary + 70% of variable income | Monthly budgeting |
| **Expected Outcome** | All income at expected amounts | Long-term planning |

### 3.3 Expense Configuration

#### 3.3.1 Fixed Expenses

Expenses that don't change month-to-month:

- Mortgage/rent payment
- Car payment
- Insurance premiums (car, life, etc.)
- Subscriptions (streaming, gym, etc.)
- Loan payments

#### 3.3.2 Variable Expenses

Expenses that fluctuate:

- Groceries
- Gas/transportation
- Utilities
- Entertainment
- Dining out

#### 3.3.3 One-Time Events

Calendar of expected one-time expenses:

| Field | Description |
|-------|-------------|
| Event name | Description of expense |
| Amount | Expected cost |
| Month | When it occurs |
| Category | Type (moving, travel, purchase, etc.) |

### 3.4 Housing Affordability Analysis

#### 3.4.1 Calculation

**Housing Percentage** = (Total Monthly Housing Cost / Gross Monthly Income) × 100

#### 3.4.2 Affordability Zones

| Zone | Percentage | Indicator |
|------|------------|-----------|
| Green (Comfortable) | < 28% | ✓ |
| Yellow (Stretching) | 28-36% | ⚠ |
| Orange (Aggressive) | 36-43% | ⚠⚠ |
| Red (Risky) | > 43% | ✗ |

### 3.5 Month-by-Month Projection

Interactive calendar view showing:

| Column | Description |
|--------|-------------|
| Month | Calendar month |
| Income | Total income for month |
| Fixed Expenses | Sum of fixed expenses |
| Variable Expenses | Sum of variable expenses |
| One-Time Events | Any special expenses |
| Surplus/Deficit | Income minus all expenses |
| Cumulative | Running total |

### 3.6 Emergency Fund Tracking

#### 3.6.1 Configuration

| Field | Default | Description |
|-------|---------|-------------|
| Target amount | 6 months expenses | Goal for emergency fund |
| Current balance | (user input) | Starting amount |

#### 3.6.2 Tracking Features

- Month-by-month balance projection
- Automatic deficit coverage (draws from emergency fund)
- Recovery timeline after draws
- Visual indicator of fund health
- "Crunch month" identification (months requiring draws)

---

## 4. Funding Strategy Calculator

The Funding Strategy Calculator helps users plan and optimize their down payment funding from multiple sources while understanding tax implications.

### 4.1 Funding Source Types

| Source Type | Tax Treatment | Timing Considerations | Documentation |
|-------------|---------------|----------------------|---------------|
| **Brokerage Account** | Capital gains on appreciation | 3-day settlement | Cost basis records |
| **Traditional IRA** | Ordinary income + 10% penalty | First-time buyer exemption | 1099-R |
| **Inherited IRA** | Ordinary income, no penalty | 10-year distribution rule | 1099-R |
| **Roth IRA** | Contributions tax-free | 5-year rule for earnings | Contribution records |
| **Family Gift** | No tax to recipient | Lender seasoning rules | Gift letter required |
| **Savings Account** | Interest already taxed | Immediate availability | Bank statements |

### 4.2 Tax Impact Calculator

#### 4.2.1 Inputs

- User's marginal tax bracket
- Amount from each funding source
- Cost basis for investment accounts
- Holding periods for capital gains treatment

#### 4.2.2 Outputs

- Estimated tax liability by source
- Total additional tax due from funding strategy
- Optimal ordering of source liquidation
- Tax payment timeline and savings plan

### 4.3 Rate Buydown Analysis

When seller credits or lender credits are available, the system calculates optimal allocation:

#### 4.3.1 Credit Types

- Seller credits (negotiated)
- Lender/broker credits (Redfin partnership, etc.)
- Points purchase options

#### 4.3.2 Calculation

- Breakeven analysis on rate buydown vs. closing cost credit
- Monthly payment impact per point purchased
- Time to recoup point cost through lower payments

### 4.4 User Experience Flow

All funding information is entered manually by the user - there are no connections to banks or brokerages. This maintains privacy and works offline.

#### 4.4.1 Step 1: Tax Bracket Setup

Before adding funding sources, user provides their tax information:

**Screen:** "Let's understand your tax situation"

| Field | Input Type |
|-------|------------|
| **Filing Status** | Dropdown: Single, Married Filing Jointly, Married Filing Separately, Head of Household |
| **Estimated Taxable Income** | Currency input (used to determine marginal bracket) |
| **State** | Dropdown of US states (for state tax estimate) |

The app calculates the user's marginal federal and state tax brackets from this information. User can also override with a manual bracket if preferred.

#### 4.4.2 Step 2: Funding Source Selection

**Screen:** "Where is your down payment coming from?"

User selects all applicable sources (checkboxes):

- ☐ Savings account
- ☐ Brokerage account (stocks, ETFs, mutual funds)
- ☐ Traditional IRA
- ☐ Roth IRA
- ☐ Inherited IRA
- ☐ 401(k) loan or withdrawal
- ☐ Gift from family member
- ☐ Other source

After selection, user proceeds through a form for each selected source.

#### 4.4.3 Step 3: Source-Specific Forms

Each funding source type has a tailored input form:

**Savings Account Form:**

| Field | Input |
|-------|-------|
| **Amount to use** | Currency input: $________ |
| **Account nickname (optional)** | Text: e.g., "Chase savings", "Emergency fund" |

Tax impact: None (already taxed)

---

**Brokerage Account Form:**

| Field | Input |
|-------|-------|
| **Amount to sell** | Currency input: $________ |
| **Cost basis** | Currency input: $________ (what you originally paid) |
| **Holding period** | Radio: ○ Less than 1 year (short-term) ○ More than 1 year (long-term) |
| **Account nickname (optional)** | Text: e.g., "Fidelity", "MSFT shares" |

Tax impact calculated: (Amount - Cost Basis) × Capital Gains Rate

Helper text explains: "Short-term gains are taxed as ordinary income. Long-term gains have preferential rates (0%, 15%, or 20%)."

---

**Traditional IRA Form:**

| Field | Input |
|-------|-------|
| **Amount to withdraw** | Currency input: $________ |
| **First-time home buyer?** | Toggle: Yes / No |
| **Age** | Number input (for penalty calculation) |

Tax impact calculated: Full amount taxed as ordinary income + 10% penalty (unless first-time buyer exemption up to $10K, or age 59½+)

---

**Inherited IRA Form:**

| Field | Input |
|-------|-------|
| **Amount to withdraw** | Currency input: $________ |
| **Year inherited** | Year picker (affects 10-year rule calculation) |
| **Remaining balance (optional)** | Currency input: $________ (for distribution planning) |

Tax impact calculated: Full amount taxed as ordinary income (no early withdrawal penalty)

Helper text: "Inherited IRAs must be fully distributed within 10 years of inheritance."

---

**Roth IRA Form:**

| Field | Input |
|-------|-------|
| **Total contributions made** | Currency input: $________ (can withdraw tax-free) |
| **Amount from contributions** | Currency input: $________ |
| **Amount from earnings** | Currency input: $________ (may have tax + penalty) |
| **Account open 5+ years?** | Toggle: Yes / No |
| **First-time home buyer?** | Toggle: Yes / No |

Tax impact calculated: Contributions are always tax-free. Earnings may be taxable + 10% penalty depending on 5-year rule and first-time buyer status.

---

**Family Gift Form:**

| Field | Input |
|-------|-------|
| **Gift amount** | Currency input: $________ |
| **Donor name** | Text: e.g., "Mom and Dad", "Grandma" |
| **Donor relationship** | Dropdown: Parent, Grandparent, Sibling, Other relative, Non-relative |
| **Expected receipt date** | Date picker |

Tax impact: None to recipient. Helper text: "Your lender will require a gift letter. Gifts typically need to be 'seasoned' in your account for 60 days before closing."

---

**401(k) Form:**

| Field | Input |
|-------|-------|
| **Type** | Radio: ○ Loan (repay to yourself) ○ Hardship withdrawal |
| **Amount** | Currency input: $________ |
| **(If loan) Repayment term** | Dropdown: 5 years, 10 years, 15 years |
| **(If loan) Interest rate** | Percentage input (typically prime + 1%) |

Tax impact: Loans have no tax impact if repaid. Hardship withdrawals are taxed as ordinary income + 10% penalty if under 59½.

#### 4.4.4 Step 4: Funding Summary

After entering all sources, user sees a summary screen:

**Screen:** "Your Down Payment Funding Plan"

| Source | Gross Amount | Est. Tax | Net Amount |
|--------|--------------|----------|------------|
| Chase Savings | $25,000 | $0 | $25,000 |
| Fidelity (MSFT shares) | $110,000 | $8,250 | $101,750 |
| Inherited IRA | $50,000 | $12,000 | $38,000 |
| Gift from Parents | $50,000 | $0 | $50,000 |
| **TOTAL** | **$235,000** | **$20,250** | **$214,750** |

Below the table:

- Target down payment needed: $162,000 (from property module)
- Estimated closing costs: $15,000
- Total needed: $177,000
- Surplus after funding: $37,750 (available for reserves/moving costs)

#### 4.4.5 Step 5: Tax Payment Planning

**Screen:** "When will you owe these taxes?"

The app shows when estimated taxes will be due:

- Capital gains and IRA distributions: Due with tax return (April of following year)
- If large amount: May need quarterly estimated payments to avoid penalty

Suggested action: "Set aside $20,250 for taxes. Consider putting this in a high-yield savings account until tax time."

Option to add this as a line item in the Budget module automatically.

#### 4.4.6 Editing and Updates

Users can return to edit funding sources anytime:

- Tap any source in the summary to edit amounts or details
- Add new sources with [+ Add Source] button
- Remove sources by swiping left (mobile) or clicking delete icon (desktop)
- All calculations update in real-time as changes are made

Changes to funding sources automatically update the Budget module if integration is enabled.

---

## 5. Reports and Exports

The application generates comprehensive reports for decision-making and record-keeping.

### 5.1 Property Comparison Report

- Executive summary with recommendation
- Weighted scoring matrix
- Financial comparison (monthly and 30-year)
- Commute analysis with time value
- Property-by-property detailed evaluation
- Decision tree for negotiation scenarios

### 5.2 Budget Plan Report

- Income structure summary
- Affordability analysis by scenario
- Month-by-month cash flow table
- Emergency fund trajectory chart
- Key financial milestones
- Risk scenarios and mitigation

### 5.3 Export Formats

| Format | Use Case |
|--------|----------|
| **PDF** | Sharing with partners, advisors, lenders |
| **Excel/CSV** | Further analysis, custom modeling |
| **JSON** | Backup, transfer between devices |

---

## 6. Technical Architecture

### 6.1 Core Design Principles

#### 6.1.1 Offline-First Architecture

The application is designed to work fully offline, with optional cloud sync as an enhancement:

- All core functionality works without internet connection
- Data stored locally on user's device by default
- No required account creation or login
- Optional cloud backup/sync for users who want cross-device access
- Works on airplane mode, in basements, rural areas with poor connectivity

#### 6.1.2 Privacy-First Design

Home buying is a deeply personal financial decision. Users should feel completely comfortable using this tool:

- No analytics or tracking of user behavior
- No data transmitted to servers unless user explicitly enables sync
- No AI/LLM processing of user data
- All calculations performed locally via native code
- User can export all their data at any time
- User can delete all data with one action

#### 6.1.3 Native-First Development

The application is built as a native app, not a web app:

- Native UI components for each platform (feels like a real app, not a website)
- Direct access to device storage and file system
- No browser dependencies or web view wrappers
- Better performance and battery efficiency
- Available through official app stores (App Store, Google Play, Microsoft Store)

### 6.2 Technology Stack

#### 6.2.1 Framework: .NET MAUI

.NET Multi-platform App UI (MAUI) is the chosen framework for cross-platform native development:

| Aspect | Details |
|--------|---------|
| **Language** | C# (modern, strongly typed, excellent tooling) |
| **Platforms** | iOS, Android, Windows, macOS - all from single codebase |
| **UI Rendering** | Native controls on each platform (not web views) |
| **IDE** | VS Code with C# Dev Kit extension, paired with AI coding assistant |
| **Local Storage** | SQLite with Entity Framework Core or sqlite-net |
| **File Access** | Full native file system access via .NET APIs |
| **Offline Support** | Built-in - no internet required for any core functionality |
| **Maturity** | Production-ready, backed by Microsoft, evolution of Xamarin |

#### 6.2.2 Why .NET MAUI

- C# is familiar (similar patterns to TypeScript, existing C# experience)
- Single codebase for all four target platforms
- Native UI controls - apps look and feel right on each platform
- Strong typing and compile-time checks reduce bugs
- Excellent debugging and profiling tools in Visual Studio
- Large ecosystem of NuGet packages
- Hot reload for rapid UI development
- Long-term support from Microsoft

#### 6.2.3 Development Environment

**Primary IDE:** VS Code with AI Coding Assistant

- VS Code with C# Dev Kit extension for IntelliSense, debugging, and project management
- AI coding assistant: Roo Code extension (VS Code) or Claude Code (terminal)
- .NET 8 SDK (LTS version)
- MAUI workload: `dotnet workload install maui`

**Platform-Specific Requirements:**

- iOS/macOS builds: Mac with Xcode 15+ installed
- Android builds: Android SDK (via VS Code or Android Studio)
- Windows builds: Windows 10/11 with Windows App SDK

**AI Coding Assistant Usage:**

The AI assistant accelerates development by:

- Generating boilerplate: ViewModels, Services, SQLite repositories
- Writing XAML layouts from descriptions or wireframes
- Implementing calculation logic (mortgage formulas, scoring algorithms)
- Writing unit tests for business logic in Core project
- Debugging platform-specific issues with context from error messages
- Explaining unfamiliar C#/.NET patterns during learning curve

#### 6.2.4 Key Libraries

- **sqlite-net-pcl**: Lightweight SQLite ORM for local storage
- **CommunityToolkit.Mvvm**: MVVM pattern helpers and source generators
- **CommunityToolkit.Maui**: Additional UI controls and behaviors
- **SkiaSharp**: 2D graphics for charts and visualizations
- **LiveCharts2**: Charting library for data visualization
- **QuestPDF** or **iText7**: PDF report generation

#### 6.2.5 Project Structure

Recommended solution organization:

```
HomeBuyerHelper/
├── HomeBuyerHelper/           # Main MAUI app (shared UI)
├── HomeBuyerHelper.Core/      # Business logic, models, calculations
├── HomeBuyerHelper.Data/      # SQLite repositories, data access
└── HomeBuyerHelper.Tests/     # Unit and integration tests
```

The Core and Data projects are .NET Standard libraries, making business logic easily testable without device dependencies.

### 6.3 Local Data Storage

#### 6.3.1 Database: SQLite with sqlite-net

SQLite is the recommended local database, accessed via sqlite-net-pcl:

- Embedded database - no separate server process
- Single file storage - easy backup and transfer
- Battle-tested reliability (used in iOS, Android, browsers, etc.)
- LINQ support for type-safe queries in C#
- Optional encryption with SQLCipher

#### 6.3.2 Data Model (C# Classes)

Core entity classes:

```csharp
public class Property
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Nickname { get; set; }
    public string Address { get; set; }
    public decimal AskingPrice { get; set; }
    public int Bedrooms { get; set; }
    public decimal Bathrooms { get; set; }
    public int SquareFeet { get; set; }
    public int YearBuilt { get; set; }
    public decimal MonthlyHOA { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

Additional entities: EvaluationCriterion, PropertyScore, BudgetConfig, IncomeSource, Expense, FundingSource, UserPreferences

#### 6.3.3 Data Safety

- Automatic local backup on each save (keeps last 10 versions)
- "Undo" support for accidental deletions
- Manual export to JSON or CSV at any time
- Import from backup file to restore or transfer devices

### 6.4 Optional Cloud Features (User Opt-In Only)

For users who want cross-device access or backup, optional cloud sync can be enabled:

#### 6.4.1 Sync Options

- **Export to file**: Manual backup to device storage (always available)
- **Platform cloud**: iCloud (Apple devices), Google Drive (Android), OneDrive (Windows)
- **Third-party cloud**: Dropbox integration (all platforms)

All cloud sync uses the user's own cloud storage account - we never store user data on our servers.

#### 6.4.2 What Gets Synced (If Enabled)

- Complete database file (encrypted)
- Property photos (optional - can exclude to save space)

#### 6.4.3 What Never Leaves the Device

- Usage analytics (there are none)
- Behavioral data (there is none)
- Crash reports (unless user explicitly sends)
- Any data to our servers (we don't have user data servers)

### 6.5 Calculation Engine

All calculations are deterministic and run locally:

#### 6.5.1 Scoring Calculations

**Weighted Score** = Σ (Criterion Score × Criterion Weight)

Example: If "Commute" is weighted 20% and scored 8, contribution = 0.20 × 8 = 1.6 points

#### 6.5.2 Financial Calculations

**Mortgage Payment:** M = P × [r(1+r)^n] / [(1+r)^n - 1]

**Housing Affordability:** (Monthly Housing Cost / Monthly Income) × 100

**True Total Cost:** Housing + Utilities + Commute Time Value

#### 6.5.3 Budget Projections

Month-by-month calculations using:

- User-defined income schedule (salary, bonus months, RSU vest dates)
- Fixed and variable expense categories
- One-time event calendar
- Running surplus/deficit accumulator

### 6.6 Platform-Specific Considerations

#### 6.6.1 iOS

- Distribution via App Store
- iCloud sync integration for cross-device (iPhone, iPad, Mac)
- Support for iPad multitasking and larger screens
- Photo picker integration for property photos
- Requires Mac with Xcode for build/deploy

#### 6.6.2 Android

- Distribution via Google Play Store
- Google Drive backup integration
- Material Design UI via MAUI controls
- Support for tablets and foldables
- Build from Windows or Mac via Visual Studio

#### 6.6.3 Windows

- Distribution via Microsoft Store and direct download (MSIX)
- OneDrive backup integration
- Windows 10/11 support with WinUI 3 controls
- Keyboard shortcuts for power users
- Native Windows notifications and taskbar integration

#### 6.6.4 macOS

- Distribution via Mac App Store and direct download
- iCloud sync shared with iOS version
- Mac Catalyst-based rendering
- Native macOS menu bar and keyboard shortcuts
- Apple Silicon and Intel support

### 6.7 Security Considerations

- No server-side data storage (nothing to breach)
- Optional local database encryption (SQLCipher)
- No third-party analytics SDKs
- No network calls except user-initiated cloud sync
- Open source codebase (users can verify privacy claims)

---

## 7. User Experience Design

### 7.1 Guided Onboarding Quiz

The application uses a conversational, quiz-style onboarding to help users set up their evaluation criteria without feeling overwhelmed. Users answer simple questions, and the system builds their personalized framework.

#### 7.1.1 Onboarding Flow Overview

1. Welcome & Goal Setting (1-2 questions)
2. Life Situation Discovery (3-5 questions)
3. Priority Ranking (interactive sorting)
4. Criteria Selection & Customization
5. Weight Fine-Tuning
6. Financial Setup (for budget planning)

Total onboarding time: 5-10 minutes. Users can skip ahead or return to modify answers later.

#### 7.1.2 Step 1: Welcome & Goal Setting

**Screen:** "Let's find your perfect home"

**Question 1:** "What best describes your situation?"

- Buying my first home
- Upgrading to a bigger/better home
- Downsizing or simplifying
- Relocating to a new area
- Buying an investment property

**Question 2:** "How many properties are you currently considering?"

- Just starting my search (0-1)
- A few options (2-3)
- Several to compare (4+)

Purpose: Sets context and adjusts subsequent questions. Investment buyers see different criteria than first-time buyers.

#### 7.1.3 Step 2: Life Situation Discovery

Questions adapt based on Step 1 answers. Examples for a first-time buyer:

**Question 3:** "Who will be living in this home?"

- Just me
- Me and a partner/spouse
- Family with kids
- Roommates
- Multi-generational household

**Question 4:** "How do you and others in your household work?"

- Everyone commutes to an office daily
- Hybrid (some days office, some days home)
- Fully remote / work from home
- Retired or not working
- Mix of the above

**Question 5:** "Do you have any pets or plan to?"

- No pets
- Dog(s)
- Cat(s)
- Other pets

**Question 6:** "What's most important about where you live?" (Select up to 3)

- Short commute to work
- Good schools nearby
- Walkable neighborhood
- Peace and quiet
- Near family/friends
- Access to outdoor activities
- Nightlife and entertainment
- Low cost of living

**Question 7:** "What matters most about the home itself?" (Select up to 3)

- Move-in ready condition
- Space for home office(s)
- Outdoor space (yard, patio, balcony)
- Modern kitchen
- Lots of natural light
- Garage or parking
- Storage space
- Guest accommodations
- Low maintenance

#### 7.1.4 Step 3: Priority Ranking

Based on answers, the system presents a shortlist of relevant factors. Users drag to rank them.

**Screen:** "Drag to rank these from most to least important to you"

Example for a hybrid-working couple with a dog:

1. Commute time (you selected 'hybrid work')
2. Home office space (you selected 'hybrid work')
3. Dog-friendly features (you have a dog)
4. Overall feel of the home
5. Neighborhood quality
6. Price and value
7. Condition and maintenance needs

The ranking order is used to suggest initial weights (higher rank = higher weight).

#### 7.1.5 Step 4: Criteria Selection & Customization

**Screen:** "Here's your personalized evaluation criteria"

The system shows the suggested criteria based on quiz answers, with checkboxes to include/exclude:

| Include | Criterion | Suggested Weight | Why Suggested |
|---------|-----------|------------------|---------------|
| ☑ | Commute Time | 20% | You work hybrid |
| ☑ | Home Office Suitability | 15% | You work hybrid |
| ☑ | Dog-Friendly Features | 10% | You have a dog |
| ☑ | Overall Feel / Gut Reaction | 20% | Always important |
| ☑ | Neighborhood Quality | 15% | You ranked this #5 |
| ☑ | Price & Value | 10% | You ranked this #6 |
| ☑ | Condition & Maintenance | 10% | You ranked this #7 |

Below the suggested criteria, users see options to add more:

**"Want to add more criteria?"**

- [Browse Common Criteria] - Opens categorized list of pre-built options
- [Create Custom Criterion] - Opens form to define something unique

#### 7.1.6 Browse Common Criteria

Pre-built criteria organized by category that users can add with one click:

**Location & Neighborhood:**

- Walkability Score
- School District Quality
- Safety / Crime Rate
- Proximity to Family/Friends
- Public Transit Access
- Restaurant & Shopping Options
- Parks & Recreation Nearby

**Home Features:**

- Kitchen Quality
- Bathroom Quality
- Natural Light
- Storage Space
- Garage / Parking
- Outdoor Space (Yard/Patio)
- Energy Efficiency
- Smart Home Features

**Practical Considerations:**

- HOA Rules & Fees
- Future Renovation Potential
- Resale Value Potential
- Internet / Connectivity
- Guest Parking
- Noise Level

#### 7.1.7 Create Custom Criterion

Simple form for user-defined criteria:

| Field | User Input |
|-------|------------|
| **Name** | [Text field] e.g., "Distance to Mom's House" |
| **What does a 10 look like?** | [Text field] e.g., "Less than 10 minutes away" |
| **What does a 1 look like?** | [Text field] e.g., "More than an hour away" |
| **Initial Weight** | [Slider] Default 10%, user adjusts |

#### 7.1.8 Step 5: Weight Fine-Tuning

**Screen:** "Adjust how much each factor matters"

Interactive slider interface showing all selected criteria:

- Visual sliders for each criterion (drag to adjust weight)
- Real-time pie chart showing weight distribution
- Running total that must equal 100%
- "Lock" toggle to freeze a criterion while adjusting others
- "Reset to Suggested" button to start over

Helper text: "Tip: Your top 2-3 priorities should usually total 50-60% of the weight."

#### 7.1.9 Step 6: Financial Setup (Optional)

Users can skip this and add later, or complete for full budget planning:

**Question:** "Would you like to set up budget planning now?"

- Yes, let's do it now → Proceeds to income/expense setup
- Maybe later → Skips to property entry

If yes, a similar guided flow collects:

- Annual salary and pay frequency
- Bonus structure (if any)
- Other income (RSU, rental, side business)
- Current monthly expenses
- Savings and down payment funds

#### 7.1.10 Onboarding Complete

**Screen:** "You're all set! Here's your personalized home finder."

Summary card showing:

- Number of criteria selected
- Top 3 priorities by weight
- Budget planning status (set up / not set up)

Call to action: [Add Your First Property] or [Browse Example]

### 7.2 Property Entry Flow

After onboarding, adding properties uses a simple guided form:

#### 7.2.1 Basic Property Information

Users enter essential details manually:

| Field | Input Type |
|-------|------------|
| **Nickname** | Text (e.g., "Blue house on Oak St") - for easy reference |
| **Address** | Text (street, city, state, zip) |
| **Asking Price** | Currency input |
| **Bedrooms** | Number selector (1-10+) |
| **Bathrooms** | Number selector (1, 1.5, 2, 2.5, etc.) |
| **Square Footage** | Number input |
| **Year Built** | Year selector |
| **Property Type** | Dropdown (Single Family, Townhome, Condo, Multi-family) |
| **HOA Fee** | Currency input (monthly), or "None" |
| **Listing URL** | Optional text field (for user's reference only) |

All fields except Nickname are optional - users can add what they know and fill in details later.

#### 7.2.2 Scoring Walkthrough

For each property, the app walks through each criterion one at a time:

**Screen:** "How would you rate [Property Address] on [Criterion Name]?"

- Large 1-10 scale buttons or slider
- Score anchors displayed as reference (what is a 5? what is a 10?)
- Optional notes field for each score
- Progress indicator (Criterion 3 of 7)
- "Skip for now" option

### 7.3 Main Application Screens

**Dashboard**

Overview of all properties being considered with scores, budget status, and key metrics at a glance. Quick comparison cards show top 3 properties by score.

**Property Comparison View**

Side-by-side comparison with scores by criterion. Color-coded cells (green = 8+, yellow = 5-7, red = <5). Sortable by any criterion or total score.

**Budget Timeline**

Interactive month-by-month cash flow view. Expandable details per month. Visual indicators for deficit months and windfall months.

**Settings & Criteria Management**

Edit criteria anytime: add, remove, rename, adjust weights. Re-run the quiz to start fresh. Import/export criteria templates.

### 7.4 Platform-Specific UX Considerations

**Mobile (iOS/Android)**

- Touch-optimized scoring interface (large tap targets)
- Quick property entry while touring homes
- Camera integration for property photos
- Swipe gestures for navigation between properties
- Platform-native share sheet for exports

**Desktop (Windows/macOS)**

- Keyboard shortcuts for power users
- Side-by-side comparison views on larger screens
- Drag-and-drop file import/export
- Multi-window support for comparing while browsing listings
- Native menu bar integration

**Tablet (iPad/Android Tablets)**

- Optimized layouts that use extra screen space
- Split-view comparison mode
- Pencil/stylus support for quick notes

---

## 8. Product Roadmap

### 8.1 Phase 1: MVP (Months 1-3)

Core native app with property evaluation - target: iOS and Android first.

- .NET MAUI project setup with iOS and Android targets
- Guided onboarding quiz to build personalized criteria
- Custom criteria creation with weights and score anchors
- Manual property entry with guided scoring walkthrough
- Weighted comparison matrix and rankings
- Basic monthly cost calculator (mortgage, tax, HOA, insurance)
- SQLite local storage - 100% offline functionality
- Export to JSON file for manual backup
- App Store and Google Play submission

### 8.2 Phase 2: Budget Planning + Desktop (Months 4-6)

Add financial planning and expand to desktop platforms.

- Windows and macOS app builds from same codebase
- Full income structure configuration (salary, bonus, RSU, etc.)
- Month-by-month cash flow projection
- Emergency fund tracking
- One-time event calendar (moving costs, trips, etc.)
- Partner/roommate contribution modeling
- Multiple income scenarios (conservative, planning, expected)
- PDF report export via QuestPDF
- Microsoft Store and Mac App Store submission

### 8.3 Phase 3: Advanced Analysis + Sync (Months 7-9)

Deeper analytical features and optional cloud sync.

- Commute time value analysis with monetization
- Funding strategy calculator with tax impact estimates
- True total cost comparison (housing + commute)
- 30-year total cost projections
- Optional iCloud sync (Apple devices)
- Optional Google Drive sync (Android)
- Optional OneDrive sync (Windows)
- Photo attachments for properties
- Notes and pros/cons lists per property

### 8.4 Phase 4: Polish + Sharing (Months 10-12)

Quality of life improvements and collaboration features.

- Dropbox sync (all platforms)
- Share read-only view with partner or advisor (via export)
- Excel/CSV export with formulas intact
- Rent vs. buy comparison calculator
- Multiple scenarios ("What if we wait 6 months?")
- Criteria template sharing (export/import anonymized templates)
- Tablet-optimized layouts (iPad, Android tablets)
- Dark mode support across all platforms

### 8.5 Future Considerations (Year 2+)

Potential future directions, always maintaining privacy-first principles:

- Listing URL parsing for auto-fill (on-device, optional)
- Integration with mortgage calculator APIs (user-initiated only)
- Community-contributed criteria templates
- Localization for international markets
- Open source release
- Widget support (iOS, Android, Windows)

---

## 9. Glossary

| Term | Definition |
|------|------------|
| **True Total Cost** | Housing costs + utilities + commute time value penalty |
| **Commute Time Value** | Monetized cost of commute differences between properties |
| **Planning Income** | Conservative estimate using salary + 70% of variable income |
| **Housing Percentage** | Total housing cost as percentage of gross monthly income |
| **Crunch Month** | Month where expenses exceed income, requiring emergency fund draw |
| **Score Anchor** | Definition of what a specific score (1, 5, 10) means for a criterion |
| **First-Time Buyer** | IRS definition: Haven't owned a home in the past 2 years |
| **Cost Basis** | Original purchase price of an investment (for capital gains calculation) |
| **Seasoning** | Lender requirement that funds be in your account for a period (typically 60 days) |

---

## 10. Appendix: Default Values

| Setting | Default Value | Notes |
|---------|---------------|-------|
| Property Tax Rate | 0.96% of purchase price | Can be overridden per property |
| Homeowner's Insurance | $125/month | Estimate for HO-6 condo/townhome |
| Time Value Rate | $100/hour | User configurable |
| Work Days per Month | 22 days | Standard assumption |
| Emergency Fund Target | 6 months expenses | User configurable |
| Variable Income Factor | 70% | For planning scenario |
| Down Payment Default | 20% | For affordability calculations |
| Mortgage Term Default | 30 years | Can select 15, 20, or 30 |

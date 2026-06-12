# HomeBuyerHelper

Tool for helping first time homebuyers scope out their budget, compare properties, and plan offer strategies and structures that make the most sense for them. Anyone should be able to buy a home, it's not Rocket Science!

A privacy-first, offline-capable .NET MAUI app (iOS, Android, Windows, macOS). No accounts, no tracking, no servers — your data stays on your device.

## Features

**Property Evaluation (Phase 1)**
- Guided quiz-style onboarding that builds personalized evaluation criteria
- Custom criteria with weights, score anchors, and auto-rebalancing
- Property entry with guided 1-10 scoring walkthrough
- Color-coded weighted comparison matrix with rankings
- Monthly cost calculator (P&I, taxes, insurance, HOA, PMI)
- JSON backup/restore and PDF reports

**Budget Planner (Phase 2)**
- Income sources with bonus/RSU timing, probability, and start/end dates
- Conservative / Realistic / Expected income scenarios
- Fixed and variable expenses plus a one-time event calendar
- 24-month cash flow projection with crunch-month detection
- Emergency fund tracking with automatic draws and recovery
- Affordability zones (green <28% to red >43%) per scenario

**Advanced Analysis (Phase 3)**
- Commute time value: monetizes commute differences ($/month, hours and days lost per year, 30-year cost)
- True Total Cost: housing + utilities + commute time value
- Down payment funding planner with per-source tax estimates (brokerage gains, Traditional/Roth/Inherited IRA rules, 401(k) loan vs withdrawal, gifts)
- Tax set-aside straight into the budget calendar
- Property photos and pros/cons lists
- Cloud backup via share sheet to any provider (your account, your data)

**Polish + Sharing (Phase 4)**
- Rent vs. buy calculator with breakeven year
- "What if" scenario planner including "what if we wait N months?"
- Read-only HTML report sharing with privacy controls
- CSV exports (comparison matrix, cash flow)
- Criteria template export/import + 9 built-in templates
- Dark mode (system/light/dark) and tablet grid layouts

## Web Preview (GitHub Pages)

A Blazor WebAssembly preview of the app deploys to GitHub Pages on every push to `main` (`.github/workflows/deploy-pages.yml`). It reuses `HomeBuyerHelper.Core` for all calculations, with browser localStorage standing in for SQLite — data stays in the visitor's browser, keeping the privacy promise. One-time setup: in the repo's **Settings → Pages**, set the source to **GitHub Actions**.

Run it locally with `dotnet run --project src/HomeBuyerHelper.Web`.

## Solution Layout

```
src/HomeBuyerHelper/        # MAUI app (UI, ViewModels, platform heads)
src/HomeBuyerHelper.Core/   # Business logic, models, calculation services
src/HomeBuyerHelper.Data/   # SQLite repositories (sqlite-net-pcl)
src/HomeBuyerHelper.Web/    # Blazor WebAssembly preview (GitHub Pages)
tests/                      # xUnit test projects (163 tests)
```

## Building

Requires .NET 8 SDK with the `maui-android` workload (plus Xcode on macOS for Apple targets, Windows App SDK on Windows).

```bash
# Core/Data libraries and tests run anywhere
dotnet test tests/HomeBuyerHelper.Core.Tests
dotnet test tests/HomeBuyerHelper.Data.Tests

# Android app (needs Android SDK)
dotnet build src/HomeBuyerHelper -f net8.0-android
```

See `DesignSpec.md` and `docs/implementation-plan/` for the full design and phase plans.

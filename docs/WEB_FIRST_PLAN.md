# Web-First Implementation Plan

**Decision:** The Blazor web app is the primary surface. The MAUI app stays
compiling (shared Core/Data, DI registrations kept current) but gets no new
UI until the web app is first class.

This plan turns the July 2026 project review into sequenced, buildable
milestones. Each milestone is independently shippable and verified by
driving the running app (local build + Playwright walkthrough — no CI
dependency, which matters while the Actions budget is exhausted).

## What "first class" means here

1. **Trustworthy numbers** — every assumption that drives a displayed number
   is visible and editable in the web app itself.
2. **Durable data** — a user can back up, restore, and wipe their data;
   losing a browser profile must not mean losing a home-buying plan.
3. **Integrated modules** — properties, offers, budget, and funding inform
   each other; the app answers "can I afford *this* house with *this*
   offer?" rather than four disconnected calculators.
4. **Complete on web** — nothing in the UI refers the user to the native
   app; parity features (onboarding, scenarios) exist in web form.
5. **Offline-installable** — the PWA shell works without a network,
   honoring the offline-first pitch.

## Guiding constraints

- **Core stays UI-agnostic and fully tested.** All new math/logic lands in
  `HomeBuyerHelper.Core` with xunit coverage, so MAUI inherits it for free
  later.
- **localStorage stays for now.** The JSON-per-collection `LocalStore` is
  adequate for plan-sized data. Photos are out of scope for web until an
  IndexedDB store exists (noted in M7). Every store gains a schema-version
  stamp in M2 so a future migration is possible.
- **QuestPDF does not run in WASM** (native Skia dependency). Web reports
  use print-optimized pages + browser print-to-PDF, and CSV/JSON downloads
  via JS interop.

---

## M1 — Visible, editable assumptions (Settings) — small

The review's top trust issue: 7% rate, 20% down, 30-year term, 0.96% tax,
$125 insurance, $100/hr time value, 22 workdays all drive Compare,
Rent-vs-Buy, and Offers, and are invisible/unchangeable on web (Compare
literally says "set your rate in the native app").

**Scope**
- New `Pages/Settings.razor` (`/settings`): loan defaults (down %, rate,
  term), cost defaults (property tax rate, monthly insurance), commute
  value (hourly rate, workdays/month, work address), all persisted to
  `UserPreferences` (fields already exist).
- Nav: gear icon in header; add to dashboard module grid.
- Inline assumption chips: Compare, Rent-vs-Buy, and Offers show
  "Assumes 7.0% · 20% down · 30 yr — *edit*" linking to `/settings`.
  Remove the "native app" dead-end copy.
- Rent-vs-Buy: surface down payment/rate/term as pre-filled editable inputs
  instead of hidden defaults.

**Acceptance**
- Changing the default rate in Settings changes Offers/Rent-vs-Buy/Compare
  outputs on next visit.
- Changing time value from $100 → $25/hr visibly re-weights True Total Cost.
- No page mentions the native app.

## M2 — Data durability (export / import / reset) — small-medium

**Scope**
- `Storage/WebBackupService.cs`: serialize all nine `LocalStore` collections
  plus preferences into one versioned JSON document
  (`{ schemaVersion, exportedAt, data: {...} }`); restore replaces all
  collections after validation; both round-trip through the existing Core
  models so MAUI backups stay conceptually compatible.
- JS interop download helper (blob + `URL.createObjectURL`) in a small
  `wwwroot/js/app.js`; file-picker import via `InputFile`.
- Settings page gains a **Data** section: Export backup, Import backup
  (confirm-overwrite dialog), Delete all data (typed confirmation).
- Stamp `schemaVersion` into every `LocalStore` payload on save.
- Dashboard nudge: subtle "last backup: never/date" hint once the user has
  ≥1 property (stored in preferences).

**Acceptance**
- Export → wipe browser storage → import restores properties, scores,
  criteria, budget, funding, offers, and preferences exactly (verified by
  Playwright walkthrough).
- Malformed/old-version file imports fail with a clear message and change
  nothing.

## M3 — UX debt sweep — small

Batched small fixes from the review:

- Criteria: confirm dialog before "Apply template" destroys existing
  criteria; warn when any single weight exceeds 40% (service exists).
- Scoring: input `min=1` (currently 0 on a 1–10 scale); reject 0 on save.
- Properties form: add Property Type, Address/City/State/Zip, Annual
  Property Tax, Annual Insurance, Listing URL in a collapsible "More
  details" section (models already support all of them; tax/insurance feed
  True Total Cost and Offers instead of defaults).
- Dashboard module grid: add Offer Planner card (mobile currently can't
  reach it from the tab bar).
- Mobile tab bar: replace nothing — but every page not in the tab bar must
  be reachable from a dashboard card (Offers was the only gap).

**Acceptance:** each fix demonstrated in the walkthrough script; entering a
real tax figure on a property changes its Offers/Compare numbers.

## M4 — "My Plan": module integration — the flagship, medium-large

The app's thesis is *integrated* planning. One thread ties it together:
the user designates a target property + offer structure, and every module
reflects it.

**Core (all unit-tested)**
- `UserPreferences`: add `TargetPropertyId`, `TargetOfferScenarioId`
  (nullable ints; sqlite-net adds columns on CreateTable, localStorage
  serializes transparently).
- New `PlanService` (`IPlanService`): resolves the target property + offer
  into a `PlanSnapshot` — offer evaluation (reusing `OfferPlanningService`),
  monthly housing cost, cash-to-close, funding coverage
  (net-of-tax funding total vs cash-to-close → surplus/shortfall), and
  affordability zone per income scenario (reusing `AffordabilityService` +
  `IncomeScenarioService`).
- `CashFlowProjectionInput`: optional `PlannedHousing` (monthly cost +
  purchase month + IDs of expenses it replaces, e.g. current rent) so the
  projection can model "rent until month N, then own".

**Web**
- Offers page: "Set as plan" button per column; planned offer highlighted;
  affordability zone chip per offer (uses Realistic income when available).
- Budget page: banner when a plan exists — "Includes Craftsman on 5th @
  $3,192/mo from month X (replaces: Rent)"; expense rows get a "replaced by
  plan" toggle; projection uses `PlannedHousing`.
- Funding page: target card — cash to close needed, net available after
  taxes, surplus/shortfall badge (closes spec §4.4.4).
- Dashboard: plan summary card (property, offer name, monthly cost, zone,
  funding status) replacing generic copy when a plan is set.

**Acceptance**
- Setting a plan on Offers immediately changes Budget projection and
  Funding target; clearing the plan reverts all three.
- Affordability zone matches hand-computed housing % for the seeded data.
- Deleting the target property or offer clears the plan gracefully
  (cascade already deletes offers; `PlanService` must tolerate dangling
  IDs).

## M5 — Web parity: scenarios + onboarding-lite — medium

- **Scenarios page** (`/scenarios`): the "what if we wait 6 months" tool.
  `ScenarioService` is fully built and tested; UI mirrors the MAUI
  ScenariosPage — base scenario from the current plan (M4) or manual
  inputs, wait-scenario builder (months, monthly savings, market %/yr,
  current rent), results table with cash-to-close/monthly/zone deltas.
  Add to nav + dashboard grid.
- **Onboarding-lite** (`/welcome`): single-page, 4-question quiz (situation,
  household, work arrangement, top priorities) → recommends a criteria
  template with adjusted weights → applies on confirm → sets
  `HasCompletedOnboarding`. First visit with zero criteria redirects here;
  skippable. Reuses `CriteriaTemplateService`; no new Core logic beyond a
  small recommendation mapping (unit-tested).

**Acceptance:** fresh profile lands on `/welcome`, finishes in under a
minute, arrives at a dashboard with weighted criteria; scenarios table
reproduces the ScenarioServiceTests example numbers.

## M6 — PWA offline + install — small-medium

- Add Blazor WASM service worker (`ServiceWorkerAssetsManifest` in csproj,
  `service-worker.js` dev no-op + `service-worker.published.js`
  cache-first over the assets manifest), registered from `index.html`.
- Verify installability (manifest + icons already exist) and full offline
  reload of every route (localStorage data is already local).
- Add an "offline ready / update available" toast hook.

**Acceptance:** Playwright with network blocked after first load — every
page renders and data persists; Lighthouse PWA installable check passes.

## M7 — Reports & sharing — medium (after M4; needs the integrated picture)

- `/report` print-optimized route: comparison matrix, true total cost, plan
  summary (offer, cash-to-close, funding coverage), budget projection —
  print stylesheet, "Print / Save as PDF" button (browser-native, avoids
  QuestPDF-in-WASM).
- CSV download for comparison and cash-flow tables (reuse M2 JS helper).
- Explicitly out of scope: photos on web (needs IndexedDB), cloud sync,
  criteria-template sharing.

---

## Sequencing and sizing

| Order | Milestone | Size | Depends on |
|---|---|---|---|
| 1 | M1 Settings & assumptions | ~1 session | — |
| 2 | M2 Export/import/reset | ~1 session | — |
| 3 | M3 UX debt sweep | ~1 session | — |
| 4 | M4 My Plan integration | ~2–3 sessions | M1 (settings), M3 (tax fields) |
| 5 | M5 Scenarios + onboarding | ~1–2 sessions | M4 (plan feeds scenarios) |
| 6 | M6 PWA offline | ~1 session | — (any time) |
| 7 | M7 Reports | ~1–2 sessions | M4 |

M1–M3 are independent and could land as one "first-class basics" release.
M4 is the value milestone; M5–M7 make it complete. If capacity is tight,
ship M1+M2 first — they close the two failures that would actually burn a
real user today (untrustworthy defaults, unrecoverable data).

## Verification strategy (no CI required)

- Every milestone: `dotnet build` + full local test run (Core + Data), plus
  a Playwright journey script extended to cover the new flow (the scripts
  from the July review live in session scratchpad; M1 should commit a
  reusable `tools/verify-journey.mjs` and a `.claude/skills/verify` recipe
  so future sessions replay it).
- Core logic changes always land with unit tests in
  `HomeBuyerHelper.Core.Tests`.

## Risks / open questions

- **localStorage quota (~5 MB)** — fine for plan data; photos must wait for
  an IndexedDB store. Revisit after M7.
- **MAUI drift** — Core/Data changes keep MAUI compiling, but its UI will
  lag web (no Offers, Plan, or new Settings pages). Accepted cost of
  web-first; revisit MAUI after M7 or drop it deliberately.
- **Multi-tab writes** — two tabs can clobber each other's localStorage.
  Low likelihood for this tool; M2's export mitigates worst case. Not
  planned for fix.
- **Income detail (bonus month, RSU, start/end dates)** — model and
  projection support them; the Budget form doesn't expose them. Candidate
  for M3 if sizing allows, otherwise M5.

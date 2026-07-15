---
name: verify
description: Build, run, and drive the HomeBuyerHelper web app end-to-end to verify changes at the UI surface.
---

# Verifying HomeBuyerHelper changes

## Build & test (fast checks)

```bash
dotnet build src/HomeBuyerHelper.Web/HomeBuyerHelper.Web.csproj   # web + Core
dotnet test tests/HomeBuyerHelper.Core.Tests/HomeBuyerHelper.Core.Tests.csproj
dotnet test tests/HomeBuyerHelper.Data.Tests/HomeBuyerHelper.Data.Tests.csproj
```

The MAUI head (`src/HomeBuyerHelper`) needs platform workloads that are not
installed in headless environments — don't try to build it; keeping its DI
registrations compiling is covered by building Core/Data.

## Run the app

```bash
dotnet run --project src/HomeBuyerHelper.Web --no-build
# Listens on http://localhost:5191 (launchSettings wins over ASPNETCORE_URLS)
```

Wait for readiness: `curl -sf http://localhost:5191/ --noproxy localhost`.

## Drive it

```bash
node tools/verify-journey.mjs            # full user journey, exits non-zero on failure
```

- Playwright is preinstalled globally; the script defaults to
  `/opt/pw-browsers` chromium via the global playwright module. Override
  with `PLAYWRIGHT_MODULE` if the environment differs.
- The journey uses a fresh browser profile each run, so it exercises the
  first-run experience; run it twice in the same profile only via a custom
  script if you need persistence checks.
- Screenshots land in `verify-shots/` (gitignored); attach the relevant one
  to your report.

## Gotchas

- Data lives in browser localStorage (`hbh_*` keys) — restarting the server
  does NOT reset app data; a fresh Playwright context does.
- Currency formatting depends on the invariant-culture override in
  `Program.cs`; if amounts render as `¤` something removed it.
- Form fields are located by their `<label>` text — keep the
  `span > label + input` structure when adding form rows, or update
  `tools/verify-journey.mjs` selectors alongside.

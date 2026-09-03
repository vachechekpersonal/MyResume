# MyResume

An interactive CV for Vache Chek, built as a small showcase of clean .NET engineering.

- **Stack:** .NET 10, Blazor WebAssembly (standalone, static hosting), hand-written CSS, xUnit v3 + bUnit.
- **Design spec:** `docs/superpowers/specs/2026-09-03-myresume-design.md`

## Structure

| Project | Responsibility |
|---|---|
| `src/MyResume.Core` | CV model, JSON contract, skill-filter logic. No UI dependency. |
| `src/MyResume.Web` | Blazor components, browser services (theme, data loading), styles. |
| `tests/MyResume.Tests` | Core unit tests, bUnit component tests, and integrity tests for `cv.json`. |

Dependencies flow one way: `Web → Core`, and `Tests` references both. Package versions are managed centrally in
`Directory.Packages.props`; build settings shared by every project live in `Directory.Build.props`.

## Run locally

    dotnet run --project src/MyResume.Web

## Test

    dotnet test

Tests run on Microsoft.Testing.Platform (opted in via `global.json`), which is required for xUnit v3 on the .NET 10 SDK.

## Edit the CV

All content lives in `src/MyResume.Web/wwwroot/data/cv.json`. Rules enforced by tests:

- experiences are listed newest first and must not overlap;
- every `skills` tag on an experience must appear in `skillGroups`;
- no phone number or email address (LinkedIn only).

## Deploy

Pushing to `main` builds, tests and deploys to GitHub Pages via `.github/workflows/ci.yml`.
Enable Pages once (Settings → Pages → Source: GitHub Actions).

For a custom domain, add the domain in the Pages settings and create `src/MyResume.Web/wwwroot/CNAME`
containing the domain. `<base href="/">` in `index.html` is correct for a root domain. If you ever
host under a sub-path (e.g. `user.github.io/MyResume/`) change it to `<base href="/MyResume/">`.

## Licence

MIT. Third-party dependencies: bUnit (MIT), xUnit (Apache-2.0), .NET (MIT).

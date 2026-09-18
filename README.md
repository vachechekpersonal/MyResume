# MyResume

[![CI](https://github.com/vachechekpersonal/MyResume/actions/workflows/ci.yml/badge.svg)](https://github.com/vachechekpersonal/MyResume/actions/workflows/ci.yml)

An interactive CV for Vache Chek, built as a small showcase of clean .NET engineering.

- **Stack:** .NET 10, Blazor WebAssembly (standalone, static hosting), hand-written CSS, xUnit v3 + bUnit.
- **Design spec:** `docs/design.md`

## Structure

| Project | Responsibility |
|---|---|
| `src/MyResume.Core` | CV model, JSON contract, skill-filter logic. No UI dependency. |
| `src/MyResume.Web` | Blazor components, browser services (theme, data loading), styles. |
| `tools/MyResume.Prerender` | Build-time tool: renders `Home` to static HTML with `HtmlRenderer` and injects it, Open Graph tags and JSON-LD into the published `index.html`. |
| `tests/MyResume.Tests` | Core unit tests, bUnit component tests, prerender tests, and integrity tests for `cv.json`. |

Dependencies flow one way: `Web → Core`, `Prerender → Web`, and `Tests` references all three. Package versions are managed centrally in
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
- every skill chip in `skillGroups` is used by at least one experience (a chip that filters to zero roles is a data error);
- no phone number or email address (LinkedIn only).

## Deploy

Pushing to `main` builds, tests and deploys to GitHub Pages via `.github/workflows/ci.yml`.
After publishing, CI runs `MyResume.Prerender` so the deployed `index.html` already contains the full CV markup,
Open Graph / Twitter tags and a JSON-LD `Person` block. Visitors and crawlers see content before WebAssembly
loads; Blazor then takes over the same DOM.
Enable Pages once (Settings → Pages → Source: GitHub Actions).

For a custom domain, add the domain in the Pages settings and create `src/MyResume.Web/wwwroot/CNAME`
containing the domain. `<base href="/">` in `index.html` is correct for a root domain. If you ever
host under a sub-path (e.g. `user.github.io/MyResume/`) change it to `<base href="/MyResume/">`.

## Licence

MIT. Third-party dependencies: bUnit (MIT), xUnit (Apache-2.0), .NET (MIT).

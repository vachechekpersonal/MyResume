# MyResume improvements – design

Date: 18 September 2026. Supplements `docs/design.md`; nothing here changes the decisions recorded there.

## Goals
1. The CV is visible before WebAssembly runs: crawlers, link previews and first paint all get real content.
2. The deployed site is verified end to end, not just at component level.
3. The skill filter becomes more useful: shareable by URL and enriched with derived experience per skill.
4. The download shrinks and CI hygiene improves.

## Work items, in order
| # | Item | Design |
|---|---|---|
| 1 | Polish | `NotFound` links relative to `<base>`. New integrity test: every skill in `skillGroups` is used by at least one experience. Skills that fail it are removed from `cv.json` and flagged for the author. |
| 2 | Payload | `InvariantGlobalization=true` (the code only uses `CultureInfo.InvariantCulture`). CI installs the `wasm-tools` workload so the runtime is relinked. |
| 3 | CI hygiene | NuGet cache in `setup-dotnet`, Dependabot for NuGet and Actions, CI badge in README, test results always uploaded. |
| 4 | Prerender + share metadata | New `tools/MyResume.Prerender` console project. It loads the published `cv.json`, renders `Home` with Blazor's `HtmlRenderer` and a `FileCvSource`, and writes the markup into `#app` of the published `index.html`. It also injects Open Graph / Twitter meta and a JSON-LD `Person` block built from the same `Cv`. Runs in CI after publish; tested with xUnit against the real `cv.json`. Blazor replaces the prerendered DOM when it starts, so no hydration is needed. |
| 5 | Deep-linkable filter | `SkillSelection` gains `ReplaceWith(IEnumerable<string>)`. A `SkillQuery` Core helper parses/formats `?skills=a,b`. `Home` reads the query on load and a small `UrlSync` in Web writes it back with `NavigationManager.NavigateTo(..., replace: true)` whenever the selection changes. |
| 6 | E2E | `tests/MyResume.E2E` with Microsoft.Playwright + xUnit. CI serves `publish/wwwroot` under `/MyResume/` with a tiny static server (`dotnet serve` style via Kestrel in the test fixture) and checks: content loads, chip filter dims roles, theme persists across reload, `?skills=` deep link applies, 404 fallback renders the not-found page. |
| 7 | Years per skill | Pure `SkillExperience.Compute(cv, today)` in Core: for each skill, months spanned by the union of role periods that used it (union, so overlapping roles do not double count). Chips show "C# · 17 yrs" and the tooltip lists the count of roles. Sorted within group by experience, descending. |
| 8 | Navigation | `Expand all` / `Collapse all` control in the Experience header, implemented through a `TimelineExpansion` signal passed down as a parameter (not a service) so `TimelineEntry` stays presentational. A sticky section nav (`About · Skills · Experience · Education · Languages`) with `aria-current` tracking via a small IntersectionObserver JS module. |
| 9 | Mutation testing | **Blocked (18 Sep 2026).** Stryker.NET 5.0 refuses projects that use Microsoft.Testing.Platform (stryker-mutator/stryker-net#3094), and xUnit v3 on the .NET 10 SDK requires that platform. Adding the VSTest adapter did not help because Stryker keys off the project property. Revisit when Stryker adds MTP support. |

## Testing
Every item follows TDD: Core logic in xUnit, components in bUnit, delivery in Playwright. The prerender tool is tested by rendering the real CV and asserting the profile name, section ids and JSON-LD parse.

## Out of scope
Blog, i18n, analytics, server hosting (unchanged from `docs/design.md`).

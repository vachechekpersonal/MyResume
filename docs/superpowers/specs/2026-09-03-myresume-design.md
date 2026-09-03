# MyResume – Interactive CV Design Spec

Date: 3 September 2026

### Purpose
A personal website that is an interactive version of Vache Chek's CV, hosted statically on a personal domain. It doubles as a portfolio piece, so the code itself must demonstrate clean, SOLID, well-tested, maintainable engineering at a deliberately small scale.

### Decisions (from brainstorming, 3 Sep 2026)
| Topic | Decision |
|---|---|
| Hosting | Static files (GitHub Pages initially, custom domain later) → Blazor WebAssembly standalone |
| Content source | Transcribed from the original Word CV into `wwwroot/data/cv.json` |
| Interactivity | Skill-chip filter that highlights matching roles; collapsible timeline entries; light/dark theme with persistence; print-to-PDF |
| Contact | LinkedIn link only |
| Tests/CI | xUnit + bUnit unit tests; GitHub Actions build/test/publish/deploy |
| Styling | Hand-written modern CSS, scoped per component, no framework |
| Structure | `MyResume.Core` + `MyResume.Web` + `MyResume.Tests` |

### Architecture
- **Core**: records `Cv`, `Profile`, `ContactLink`, `SkillGroup`, `Experience`, `DateRange`, `Qualification`, enum `ExperienceKind`; `ICvSource` abstraction; `CvJsonContext` (source-generated); `ExperienceFilter` (pure); `SkillSelection` (observable selection state, no UI dependency).
- **Web**: `HttpCvSource` implements `ICvSource` over `HttpClient`; `ThemeService` wraps a tiny ES module `js/theme.js`; components are small and single-purpose; `Home.razor` is the only page and the only component that loads data. State flows down through parameters; the only shared state is the scoped `SkillSelection` and `ThemeService` services, injected where needed.
- **Tests**: Core logic tested without Blazor; components tested with bUnit against a fake `ICvSource`; real `cv.json` validated against the model.

### Behaviour
- Skill filter: clicking chips toggles membership in `SkillSelection`. A role matches when it uses **any** selected skill (case-insensitive). Matching roles are expanded and full-strength; non-matching roles collapse and dim. A summary line shows "n of m roles" and a Clear button. Zero matches is allowed and honest.
- Timeline: newest first, vertical line, first role expanded by default, others collapsed. Career break shown as a quiet marker, not a card. Each entry shows period, computed duration, role, company, location, highlights, and skill tags (selected tags highlighted).
- Theme: inline script in `index.html` applies the saved theme before first paint. Falls back to `prefers-color-scheme`. Toggle persists to `localStorage`.
- Print/PDF: `@media print` expands all entries, hides controls, forces light palette; button calls `window.print()`.
- Loading/error: "Loading CV…" while fetching; a plain `role="alert"` message if `cv.json` fails to load or parse.
- Accessibility: semantic landmarks, `aria-pressed` on chips, `aria-expanded`/`aria-controls` on entry toggles, visible focus rings, colour contrast ≥ 4.5:1 in both themes, respects `prefers-reduced-motion`.

### Visual design
Restrained and professional. System font stack. Max content width 72rem, generous vertical rhythm, one accent colour (deep teal `#0f766e` light / `#2dd4bf` dark), neutral greys via custom properties. Chips are pill buttons; the timeline uses a 2px vertical rule with accent dots. Responsive to 360px wide.

### Out of scope
Blog, multiple pages, CMS, analytics, contact form, i18n, server-side rendering.


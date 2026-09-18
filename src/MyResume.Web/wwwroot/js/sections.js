// Section navigation: smooth-scroll on click without losing the query string, and highlight the section in view.
// Used by SectionNav.razor after Blazor starts; before that the plain fragment links work on their own.
const state = new WeakMap();

export function observe(nav) {
    const links = [...nav.querySelectorAll("a[data-section]")];
    const sections = links
        .map((link) => document.getElementById(link.dataset.section))
        .filter((section) => section !== null);

    const onClick = (event) => {
        const link = event.target.closest("a[data-section]");
        if (!link) {
            return;
        }
        const target = document.getElementById(link.dataset.section);
        if (!target) {
            return;
        }
        event.preventDefault();
        const reduceMotion = window.matchMedia("(prefers-reduced-motion: reduce)").matches;
        target.scrollIntoView({ behavior: reduceMotion ? "auto" : "smooth", block: "start" });
        // Resolve against the current URL so ?skills= is preserved; no history entry per click.
        history.replaceState(null, "", new URL("#" + link.dataset.section, location.href));
    };
    nav.addEventListener("click", onClick);

    const visible = new Map();
    const observer = new IntersectionObserver((entries) => {
        for (const entry of entries) {
            visible.set(entry.target.id, entry.isIntersecting ? entry.intersectionRatio : 0);
        }
        let best = null;
        for (const [id, ratio] of visible) {
            if (ratio > 0 && (best === null || ratio > visible.get(best))) {
                best = id;
            }
        }
        for (const link of links) {
            if (link.dataset.section === best) {
                link.setAttribute("aria-current", "true");
            } else {
                link.removeAttribute("aria-current");
            }
        }
    }, { rootMargin: "-20% 0px -60% 0px", threshold: [0, 0.25, 0.5, 1] });
    sections.forEach((section) => observer.observe(section));

    state.set(nav, { onClick, observer });
}

export function unobserve(nav) {
    const entry = state.get(nav);
    if (!entry) {
        return;
    }
    nav.removeEventListener("click", entry.onClick);
    entry.observer.disconnect();
    state.delete(nav);
}

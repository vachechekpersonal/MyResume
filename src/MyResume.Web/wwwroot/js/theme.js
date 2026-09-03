// Theme persistence. The inline script in index.html applies the saved value before first paint;
// this module is used by ThemeService after Blazor starts.
const STORAGE_KEY = "theme";

export function getTheme() {
    const explicit = document.documentElement.dataset.theme;
    if (explicit === "light" || explicit === "dark") {
        return explicit;
    }
    return window.matchMedia("(prefers-color-scheme: dark)").matches ? "dark" : "light";
}

export function setTheme(theme) {
    document.documentElement.dataset.theme = theme;
    try {
        localStorage.setItem(STORAGE_KEY, theme);
    } catch {
        // Storage may be unavailable (private mode); the theme still applies for this visit.
    }
}

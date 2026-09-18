using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace MyResume.E2E;

/// <summary>
/// Drives the published site in a real browser. Skipped unless <see cref="StaticSite.EnvironmentVariable"/>
/// points at a published <c>wwwroot</c>; CI sets it after the prepare and prerender steps.
/// </summary>
public sealed class SiteTests(SiteFixture fixture) : IClassFixture<SiteFixture>
{
    private const string Ready = "html[data-blazor='ready']";

    [Fact]
    public async Task Prerendered_page_shows_the_cv_before_any_script_runs()
    {
        var page = await NewPageAsync(javaScript: false);

        var response = await page.GotoAsync(Root);

        Assert.Equal(200, response!.Status);
        await Expect(page.Locator("h1")).ToHaveTextAsync("Vache Chek");
        await Expect(page.Locator("section#experience")).ToBeVisibleAsync();
        await Expect(page.Locator("""meta[property="og:title"]""")).ToHaveAttributeAsync("content", new Regex("Vache Chek"));
        Assert.Contains("\"@type\":\"Person\"", await page.ContentAsync(), StringComparison.Ordinal);
    }

    [Fact]
    public async Task Blazor_starts_and_the_skill_filter_updates_roles_and_url()
    {
        var page = await OpenAsync(Root);

        var chip = page.Locator("button.chip", new() { HasText = "Kubernetes" });
        await chip.ClickAsync();

        await Expect(chip).ToHaveAttributeAsync("aria-pressed", "true");
        await Expect(page.Locator("p.filter-summary")).ToContainTextAsync("1 of 8 roles");
        await Expect(page.Locator("li.entry--dimmed")).ToHaveCountAsync(7);
        await Expect(page).ToHaveURLAsync(new Regex(@"\?skills=Kubernetes$"));
    }

    [Fact]
    public async Task Deep_link_applies_the_filter_on_load()
    {
        var page = await OpenAsync(Root + "?skills=Neo4j");

        await Expect(page.Locator("button.chip[aria-pressed='true']")).ToContainTextAsync("Neo4j");
        await Expect(page.Locator("p.filter-summary")).ToContainTextAsync("Neo4j");
    }

    [Fact]
    public async Task Theme_choice_survives_a_reload_without_a_flash()
    {
        var page = await OpenAsync(Root);
        await page.GetByRole(AriaRole.Button, new() { Name = "Switch to dark theme" }).ClickAsync();
        await Expect(page.Locator("html")).ToHaveAttributeAsync("data-theme", "dark");

        await page.ReloadAsync(new() { WaitUntil = WaitUntilState.Commit });

        // Applied by the inline script in <head>, i.e. before Blazor has even started downloading.
        await Expect(page.Locator("html")).ToHaveAttributeAsync("data-theme", "dark");
        await page.WaitForSelectorAsync(Ready);
        await Expect(page.GetByRole(AriaRole.Button, new() { Name = "Switch to light theme" })).ToBeVisibleAsync();
    }

    [Fact]
    public async Task Unknown_path_returns_404_and_routes_to_the_not_found_page_inside_the_base_path()
    {
        var page = await NewPageAsync();

        var response = await page.GotoAsync(Root + "no-such-page");

        Assert.Equal(404, response!.Status);
        await page.WaitForSelectorAsync(Ready);
        await Expect(page.Locator("h1")).ToHaveTextAsync("Page not found");
        var back = page.GetByRole(AriaRole.Link, new() { Name = "Back to the CV" });
        Assert.Equal(Root, await back.EvaluateAsync<string>("a => a.href"));
    }

    [Fact]
    public async Task Section_nav_scrolls_and_keeps_the_query_string()
    {
        var page = await OpenAsync(Root + "?skills=Redis");

        await page.GetByRole(AriaRole.Navigation, new() { Name = "Sections" })
            .GetByRole(AriaRole.Link, new() { Name = "Education" }).ClickAsync();

        await Expect(page).ToHaveURLAsync(new Regex(@"\?skills=Redis#education$"));
        await Expect(page.Locator("button.chip[aria-pressed='true']")).ToContainTextAsync("Redis");
        await Expect(page.Locator("section#education")).ToBeInViewportAsync();
    }

    [Fact]
    public async Task Expand_all_opens_every_role()
    {
        var page = await OpenAsync(Root);

        await page.GetByRole(AriaRole.Button, new() { Name = "Expand all" }).ClickAsync();

        await Expect(page.Locator("button.entry__toggle[aria-expanded='false']")).ToHaveCountAsync(0);
        await Expect(page.Locator("button.entry__toggle[aria-expanded='true']")).ToHaveCountAsync(8);
    }

    [Fact]
    public async Task Print_stylesheet_expands_every_role()
    {
        var page = await OpenAsync(Root);
        await page.EmulateMediaAsync(new() { Media = Media.Print });

        var hidden = await page.Locator("div.entry__body--collapsed").EvaluateAllAsync<int>(
            "els => els.filter(e => getComputedStyle(e).display === 'none').length");

        Assert.Equal(0, hidden);
        await Expect(page.Locator("button.chip").First).ToBeVisibleAsync();
        await Expect(page.Locator(".site-header__actions")).ToBeHiddenAsync();
    }

    [Theory]
    [InlineData("light", 1280)]
    [InlineData("dark", 1280)]
    [InlineData("light", 390)]
    public async Task Screenshots_for_visual_review(string theme, int width)
    {
        var directory = Environment.GetEnvironmentVariable("MYRESUME_SCREENSHOTS");
        if (string.IsNullOrEmpty(directory))
        {
            Assert.Skip("Set MYRESUME_SCREENSHOTS to a directory to capture full-page screenshots.");
        }

        var browser = fixture.Browser ?? throw Skipped();
        var context = await browser.NewContextAsync(new() { ViewportSize = new() { Width = width, Height = 900 }, ColorScheme = theme == "dark" ? ColorScheme.Dark : ColorScheme.Light });
        var page = await context.NewPageAsync();
        await page.GotoAsync(Root + "?skills=Azure");
        await page.WaitForSelectorAsync(Ready);
        await page.GetByRole(AriaRole.Button, new() { Name = "Expand all" }).ClickAsync();

        Directory.CreateDirectory(directory);
        await page.ScreenshotAsync(new() { Path = Path.Combine(directory, $"cv-{theme}-{width}.png"), FullPage = true });
    }

    private string Root => (fixture.Site ?? throw Skipped()).Root.ToString();

    private async Task<IPage> OpenAsync(string url)
    {
        var page = await NewPageAsync();
        await page.GotoAsync(url);
        await page.WaitForSelectorAsync(Ready);
        return page;
    }

    private async Task<IPage> NewPageAsync(bool javaScript = true)
    {
        var browser = fixture.Browser ?? throw Skipped();
        var context = await browser.NewContextAsync(new() { JavaScriptEnabled = javaScript });
        return await context.NewPageAsync();
    }

    private static Exception Skipped()
    {
        Assert.Skip($"Set {StaticSite.EnvironmentVariable} to a published wwwroot directory to run the browser tests.");
        return new InvalidOperationException("unreachable");
    }
}

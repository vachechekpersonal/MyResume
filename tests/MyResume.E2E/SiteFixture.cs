using Microsoft.Playwright;

namespace MyResume.E2E;

/// <summary>One static server and one headless browser shared by every test in the class.</summary>
public sealed class SiteFixture : IAsyncLifetime
{
    private IPlaywright? _playwright;

    public StaticSite? Site { get; private set; }

    public IBrowser? Browser { get; private set; }

    public async ValueTask InitializeAsync()
    {
        if (StaticSite.ConfiguredDirectory is not { Length: > 0 } directory)
        {
            return;
        }

        Site = await StaticSite.StartAsync(directory);
        _playwright = await Playwright.CreateAsync();
        Browser = await _playwright.Chromium.LaunchAsync();
    }

    public async ValueTask DisposeAsync()
    {
        if (Browser is not null)
        {
            await Browser.DisposeAsync();
        }

        _playwright?.Dispose();

        if (Site is not null)
        {
            await Site.DisposeAsync();
        }
    }
}

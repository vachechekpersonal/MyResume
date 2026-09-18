using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using MyResume.Core.Data;
using MyResume.Core.Filtering;
using MyResume.Web.Pages;
using MyResume.Web.Services;

namespace MyResume.Prerender;

/// <summary>
/// Renders <see cref="Home"/> to static HTML with Blazor's <see cref="HtmlRenderer"/>, using the same
/// components and services as the browser app so the prerendered page matches the first interactive frame.
/// </summary>
public static class PageRenderer
{
    public static async Task<string> RenderHomeAsync(ICvSource cvSource, TimeProvider clock, Uri siteUrl)
    {
        ArgumentNullException.ThrowIfNull(cvSource);
        ArgumentNullException.ThrowIfNull(clock);
        ArgumentNullException.ThrowIfNull(siteUrl);

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton(cvSource);
        services.AddSingleton(clock);
        services.AddSingleton<NavigationManager>(new StaticNavigationManager(siteUrl));
        services.AddSingleton<IJSRuntime, NoJSRuntime>();
        services.AddScoped<SkillSelection>();
        services.AddScoped<ThemeService>();

        await using var provider = services.BuildServiceProvider();
        await using var renderer = new HtmlRenderer(provider, provider.GetRequiredService<ILoggerFactory>());

        return await renderer.Dispatcher.InvokeAsync(async () =>
        {
            var output = await renderer.RenderComponentAsync<Home>(ParameterView.Empty);
            return output.ToHtmlString();
        });
    }
}

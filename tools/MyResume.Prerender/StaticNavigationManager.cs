using Microsoft.AspNetCore.Components;

namespace MyResume.Prerender;

/// <summary>
/// The site's root URL, fixed for the whole render. Components read it to build links and query strings;
/// nothing navigates during static rendering, so <see cref="NavigateToCore"/> is a programming error.
/// </summary>
internal sealed class StaticNavigationManager : NavigationManager
{
    public StaticNavigationManager(Uri siteUrl)
    {
        var root = siteUrl.ToString();
        Initialize(root, root);
    }

    protected override void NavigateToCore(string uri, NavigationOptions options) =>
        throw new InvalidOperationException($"Navigation to '{uri}' is not possible during prerendering.");
}

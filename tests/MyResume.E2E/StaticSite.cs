using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MyResume.E2E;

/// <summary>
/// Serves a published <c>wwwroot</c> on a random local port the way GitHub Pages does: under the sub-path
/// named by the page's <c>&lt;base href&gt;</c>, with <c>404.html</c> returned (status 404) for unknown paths.
/// </summary>
public sealed partial class StaticSite : IAsyncDisposable
{
    /// <summary>Directory of the published site to test, e.g. <c>publish/wwwroot</c> after CI's prepare step.</summary>
    public const string EnvironmentVariable = "MYRESUME_SITE";

    private readonly WebApplication _app;

    private StaticSite(WebApplication app, Uri root)
    {
        _app = app;
        Root = root;
    }

    /// <summary>Absolute URL of the CV page, including the sub-path and a trailing slash.</summary>
    public Uri Root { get; }

    public static string? ConfiguredDirectory => Environment.GetEnvironmentVariable(EnvironmentVariable);

    public static async Task<StaticSite> StartAsync(string directory)
    {
        var index = await File.ReadAllTextAsync(Path.Combine(directory, "index.html"));
        var basePath = BaseHref().Match(index) is { Success: true } m ? m.Groups[1].Value.TrimEnd('/') : string.Empty;
        var notFound = Path.Combine(directory, "404.html");

        var builder = WebApplication.CreateSlimBuilder();
        builder.Logging.SetMinimumLevel(LogLevel.Warning);
        builder.WebHost.UseUrls("http://127.0.0.1:0");
        var app = builder.Build();

        if (basePath.Length > 0)
        {
            app.UsePathBase(basePath);
        }

        var files = new PhysicalFileProvider(directory);
        app.UseDefaultFiles(new DefaultFilesOptions { FileProvider = files });
        app.UseStaticFiles(new StaticFileOptions { FileProvider = files });
        app.Run(async context =>
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            context.Response.ContentType = "text/html; charset=utf-8";
            await context.Response.SendFileAsync(File.Exists(notFound) ? notFound : Path.Combine(directory, "index.html"));
        });

        await app.StartAsync();
        var address = app.Services.GetRequiredService<IServer>().Features.Get<IServerAddressesFeature>()!.Addresses.First();
        return new StaticSite(app, new Uri($"{address}{basePath}/"));
    }

    public async ValueTask DisposeAsync()
    {
        await _app.StopAsync();
        await _app.DisposeAsync();
    }

    [GeneratedRegex("""<base href="([^"]*)"\s*/?>""")]
    private static partial Regex BaseHref();
}

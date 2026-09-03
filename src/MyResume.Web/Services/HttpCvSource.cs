using System.Net.Http.Json;
using MyResume.Core.Data;
using MyResume.Core.Models;

namespace MyResume.Web.Services;

/// <summary>Loads the CV from the static <c>data/cv.json</c> file served alongside the app.</summary>
public sealed class HttpCvSource(HttpClient http) : ICvSource
{
    private const string Path = "data/cv.json";

    public async Task<Cv> LoadAsync(CancellationToken cancellationToken = default) =>
        await http.GetFromJsonAsync(Path, CvJsonContext.Default.Cv, cancellationToken).ConfigureAwait(false)
        ?? throw new InvalidOperationException($"{Path} contained no CV document.");
}

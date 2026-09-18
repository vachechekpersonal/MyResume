using System.Text.Json;
using MyResume.Core.Data;
using MyResume.Core.Models;

namespace MyResume.Prerender;

/// <summary>Loads the CV from a file on disk, for rendering at build time.</summary>
public sealed class FileCvSource(string path) : ICvSource
{
    public async Task<Cv> LoadAsync(CancellationToken cancellationToken = default)
    {
        await using var stream = File.OpenRead(path);
        return await JsonSerializer.DeserializeAsync(stream, CvJsonContext.Default.Cv, cancellationToken)
            ?? throw new InvalidOperationException($"{path} contained no CV document.");
    }
}

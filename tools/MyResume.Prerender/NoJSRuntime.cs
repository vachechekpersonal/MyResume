using Microsoft.JSInterop;

namespace MyResume.Prerender;

/// <summary>
/// There is no browser at build time. Components only call JavaScript from event handlers, which never
/// fire during static rendering, so any call reaching this class is a bug worth surfacing.
/// </summary>
internal sealed class NoJSRuntime : IJSRuntime
{
    public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args) =>
        throw new InvalidOperationException($"JavaScript ('{identifier}') is not available during prerendering.");

    public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args) =>
        InvokeAsync<TValue>(identifier, args);
}

using Microsoft.JSInterop;

namespace MyResume.Web.Services;

public static class Themes
{
    public const string Light = "light";
    public const string Dark = "dark";
}

/// <summary>Owns the current colour theme and persists it through the <c>js/theme.js</c> module.</summary>
public sealed class ThemeService(IJSRuntime js) : IAsyncDisposable
{
    private IJSObjectReference? _module;

    public event Action? Changed;

    public string Current { get; private set; } = Themes.Light;

    public bool IsInitialised => _module is not null;

    public async Task InitialiseAsync()
    {
        _module ??= await js.InvokeAsync<IJSObjectReference>("import", "./js/theme.js").ConfigureAwait(false);
        Current = await _module.InvokeAsync<string>("getTheme").ConfigureAwait(false);
        Changed?.Invoke();
    }

    public async Task ToggleAsync()
    {
        if (_module is null)
        {
            await InitialiseAsync().ConfigureAwait(false);
        }

        var next = Current == Themes.Dark ? Themes.Light : Themes.Dark;
        await _module!.InvokeVoidAsync("setTheme", next).ConfigureAwait(false);
        Current = next;
        Changed?.Invoke();
    }

    public async ValueTask DisposeAsync()
    {
        if (_module is not null)
        {
            await _module.DisposeAsync().ConfigureAwait(false);
        }
    }
}

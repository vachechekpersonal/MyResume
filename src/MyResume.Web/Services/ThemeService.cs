using Microsoft.JSInterop;

namespace MyResume.Web.Services;

public static class Themes
{
    public const string Light = "light";
    public const string Dark = "dark";
}

/// <summary>
/// Owns the current colour theme and persists it through the <c>js/theme.js</c> module.
/// <see cref="InitialiseAsync"/> is called once at start-up (see <c>Program.cs</c>) so components
/// always see the real theme on their first render.
/// </summary>
public sealed class ThemeService(IJSRuntime js) : IAsyncDisposable
{
    private IJSObjectReference? _module;

    public event Action? Changed;

    public string Current { get; private set; } = Themes.Light;

    public async Task InitialiseAsync()
    {
        _module ??= await js.InvokeAsync<IJSObjectReference>("import", "./js/theme.js");
        Current = await _module.InvokeAsync<string>("getTheme");
        Changed?.Invoke();
    }

    public async Task ToggleAsync()
    {
        if (_module is null)
        {
            throw new InvalidOperationException($"{nameof(InitialiseAsync)} must be called before toggling the theme.");
        }

        var next = Current == Themes.Dark ? Themes.Light : Themes.Dark;
        await _module.InvokeVoidAsync("setTheme", next);
        Current = next;
        Changed?.Invoke();
    }

    public async ValueTask DisposeAsync()
    {
        if (_module is not null)
        {
            await _module.DisposeAsync();
        }
    }
}

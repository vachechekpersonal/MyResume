using Microsoft.Extensions.DependencyInjection;
using MyResume.Web.Components;
using MyResume.Web.Services;

namespace MyResume.Tests.Web;

public sealed class ThemeToggleTests : BunitContext
{
    private readonly BunitJSModuleInterop _module;

    public ThemeToggleTests()
    {
        Services.AddScoped<ThemeService>();
        _module = JSInterop.SetupModule("./js/theme.js");
        _module.SetupVoid("setTheme", _ => true).SetVoidResult();
    }

    [Fact]
    public async Task Reflects_the_theme_read_at_start_up()
    {
        _module.Setup<string>("getTheme").SetResult(Themes.Dark);
        await InitialiseThemeAsync();

        var cut = Render<ThemeToggle>();

        Assert.Equal("Switch to light theme", cut.Find("button").GetAttribute("aria-label"));
    }

    [Fact]
    public async Task Click_sets_dark_theme_via_js_and_updates_label()
    {
        _module.Setup<string>("getTheme").SetResult(Themes.Light);
        await InitialiseThemeAsync();
        var cut = Render<ThemeToggle>();
        Assert.Equal("Switch to dark theme", cut.Find("button").GetAttribute("aria-label"));

        cut.Find("button").Click();

        cut.WaitForAssertion(() =>
        {
            var call = Assert.Single(_module.Invocations["setTheme"]);
            Assert.Equal(Themes.Dark, call.Arguments[0]);
            Assert.Equal("Switch to light theme", cut.Find("button").GetAttribute("aria-label"));
        });
    }

    [Fact]
    public async Task Toggling_before_initialisation_is_a_programming_error()
    {
        var theme = Services.GetRequiredService<ThemeService>();

        await Assert.ThrowsAsync<InvalidOperationException>(theme.ToggleAsync);
    }

    private Task InitialiseThemeAsync() => Services.GetRequiredService<ThemeService>().InitialiseAsync();
}

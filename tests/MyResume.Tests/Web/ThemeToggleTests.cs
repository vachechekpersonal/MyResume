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
        _module.Setup<string>("getTheme").SetResult(Themes.Light);
        _module.SetupVoid("setTheme", _ => true).SetVoidResult();
    }

    [Fact]
    public void Offers_to_switch_to_dark_when_light()
    {
        var cut = Render<ThemeToggle>();

        cut.WaitForAssertion(() =>
            Assert.Equal("Switch to dark theme", cut.Find("button").GetAttribute("aria-label")));
    }

    [Fact]
    public void Click_sets_dark_theme_via_js_and_updates_label()
    {
        var cut = Render<ThemeToggle>();
        cut.WaitForAssertion(() => Assert.Equal("Switch to dark theme", cut.Find("button").GetAttribute("aria-label")));

        cut.Find("button").Click();

        cut.WaitForAssertion(() =>
        {
            var call = Assert.Single(_module.Invocations["setTheme"]);
            Assert.Equal(Themes.Dark, call.Arguments[0]);
            Assert.Equal("Switch to light theme", cut.Find("button").GetAttribute("aria-label"));
        });
    }
}

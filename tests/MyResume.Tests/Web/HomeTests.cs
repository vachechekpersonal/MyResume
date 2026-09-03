using Microsoft.Extensions.DependencyInjection;
using MyResume.Core.Data;
using MyResume.Core.Filtering;
using MyResume.Web.Pages;

namespace MyResume.Tests.Web;

public sealed class HomeTests : BunitContext
{
    public HomeTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddScoped<SkillSelection>();
        Services.AddSingleton<TimeProvider>(FixedTimeProvider.September2026);
    }

    [Fact]
    public void Shows_loading_state_until_cv_arrives()
    {
        Services.AddSingleton<ICvSource>(FakeCvSource.Pending());

        var cut = Render<Home>();

        var status = cut.Find("p.status");
        Assert.Equal("true", status.GetAttribute("aria-busy"));
        Assert.Contains("Loading", status.TextContent, StringComparison.Ordinal);
    }

    [Fact]
    public void Shows_error_when_cv_cannot_be_loaded()
    {
        Services.AddSingleton<ICvSource>(FakeCvSource.Failing(new HttpRequestException("boom")));

        var cut = Render<Home>();

        cut.WaitForAssertion(() =>
        {
            var alert = cut.Find("[role=alert]");
            Assert.Contains("could not be loaded", alert.TextContent, StringComparison.Ordinal);
        });
    }

    [Fact]
    public void Renders_profile_name_when_loaded()
    {
        Services.AddSingleton<ICvSource>(FakeCvSource.Returning(TestData.Cv()));

        var cut = Render<Home>();

        cut.WaitForAssertion(() => Assert.Contains("Test Person", cut.Find("h1").TextContent, StringComparison.Ordinal));
    }
}

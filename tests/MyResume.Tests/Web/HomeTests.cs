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
    public void Renders_header_profile_and_footer_when_loaded()
    {
        Services.AddSingleton<ICvSource>(FakeCvSource.Returning(TestData.Cv()));

        var cut = Render<Home>();

        cut.WaitForAssertion(() =>
        {
            Assert.Equal("Test Person", cut.Find("header h1").TextContent);
            Assert.Contains("A summary.", cut.Find("#about").TextContent, StringComparison.Ordinal);
            var link = cut.Find("#about a");
            Assert.Equal("https://example.com/in/test", link.GetAttribute("href"));
            Assert.Equal("noopener noreferrer", link.GetAttribute("rel"));
            Assert.Contains("2026", cut.Find("footer").TextContent, StringComparison.Ordinal);
        });
    }

    [Fact]
    public void Renders_all_sections_when_loaded()
    {
        Services.AddSingleton<ICvSource>(FakeCvSource.Returning(TestData.Cv()));

        var cut = Render<Home>();

        cut.WaitForAssertion(() =>
        {
            foreach (var id in new[] { "about", "skills", "experience", "education", "languages" })
            {
                Assert.NotNull(cut.Find($"section#{id}"));
            }

            Assert.Contains("Azure Developer Associate", cut.Find("#education").TextContent, StringComparison.Ordinal);
            Assert.Contains("English", cut.Find("#languages").TextContent, StringComparison.Ordinal);
        });
    }
}

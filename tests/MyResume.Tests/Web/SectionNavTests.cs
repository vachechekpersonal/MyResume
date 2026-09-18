using MyResume.Web.Components;

namespace MyResume.Tests.Web;

public sealed class SectionNavTests : BunitContext
{
    private static readonly IReadOnlyList<SectionLink> Links = [new("about", "Profile"), new("skills", "Skills")];

    private readonly BunitJSModuleInterop _module;

    public SectionNavTests()
    {
        _module = JSInterop.SetupModule("./js/sections.js");
        _module.SetupVoid("observe", _ => true).SetVoidResult();
        _module.SetupVoid("unobserve", _ => true).SetVoidResult();
    }

    [Fact]
    public void Renders_a_fragment_link_per_section()
    {
        var cut = Render<SectionNav>(p => p.Add(c => c.Sections, Links));

        var anchors = cut.FindAll("nav[aria-label='Sections'] a");
        Assert.Equal(["#about", "#skills"], anchors.Select(a => a.GetAttribute("href")));
        Assert.Equal(["Profile", "Skills"], anchors.Select(a => a.TextContent));
        Assert.Equal("skills", anchors[1].GetAttribute("data-section"));
    }

    [Fact]
    public async Task Starts_observing_after_first_render_and_stops_on_dispose()
    {
        Render<SectionNav>(p => p.Add(c => c.Sections, Links));
        Assert.Single(_module.Invocations["observe"]);

        await DisposeComponentsAsync();

        Assert.Single(_module.Invocations["unobserve"]);
    }
}

using Microsoft.Extensions.DependencyInjection;
using MyResume.Core.Filtering;
using MyResume.Web.Components;

namespace MyResume.Tests.Web;

public sealed class SkillsSectionTests : BunitContext
{
    private readonly SkillSelection _selection = new();

    public SkillsSectionTests() => Services.AddSingleton(_selection);

    [Fact]
    public void Renders_one_heading_and_chip_group_per_skill_group()
    {
        var cut = RenderSection();

        Assert.Equal(["Languages", "Cloud"], cut.FindAll("h3").Select(h => h.TextContent));
        Assert.Equal(3, cut.FindAll("button.chip").Count);
    }

    [Fact]
    public void Clear_button_appears_only_when_selection_is_active()
    {
        var cut = RenderSection();
        Assert.Empty(cut.FindAll("button.clear"));

        cut.FindAll("button.chip")[0].Click();

        var clear = cut.Find("button.clear");
        Assert.Contains("Clear", clear.TextContent, StringComparison.Ordinal);

        clear.Click();

        Assert.False(_selection.IsActive);
        Assert.Empty(cut.FindAll("button.clear"));
    }

    private IRenderedComponent<SkillsSection> RenderSection() =>
        Render<SkillsSection>(p => p.Add(c => c.Groups, TestData.Cv().SkillGroups));
}

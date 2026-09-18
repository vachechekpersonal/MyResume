using Microsoft.Extensions.DependencyInjection;
using MyResume.Core.Filtering;
using MyResume.Web.Components;

namespace MyResume.Tests.Web;

public sealed class SkillsSectionTests : BunitContext
{
    private readonly SkillSelection _selection = new();

    public SkillsSectionTests()
    {
        Services.AddSingleton(_selection);
        Services.AddSingleton<TimeProvider>(FixedTimeProvider.September2026);
    }

    [Fact]
    public void Renders_one_heading_and_chip_group_per_skill_group()
    {
        var cut = RenderSection();

        Assert.Equal(["Languages", "Cloud"], cut.FindAll("h3").Select(h => h.TextContent));
        Assert.Equal(3, cut.FindAll("button.chip").Count);
    }

    [Fact]
    public void Clicking_a_chip_toggles_the_shared_selection_and_presses_it()
    {
        var cut = RenderSection();

        cut.FindAll("button.chip")[0].Click();

        Assert.True(_selection.IsSelected("C#"));
        Assert.Equal("true", cut.FindAll("button.chip")[0].GetAttribute("aria-pressed"));
    }

    [Fact]
    public void Re_renders_when_selection_changes_elsewhere()
    {
        var cut = RenderSection();

        _selection.Toggle("Azure");

        cut.WaitForAssertion(() => Assert.Equal("true", cut.Find("button.chip[aria-pressed='true']").GetAttribute("aria-pressed")));
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

    [Fact]
    public void Chips_show_years_of_experience_and_sort_by_it_within_a_group()
    {
        var cut = RenderSection(withExperiences: true);

        var chips = cut.FindAll("button.chip");
        // Languages group: C# (Apr 2021 – Sep 2026, 66 months) outranks React (Oct 2019 – Mar 2021, 18 months).
        Assert.StartsWith("C#", chips[0].TextContent.Trim(), StringComparison.Ordinal);
        Assert.Equal("6 yrs", chips[0].QuerySelector(".chip__detail")!.TextContent);
        Assert.Equal("Used in 1 role over 5 yrs 6 mos", chips[0].GetAttribute("title"));
        Assert.Equal("2 yrs", chips[1].QuerySelector(".chip__detail")!.TextContent);
    }

    [Fact]
    public void Chips_without_experience_data_have_no_detail()
    {
        var cut = RenderSection();

        Assert.Empty(cut.FindAll(".chip__detail"));
    }

    private IRenderedComponent<SkillsSection> RenderSection(bool withExperiences = false) =>
        Render<SkillsSection>(p => p
            .Add(c => c.Groups, TestData.Cv().SkillGroups)
            .Add(c => c.Experiences, withExperiences ? TestData.Cv().Experiences : []));
}

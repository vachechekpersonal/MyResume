using Microsoft.Extensions.DependencyInjection;
using MyResume.Core.Filtering;
using MyResume.Core.Models;
using MyResume.Web.Components;

namespace MyResume.Tests.Web;

public sealed class TimelineSectionTests : BunitContext
{
    private readonly SkillSelection _selection = new();

    public TimelineSectionTests()
    {
        Services.AddSingleton(_selection);
        Services.AddSingleton<TimeProvider>(FixedTimeProvider.September2026);
    }

    [Fact]
    public void Orders_newest_first_regardless_of_input_order()
    {
        var older = TestData.Role("Old", "Dev", new DateOnly(2015, 1, 1), new DateOnly(2016, 1, 1), "C#");
        var newer = TestData.Role("New", "Dev", new DateOnly(2020, 1, 1), null, "C#");

        var cut = RenderSection([older, newer]);

        var companies = cut.FindAll(".entry__company").Select(e => e.TextContent.Trim()).ToList();
        Assert.StartsWith("New", companies[0], StringComparison.Ordinal);
        Assert.StartsWith("Old", companies[1], StringComparison.Ordinal);
    }

    [Fact]
    public void First_entry_is_expanded_by_default_others_collapsed()
    {
        var cut = RenderSection(TestData.Cv().Experiences);

        var toggles = cut.FindAll("button.entry__toggle");
        Assert.Equal("true", toggles[0].GetAttribute("aria-expanded"));
        Assert.Equal("false", toggles[1].GetAttribute("aria-expanded"));
    }

    [Fact]
    public void Filter_summary_is_a_persistent_live_region_shown_only_when_active()
    {
        var cut = RenderSection(TestData.Cv().Experiences);
        var summary = cut.Find("p.filter-summary[role=status]");
        Assert.True(summary.HasAttribute("hidden"));

        _selection.Toggle("React");

        cut.WaitForAssertion(() =>
        {
            summary = cut.Find("p.filter-summary");
            Assert.False(summary.HasAttribute("hidden"));
            Assert.Contains("1 of 2 roles", summary.TextContent, StringComparison.Ordinal);
        });
        Assert.Single(cut.FindAll("li.entry--dimmed"));

        cut.Find("p.filter-summary button").Click();

        Assert.False(_selection.IsActive);
        Assert.True(cut.Find("p.filter-summary").HasAttribute("hidden"));
        Assert.Empty(cut.FindAll("li.entry--dimmed"));
    }

    [Fact]
    public void Selected_skills_are_listed_alphabetically()
    {
        var cut = RenderSection(TestData.Cv().Experiences);

        _selection.Toggle("React");
        _selection.Toggle("Azure");

        cut.WaitForAssertion(() =>
            Assert.Contains("using Azure, React", cut.Find("p.filter-summary").TextContent, StringComparison.Ordinal));
    }

    private IRenderedComponent<TimelineSection> RenderSection(IReadOnlyList<Experience> experiences) =>
        Render<TimelineSection>(p => p.Add(c => c.Experiences, experiences));
}

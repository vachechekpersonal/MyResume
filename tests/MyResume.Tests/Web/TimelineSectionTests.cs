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
    public void Filter_summary_shows_match_count_and_clears()
    {
        var cut = RenderSection(TestData.Cv().Experiences);
        Assert.Empty(cut.FindAll("p.filter-summary"));

        _selection.Toggle("React");

        cut.WaitForAssertion(() =>
            Assert.Contains("1 of 2 roles", cut.Find("p.filter-summary").TextContent, StringComparison.Ordinal));

        cut.Find("p.filter-summary button").Click();

        Assert.False(_selection.IsActive);
    }

    private IRenderedComponent<TimelineSection> RenderSection(IReadOnlyList<Experience> experiences) =>
        Render<TimelineSection>(p => p.Add(c => c.Experiences, experiences));
}

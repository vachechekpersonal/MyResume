using Microsoft.Extensions.DependencyInjection;
using MyResume.Core.Models;
using MyResume.Web.Components;

namespace MyResume.Tests.Web;

public sealed class TimelineEntryTests : BunitContext
{
    private static readonly Experience Role =
        TestData.Role("Acme", "Senior Engineer", new DateOnly(2021, 4, 1), null, "C#", "Azure");

    private static readonly IReadOnlySet<string> Nothing = new HashSet<string>();

    public TimelineEntryTests() => Services.AddSingleton<TimeProvider>(FixedTimeProvider.September2026);

    [Fact]
    public void Shows_period_duration_role_and_company()
    {
        var cut = RenderEntry(Role);

        var text = cut.Find("button.entry__toggle").TextContent;
        Assert.Contains("Apr 2021 – Present", text, StringComparison.Ordinal);
        Assert.Contains("5 yrs 6 mos", text, StringComparison.Ordinal);
        Assert.Contains("Senior Engineer", text, StringComparison.Ordinal);
        Assert.Contains("Acme", text, StringComparison.Ordinal);
    }

    [Fact]
    public void Collapsed_by_default_and_toggle_expands()
    {
        var cut = RenderEntry(Role);
        var toggle = cut.Find("button.entry__toggle");
        Assert.Equal("false", toggle.GetAttribute("aria-expanded"));
        Assert.NotNull(cut.Find("div.entry__body--collapsed"));

        toggle.Click();

        Assert.Equal("true", cut.Find("button.entry__toggle").GetAttribute("aria-expanded"));
        Assert.Empty(cut.FindAll("div.entry__body--collapsed"));
    }

    [Fact]
    public void Toggle_controls_the_body_element()
    {
        var cut = RenderEntry(Role);

        var controls = cut.Find("button.entry__toggle").GetAttribute("aria-controls");
        Assert.Equal(controls, cut.Find("div.entry__body").Id);
    }

    [Fact]
    public void InitiallyExpanded_renders_open()
    {
        var cut = RenderEntry(Role, expanded: true);

        Assert.Equal("true", cut.Find("button.entry__toggle").GetAttribute("aria-expanded"));
    }

    [Fact]
    public void Matching_filter_expands_and_highlights_the_hit_tag()
    {
        var cut = RenderEntry(Role);

        Select(cut, "Azure");

        Assert.Empty(cut.FindAll("li.entry--dimmed"));
        Assert.Equal("true", cut.Find("button.entry__toggle").GetAttribute("aria-expanded"));
        Assert.Equal("Azure", cut.Find("li.tag--hit").TextContent.Trim());
    }

    [Fact]
    public void Non_matching_filter_dims_and_collapses()
    {
        var cut = RenderEntry(Role, expanded: true);

        Select(cut, "React");

        Assert.NotNull(cut.Find("li.entry--dimmed"));
        Assert.Equal("false", cut.Find("button.entry__toggle").GetAttribute("aria-expanded"));
    }

    [Fact]
    public void Clearing_the_filter_restores_the_initial_expansion()
    {
        var cut = RenderEntry(Role, expanded: false);
        Select(cut, "Azure");
        Assert.Equal("true", cut.Find("button.entry__toggle").GetAttribute("aria-expanded"));

        Select(cut);

        Assert.Equal("false", cut.Find("button.entry__toggle").GetAttribute("aria-expanded"));
        Assert.Empty(cut.FindAll("li.entry--dimmed"));
    }

    [Fact]
    public void Re_render_with_unchanged_filter_keeps_manual_expansion()
    {
        var cut = RenderEntry(Role);
        cut.Find("button.entry__toggle").Click();

        Select(cut);

        Assert.Equal("true", cut.Find("button.entry__toggle").GetAttribute("aria-expanded"));
    }

    [Fact]
    public void Career_break_renders_as_marker_without_toggle()
    {
        var cut = RenderEntry(TestData.CareerBreak(new DateOnly(2011, 4, 1), new DateOnly(2011, 10, 1)));

        Assert.NotNull(cut.Find("li.entry--break"));
        Assert.Empty(cut.FindAll("button"));
        Assert.Contains("Full-time caregiver", cut.Markup, StringComparison.Ordinal);
    }

    private IRenderedComponent<TimelineEntry> RenderEntry(Experience experience, bool expanded = false) =>
        Render<TimelineEntry>(p => p
            .Add(c => c.Experience, experience)
            .Add(c => c.SelectedSkills, Nothing)
            .Add(c => c.InitiallyExpanded, expanded));

    private static void Select(IRenderedComponent<TimelineEntry> cut, params string[] skills) =>
        cut.Render(p => p.Add(c => c.SelectedSkills, new HashSet<string>(skills, StringComparer.OrdinalIgnoreCase)));
}

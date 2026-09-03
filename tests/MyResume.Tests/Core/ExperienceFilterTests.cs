using MyResume.Core.Filtering;

namespace MyResume.Tests.Core;

public sealed class ExperienceFilterTests
{
    private static readonly DateOnly Start = new(2020, 1, 1);

    private static HashSet<string> Selected(params string[] skills) => new(skills, StringComparer.OrdinalIgnoreCase);

    [Fact]
    public void Empty_selection_matches_everything() =>
        Assert.True(ExperienceFilter.Matches(TestData.Role("A", "Dev", Start, null, "C#"), Selected()));

    [Fact]
    public void Matches_when_any_selected_skill_is_used() =>
        Assert.True(ExperienceFilter.Matches(TestData.Role("A", "Dev", Start, null, "C#", "Azure"), Selected("React", "Azure")));

    [Fact]
    public void Does_not_match_when_no_selected_skill_is_used() =>
        Assert.False(ExperienceFilter.Matches(TestData.Role("A", "Dev", Start, null, "C#"), Selected("React")));

    [Fact]
    public void Matching_ignores_case() =>
        Assert.True(ExperienceFilter.Matches(TestData.Role("A", "Dev", Start, null, "azure"), Selected("Azure")));

    [Fact]
    public void Career_break_never_matches_an_active_filter() =>
        Assert.False(ExperienceFilter.Matches(TestData.CareerBreak(Start, Start.AddMonths(6)), Selected("C#")));
}

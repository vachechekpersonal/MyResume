using MyResume.Core.Insights;
using MyResume.Core.Models;

namespace MyResume.Tests.Core;

public sealed class SkillExperienceTests
{
    private static readonly DateOnly Today = new(2026, 9, 3);

    [Fact]
    public void Sums_months_across_separate_roles_and_counts_them()
    {
        var roles = new[]
        {
            TestData.Role("A", "Dev", new DateOnly(2020, 1, 1), new DateOnly(2020, 12, 1), "C#"),   // 12 months
            TestData.Role("B", "Dev", new DateOnly(2022, 1, 1), new DateOnly(2022, 6, 1), "C#"),    // 6 months
        };

        var result = SkillExperienceCalculator.Compute(roles, Today);

        Assert.Equal(new SkillExperience("C#", 2, 18), result["C#"]);
    }

    [Fact]
    public void Overlapping_roles_count_shared_months_once()
    {
        var roles = new[]
        {
            TestData.Role("A", "Dev", new DateOnly(2020, 1, 1), new DateOnly(2020, 12, 1), "SQL"),
            TestData.Role("B", "Dev", new DateOnly(2020, 6, 1), new DateOnly(2021, 5, 1), "SQL"),
        };

        var result = SkillExperienceCalculator.Compute(roles, Today);

        Assert.Equal(17, result["SQL"].Months); // Jan 2020 – May 2021 inclusive
        Assert.Equal(2, result["SQL"].RoleCount);
    }

    [Fact]
    public void Adjacent_roles_merge_without_a_gap()
    {
        var roles = new[]
        {
            TestData.Role("A", "Dev", new DateOnly(2020, 1, 1), new DateOnly(2020, 6, 1), "Go"),
            TestData.Role("B", "Dev", new DateOnly(2020, 7, 1), new DateOnly(2020, 12, 1), "Go"),
        };

        Assert.Equal(12, SkillExperienceCalculator.Compute(roles, Today)["Go"].Months);
    }

    [Fact]
    public void Current_role_runs_to_today()
    {
        var roles = new[] { TestData.Role("A", "Dev", new DateOnly(2026, 7, 1), null, "Azure") };

        Assert.Equal(3, SkillExperienceCalculator.Compute(roles, Today)["Azure"].Months);
    }

    [Fact]
    public void Skill_keys_ignore_case_and_career_breaks_contribute_nothing()
    {
        var experiences = new[]
        {
            TestData.Role("A", "Dev", new DateOnly(2020, 1, 1), new DateOnly(2020, 3, 1), "azure"),
            TestData.CareerBreak(new DateOnly(2021, 1, 1), new DateOnly(2021, 6, 1)),
        };

        var result = SkillExperienceCalculator.Compute(experiences, Today);

        Assert.Single(result);
        Assert.Equal(3, result["Azure"].Months);
    }

    [Fact]
    public void Skill_never_used_is_absent() =>
        Assert.False(SkillExperienceCalculator.Compute([], Today).ContainsKey("C#"));

    [Theory]
    [InlineData(1, "1 mo")]
    [InlineData(11, "11 mos")]
    [InlineData(12, "1 yr")]
    [InlineData(17, "1 yr")]
    [InlineData(18, "2 yrs")]
    [InlineData(66, "6 yrs")]
    [InlineData(200, "17 yrs")]
    public void Summary_is_months_under_a_year_then_years_rounded_to_nearest(int months, string expected) =>
        Assert.Equal(expected, new SkillExperience("X", 1, months).Summary);

    [Fact]
    public void Zero_months_is_rejected_by_the_formatter() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => DurationFormat.Approximate(0));
}

using MyResume.Core.Filtering;

namespace MyResume.Tests.Core;

public sealed class SkillQueryTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(", ,")]
    public void Parse_of_nothing_is_empty(string? value) => Assert.Empty(SkillQuery.Parse(value));

    [Fact]
    public void Parse_splits_trims_and_deduplicates_case_insensitively() =>
        Assert.Equal(["C#", "Azure"], SkillQuery.Parse(" C#, Azure ,, azure "));

    [Fact]
    public void Format_sorts_case_insensitively() =>
        Assert.Equal("Azure,C#,react", SkillQuery.Format(["react", "C#", "Azure"]));

    [Fact]
    public void Format_of_empty_selection_is_null_so_the_parameter_is_removed() =>
        Assert.Null(SkillQuery.Format([]));

    [Fact]
    public void Format_then_parse_round_trips() =>
        Assert.Equal(["Azure", "C#"], SkillQuery.Parse(SkillQuery.Format(["C#", "Azure"])));
}

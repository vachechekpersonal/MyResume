using System.Text.Json;
using MyResume.Core.Data;
using MyResume.Core.Models;

namespace MyResume.Tests.Core;

public sealed class CvJsonTests
{
    private static string RealCvPath => Path.Combine(AppContext.BaseDirectory, "data", "cv.json");

    [Fact]
    public void Cv_round_trips_through_source_generated_json()
    {
        var original = TestData.Cv();

        var json = JsonSerializer.Serialize(original, CvJsonContext.Default.Cv);
        var restored = JsonSerializer.Deserialize(json, CvJsonContext.Default.Cv);

        Assert.NotNull(restored);
        // Record equality is shallow for collection members, so compare scalar fields and elements.
        Assert.Equal(original.Profile.Name, restored.Profile.Name);
        Assert.Equal(original.Profile.Links[0], restored.Profile.Links[0]);
        Assert.Equal(original.Experiences.Count, restored.Experiences.Count);
        Assert.Equal(original.Experiences[0].Period, restored.Experiences[0].Period);
        Assert.Equal(original.Experiences[0].Skills, restored.Experiences[0].Skills);
    }

    [Fact]
    public void Json_uses_camel_case_and_string_enums()
    {
        var json = JsonSerializer.Serialize(TestData.CareerBreak(new DateOnly(2011, 4, 1), new DateOnly(2011, 10, 1)), CvJsonContext.Default.Experience);

        Assert.Contains("\"kind\": \"CareerBreak\"", json, StringComparison.Ordinal);
        Assert.Contains("\"start\": \"2011-04-01\"", json, StringComparison.Ordinal);
    }

    [Fact]
    public void AllSkills_flattens_groups()
    {
        Assert.Equal(["C#", "React", "Azure"], TestData.Cv().AllSkills);
    }

    [Fact]
    public void Real_cv_json_deserialises_with_expected_shape()
    {
        var cv = LoadRealCv();

        Assert.Equal("Vache Chek", cv.Profile.Name);
        Assert.Single(cv.Profile.Links);
        Assert.Equal("https://uk.linkedin.com/in/vache-chek", cv.Profile.Links[0].Url);
        Assert.Equal(9, cv.Experiences.Count);
        Assert.Equal(5, cv.Qualifications.Count);
        Assert.Equal(["Armenian", "Farsi", "English"], cv.Languages);
    }

    [Fact]
    public void Every_experience_skill_tag_exists_in_skill_groups()
    {
        var cv = LoadRealCv();
        var known = cv.AllSkills.ToHashSet(StringComparer.Ordinal);

        var unknown = cv.Experiences.SelectMany(e => e.Skills).Where(s => !known.Contains(s)).Distinct().ToList();

        Assert.Empty(unknown);
    }

    [Fact]
    public void Employment_entries_have_highlights_and_skills()
    {
        var cv = LoadRealCv();

        foreach (var role in cv.Experiences.Where(e => e.Kind == ExperienceKind.Employment))
        {
            Assert.NotEmpty(role.Highlights);
            Assert.NotEmpty(role.Skills);
        }
    }

    [Fact]
    public void Experiences_are_listed_newest_first_and_do_not_overlap()
    {
        var cv = LoadRealCv();

        for (var i = 1; i < cv.Experiences.Count; i++)
        {
            var newer = cv.Experiences[i - 1];
            var older = cv.Experiences[i];
            Assert.True(newer.Period.Start > older.Period.Start, $"{newer.Company} should be after {older.Company}");
            Assert.True(older.Period.End is { } end && end <= newer.Period.Start, $"{older.Company} overlaps the next role");
        }
    }

    [Fact]
    public void Only_the_first_experience_is_current()
    {
        var cv = LoadRealCv();

        Assert.True(cv.Experiences[0].Period.IsCurrent);
        Assert.All(cv.Experiences.Skip(1), e => Assert.False(e.Period.IsCurrent));
    }

    [Fact]
    public void No_phone_number_or_email_is_published()
    {
        var json = File.ReadAllText(RealCvPath);

        Assert.DoesNotMatch(@"\b0\d{10}\b", json);
        Assert.DoesNotContain("@", json, StringComparison.Ordinal);
    }

    private static Cv LoadRealCv()
    {
        var cv = JsonSerializer.Deserialize(File.ReadAllText(RealCvPath), CvJsonContext.Default.Cv);
        return cv ?? throw new InvalidOperationException("cv.json deserialised to null.");
    }
}

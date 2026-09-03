using System.Text.Json;
using MyResume.Core.Data;
using MyResume.Core.Models;

namespace MyResume.Tests.Core;

public sealed class CvJsonTests
{
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
}

using MyResume.Core.Models;

namespace MyResume.Tests;

internal static class TestData
{
    public static Experience Role(
        string company,
        string role,
        DateOnly start,
        DateOnly? end,
        params string[] skills) =>
        new(company, role, "Reading, UK", new DateRange(start, end), ["Delivered things."], skills);

    public static Experience CareerBreak(DateOnly start, DateOnly end) =>
        new("Career Break", "Full-time caregiver", "", new DateRange(start, end), [], [], ExperienceKind.CareerBreak);

    public static Cv Cv() => new(
        new Profile("Test Person", "Engineer", "Reading, UK", "A summary.", [new ContactLink("LinkedIn", "https://example.com/in/test")]),
        [new SkillGroup("Languages", ["C#", "React"]), new SkillGroup("Cloud", ["Azure"])],
        [
            Role("Acme", "Senior Engineer", new DateOnly(2021, 4, 1), null, "C#", "Azure"),
            Role("Beta", "Engineer", new DateOnly(2019, 10, 1), new DateOnly(2021, 3, 1), "React"),
        ],
        [new Qualification("Azure Developer Associate", "Microsoft", "March 2022", null)],
        ["English"]);
}

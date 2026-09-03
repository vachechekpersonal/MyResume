namespace MyResume.Core.Models;

public sealed record Profile(
    string Name,
    string Title,
    string Location,
    string Summary,
    IReadOnlyList<ContactLink> Links);

public sealed record ContactLink(string Label, string Url);

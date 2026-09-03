namespace MyResume.Core.Models;

public sealed record Qualification(
    string Title,
    string? Institution,
    string Period,
    string? Note);

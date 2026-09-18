namespace MyResume.Web.Components;

/// <summary>
/// A one-shot "expand all" / "collapse all" instruction passed down from <c>TimelineSection</c>.
/// <see cref="Version"/> increments on every click so an entry can tell a new command from a re-render
/// that merely repeats the previous one, and manual toggles in between are preserved.
/// </summary>
public readonly record struct ExpansionCommand(int Version, bool Expanded)
{
    public ExpansionCommand Next(bool expanded) => new(Version + 1, expanded);
}

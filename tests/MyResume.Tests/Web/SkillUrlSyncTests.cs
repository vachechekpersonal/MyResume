using Bunit.TestDoubles;
using Microsoft.Extensions.DependencyInjection;
using MyResume.Core.Filtering;
using MyResume.Web.Components;

namespace MyResume.Tests.Web;

public sealed class SkillUrlSyncTests : BunitContext
{
    private readonly SkillSelection _selection = new();

    public SkillUrlSyncTests() => Services.AddSingleton(_selection);

    private BunitNavigationManager Navigation => Services.GetRequiredService<BunitNavigationManager>();

    [Fact]
    public void Applies_known_skills_from_the_url_and_ignores_unknown_ones()
    {
        Navigation.NavigateTo("/?skills=azure,Cobol,C%23");

        RenderSync();

        Assert.Equal(["Azure", "C#"], _selection.Selected.Order());
    }

    [Fact]
    public void Leaves_the_selection_alone_when_the_url_has_no_skills()
    {
        _selection.Toggle("React");

        RenderSync();

        Assert.Equal(["React"], _selection.Selected);
    }

    [Fact]
    public void Writes_the_selection_back_to_the_url_without_adding_history()
    {
        RenderSync();

        _selection.Toggle("React");
        _selection.Toggle("Azure");

        Assert.EndsWith("/?skills=Azure,React", Uri.UnescapeDataString(Navigation.Uri), StringComparison.Ordinal);
        Assert.All(Navigation.History, entry => Assert.True(entry.Options.ReplaceHistoryEntry));
    }

    [Fact]
    public void Clearing_the_selection_removes_the_parameter()
    {
        Navigation.NavigateTo("/?skills=Azure");
        RenderSync();

        _selection.Clear();

        Assert.DoesNotContain("skills", Navigation.Uri, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Stops_listening_when_disposed()
    {
        var cut = RenderSync();
        Assert.Empty(cut.Markup.Trim());
        var before = Navigation.Uri;

        await DisposeComponentsAsync();
        _selection.Toggle("Azure");

        Assert.Equal(before, Navigation.Uri);
    }

    private IRenderedComponent<SkillUrlSync> RenderSync() =>
        Render<SkillUrlSync>(p => p.Add(c => c.Groups, TestData.Cv().SkillGroups));
}

using Microsoft.Extensions.DependencyInjection;
using MyResume.Core.Filtering;
using MyResume.Web.Components;

namespace MyResume.Tests.Web;

public sealed class SkillChipTests : BunitContext
{
    private readonly SkillSelection _selection = new();

    public SkillChipTests() => Services.AddSingleton(_selection);

    [Fact]
    public void Renders_unpressed_by_default()
    {
        var cut = Render<SkillChip>(p => p.Add(c => c.Skill, "Azure"));

        var button = cut.Find("button");
        Assert.Equal("Azure", button.TextContent.Trim());
        Assert.Equal("false", button.GetAttribute("aria-pressed"));
    }

    [Fact]
    public void Click_toggles_selection_and_pressed_state()
    {
        var cut = Render<SkillChip>(p => p.Add(c => c.Skill, "Azure"));

        cut.Find("button").Click();

        Assert.True(_selection.IsSelected("Azure"));
        Assert.Equal("true", cut.Find("button").GetAttribute("aria-pressed"));
    }

    [Fact]
    public void Re_renders_when_selection_changes_elsewhere()
    {
        var cut = Render<SkillChip>(p => p.Add(c => c.Skill, "Azure"));

        _selection.Toggle("Azure");

        cut.WaitForAssertion(() => Assert.Equal("true", cut.Find("button").GetAttribute("aria-pressed")));
    }
}

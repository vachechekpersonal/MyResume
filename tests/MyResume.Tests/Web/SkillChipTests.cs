using MyResume.Web.Components;

namespace MyResume.Tests.Web;

public sealed class SkillChipTests : BunitContext
{
    [Fact]
    public void Renders_skill_name_and_pressed_state()
    {
        var cut = Render<SkillChip>(p => p.Add(c => c.Skill, "Azure").Add(c => c.Selected, false));

        var button = cut.Find("button");
        Assert.Equal("Azure", button.TextContent.Trim());
        Assert.Equal("false", button.GetAttribute("aria-pressed"));
        Assert.DoesNotContain("chip--selected", button.ClassName, StringComparison.Ordinal);
    }

    [Fact]
    public void Selected_chip_is_pressed_and_styled()
    {
        var cut = Render<SkillChip>(p => p.Add(c => c.Skill, "Azure").Add(c => c.Selected, true));

        var button = cut.Find("button");
        Assert.Equal("true", button.GetAttribute("aria-pressed"));
        Assert.Contains("chip--selected", button.ClassName, StringComparison.Ordinal);
    }

    [Fact]
    public void Click_raises_OnToggle()
    {
        var toggled = 0;
        var cut = Render<SkillChip>(p => p.Add(c => c.Skill, "Azure").Add(c => c.OnToggle, () => toggled++));

        cut.Find("button").Click();

        Assert.Equal(1, toggled);
    }
}

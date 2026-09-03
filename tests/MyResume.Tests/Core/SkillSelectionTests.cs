using MyResume.Core.Filtering;

namespace MyResume.Tests.Core;

public sealed class SkillSelectionTests
{
    [Fact]
    public void Starts_empty_and_inactive()
    {
        var selection = new SkillSelection();

        Assert.Empty(selection.Selected);
        Assert.False(selection.IsActive);
    }

    [Fact]
    public void Toggle_adds_then_removes()
    {
        var selection = new SkillSelection();

        selection.Toggle("C#");
        Assert.True(selection.IsSelected("c#"));
        Assert.True(selection.IsActive);

        selection.Toggle("C#");
        Assert.False(selection.IsSelected("C#"));
    }

    [Fact]
    public void Toggle_rejects_blank_skill()
    {
        var selection = new SkillSelection();

        Assert.Throws<ArgumentException>(() => selection.Toggle(" "));
    }

    [Fact]
    public void Clear_removes_everything_and_raises_Changed_once()
    {
        var selection = new SkillSelection();
        selection.Toggle("C#");
        selection.Toggle("Azure");
        var raised = 0;
        selection.Changed += () => raised++;

        selection.Clear();

        Assert.Empty(selection.Selected);
        Assert.Equal(1, raised);
    }

    [Fact]
    public void Clear_on_empty_selection_does_not_raise_Changed()
    {
        var selection = new SkillSelection();
        var raised = 0;
        selection.Changed += () => raised++;

        selection.Clear();

        Assert.Equal(0, raised);
    }

    [Fact]
    public void Toggle_raises_Changed()
    {
        var selection = new SkillSelection();
        var raised = 0;
        selection.Changed += () => raised++;

        selection.Toggle("React");

        Assert.Equal(1, raised);
    }
}

namespace MyResume.Core.Filtering;

/// <summary>
/// The set of skills the visitor has selected. Registered as a scoped service; UI components
/// subscribe to <see cref="Changed"/> and re-render. Comparison is case-insensitive.
/// </summary>
public sealed class SkillSelection
{
    private readonly HashSet<string> _selected = new(StringComparer.OrdinalIgnoreCase);

    public event Action? Changed;

    public IReadOnlySet<string> Selected => _selected;

    public bool IsActive => _selected.Count > 0;

    public bool IsSelected(string skill) => _selected.Contains(skill);

    public void Toggle(string skill)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(skill);

        if (!_selected.Remove(skill))
        {
            _selected.Add(skill);
        }

        Changed?.Invoke();
    }

    public void Clear()
    {
        if (_selected.Count == 0)
        {
            return;
        }

        _selected.Clear();
        Changed?.Invoke();
    }
}

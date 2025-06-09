using System.Collections.Generic;

namespace Avalonia.PropertyGrid.Controls.Implements;

internal class PropertyGridExpandableCache : IExpandableObjectCache
{
    private readonly List<object?> _targets = [];

    public void Add(object? target)
    {
        if (!_targets.Contains(target))
        {
            _targets.Add(target);
        }
    }

    public void Clear() => _targets.Clear();

    public bool IsExists(object? target) => _targets.Contains(target);

    public void Merge(IExpandableObjectCache cache)
    {
        if (cache is PropertyGridExpandableCache c)
        {
            foreach (var target in c._targets)
            {
                Add(target);
            }
        }
    }

    public void Remove(object? target) => _ = _targets.Remove(target);
}
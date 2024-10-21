using System;
using System.Collections.Generic;
using System.Text;

namespace Avalonia.PropertyGrid.Controls.Implements
{
    internal class PropertyGridExpandableCache : IExpandableObjectCache
    {
        private readonly List<object?> Targets = [];

        public void Add(object? target)
        {
            if(!Targets.Contains(target))
            {
                Targets.Add(target);
            }            
        }

        public void Clear()
        {
            Targets.Clear();
        }

        public bool IsExists(object? target)
        {
            return Targets.Contains(target);
        }

        public void Merge(IExpandableObjectCache cache)
        {
            if(cache is PropertyGridExpandableCache c)
            {
                foreach(var target in c.Targets)
                {
                    Add(target);
                }
            }
        }

        public void Remove(object? target)
        {
            Targets.Remove(target);
        }

    }
}

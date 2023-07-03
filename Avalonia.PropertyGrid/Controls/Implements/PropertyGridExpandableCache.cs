using System;
using System.Collections.Generic;
using System.Text;

namespace Avalonia.PropertyGrid.Controls.Implements
{
    internal class PropertyGridExpandableCache : IExpandableObjectCache
    {
        readonly List<object> Targets = new List<object>();

        public void Add(object target)
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

        public bool IsExists(object target)
        {
            return Targets.Contains(target);
        }

        public void Remove(object target)
        {
            Targets.Remove(target);
        }
    }
}

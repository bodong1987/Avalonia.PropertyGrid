using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.PropertyGrid.Controls.ViewInfos
{
    public abstract class AbstractPropertyGridViewInfo : IPropertyGridViewInfo
    {
        public PropertyGrid PropertyGrid { get; private set; }

        public PropertyGridShowStyle Style { get; private set; }

        public abstract void ApplySearch(string searchPattern);

        public AbstractPropertyGridViewInfo(PropertyGrid propertyGrid, PropertyGridShowStyle style)
        {
            PropertyGrid = propertyGrid;
            Style = style;
        }
    }
}

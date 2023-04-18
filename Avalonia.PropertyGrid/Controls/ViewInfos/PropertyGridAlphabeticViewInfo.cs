using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.PropertyGrid.Controls.ViewInfos
{
    internal class PropertyGridAlphabeticViewInfo : AbstractPropertyGridViewInfo
    {
        public PropertyGridAlphabeticViewInfo(PropertyGrid propertyGrid, object context) :
            base(propertyGrid, PropertyGridShowStyle.Alphabetic)
        {
        }

        public override void ApplySearch(string searchPattern)
        {
            
        }
    }
}

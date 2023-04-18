using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.PropertyGrid.Controls.ViewInfos
{
    internal class PropertyGridCategoryViewInfo : AbstractPropertyGridViewInfo
    {
        public PropertyGridCategoryViewInfo(PropertyGrid propertyGrid, object context) :
            base(propertyGrid, PropertyGridShowStyle.Category)
        {
        }

        public override void ApplySearch(string searchPattern)
        {
            
        }
    }
}

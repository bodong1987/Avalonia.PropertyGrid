using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.PropertyGrid.Controls
{
    public interface IPropertyGridViewInfo
    {
        PropertyGrid PropertyGrid { get; }

        PropertyGridShowStyle Style { get; }

        void ApplySearch(string searchPattern);
    }
}

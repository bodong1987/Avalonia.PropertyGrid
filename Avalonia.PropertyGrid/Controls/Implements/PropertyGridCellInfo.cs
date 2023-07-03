using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Avalonia.PropertyGrid.Controls.Implements
{
    internal class PropertyGridCellInfo : IPropertyGridCellInfo
    {
        public string ReferencePath { get; set; }

        public Control NameControl { get; set; }

        public Control CellEdit { get; set; }

        public PropertyDescriptor Property { get; set; }

        public string Category { get; set; }

        public object OwnerObject { get; set; }

        public Expander Container { get; set; }


        readonly List<IPropertyGridCellInfo> _Children = new List<IPropertyGridCellInfo>();

        public IPropertyGridCellInfo[] Children => _Children.ToArray();

        public void Add(IPropertyGridCellInfo cellInfo)
        {
            _Children.Add(cellInfo);
        }

        public void Clear()
        {
            _Children.Clear();
        }

        public void Remove(IPropertyGridCellInfo cellInfo)
        {
            _Children.Remove(cellInfo);
        }
    }

    internal class PropertyGridCellInfoCache : IPropertyGridCellInfoCache
    {
        readonly List<IPropertyGridCellInfo> _Cells = new List<IPropertyGridCellInfo>();

        public IPropertyGridCellInfo[] Infos => _Cells.ToArray();

        public void Add(IPropertyGridCellInfo info)
        {
            _Cells.Add(info);
        }

        public void Clear()
        {
            _Cells.Clear();
        }

        public void Remove(IPropertyGridCellInfo info)
        {
            _Cells.Remove(info);
        }
    }
}

using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Avalonia.PropertyGrid.Controls.Implements
{
    internal class PropertyGridCellInfoContainer : IPropertyGridCellInfoContainer
    {
        readonly List<IPropertyGridCellInfo> _Children = new List<IPropertyGridCellInfo>();

        public IPropertyGridCellInfo[] Children => _Children.ToArray();

        public void Add(IPropertyGridCellInfo cellInfo)
        {
            if(!_Children.Contains(cellInfo))
            {
                _Children.Add(cellInfo);
            }            
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

    internal class PropertyGridCellInfo : PropertyGridCellInfoContainer, IPropertyGridCellInfo
    {
        public string ReferencePath { get; set; }

        public PropertyGridCellType CellType { get; set; }

        public Control NameControl { get; set; }

        public Control CellEdit { get; set; }

        public PropertyDescriptor Property { get; set; }

        public string Category { get; set; }

        public object OwnerObject { get; set; }

        public object Target { get; set; }

        public Expander Container { get; set; }

        public ICellEditFactory Factory { get; set; }

        public override string ToString()
        {
            return ReferencePath;
        }

        bool _IsVisible = true;
        public bool IsVisible
        {
            get
            {
                return _IsVisible;
            }
            set
            {
                if (_IsVisible != value)
                {
                    if(NameControl != null)
                    {
                        NameControl.IsVisible = value;
                    }

                    if(CellEdit != null)
                    {
                        CellEdit.IsVisible = value;
                    }

                    if(Container != null && CellEdit == null && NameControl == null)
                    {
                        Container.IsVisible = value;
                    }

                    _IsVisible = value;

                }
            }
        }
    }

    internal class PropertyGridCellInfoCache : PropertyGridCellInfoContainer, IPropertyGridCellInfoCache
    {
    }
}

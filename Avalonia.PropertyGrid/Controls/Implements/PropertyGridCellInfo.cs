using Avalonia.Controls;
using Newtonsoft.Json.Linq;
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

        public virtual void Add(IPropertyGridCellInfo cellInfo)
        {
            if(!_Children.Contains(cellInfo))
            {
                _Children.Add(cellInfo);

                cellInfo.AddPropertyChangedObserver();
            }            
        }

        public virtual void Clear()
        {
            foreach(var child in _Children)
            {
                child.Clear();
            }

            _Children.Clear();
        }

        public virtual void Remove(IPropertyGridCellInfo cellInfo)
        {
            cellInfo.RemovePropertyChangedObserver();

            _Children.Remove(cellInfo);
        }

        public int Count => _Children.Count;
    }

    internal class PropertyGridCellInfo : PropertyGridCellInfoContainer, IPropertyGridCellInfo
    {
        public PropertyCellContext Context { get; set; }

        public string ReferencePath { get; set; }

        public PropertyGridCellType CellType { get; set; }

        public Control NameControl { get; set; }

        public Control CellEdit => Context?.CellEdit;

        public PropertyDescriptor Property => Context?.Property;

        public string Category { get; set; }

        public object OwnerObject { get; set; }

        public object Target { get; set; }

        public Expander Container { get; set; }

        public ICellEditFactory Factory => Context?.Factory;

        /// <summary>
        /// Occurs when [cell property changed].
        /// </summary>
        public event EventHandler<CellPropertyChangedEventArgs> CellPropertyChanged;

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

        public PropertyGridCellInfo(PropertyCellContext context)
        {
            Context = context;
        }


        public override void Clear()
        {
            RemovePropertyChangedObserver();

            base.Clear();
        }

        public void AddPropertyChangedObserver()
        {
            if (Target is System.ComponentModel.INotifyPropertyChanged npc2)
            {
                npc2.PropertyChanged += OnPropertyChanged;
            }
            else if (Target is IEnumerable<System.ComponentModel.INotifyPropertyChanged> npcs2)
            {
                foreach (var n in npcs2)
                {
                    n.PropertyChanged += OnPropertyChanged;
                }
            }
        }

        public void RemovePropertyChangedObserver()
        {
            if (Target is System.ComponentModel.INotifyPropertyChanged npc)
            {
                npc.PropertyChanged -= OnPropertyChanged;
            }
            else if (Target is IEnumerable<System.ComponentModel.INotifyPropertyChanged> npcs)
            {
                foreach (var n in npcs)
                {
                    n.PropertyChanged -= OnPropertyChanged;
                }
            }
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (Property != null && e.PropertyName == Property.Name)
            {
                Factory?.HandlePropertyChanged(Context);

                CellPropertyChanged?.Invoke(this, new CellPropertyChangedEventArgs(this));
            }
        }
    }

    internal class PropertyGridCellInfoCache : PropertyGridCellInfoContainer, IPropertyGridCellInfoCache
    {        
    }
}

using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace Avalonia.PropertyGrid.Controls.Implements
{
    internal class PropertyGridCellInfoContainer : IPropertyGridCellInfoContainer
    {
        private readonly List<IPropertyGridCellInfo> _children = new();

        public IPropertyGridCellInfo[] Children => _children.ToArray();

        public virtual void Add(IPropertyGridCellInfo cellInfo)
        {
            if(!_children.Contains(cellInfo))
            {
                _children.Add(cellInfo);

                cellInfo.AddPropertyChangedObserver();
            }            
        }

        public virtual void Clear()
        {
            foreach(var child in _children)
            {
                child.Clear();
            }

            _children.Clear();
        }

        public virtual void Remove(IPropertyGridCellInfo cellInfo)
        {
            cellInfo.RemovePropertyChangedObserver();

            _children.Remove(cellInfo);
        }

        public int Count => _children.Count;
    }

    internal class PropertyGridCellInfo : PropertyGridCellInfoContainer, IPropertyGridCellInfo
    {
        public PropertyCellContext? Context { get; set; }

        public string? ReferencePath { get; set; }

        public PropertyGridCellType CellType { get; set; }

        public Control? NameControl { get; set; }

        public Control? CellEdit => Context?.CellEdit;

        public PropertyDescriptor? Property => Context?.Property;

        public string? Category { get; set; }

        public object? OwnerObject { get; set; }

        public object? Target { get; set; }

        public Expander? Container { get; set; }

        public ICellEditFactory? Factory => Context?.Factory!;

        /// <summary>
        /// Occurs when [cell property changed].
        /// </summary>
        public event EventHandler<CellPropertyChangedEventArgs>? CellPropertyChanged;

        public override string ToString()
        {
            return ReferencePath ?? string.Empty;
        }

        private bool _isVisible = true;
        public bool IsVisible
        {
            get
            {
                return _isVisible;
            }
            set
            {
                if (_isVisible != value)
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

                    _isVisible = value;

                }
            }
        }

        public PropertyGridCellInfo(PropertyCellContext? context)
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
            if (Target is INotifyPropertyChanged npc2)
            {
                npc2.PropertyChanged += OnPropertyChanged;
            }
            else if (Target is IEnumerable<INotifyPropertyChanged> npcs2)
            {
                foreach (var n in npcs2)
                {
                    n.PropertyChanged += OnPropertyChanged;
                }
            }
        }

        public void RemovePropertyChangedObserver()
        {
            if (Target is INotifyPropertyChanged npc)
            {
                npc.PropertyChanged -= OnPropertyChanged;
            }
            else if (Target is IEnumerable<INotifyPropertyChanged> npcs)
            {
                foreach (var n in npcs)
                {
                    n.PropertyChanged -= OnPropertyChanged;
                }
            }
        }

        private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (Property != null && e.PropertyName == Property.Name)
            {
                Debug.Assert(Context != null);

                Factory?.HandlePropertyChanged(Context);

                CellPropertyChanged?.Invoke(this, new CellPropertyChangedEventArgs(this));
            }
        }
    }

    internal class PropertyGridCellInfoCache : PropertyGridCellInfoContainer, IPropertyGridCellInfoCache
    {        
    }
}

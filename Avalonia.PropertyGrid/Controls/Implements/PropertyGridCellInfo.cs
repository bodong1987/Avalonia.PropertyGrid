using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;

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

        private bool _isVisible = true;
        public bool IsVisible
        {
            get => _isVisible;
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

        // ReSharper disable once ConvertToPrimaryConstructor
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
            switch (Target)
            {
                case INotifyPropertyChanged npc2:
                    npc2.PropertyChanged += OnPropertyChanged;
                    break;
                case IEnumerable<INotifyPropertyChanged> propertyChangedTargetList:
                {
                    foreach (var n in propertyChangedTargetList)
                    {
                        n.PropertyChanged += OnPropertyChanged;
                    }

                    break;
                }
            }
        }

        public void RemovePropertyChangedObserver()
        {
            switch (Target)
            {
                case INotifyPropertyChanged npc:
                    npc.PropertyChanged -= OnPropertyChanged;
                    break;
                case IEnumerable<INotifyPropertyChanged> propertyChangedTargetList:
                {
                    foreach (var n in propertyChangedTargetList)
                    {
                        n.PropertyChanged -= OnPropertyChanged;
                    }

                    break;
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

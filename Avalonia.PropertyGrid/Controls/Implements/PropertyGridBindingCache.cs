using Avalonia.Controls;
using Avalonia.PropertyGrid.Model.ComponentModel;
using Avalonia.PropertyGrid.Model.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace Avalonia.PropertyGrid.Controls.Implements
{
    internal class PropertyBinding
    {
        public string BindingKey;
        public PropertyDescriptor Property;
        public Control BindingControl;
        public TextBlock BindingNameControl;
        public ICellEditFactory Factory;
        public Expander BindingExpander;
        public object Target;
        public int Depth;

        public string PropertyName => Property?.Name;

        public override string ToString()
        {
            return BindingKey;
        }
    }

    internal class PropertyGridBindingCache : MiniReactiveObject, IEnumerable<PropertyBinding>
    {
        public readonly Dictionary<object, List<PropertyBinding>> BindingObjects = new Dictionary<object, List<PropertyBinding>>();

        public event EventHandler<BindingPropertyChangedEventArgs> PropertyChangedEvent;

        public PropertyGridBindingCache()
        {
        }

        public bool IsBinding(object target)
        {
            return target != null && BindingObjects.ContainsKey(target);
        }

        void AddPropertyChangedObserver(object target)
        {
            if (target is System.ComponentModel.INotifyPropertyChanged npc2)
            {
                npc2.PropertyChanged += OnSelectedObjectPropertyChanged;
            }
            else if (target is IEnumerable<System.ComponentModel.INotifyPropertyChanged> npcs2)
            {
                foreach (var n in npcs2)
                {
                    n.PropertyChanged += OnSelectedObjectPropertyChanged;
                }
            }
        }

        void RemovePropertyChangedObserver(object target)
        {
            if (target is System.ComponentModel.INotifyPropertyChanged npc)
            {
                npc.PropertyChanged -= OnSelectedObjectPropertyChanged;
            }
            else if (target is IEnumerable<System.ComponentModel.INotifyPropertyChanged> npcs)
            {
                foreach (var n in npcs)
                {
                    n.PropertyChanged -= OnSelectedObjectPropertyChanged;
                }
            }
        }        

        public void AddBinding(PropertyBinding binding)
        {
            Debug.Assert(binding != null);
            Debug.Assert(binding.BindingKey.IsNotNullOrEmpty());

            if(!BindingObjects.TryGetValue(binding.Target, out var list))
            {
                list = new List<PropertyBinding>() { binding };
                BindingObjects.Add(binding.Target, list);

                AddPropertyChangedObserver(binding.Target);
            }
            else
            {
                list.Add(binding);
            }            
        }
               

        public void ClearBindings()
        {
            foreach(var obj in BindingObjects.Keys)
            {
                RemovePropertyChangedObserver(obj);
            }

            BindingObjects.Clear();
        }

        public void Clear()
        {
            ClearBindings();         
        }

        private void OnSelectedObjectPropertyChanged(object sender, PropertyChangedEventArgs e)
        {            
            if(BindingObjects.TryGetValue(sender, out var list))
            {
                var binding = list.Find(x=>x.PropertyName == e.PropertyName);

                if(binding != null)
                {
                    if (PropertyChangedEvent != null)
                    {
                        BindingPropertyChangedEventArgs evt = new BindingPropertyChangedEventArgs(binding);

                        PropertyChangedEvent(this, evt);

                        if (evt.Handled)
                        {
                            return;
                        }
                    }

                    binding.Factory.HandlePropertyChanged(binding.Target, binding.Property, binding.BindingControl);
                }
            }
        }

        internal double CalcBindingNameMaxLength()
        {
            double maxLength = 0;
            foreach (var list in BindingObjects.Values)
            {
                foreach(var info in list)
                {
                    if (info.BindingNameControl != null)
                    {
                        if (info.BindingNameControl.Width >= maxLength)
                        {
                            maxLength = info.BindingNameControl.Width;
                        }
                        else if (info.BindingNameControl.DesiredSize.Width >= maxLength)
                        {
                            maxLength = info.BindingNameControl.DesiredSize.Width;
                        }
                    }
                }
            }

            return maxLength;
        }

        internal void SyncWidth(double width)
        {
            foreach (var list in BindingObjects.Values)
            {
                foreach (var info in list)
                {
                    if (info.BindingNameControl != null)
                    {
                        info.BindingNameControl.Width = Math.Max(width - info.Depth*6/2, 0);
                    }
                }
            }
        }

        public IEnumerator<PropertyBinding> GetEnumerator()
        {
            foreach(var list in BindingObjects.Values)
            {
                foreach(var info in list)
                {
                    yield return info;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    internal class BindingPropertyChangedEventArgs : HandledEventArgs
    {
        public readonly PropertyBinding Binding;

        public BindingPropertyChangedEventArgs(PropertyBinding binding)
        {
            Binding = binding;
        }
    }

}

using Avalonia.Controls;
using Avalonia.PropertyGrid.Model.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Avalonia.PropertyGrid.Controls.Implements
{
    internal struct PropertyBinding
    {
        public string BindingKey;
        public PropertyDescriptor Property;
        public Control BindingControl;
        public TextBlock BindingNameControl;
        public ICellEditFactory Factory;
        public Expander BindingExpander;
        public object Target;
    }

    internal class PropertyGridBindingCache : MiniReactiveObject
    {
        public readonly Dictionary<string, PropertyBinding> PropertyBindingDict = new Dictionary<string, PropertyBinding>();
        public readonly HashSet<object> ExtraBindingObjects = new HashSet<object>();

        public event EventHandler<BindingPropertyChangedEventArgs> PropertyChangedEvent;

        object _SelectObject;
        public object SelectedObject
        {
            get => _SelectObject;
            set
            {
                if(_SelectObject != value)
                {
                    RemovePropertyChangedObserver(_SelectObject);

                    _SelectObject = value;

                    AddPropertyChangedObserver(_SelectObject);

                    RaisePropertyChanged(nameof(SelectedObject));
                }
            }
        }

        public PropertyGridBindingCache()
        {
        }

        void AddPropertyChangedObserver(object target)
        {
            if (_SelectObject is System.ComponentModel.INotifyPropertyChanged npc2)
            {
                npc2.PropertyChanged += OnSelectedObjectPropertyChanged;
            }
            else if (_SelectObject is IEnumerable<System.ComponentModel.INotifyPropertyChanged> npcs2)
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
            if(!PropertyBindingDict.ContainsKey(binding.BindingKey))
            {
                PropertyBindingDict.Add(binding.BindingKey, binding);

                if(binding.Target != SelectedObject && !ExtraBindingObjects.Contains(binding.Target))
                {
                    ExtraBindingObjects.Add(binding.Target);

                    AddPropertyChangedObserver(binding.Target);
                }
            }            
        }

        public static string CalcPropertyKey(object target, string name)
        {
            return (target != null ? target.ToString() : "") + "|" + name;
        }

        public void RemoveBinding(string key)
        {
            PropertyBindingDict.Remove(key);
        }

        public void ClearBindings()
        {
            PropertyBindingDict.Clear();

            foreach(var obj in ExtraBindingObjects)
            {
                RemovePropertyChangedObserver(obj);
            }

            ExtraBindingObjects.Clear();
        }

        public void Clear()
        {
            ClearBindings();
            SelectedObject = null;
        }

        public PropertyBinding? this[string key]
        {
            get
            {
                if(PropertyBindingDict.TryGetValue(key, out var binding))
                {
                    return binding;
                }

                return null;
            }
        }

        private void OnSelectedObjectPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var key = CalcPropertyKey(sender, e.PropertyName);
            if(PropertyBindingDict.TryGetValue((string)key, out var binding))
            {
                if(PropertyChangedEvent!=null)
                {
                    BindingPropertyChangedEventArgs evt = new BindingPropertyChangedEventArgs(binding);

                    PropertyChangedEvent(this, evt);

                    if(evt.Handled)
                    {
                        return;
                    }
                }

                binding.Factory.HandlePropertyChanged(binding.Target, binding.Property, binding.BindingControl);
            }
        }

        internal double CalcBindingNameMaxLength()
        {
            double maxLength = 0;
            foreach (var info in PropertyBindingDict)
            {
                if (info.Value.BindingNameControl != null)
                {
                    if (info.Value.BindingNameControl.Width >= maxLength)
                    {
                        maxLength = info.Value.BindingNameControl.Width;
                    }
                    else if (info.Value.BindingNameControl.DesiredSize.Width >= maxLength)
                    {
                        maxLength = info.Value.BindingNameControl.DesiredSize.Width;
                    }
                }
            }

            return maxLength;
        }

        internal void SyncWidth(double width)
        {
            foreach (var info in PropertyBindingDict)
            {
                if (info.Value.BindingNameControl != null)
                {
                    info.Value.BindingNameControl.Width = width;
                }
            }
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

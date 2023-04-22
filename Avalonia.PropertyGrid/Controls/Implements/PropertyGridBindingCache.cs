using Avalonia.Controls;
using Avalonia.PropertyGrid.Model.ComponentModel;
using Avalonia.PropertyGrid.Model.Extensions;
using Avalonia.PropertyGrid.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace Avalonia.PropertyGrid.Controls.Implements
{
    internal abstract class PropertyBinding
    {
        public readonly string BindingKey;
        public readonly PropertyDescriptor Property;
        public readonly Expander BindingExpander;
        public readonly object Target;
        public readonly int Depth;
        private PropertyVisibility _Visibility = PropertyVisibility.AlwaysVisible;

        public string PropertyName => Property?.Name;

        public PropertyVisibility Visibility
        {
            get => _Visibility;
            set
            {
                if(_Visibility != value)
                {
                    _Visibility = value;

                    OnVisiblityChanged(_Visibility);
                }
            }
        }

        public PropertyBinding(
            string key, 
            PropertyDescriptor propertyDescriptor, 
            Expander expander, 
            object target, 
            int depth)
        {
            BindingKey = key;
            Property = propertyDescriptor;
            BindingExpander = expander;
            Target = target;
            Depth = depth;
        }
                
        public override string ToString()
        {
            return BindingKey;
        }

        public abstract bool HandlePropertyChanged();
        public abstract double CalcBindingNameMaxLength();
        public abstract void ApplyWidth(double width);
        protected abstract void OnVisiblityChanged(PropertyVisibility visibility);

        public abstract void PropagateVisiblityState();
    }

    internal class DirectPropertyBinding : PropertyBinding
    {
        public Control BindingControl;
        public TextBlock BindingNameControl;
        public ICellEditFactory Factory;

        public DirectPropertyBinding(
            string key,
            PropertyDescriptor propertyDescriptor,
            Expander expander,
            object target,
            int depth,
            Control bindingControl, 
            TextBlock bindingNameControl, 
            ICellEditFactory factory) :
            base(key, propertyDescriptor, expander,target,depth)
        {
            BindingControl = bindingControl;
            BindingNameControl = bindingNameControl;
            Factory = factory;
        }

        public override bool HandlePropertyChanged()
        {
            Debug.Assert(Factory != null);
            Debug.Assert(BindingControl != null);

            return Factory.HandlePropertyChanged(Target, Property, BindingControl);
        }

        public override double CalcBindingNameMaxLength()
        {
            Debug.Assert(BindingNameControl != null);

            if (BindingNameControl.Width > 0)
            {
                return BindingNameControl.Width;
            }
            else if (BindingNameControl.DesiredSize.Width > 0)
            {
                return BindingNameControl.DesiredSize.Width;
            }

            return 0;
        }

        public override void ApplyWidth(double width)
        {
            if (BindingNameControl != null)
            {
                BindingNameControl.Width = Math.Max(width - Depth * 6 / 2, 0);
            }
        }

        protected override void OnVisiblityChanged(PropertyVisibility visibility)
        {
            BindingNameControl.IsVisible = visibility == PropertyVisibility.AlwaysVisible;
            BindingControl.IsVisible = visibility == PropertyVisibility.AlwaysVisible;
        }

        public override void PropagateVisiblityState()
        {
            
        }

        public override string ToString()
        {
            return $"{PropertyName}[{BindingKey}]({Visibility})";
        }
    }

    internal class IndirectPropertyBinding : PropertyBinding
    {
        public readonly List<PropertyBinding> Children = new List<PropertyBinding>();

        public bool IsCategoryBinding => Property == null;
        
        public IndirectPropertyBinding(
            string key,
            PropertyDescriptor propertyDescriptor,
            Expander expander,
            object target,
            int depth) :
            base(key, propertyDescriptor, expander, target, depth)
        {
        }

        public void AddBinding(PropertyBinding binding)
        {
            Children.Add(binding);
        }

        public override bool HandlePropertyChanged()
        {
            foreach(var binding in Children)
            {
                binding.HandlePropertyChanged();
            }

            return true;
        }

        public override double CalcBindingNameMaxLength()
        {
            double maxLength = 0;

            foreach(var binding in Children)
            {
                var width = binding.CalcBindingNameMaxLength();

                if(width > maxLength)
                {
                    maxLength = width;
                }
            }

            return maxLength;
        }

        public override void ApplyWidth(double width)
        {
            foreach(var binding in Children)
            {
                binding.ApplyWidth(width);
            }
        }

        protected override void OnVisiblityChanged(PropertyVisibility visibility)
        {            
            
        }

        public override void PropagateVisiblityState()
        {
            bool AnyVisible = false;
            foreach(var binding in Children)
            {
                if(Visibility.HasFlag(PropertyVisibility.HiddenByCondition))
                {
                    binding.Visibility |= PropertyVisibility.HiddenByCondition;
                }

                binding.PropagateVisiblityState();

                if(binding.Visibility == PropertyVisibility.AlwaysVisible)
                {
                    AnyVisible = true;
                }
            }

            Visibility = AnyVisible ? PropertyVisibility.AlwaysVisible : PropertyVisibility.HiddenByNoVisibleChidlren;

            BindingExpander.IsVisible = Visibility == PropertyVisibility.AlwaysVisible;
        }

        public override string ToString()
        {
            if (IsCategoryBinding)
            {
                return $"Category {BindingKey}({Visibility})";
            }
            else
            {
                return $"{PropertyName}[{BindingKey}]({Visibility})";
            }
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

                    binding.HandlePropertyChanged();
                }
            }
        }

        internal double CalcBindingNameMaxLength()
        {
            double maxLength = 0;
            foreach (var info in this)
            {
                var width = info.CalcBindingNameMaxLength();
                if (width > maxLength)
                {
                    maxLength = width;
                }
            }

            return maxLength;
        }

        internal void SyncWidth(double width)
        {
            foreach (var info in this)
            {
                info.ApplyWidth(width);
            }
        }

        public IEnumerator<PropertyBinding> GetEnumerator()
        {
            foreach(var list in BindingObjects.Values)
            {
                foreach (var info in list)
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

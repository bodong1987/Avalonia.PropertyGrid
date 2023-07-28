using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.PropertyGrid.Model.Extensions;

namespace Avalonia.PropertyGrid.Samples.Models
{
    public class TestCustomObject
    {
        public string First { get; set; } = "";

        public string Second { get; set; } = "";

        [Browsable(false)]
        public string[] StringArray { get; set; } = new string[3] { "ABC", "DEF", "HJK" };
    }

    #region Helpers
    public class DynamicPropertyManager<TTarget> : IDisposable
    {
        private readonly DynamicTypeDescriptionProvider provider;
        private readonly TTarget target;

        public DynamicPropertyManager()
        {
            Type type = typeof(TTarget);

            provider = new DynamicTypeDescriptionProvider(type);
            TypeDescriptor.AddProvider(provider, type);
        }

        public DynamicPropertyManager(TTarget target)
        {
            this.target = target;

            provider = new DynamicTypeDescriptionProvider(typeof(TTarget));
            TypeDescriptor.AddProvider(provider, target);
        }

        public IList<PropertyDescriptor> Properties
        {
            get { return provider.Properties; }
        }

        public void Dispose()
        {
            if (ReferenceEquals(target, null))
            {
                TypeDescriptor.RemoveProvider(provider, typeof(TTarget));
            }
            else
            {
                TypeDescriptor.RemoveProvider(provider, target);
            }
        }

        public static DynamicPropertyDescriptor<TTargetType, TPropertyType>
           CreateProperty<TTargetType, TPropertyType>(
               string displayName,
               Func<TTargetType, TPropertyType> getter,
               Action<TTargetType, TPropertyType> setter,
               Attribute[] attributes)
        {
            return new DynamicPropertyDescriptor<TTargetType, TPropertyType>(
               displayName, getter, setter, attributes);
        }

        public static DynamicPropertyDescriptor<TTargetType, TPropertyType>
           CreateProperty<TTargetType, TPropertyType>(
              string displayName,
              Func<TTargetType, TPropertyType> getHandler,
              Attribute[] attributes)
        {
            return new DynamicPropertyDescriptor<TTargetType, TPropertyType>(
               displayName, getHandler, (t, p) => { }, attributes);
        }
    }

    public class DynamicTypeDescriptionProvider : TypeDescriptionProvider
    {
        private readonly TypeDescriptionProvider provider;
        private readonly List<PropertyDescriptor> properties = new List<PropertyDescriptor>();

        public DynamicTypeDescriptionProvider(Type type)
        {
            provider = TypeDescriptor.GetProvider(type);
        }

        public IList<PropertyDescriptor> Properties
        {
            get { return properties; }
        }

        public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
        {
            return new DynamicCustomTypeDescriptor(
               this, provider.GetTypeDescriptor(objectType, instance));
        }

        private class DynamicCustomTypeDescriptor : CustomTypeDescriptor
        {
            private readonly DynamicTypeDescriptionProvider provider;

            public DynamicCustomTypeDescriptor(DynamicTypeDescriptionProvider provider,
               ICustomTypeDescriptor descriptor)
                  : base(descriptor)
            {
                this.provider = provider;
            }

            public override PropertyDescriptorCollection GetProperties()
            {
                return GetProperties(null);
            }

            public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
            {
                var properties = new PropertyDescriptorCollection(null);

                foreach (PropertyDescriptor property in base.GetProperties(attributes))
                {
                    properties.Add(property);
                }

                foreach (PropertyDescriptor property in provider.Properties)
                {
                    properties.Add(property);
                }
                return properties;
            }
        }
    }

    public class DynamicPropertyDescriptor<TTarget, TProperty> : PropertyDescriptor
    {
        private readonly Func<TTarget, TProperty> getter;
        private readonly Action<TTarget, TProperty> setter;
        private readonly string propertyName;

        public DynamicPropertyDescriptor(
           string propertyName,
           Func<TTarget, TProperty> getter,
           Action<TTarget, TProperty> setter,
           Attribute[] attributes)
              : base(propertyName, attributes ?? new Attribute[] { })
        {
            this.setter = setter;
            this.getter = getter;
            this.propertyName = propertyName;
        }

        public override bool Equals(object obj)
        {
            var o = obj as DynamicPropertyDescriptor<TTarget, TProperty>;
            return o != null && o.propertyName.Equals(propertyName);
        }

        public override int GetHashCode()
        {
            return propertyName.GetHashCode();
        }

        public override bool CanResetValue(object component)
        {
            return true;
        }

        public override Type ComponentType
        {
            get { return typeof(TTarget); }
        }

        public override object GetValue(object component)
        {
            return getter((TTarget)component);
        }

        public override bool IsReadOnly
        {
            get 
            { 
                if(setter == null)
                {
                    return true;
                }

                var attr = this.GetCustomAttribute<ReadOnlyAttribute>();

                return attr != null && attr.IsReadOnly;
            }
        }

        public override Type PropertyType
        {
            get { return typeof(TProperty); }
        }

        public override void ResetValue(object component)
        {
        }

        public override void SetValue(object component, object value)
        {
            setter((TTarget)component, (TProperty)value);
        }

        public override bool ShouldSerializeValue(object component)
        {
            return true;
        }
    }
    #endregion
}
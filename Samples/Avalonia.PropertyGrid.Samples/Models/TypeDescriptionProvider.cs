using System;
using System.Collections.Generic;
using System.ComponentModel;
using PropertyModels.Extensions;

namespace Avalonia.PropertyGrid.Samples.Models
{
    public class TestCustomObject
    {
        public string First { get; set; } = string.Empty;

        public string Second { get; set; } = string.Empty;

        [Browsable(false)]
        public string[] StringArray { get; set; } = ["ABC", "DEF", "HJK"];
    }

    #region Helpers

    public class DynamicPropertyManager<TTarget> : IDisposable
    {
        private readonly DynamicTypeDescriptionProvider provider;
        private readonly TTarget? target;

        public DynamicPropertyManager()
        {
            var type = typeof(TTarget);

            provider = new DynamicTypeDescriptionProvider(type);
            TypeDescriptor.AddProvider(provider, type);
        }

        public DynamicPropertyManager(TTarget target)
        {
            this.target = target;

            provider = new DynamicTypeDescriptionProvider(typeof(TTarget));
            TypeDescriptor.AddProvider(provider, target!);
        }

        public IList<PropertyDescriptor> Properties => provider.Properties;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                TypeDescriptor.RemoveProvider(provider, target ?? (object)typeof(TTarget));
            }
        }

        public static DynamicPropertyDescriptor<TTargetType, TPropertyType> CreateProperty<TTargetType, TPropertyType>(
            string displayName,
            Func<TTargetType?, TPropertyType?> getter,
            Action<TTargetType?, TPropertyType?> setter,
            Attribute[] attributes) => new(displayName, getter, setter, attributes);

        public static DynamicPropertyDescriptor<TTargetType, TPropertyType> CreateProperty<TTargetType, TPropertyType>(
            string displayName,
            Func<TTargetType?, TPropertyType?> getHandler,
            Attribute[] attributes) => new(displayName, getHandler, (t, p) => { }, attributes);
    }

    public class DynamicTypeDescriptionProvider : TypeDescriptionProvider
    {
        private readonly TypeDescriptionProvider provider;
        private readonly List<PropertyDescriptor> properties = [];

        public DynamicTypeDescriptionProvider(Type type) => provider = TypeDescriptor.GetProvider(type);

        public IList<PropertyDescriptor> Properties => properties;

        public override ICustomTypeDescriptor? GetTypeDescriptor(Type objectType, object? instance)
            => new DynamicCustomTypeDescriptor(this, provider.GetTypeDescriptor(objectType, instance)!);

        private class DynamicCustomTypeDescriptor : CustomTypeDescriptor
        {
            private readonly DynamicTypeDescriptionProvider provider;

            public DynamicCustomTypeDescriptor(DynamicTypeDescriptionProvider provider, ICustomTypeDescriptor descriptor)
                  : base(descriptor) => this.provider = provider;

            public override PropertyDescriptorCollection GetProperties() => GetProperties(null);

            public override PropertyDescriptorCollection GetProperties(Attribute[]? attributes)
            {
                var properties = new PropertyDescriptorCollection(null);

                foreach (PropertyDescriptor property in base.GetProperties(attributes))
                {
                    properties.Add(property);
                }

                foreach (var property in provider.Properties)
                {
                    properties.Add(property);
                }

                return properties;
            }
        }
    }

    public class DynamicPropertyDescriptor<TTarget, TProperty> : PropertyDescriptor
    {
        private readonly Func<TTarget?, TProperty?> getter;
        private readonly Action<TTarget?, TProperty?> setter;
        private readonly string propertyName;

        public DynamicPropertyDescriptor(
           string propertyName,
           Func<TTarget?, TProperty?> getter,
           Action<TTarget?, TProperty?> setter,
           Attribute[] attributes)
              : base(propertyName, attributes ?? [])
        {
            this.setter = setter;
            this.getter = getter;
            this.propertyName = propertyName;
        }

        public override bool Equals(object? obj)
            => obj is DynamicPropertyDescriptor<TTarget, TProperty> o
            && o.propertyName.Equals(propertyName, StringComparison.Ordinal);

        public override int GetHashCode() => propertyName.GetHashCode();

        public override bool CanResetValue(object component) => true;

        public override Type ComponentType => typeof(TTarget);

        public override object? GetValue(object? component) => getter((TTarget?)component);

        public override bool IsReadOnly
        {
            get
            {
                if (setter == null)
                {
                    return true;
                }

                var attr = this.GetCustomAttribute<ReadOnlyAttribute>();
                return attr?.IsReadOnly == true;
            }
        }

        public override Type PropertyType => typeof(TProperty);

        public override void ResetValue(object component)
        { }

        public override void SetValue(object? component, object? value) => setter((TTarget?)component, (TProperty?)value);

        public override bool ShouldSerializeValue(object component) => true;
    }

    #endregion
}
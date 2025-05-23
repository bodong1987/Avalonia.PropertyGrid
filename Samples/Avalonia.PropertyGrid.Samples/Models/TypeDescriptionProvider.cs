using System;
using System.Collections.Generic;
using System.ComponentModel;
using PropertyModels.Extensions;
// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
// ReSharper disable NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract
// ReSharper disable ReturnTypeCanBeNotNullable
// ReSharper disable ClassWithVirtualMembersNeverInherited.Global

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
        private readonly DynamicTypeDescriptionProvider _provider;
        private readonly TTarget? _target;

        public DynamicPropertyManager()
        {
            var type = typeof(TTarget);

            _provider = new DynamicTypeDescriptionProvider(type);
            TypeDescriptor.AddProvider(_provider, type);
        }

        public DynamicPropertyManager(TTarget target)
        {
            _target = target;

            _provider = new DynamicTypeDescriptionProvider(typeof(TTarget));
            TypeDescriptor.AddProvider(_provider, target!);
        }

        public IList<PropertyDescriptor> Properties => _provider.Properties;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                TypeDescriptor.RemoveProvider(_provider, _target ?? (object)typeof(TTarget));
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
        private readonly TypeDescriptionProvider _provider;
        private readonly List<PropertyDescriptor> _properties = [];

        public DynamicTypeDescriptionProvider(Type type) => _provider = TypeDescriptor.GetProvider(type);

        public IList<PropertyDescriptor> Properties => _properties;

        public override ICustomTypeDescriptor? GetTypeDescriptor(Type objectType, object? instance)
            => new DynamicCustomTypeDescriptor(this, _provider.GetTypeDescriptor(objectType, instance)!);

        private class DynamicCustomTypeDescriptor : CustomTypeDescriptor
        {
            private readonly DynamicTypeDescriptionProvider _provider;

            public DynamicCustomTypeDescriptor(DynamicTypeDescriptionProvider provider, ICustomTypeDescriptor descriptor)
                  : base(descriptor) => _provider = provider;

            public override PropertyDescriptorCollection GetProperties() => GetProperties(null);

            public override PropertyDescriptorCollection GetProperties(Attribute[]? attributes)
            {
                var properties = new PropertyDescriptorCollection(null);

                foreach (PropertyDescriptor property in base.GetProperties(attributes))
                {
                    properties.Add(property);
                }

                foreach (var property in _provider.Properties)
                {
                    properties.Add(property);
                }

                return properties;
            }
        }
    }

    public class DynamicPropertyDescriptor<TTarget, TProperty> : PropertyDescriptor
    {
        private readonly Func<TTarget?, TProperty?> _getter;
        private readonly Action<TTarget?, TProperty?> _setter;
        private readonly string _propertyName;

        public DynamicPropertyDescriptor(
           string propertyName,
           Func<TTarget?, TProperty?> getter,
           Action<TTarget?, TProperty?> setter,
           Attribute[] attributes)
              : base(propertyName, attributes ?? [])
        {
            _setter = setter;
            _getter = getter;
            _propertyName = propertyName;
        }

        public override bool Equals(object? obj)
            => obj is DynamicPropertyDescriptor<TTarget, TProperty> o
            && o._propertyName.Equals(_propertyName, StringComparison.Ordinal);

        public override int GetHashCode() => _propertyName.GetHashCode();

        public override bool CanResetValue(object component) => true;

        public override Type ComponentType => typeof(TTarget);

        public override object? GetValue(object? component) => _getter((TTarget?)component);

        public override bool IsReadOnly
        {
            get
            {
                if (_setter == null)
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

        public override void SetValue(object? component, object? value) => _setter((TTarget?)component, (TProperty?)value);

        public override bool ShouldSerializeValue(object component) => true;
    }

    #endregion
}
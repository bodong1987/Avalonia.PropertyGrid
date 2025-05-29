using System;
using System.Collections.Generic;
using System.ComponentModel;
using PropertyModels.ComponentModel;

namespace Avalonia.PropertyGrid.Samples.FeatureDemos.Models
{
    /// <summary>
    /// Class ScriptableOptions.
    /// Implements the <see cref="ReactiveObject" />
    /// Implements the <see cref="ICustomTypeDescriptor" />
    /// </summary>
    /// <seealso cref="ReactiveObject" />
    /// <seealso cref="ICustomTypeDescriptor" />
    public class ScriptableOptions : ReactiveObject, ICustomTypeDescriptor
    {
        /// <summary>
        /// The properties
        /// </summary>
        public readonly List<ScriptableObject> Properties = [];

        /// <summary>
        /// Adds the property.
        /// </summary>
        /// <param name="property">The property.</param>
        public void AddProperty(ScriptableObject property) => Properties.Add(property);

        #region Custom Type Descriptor Interfaces
        /// <summary>
        /// Returns a collection of custom attributes for this instance of a component.
        /// </summary>
        /// <returns>An <see cref="T:System.ComponentModel.AttributeCollection" /> containing the attributes for this object.</returns>
        public AttributeCollection GetAttributes() => TypeDescriptor.GetAttributes(this, true);

        /// <summary>
        /// Returns the class name of this instance of a component.
        /// </summary>
        /// <returns>The class name of the object, or <see langword="null" /> if the class does not have a name.</returns>
        public string? GetClassName() => TypeDescriptor.GetClassName(this, true);

        /// <summary>
        /// Returns the name of this instance of a component.
        /// </summary>
        /// <returns>The name of the object, or <see langword="null" /> if the object does not have a name.</returns>
        public string? GetComponentName() => TypeDescriptor.GetComponentName(this, true);

        /// <summary>
        /// Returns a type converter for this instance of a component.
        /// </summary>
        /// <returns>A <see cref="T:System.ComponentModel.TypeConverter" /> that is the converter for this object, or <see langword="null" /> if there is no <see cref="T:System.ComponentModel.TypeConverter" /> for this object.</returns>
        public TypeConverter GetConverter() => TypeDescriptor.GetConverter(this, true);

        /// <summary>
        /// Returns the default event for this instance of a component.
        /// </summary>
        /// <returns>An <see cref="T:System.ComponentModel.EventDescriptor" /> that represents the default event for this object, or <see langword="null" /> if this object does not have events.</returns>
        public EventDescriptor? GetDefaultEvent() => TypeDescriptor.GetDefaultEvent(this, true);

        /// <summary>
        /// Returns the default property for this instance of a component.
        /// </summary>
        /// <returns>A <see cref="T:System.ComponentModel.PropertyDescriptor" /> that represents the default property for this object, or <see langword="null" /> if this object does not have properties.</returns>
        public PropertyDescriptor? GetDefaultProperty() => TypeDescriptor.GetDefaultProperty(this, true);

        /// <summary>
        /// Returns an editor of the specified type for this instance of a component.
        /// </summary>
        /// <param name="editorBaseType">A <see cref="T:System.Type" /> that represents the editor for this object.</param>
        /// <returns>An <see cref="T:System.Object" /> of the specified type that is the editor for this object, or <see langword="null" /> if the editor cannot be found.</returns>
        public object? GetEditor(Type editorBaseType) => TypeDescriptor.GetEditor(this, editorBaseType, true);

        /// <summary>
        /// Returns the events for this instance of a component.
        /// </summary>
        /// <returns>An <see cref="T:System.ComponentModel.EventDescriptorCollection" /> that represents the events for this component instance.</returns>
        public EventDescriptorCollection GetEvents() => TypeDescriptor.GetEvents(this, true);

        /// <summary>
        /// Returns the events for this instance of a component using the specified attribute array as a filter.
        /// </summary>
        /// <param name="attributes">An array of type <see cref="T:System.Attribute" /> that is used as a filter.</param>
        /// <returns>An <see cref="T:System.ComponentModel.EventDescriptorCollection" /> that represents the filtered events for this component instance.</returns>
        public EventDescriptorCollection GetEvents(Attribute[]? attributes) => TypeDescriptor.GetEvents(this, attributes, true);

        /// <summary>
        /// Returns the properties for this instance of a component.
        /// </summary>
        /// <returns>A <see cref="T:System.ComponentModel.PropertyDescriptorCollection" /> that represents the properties for this component instance.</returns>
        public PropertyDescriptorCollection GetProperties() => GetProperties(null);

        /// <summary>
        /// Returns the properties for this instance of a component using the attribute array as a filter.
        /// </summary>
        /// <param name="attributes">An array of type <see cref="T:System.Attribute" /> that is used as a filter.</param>
        /// <returns>A <see cref="T:System.ComponentModel.PropertyDescriptorCollection" /> that represents the filtered properties for this component instance.</returns>
        public PropertyDescriptorCollection GetProperties(Attribute[]? attributes)
        {
            var pds = new PropertyDescriptorCollection(null);

            foreach (var property in Properties)
            {
                var pd = new ScriptableObjectPropertyDescriptor(property);

                pds.Add(pd);
            }

            return pds;
        }

        /// <summary>
        /// Returns an object that contains the property described by the specified property descriptor.
        /// </summary>
        /// <param name="pd">A <see cref="T:System.ComponentModel.PropertyDescriptor" /> that represents the property whose owner is to be found.</param>
        /// <returns>An <see cref="T:System.Object" /> that represents the owner of the specified property.</returns>
        public object GetPropertyOwner(PropertyDescriptor? pd) => this;
        #endregion
    }

    #region Property Descriptor

    /// <summary>
    /// Class ScriptableObjectPropertyDescriptor.
    /// Implements the <see cref="PropertyDescriptor" />
    /// </summary>
    /// <seealso cref="PropertyDescriptor" />
    internal class ScriptableObjectPropertyDescriptor : PropertyDescriptor
    {
        private readonly Type _type;
        private readonly string? _displayName;
        private readonly string? _description;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptableObjectPropertyDescriptor" /> class.
        /// </summary>
        /// <param name="target">The target.</param>
        public ScriptableObjectPropertyDescriptor(ScriptableObject target)
            : base(target.Name!, target.GetAttributes())
        {
            _type = target.ValueType!;
            _displayName = target.DisplayName;
            _description = target.Description;
        }

        public override Type ComponentType => typeof(ScriptableOptions);

        public override bool IsReadOnly => Attributes[typeof(ReadOnlyAttribute)] is ReadOnlyAttribute
        {
            IsReadOnly: true
        };

        // ReSharper disable once FunctionRecursiveOnAllPaths
        // ReSharper disable once ConvertToAutoProperty
        public override Type PropertyType => _type;

        public override string DisplayName => _displayName ?? string.Empty;

        public override string Description => _description ?? string.Empty;

        public override bool CanResetValue(object component) => false;

        public override object? GetValue(object? component)
        {
            if (component is ScriptableOptions options)
            {
                var obj = options.Properties.Find(x => x.Name == Name);

                return obj?.Value;
            }

            return null;
        }

        public override void ResetValue(object component)
        { }

        public override void SetValue(object? component, object? value)
        {
            if (component is ScriptableOptions options)
            {
                var obj = options.Properties.Find(x => x.Name == Name);

                if (obj != null)
                {
                    obj.Value = value;

                    options.RaisePropertyChanged(Name);
                }
            }
        }

        public override bool ShouldSerializeValue(object component) => throw new NotImplementedException();
    }

    #endregion

}

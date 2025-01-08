using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;

namespace PropertyModels.ComponentModel
{
    /// <summary>
    /// Class BindListElementPropertyDescriptor.
    /// Implements the <see cref="PropertyDescriptor" />
    /// </summary>
    /// <seealso cref="PropertyDescriptor" />
    public class ListElementPropertyDescriptor : PropertyDescriptor
    {
        /// <summary>
        /// When overridden in a derived class, gets the type of the component this property is bound to.
        /// </summary>
        /// <value>The type of the component.</value>
        public override Type ComponentType => typeof(ListElementPropertyDescriptor);

        /// <summary>
        /// When overridden in a derived class, gets a value indicating whether this property is read-only.
        /// </summary>
        /// <value><c>true</c> if this instance is read only; otherwise, <c>false</c>.</value>
        public override bool IsReadOnly => false;

        /// <summary>
        /// When overridden in a derived class, gets the type of the property.
        /// </summary>
        /// <value>The type of the property.</value>
        public override Type PropertyType { get; }

        /// <summary>
        /// Gets the index.
        /// </summary>
        /// <value>The index.</value>
        public int Index { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListElementPropertyDescriptor" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="index">The index.</param>
        /// <param name="elementType">Type of the element.</param>
        /// <param name="attributes">The attributes.</param>
        public ListElementPropertyDescriptor(string name, int index, Type elementType, Attribute[]? attributes = null) :
            base(name, attributes)
        {
            Index = index;
            PropertyType = elementType;
        }

        /// <summary>
        /// When overridden in a derived class, returns whether resetting an object changes its value.
        /// </summary>
        /// <param name="component">The component to test for reset capability.</param>
        /// <returns><see langword="true" /> if resetting the component changes its value; otherwise, <see langword="false" />.</returns>
        public override bool CanResetValue(object component)
        {
            return false;
        }

        /// <summary>
        /// When overridden in a derived class, gets the current value of the property on a component.
        /// </summary>
        /// <param name="component">The component with the property for which to retrieve the value.</param>
        /// <returns>The value of a property for a given component.</returns>
        public override object? GetValue(object? component)
        {
            Debug.Assert(component is IList);

            return (component as IList)![Index];
        }

        /// <summary>
        /// When overridden in a derived class, resets the value for this property of the component to the default value.
        /// </summary>
        /// <param name="component">The component with the property value that is to be reset to the default value.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public override void ResetValue(object component)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// When overridden in a derived class, sets the value of the component to a different value.
        /// </summary>
        /// <param name="component">The component with the property value that is to be set.</param>
        /// <param name="value">The new value.</param>
        public override void SetValue(object? component, object? value)
        {
            Debug.Assert(component is IList);

            (component as IList)![Index] = value;
        }

        /// <summary>
        /// When overridden in a derived class, determines a value indicating whether the value of this property needs to be persisted.
        /// </summary>
        /// <param name="component">The component with the property to be examined for persistence.</param>
        /// <returns><see langword="true" /> if the property should be persisted; otherwise, <see langword="false" />.</returns>
        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace PropertyModels.ComponentModel
{
    /// <summary>
    /// Class MultiObjectPropertyDescriptor.
    /// Implements the <see cref="PropertyDescriptor" />
    /// </summary>
    /// <seealso cref="PropertyDescriptor" />
    public class MultiObjectPropertyDescriptor : PropertyDescriptor, IEnumerable<PropertyDescriptor>
    {
        #region MultiObjectAttributes
        /// <summary>
        /// Class MultiObjectAttributes.
        /// Implements the <see cref="AttributeCollection" />
        /// </summary>
        /// <seealso cref="AttributeCollection" />
        private class MultiObjectAttributes : AttributeCollection
        {
            private readonly MultiObjectPropertyDescriptor _owner;
            private AttributeCollection[]? _parentAttributes;
            private readonly Lazy<Attribute[]> _attributes;
            private Hashtable? _attributesTable;

            /// <summary>
            /// Initializes a new instance of the <see cref="MultiObjectAttributes"/> class.
            /// </summary>
            /// <param name="owner">The owner.</param>
            public MultiObjectAttributes(MultiObjectPropertyDescriptor owner)
                : base(null)
            {
                _owner = owner;
                _attributes = new Lazy<Attribute[]>(GetAttributes);
            }

            /// <summary>
            /// Gets the <see cref="Attribute"/> with the specified attribute type.
            /// </summary>
            /// <param name="attributeType">Type of the attribute.</param>
            /// <returns>Attribute.</returns>
            public override Attribute? this[Type attributeType]
            {
                get
                {
                    if (_parentAttributes == null)
                    {
                        _parentAttributes = new AttributeCollection[_owner.Descriptors.Length];
                        for (var i = 0; i < _parentAttributes.Length; i++)
                        {
                            _parentAttributes[i] = _owner.Descriptors[i].Attributes;
                        }
                    }

                    if (_parentAttributes.Length == 0)
                    {
                        return GetDefaultAttribute(attributeType);
                    }

                    Attribute? a;
                    if (_attributesTable != null)
                    {
                        a = (Attribute?)_attributesTable[attributeType];
                        if (a != null)
                        {
                            return a;
                        }
                    }

                    a = _parentAttributes[0][attributeType];
                    if (a == null)
                    {
                        return null;
                    }

                    for (var i = 1; i < _parentAttributes.Length; i++)
                    {
                        if (!a.Equals(_parentAttributes[i][attributeType]))
                        {
                            a = GetDefaultAttribute(attributeType);
                            break;
                        }
                    }

                    _attributesTable ??= [];
                    _attributesTable[attributeType] = a;

                    return a;
                }
            }

            /// <summary>
            /// Gets the attribute collection.
            /// </summary>
            /// <value>The attributes.</value>
            protected override Attribute[] Attributes => _attributes.Value;

            /// <summary>
            /// Gets the attributes.
            /// </summary>
            /// <returns>Attribute[].</returns>
            private Attribute[] GetAttributes()
            {
                return _owner.Descriptors.First().Attributes.Cast<Attribute>()
                        .Select(x => this[x.GetType()])
                        .Where(attr => attr != null)
                        .Select(attr => attr!)
                        .ToArray();
            }
        }
        #endregion

        /// <summary>
        /// Gets the descriptors.
        /// </summary>
        /// <value>The descriptors.</value>
        internal PropertyDescriptor[] Descriptors { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiObjectPropertyDescriptor"/> class.
        /// </summary>
        /// <param name="descriptors">The descriptors.</param>
        public MultiObjectPropertyDescriptor(PropertyDescriptor[] descriptors)
            : base(descriptors[0].Name, null)
        {
            Descriptors = descriptors;
        }

        /// <summary>
        /// Gets the owner.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="index">The index.</param>
        /// <returns>System.Object.</returns>
        private object? GetOwner(object[] list, int index)
        {
            var res = list[index];
            return res is not ICustomTypeDescriptor custom ? res : custom.GetPropertyOwner(Descriptors[index]);
        }

        /// <summary>
        /// Creates a collection of attributes using the array of attributes passed to the constructor.
        /// </summary>
        /// <returns>A new <see cref="T:System.ComponentModel.AttributeCollection" /> that contains the <see cref="P:System.ComponentModel.MemberDescriptor.AttributeArray" /> attributes.</returns>
        protected override AttributeCollection CreateAttributeCollection() => new MultiObjectAttributes(this);

        /// <summary>
        /// Gets an editor of the specified type.
        /// </summary>
        /// <param name="editorBaseType">The base type of editor, which is used to differentiate between multiple editors that a property supports.</param>
        /// <returns>An instance of the requested editor type, or <see langword="null" /> if an editor cannot be found.</returns>
        public override object? GetEditor(Type editorBaseType) => Descriptors[0].GetEditor(editorBaseType);

        /// <summary>
        /// When overridden in a derived class, returns whether resetting an object changes its value.
        /// </summary>
        /// <param name="component">The component to test for reset capability.</param>
        /// <returns><see langword="true" /> if resetting the component changes its value; otherwise, <see langword="false" />.</returns>
        public override bool CanResetValue(object component)
        {
            var list = (object[])component;
            for (var i = 0; i < Descriptors.Length; i++)
            {
                if (!Descriptors[i].CanResetValue(GetOwner(list, i)!))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Gets a value indicating whether value change notifications for this property may originate from outside the property descriptor.
        /// </summary>
        /// <value><c>true</c> if [supports change events]; otherwise, <c>false</c>.</value>
        public override bool SupportsChangeEvents { get { return Descriptors.Any(p => p.SupportsChangeEvents); } }

        /// <summary>
        /// Enables other objects to be notified when this property changes.
        /// </summary>
        /// <param name="component">The component to add the handler for.</param>
        /// <param name="handler">The delegate to add as a listener.</param>
        public override void AddValueChanged(object component, EventHandler handler)
        {
            var list = (object[])component;
            for (var i = 0; i < Descriptors.Length; i++)
            {
                if (!Descriptors[i].SupportsChangeEvents)
                {
                    continue;
                }

                Descriptors[i].AddValueChanged(GetOwner(list, i)!, handler);
            }
        }

        /// <summary>
        /// Enables other objects to be notified when this property changes.
        /// </summary>
        /// <param name="component">The component to remove the handler for.</param>
        /// <param name="handler">The delegate to remove as a listener.</param>
        public override void RemoveValueChanged(object component, EventHandler handler)
        {
            var list = (object[])component;
            for (var i = 0; i < Descriptors.Length; i++)
            {
                if (!Descriptors[i].SupportsChangeEvents)
                {
                    continue;
                }

                Descriptors[i].RemoveValueChanged(GetOwner(list, i)!, handler);
            }
        }

        /// <summary>
        /// When overridden in a derived class, gets the type of the component this property is bound to.
        /// </summary>
        /// <value>The type of the component.</value>
        public override Type ComponentType => Descriptors[0].ComponentType;

        /// <summary>
        /// When overridden in a derived class, gets the current value of the property on a component.
        /// </summary>
        /// <param name="component">The component with the property for which to retrieve the value.</param>
        /// <returns>The value of a property for a given component.</returns>
        public override object? GetValue(object? component)
        {
            var list = (component as IEnumerable)!.Cast<object>().ToArray();
            var res = GetValue(Descriptors[0], GetOwner(list, 0)!);
            for (var i = 0; i < Descriptors.Length; i++)
            {
                var temp = GetValue(Descriptors[i], GetOwner(list, i)!);
                if (res != temp && res?.Equals(temp) == false)
                {
                    return null;
                }
            }

            return res;
        }

        /// <summary>
        /// When overridden in a derived class, gets a value indicating whether this property is read-only.
        /// </summary>
        /// <value><c>true</c> if this instance is read only; otherwise, <c>false</c>.</value>
        public override bool IsReadOnly => Descriptors.Any(x => x.IsReadOnly);

        /// <summary>
        /// When overridden in a derived class, gets the type of the property.
        /// </summary>
        /// <value>The type of the property.</value>
        public override Type PropertyType => Descriptors[0].PropertyType;

        /// <summary>
        /// Gets the type converter for this property.
        /// </summary>
        /// <value>The converter.</value>
        public override TypeConverter Converter => Descriptors[0].Converter!;

        /// <summary>
        /// Gets the name that can be displayed in a window, such as a Properties window.
        /// </summary>
        /// <value>The display name.</value>
        public override string DisplayName => Descriptors[0].DisplayName;

        /// <summary>
        /// When overridden in a derived class, resets the value for this property of the component to the default value.
        /// </summary>
        /// <param name="component">The component with the property value that is to be reset to the default value.</param>
        public override void ResetValue(object component)
        {
            var list = (object[])component;
            for (var i = 0; i < Descriptors.Length; i++)
            {
                Descriptors[i].ResetValue(GetOwner(list, i)!);
            }
        }

        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <param name="components">The components.</param>
        /// <returns>System.Object[].</returns>
        public object?[] GetValues(object[] components)
        {
            var list = new object?[components.Length];
            for (var i = 0; i < Descriptors.Length; i++)
            {
                list[i] = GetValue(Descriptors[i], GetOwner(components, i)!);
            }

            return list;
        }

        /// <summary>
        /// When overridden in a derived class, sets the value of the component to a different value.
        /// </summary>
        /// <param name="component">The component with the property value that is to be set.</param>
        /// <param name="value">The new value.</param>
        public override void SetValue(object? component, object? value)
        {
            var list = (object[])component!;
            for (var i = 0; i < Descriptors.Length; i++)
            {
                object? clonedVal = null;
                if (value is ICloneable cloneable)
                {
                    clonedVal = cloneable.Clone();
                }

                Descriptors[i].SetValue(GetOwner(list, i), clonedVal ?? value);
            }
        }

        private static object? GetValue(PropertyDescriptor? pd, object? component)
        {
            object? value = null;
            if (component == null || pd == null)
            {
                return value;
            }

            try
            {
                component = GetPropertyOwner(pd, component);
                value = pd.GetValue(component);
            }
            catch (Exception e)
            {
                value = GetUnWindedException(e).Message;
            }

            return value;
        }

        private static Exception GetUnWindedException(Exception e)
        {
            var result = e;
            if (e is TargetInvocationException)
            {
                result = e.InnerException;
            }

            var message = result!.Message;
            while (string.IsNullOrEmpty(message) && result.InnerException != null)
            {
                message = result.InnerException.Message;
                result = result.InnerException;
            }

            return result;
        }

        private static object GetPropertyOwner(PropertyDescriptor pd, object component) => component is not ICustomTypeDescriptor typeDescriptor ? component : typeDescriptor.GetPropertyOwner(pd)!;

        /// <summary>
        /// When overridden in a derived class, determines a value indicating whether the value of this property needs to be persisted.
        /// </summary>
        /// <param name="component">The component with the property to be examined for persistence.</param>
        /// <returns><see langword="true" /> if the property should be persisted; otherwise, <see langword="false" />.</returns>
        public override bool ShouldSerializeValue(object component)
        {
            var list = (object[])component;
            for (var i = 0; i < Descriptors.Length; i++)
            {
                if (Descriptors[i].ShouldSerializeValue(GetOwner(list, i)!))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the <see cref="PropertyDescriptor"/> at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>PropertyDescriptor.</returns>
        public PropertyDescriptor this[int index] => Descriptors[index];

        /// <summary>
        /// Sets the values.
        /// </summary>
        /// <param name="components">The components.</param>
        /// <param name="values">The values.</param>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public void SetValues(object components, object values)
        {
            var valuesArray = (object[])values;
            var componentsArray = (object[])components;

            if (valuesArray.Length != Descriptors.Length || valuesArray.Length != componentsArray.Length)
            {
                throw new ArgumentOutOfRangeException();
            }

            for (var i = 0; i < Descriptors.Length; i++)
            {
                Descriptors[i].SetValue(componentsArray[i], valuesArray[i]);
            }
        }

        /// <summary>
        /// Gets the name of the category to which the member belongs, as specified in the <see cref="T:System.ComponentModel.CategoryAttribute" />.
        /// </summary>
        /// <value>The category.</value>
        public override string Category => Descriptors[0].Category ?? string.Empty;

        #region IEnumerable
        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<PropertyDescriptor> GetEnumerator()
        {
            foreach (var descriptor in Descriptors)
            {
                yield return descriptor;
            }
        }
        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        #endregion
    }
}

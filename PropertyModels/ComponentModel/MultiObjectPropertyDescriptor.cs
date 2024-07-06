using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

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
            private MultiObjectPropertyDescriptor _Owner;
            private AttributeCollection[] _ParentAttributes;
            private Lazy<Attribute[]> _Attributes;
            private Hashtable _AttributesTable;

            /// <summary>
            /// Initializes a new instance of the <see cref="MultiObjectAttributes"/> class.
            /// </summary>
            /// <param name="owner">The owner.</param>
            public MultiObjectAttributes(MultiObjectPropertyDescriptor owner)
                : base(null)
            {
                _Owner = owner;
                _Attributes = new Lazy<Attribute[]>(() => GetAttributes());
            }

            /// <summary>
            /// Gets the <see cref="Attribute"/> with the specified attribute type.
            /// </summary>
            /// <param name="attributeType">Type of the attribute.</param>
            /// <returns>Attribute.</returns>
            public override Attribute this[Type attributeType]
            {
                get
                {
                    if (_ParentAttributes == null)
                    {
                        _ParentAttributes = new AttributeCollection[_Owner._Descriptors.Length];
                        for (int i = 0; i < _ParentAttributes.Length; i++)
                        {
                            _ParentAttributes[i] = _Owner._Descriptors[i].Attributes;
                        }                            
                    }

                    if (_ParentAttributes.Length == 0)
                    {
                        return GetDefaultAttribute(attributeType);
                    }
                        
                    Attribute a;
                    if (_AttributesTable != null)
                    {
                        a = (Attribute)_AttributesTable[attributeType];
                        if (a != null)
                        {
                            return a;
                        }                            
                    }
                    a = _ParentAttributes[0][attributeType];
                    if (a == null)
                    {
                        return null;
                    }
                        
                    for (int i = 1; i < _ParentAttributes.Length; i++)
                    {
                        if (!a.Equals(_ParentAttributes[i][attributeType]))
                        {
                            a = GetDefaultAttribute(attributeType);
                            break;
                        }
                    }

                    if (_AttributesTable == null)
                    {
                        _AttributesTable = new Hashtable();
                    }
                        
                    _AttributesTable[attributeType] = a;
                    return a;
                }
            }

            /// <summary>
            /// Gets the attribute collection.
            /// </summary>
            /// <value>The attributes.</value>
            protected override Attribute[] Attributes
            {
                get { return _Attributes.Value; }
            }

            /// <summary>
            /// Gets the attributes.
            /// </summary>
            /// <returns>Attribute[].</returns>
            private Attribute[] GetAttributes()
            {
                return _Owner.Descriptors.First().Attributes.Cast<Attribute>()
                    .Select(x => this[x.GetType()])
                    .ToArray();
            }
        }
        #endregion

        private PropertyDescriptor[] _Descriptors;
        /// <summary>
        /// Gets the descriptors.
        /// </summary>
        /// <value>The descriptors.</value>
        internal PropertyDescriptor[] Descriptors => _Descriptors;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiObjectPropertyDescriptor"/> class.
        /// </summary>
        /// <param name="descriptors">The descriptors.</param>
        public MultiObjectPropertyDescriptor(PropertyDescriptor[] descriptors)
            : base(descriptors[0].Name, null)
        {
            _Descriptors = descriptors;
        }

        /// <summary>
        /// Gets the owner.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="index">The index.</param>
        /// <returns>System.Object.</returns>
        private object GetOwner(object[] list, int index)
        {
            object res = list[index];
            ICustomTypeDescriptor custom = res as ICustomTypeDescriptor;
            return custom == null ? res : custom.GetPropertyOwner(_Descriptors[index]);
        }

        /// <summary>
        /// Creates a collection of attributes using the array of attributes passed to the constructor.
        /// </summary>
        /// <returns>A new <see cref="T:System.ComponentModel.AttributeCollection" /> that contains the <see cref="P:System.ComponentModel.MemberDescriptor.AttributeArray" /> attributes.</returns>
        protected override AttributeCollection CreateAttributeCollection()
        {
            return new MultiObjectAttributes(this);
        }

        /// <summary>
        /// Gets an editor of the specified type.
        /// </summary>
        /// <param name="editorBaseType">The base type of editor, which is used to differentiate between multiple editors that a property supports.</param>
        /// <returns>An instance of the requested editor type, or <see langword="null" /> if an editor cannot be found.</returns>
        public override object GetEditor(Type editorBaseType)
        {
            return _Descriptors[0].GetEditor(editorBaseType);
        }

        /// <summary>
        /// When overridden in a derived class, returns whether resetting an object changes its value.
        /// </summary>
        /// <param name="component">The component to test for reset capability.</param>
        /// <returns><see langword="true" /> if resetting the component changes its value; otherwise, <see langword="false" />.</returns>
        public override bool CanResetValue(object component)
        {
            object[] list = (object[])component;
            for (int i = 0; i < _Descriptors.Length; i++)
            {
                if (!_Descriptors[i].CanResetValue(GetOwner(list, i)))
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
        public override bool SupportsChangeEvents { get { return _Descriptors.Any(p => p.SupportsChangeEvents); } }

        /// <summary>
        /// Enables other objects to be notified when this property changes.
        /// </summary>
        /// <param name="component">The component to add the handler for.</param>
        /// <param name="handler">The delegate to add as a listener.</param>
        public override void AddValueChanged(object component, EventHandler handler)
        {
            object[] list = (object[])component;
            for (int i = 0; i < _Descriptors.Length; i++)
            {
                if (!_Descriptors[i].SupportsChangeEvents)
                {
                    continue;
                }
                    
                _Descriptors[i].AddValueChanged(GetOwner(list, i), handler);
            }
        }

        /// <summary>
        /// Enables other objects to be notified when this property changes.
        /// </summary>
        /// <param name="component">The component to remove the handler for.</param>
        /// <param name="handler">The delegate to remove as a listener.</param>
        public override void RemoveValueChanged(object component, EventHandler handler)
        {
            object[] list = (object[])component;
            for (int i = 0; i < _Descriptors.Length; i++)
            {
                if (!_Descriptors[i].SupportsChangeEvents)
                {
                    continue;
                }

                _Descriptors[i].RemoveValueChanged(GetOwner(list, i), handler);
            }
        }

        /// <summary>
        /// When overridden in a derived class, gets the type of the component this property is bound to.
        /// </summary>
        /// <value>The type of the component.</value>
        public override Type ComponentType
        {
            get { return _Descriptors[0].ComponentType; }
        }

        /// <summary>
        /// When overridden in a derived class, gets the current value of the property on a component.
        /// </summary>
        /// <param name="component">The component with the property for which to retrieve the value.</param>
        /// <returns>The value of a property for a given component.</returns>
        public override object GetValue(object component)
        {
            object[] list = (object[])component;
            object res = GetValue(_Descriptors[0], GetOwner(list, 0));
            for (int i = 0; i < _Descriptors.Length; i++)
            {
                object temp = GetValue(_Descriptors[i], GetOwner(list, i));
                if (res != temp && res != null && !res.Equals(temp))
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
        public override bool IsReadOnly
        {
            get
            {
                for (int i = 0; i < _Descriptors.Length; i++)
                {
                    if (_Descriptors[i].IsReadOnly)
                    {
                        return true;
                    }                        
                }
                    
                return false;
            }
        }

        /// <summary>
        /// When overridden in a derived class, gets the type of the property.
        /// </summary>
        /// <value>The type of the property.</value>
        public override Type PropertyType
        {
            get { return _Descriptors[0].PropertyType; }
        }

        /// <summary>
        /// Gets the type converter for this property.
        /// </summary>
        /// <value>The converter.</value>
        public override TypeConverter Converter
        {
            get
            {
                return _Descriptors[0].Converter;
            }
        }

        /// <summary>
        /// Gets the name that can be displayed in a window, such as a Properties window.
        /// </summary>
        /// <value>The display name.</value>
        public override string DisplayName
        {
            get
            {
                return _Descriptors[0].DisplayName;
            }
        }

        /// <summary>
        /// When overridden in a derived class, resets the value for this property of the component to the default value.
        /// </summary>
        /// <param name="component">The component with the property value that is to be reset to the default value.</param>
        public override void ResetValue(object component)
        {
            object[] list = (object[])component;
            for (int i = 0; i < _Descriptors.Length; i++)
            {
                _Descriptors[i].ResetValue(GetOwner(list, i));
            }                
        }

        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <param name="components">The components.</param>
        /// <returns>System.Object[].</returns>
        public object[] GetValues(object[] components)
        {
            object[] list = new object[components.Length];
            for (int i = 0; i < _Descriptors.Length; i++)
            {
                list[i] = GetValue(_Descriptors[i], GetOwner(components, i));
            }
                
            return list;
        }

        /// <summary>
        /// When overridden in a derived class, sets the value of the component to a different value.
        /// </summary>
        /// <param name="component">The component with the property value that is to be set.</param>
        /// <param name="value">The new value.</param>
        public override void SetValue(object component, object value)
        {
            object[] list = (object[])component;
            for (int i = 0; i < _Descriptors.Length; i++)
            {
                object clonedVal = null;
                if (value is ICloneable)
                {
                    clonedVal = ((ICloneable)value).Clone();
                }
                    
                _Descriptors[i].SetValue(GetOwner(list, i), clonedVal ?? value);
            }
        }

        private static object GetValue(PropertyDescriptor pd, object component)
        {
            object value = null;
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
                value = GetUnwindedException(e).Message;
            }

            return value;
        }

        private static Exception GetUnwindedException(Exception e)
        {
            Exception result = e;
            if (e is System.Reflection.TargetInvocationException)
            {
                result = e.InnerException;
            }
                
            string message = result.Message;
            while (string.IsNullOrEmpty(message) && result.InnerException != null)
            {
                message = result.InnerException.Message;
                result = result.InnerException;
            }

            return result;
        }

        private static object GetPropertyOwner(PropertyDescriptor pd, object component)
        {
            ICustomTypeDescriptor typeDescriptor = component as ICustomTypeDescriptor;
            if (typeDescriptor == null)
            {
                return component;
            }
                
            return component = typeDescriptor.GetPropertyOwner(pd);
        }

        /// <summary>
        /// When overridden in a derived class, determines a value indicating whether the value of this property needs to be persisted.
        /// </summary>
        /// <param name="component">The component with the property to be examined for persistence.</param>
        /// <returns><see langword="true" /> if the property should be persisted; otherwise, <see langword="false" />.</returns>
        public override bool ShouldSerializeValue(object component)
        {
            object[] list = (object[])component;
            for (int i = 0; i < _Descriptors.Length; i++)
            {
                if (_Descriptors[i].ShouldSerializeValue(GetOwner(list, i)))
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
        public PropertyDescriptor this[int index]
        {
            get { return _Descriptors[index]; }
        }

        /// <summary>
        /// Sets the values.
        /// </summary>
        /// <param name="components">The components.</param>
        /// <param name="values">The values.</param>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public void SetValues(object components, object values)
        {
            object[] valuesArray = (object[])values;
            object[] componentsArray = (object[])components;

            if (valuesArray.Length != _Descriptors.Length || valuesArray.Length != componentsArray.Length)
                throw new ArgumentOutOfRangeException();

            for (int i = 0; i < _Descriptors.Length; i++)
            {
                _Descriptors[i].SetValue(componentsArray[i], valuesArray[i]);
            }                
        }

        /// <summary>
        /// Gets the name of the category to which the member belongs, as specified in the <see cref="T:System.ComponentModel.CategoryAttribute" />.
        /// </summary>
        /// <value>The category.</value>
        public override string Category { get { return _Descriptors[0].Category; } }

        #region IEnumerable
        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<PropertyDescriptor> GetEnumerator()
        {
            foreach (PropertyDescriptor descriptor in _Descriptors)
            {
                yield return descriptor;
            }
        }
        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
    }
}

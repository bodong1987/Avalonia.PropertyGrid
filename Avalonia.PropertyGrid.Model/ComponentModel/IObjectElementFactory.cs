using System;
using System.Collections.Generic;
using System.Text;

namespace Avalonia.PropertyGrid.Model.ComponentModel
{
    /// <summary>
    /// Interface IObjectElementFactory
    /// </summary>
    public interface IObjectElementFactory
    {
        /// <summary>
        /// Gets a value indicating whether this instance is cloneable.
        /// </summary>
        /// <value><c>true</c> if this instance is cloneable; otherwise, <c>false</c>.</value>
        bool IsCloneable { get; }

        /// <summary>
        /// Gets the supported types.
        /// </summary>
        /// <returns>Type[].</returns>
        Type[] GetSupportedTypes();

        /// <summary>
        /// Creates the object.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="params">The parameters.</param>
        /// <returns>System.Object.</returns>
        object CreateObject(Type type, params object[] @params);

        /// <summary>
        /// Clones the object.
        /// </summary>
        /// <param name="sourceObject">The source object.</param>
        /// <returns>System.Object.</returns>
        object CloneObject(object sourceObject);

        /// <summary>
        /// Adds the type of the supported.
        /// </summary>
        /// <param name="type">The type.</param>
        void AddSupportedType(Type type);

        /// <summary>
        /// Removes the type of the supported.
        /// </summary>
        /// <param name="type">The type.</param>
        void RemoveSupportedType(Type type);

        /// <summary>
        /// Clears the supported types.
        /// </summary>
        void ClearSupportedTypes();
    }

    /// <summary>
    /// Interface IObjectElementFactory
    /// Implements the <see cref="Avalonia.PropertyGrid.Model.ComponentModel.IObjectElementFactory" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="Avalonia.PropertyGrid.Model.ComponentModel.IObjectElementFactory" />
    public interface IObjectElementFactory<T> : IObjectElementFactory
    {
        /// <summary>
        /// Creates the object.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="params">The parameters.</param>
        /// <returns>T.</returns>
        new T CreateObject(Type type, params object[] @params);

        /// <summary>
        /// Clones the object.
        /// </summary>
        /// <param name="sourceObject">The source object.</param>
        /// <returns>T.</returns>
        T CloneObject(T sourceObject);
    }

    /// <summary>
    /// Class AbstractObjectElementFactory.
    /// Implements the <see cref="Avalonia.PropertyGrid.Model.ComponentModel.IObjectElementFactory" />
    /// </summary>
    /// <seealso cref="Avalonia.PropertyGrid.Model.ComponentModel.IObjectElementFactory" />
    public abstract class AbstractObjectElementFactory : IObjectElementFactory
    {
        readonly List<Type> SupportedTypes = new List<Type>();

        /// <summary>
        /// Gets a value indicating whether this instance is cloneable.
        /// </summary>
        /// <value><c>true</c> if this instance is cloneable; otherwise, <c>false</c>.</value>
        public virtual bool IsCloneable => false;

        /// <summary>
        /// Clones the object.
        /// </summary>
        /// <param name="sourceObject">The source object.</param>
        /// <returns>System.Object.</returns>
        public virtual object CloneObject(object sourceObject)
        {
            if(!IsCloneable)
            {
                throw new InvalidOperationException("Can't clone object when IsCloneable == false.");
            }

            return null;
        }

        /// <summary>
        /// Creates the object.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="params">The parameters.</param>
        /// <returns>System.Object.</returns>
        public abstract object CreateObject(Type type, params object[] @params);

        /// <summary>
        /// Gets the supported types.
        /// </summary>
        /// <returns>Type[].</returns>
        public virtual Type[] GetSupportedTypes()
        {
            return SupportedTypes.ToArray();
        }

        /// <summary>
        /// Adds the type of the supported.
        /// </summary>
        /// <param name="type">The type.</param>
        public void AddSupportedType(Type type)
        {
            if(!SupportedTypes.Contains(type))
            {
                SupportedTypes.Add(type);
            }            
        }

        /// <summary>
        /// Removes the type of the supported.
        /// </summary>
        /// <param name="type">The type.</param>
        public void RemoveSupportedType(Type type)
        {
            SupportedTypes.Remove(type);
        }

        /// <summary>
        /// Clears the supported types.
        /// </summary>
        public void ClearSupportedTypes()
        {
            SupportedTypes.Clear();
        }
    }

    /// <summary>
    /// Class AbstractObjectElementFactory.
    /// Implements the <see cref="Avalonia.PropertyGrid.Model.ComponentModel.AbstractObjectElementFactory" />
    /// Implements the <see cref="Avalonia.PropertyGrid.Model.ComponentModel.IObjectElementFactory{T}" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="Avalonia.PropertyGrid.Model.ComponentModel.AbstractObjectElementFactory" />
    /// <seealso cref="Avalonia.PropertyGrid.Model.ComponentModel.IObjectElementFactory{T}" />
    public abstract class AbstractObjectElementFactory<T> : AbstractObjectElementFactory, IObjectElementFactory<T>
    {
        /// <summary>
        /// Clones the object.
        /// </summary>
        /// <param name="sourceObject">The source object.</param>
        /// <returns>T.</returns>
        public T CloneObject(T sourceObject)
        {
            var obj = ((IObjectElementFactory)this).CloneObject(sourceObject);

            return obj != null ? (T)obj : default(T);
        }
        

        T IObjectElementFactory<T>.CreateObject(Type type, params object[] @params)
        {
            var obj = ((IObjectElementFactory)this).CreateObject(type, @params);

            return obj != null ? (T)obj : default(T);
        }

        /// <summary>
        /// Creates the object.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="params">The parameters.</param>
        /// <returns>System.Object.</returns>
        public override object CreateObject(Type type, params object[] @params)
        {
            return Activator.CreateInstance(type, @params);
        }
    }

    /// <summary>
    /// Class CloneableObjectElementFactory.
    /// Implements the <see cref="Avalonia.PropertyGrid.Model.ComponentModel.AbstractObjectElementFactory{T}" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="Avalonia.PropertyGrid.Model.ComponentModel.AbstractObjectElementFactory{T}" />
    public class CloneableObjectElementFactory<T> : AbstractObjectElementFactory<T> where T : ICloneable
    {
        /// <summary>
        /// Gets a value indicating whether this instance is cloneable.
        /// </summary>
        /// <value><c>true</c> if this instance is cloneable; otherwise, <c>false</c>.</value>
        public override bool IsCloneable => true;

        /// <summary>
        /// Clones the object.
        /// </summary>
        /// <param name="sourceObject">The source object.</param>
        /// <returns>System.Object.</returns>
        public override object CloneObject(object sourceObject)
        {
            if(sourceObject is T v)
            {
                return v.Clone();
            }

            return null;
        }
    }

    /// <summary>
    /// Class ObjectElementFactoryTypeAttribute.
    /// Implements the <see cref="Attribute" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="Attribute" />
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ObjectElementFactoryTypeAttribute<T> : Attribute where T : IObjectElementFactory, new()
    {
        /// <summary>
        /// The factory type
        /// </summary>
        public readonly Type FactoryType;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectElementFactoryTypeAttribute{T}"/> class.
        /// </summary>
        public ObjectElementFactoryTypeAttribute()
        {
            FactoryType = typeof(T);
        }
    }
}

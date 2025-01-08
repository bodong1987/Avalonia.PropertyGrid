using System;
using System.Collections.Generic;

namespace PropertyModels.ComponentModel.DataAnnotations
{
    /// <summary>
    /// Interface for authorizing enum values.
    /// </summary>
    public interface IEnumValueAuthorizeAttribute
    {
        /// <summary>
        /// Determines whether the specified value is allowed.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns><c>true</c> if the value is allowed; otherwise, <c>false</c>.</returns>
        bool AllowValue(object value);
    }

    /// <summary>
    /// Generic interface for authorizing enum values.
    /// </summary>
    /// <typeparam name="T">The enum type.</typeparam>
    public interface IEnumValueAuthorizeAttribute<T> : IEnumValueAuthorizeAttribute
        where T : Enum
    {
        /// <summary>
        /// Determines whether the specified enum value is allowed.
        /// </summary>
        /// <param name="value">The enum value to check.</param>
        /// <returns><c>true</c> if the value is allowed; otherwise, <c>false</c>.</returns>
        bool AllowValue(T value);
    }

    /// <summary>
    /// Attribute to specify permitted enum values.
    /// </summary>
    /// <typeparam name="T">The enum type.</typeparam>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class EnumPermitValuesAttribute<T> : Attribute, IEnumValueAuthorizeAttribute<T>
        where T : Enum
    {
        /// <summary>
        /// Gets the set of permitted values.
        /// </summary>
        public readonly HashSet<T> PermitValues;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumPermitValuesAttribute{T}"/> class.
        /// </summary>
        /// <param name="permitValues">The permitted values.</param>
        public EnumPermitValuesAttribute(params T[] permitValues)
        {
            PermitValues = new HashSet<T>(permitValues);
        }

        /// <summary>
        /// Determines whether the specified enum value is allowed.
        /// </summary>
        /// <param name="value">The enum value to check.</param>
        /// <returns><c>true</c> if the value is allowed; otherwise, <c>false</c>.</returns>
        public virtual bool AllowValue(T value)
        {
            return PermitValues.Contains(value);
        }

        bool IEnumValueAuthorizeAttribute.AllowValue(object value)
        {
            return value is T enumValue && AllowValue(enumValue);
        }
    }

    /// <summary>
    /// Attribute to specify prohibited enum values.
    /// </summary>
    /// <typeparam name="T">The enum type.</typeparam>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class EnumProhibitValuesAttribute<T> : Attribute, IEnumValueAuthorizeAttribute<T>
        where T : Enum
    {
        /// <summary>
        /// Gets the set of prohibited values.
        /// </summary>
        public readonly HashSet<T> ProhibitValues;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumProhibitValuesAttribute{T}"/> class.
        /// </summary>
        /// <param name="prohibitValues">The prohibited values.</param>
        public EnumProhibitValuesAttribute(params T[] prohibitValues)
        {
            ProhibitValues = new HashSet<T>(prohibitValues);
        }

        /// <summary>
        /// Determines whether the specified enum value is allowed.
        /// </summary>
        /// <param name="value">The enum value to check.</param>
        /// <returns><c>true</c> if the value is allowed; otherwise, <c>false</c>.</returns>
        public virtual bool AllowValue(T value)
        {
            return !ProhibitValues.Contains(value);
        }

        bool IEnumValueAuthorizeAttribute.AllowValue(object value)
        {
            return value is T enumValue && AllowValue(enumValue);
        }
    }
}
using System;
using System.Diagnostics;
using System.Reflection;

namespace PropertyModels.ComponentModel
{
    /// <summary>
    /// Class EnumDisplayNameAttribute.
    /// Implements the <see cref="Attribute" />
    /// </summary>
    /// <seealso cref="Attribute" />
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Enum)]
    public class EnumDisplayNameAttribute : Attribute
    {
        /// <summary>
        /// The display name
        /// </summary>
        public readonly string DisplayName;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumDisplayNameAttribute"/> class.
        /// </summary>
        /// <param name="displayName">The display name.</param>
        public EnumDisplayNameAttribute(string displayName)
        {
            DisplayName = displayName;
        }
    }

    /// <summary>
    /// Attribute to specify that an enum field should be excluded from the selectable list.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Enum)]
    public class EnumExcludeAttribute : Attribute
    {
    }

    /// <summary>
    /// Class EnumValueWrapper.
    /// </summary>
    public class EnumValueWrapper
    {
        /// <summary>
        /// The value
        /// </summary>
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        public Enum Value { get; set; }
        
        /// <summary>
        /// The display name
        /// </summary>
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        public string DisplayName { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumValueWrapper"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public EnumValueWrapper(Enum value)
        {
            Debug.Assert(value != null);

            Value = value;

            var fieldInfo = value.GetType().GetField(value.ToString());

            var attr = fieldInfo?.GetCustomAttribute<EnumDisplayNameAttribute>();

            DisplayName = attr?.DisplayName ?? value.ToString();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumValueWrapper"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="displayName">The display name.</param>
        public EnumValueWrapper(Enum value, string displayName) 
        {
            Debug.Assert(value != null);

            Value = value;
            DisplayName = displayName;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return DisplayName;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object? obj)
        {
            if(obj is EnumValueWrapper evw)
            {
                return Value.Equals(evw.Value);
            }

            return false;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            // ReSharper disable once NonReadonlyMemberInGetHashCode
            return Value.GetHashCode();
        }
    }
}

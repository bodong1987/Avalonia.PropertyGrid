using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace PropertyModels.ComponentModel.DataAnnotations;

/// <summary>
/// Interface for authorizing enum values.
/// </summary>
public interface IEnumValueAuthorizeAttribute
{
    /// <summary>
    /// Determines whether the specified value is allowed.
    /// </summary>
    /// <param name="enumType">Type of the enum.</param>
    /// <param name="fieldName">Name of the filed.</param>
    /// <param name="fieldValue">The field value.</param>
    /// <returns><c>true</c> if the value is allowed; otherwise, <c>false</c>.</returns>
    // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Global
    bool AllowValue(Type enumType, string fieldName, object fieldValue);
}

/// <summary>
/// Generic interface for authorizing enum values.
/// </summary>
/// <typeparam name="T">The enum type.</typeparam>
public interface IEnumValueAuthorizeAttribute<in T> : IEnumValueAuthorizeAttribute
    where T : Enum
{
    /// <summary>
    /// Determines whether the specified enum value is allowed.
    /// </summary>
    /// <param name="fieldName">Name of the filed.</param>
    /// <param name="value">The enum value to check.</param>
    /// <returns><c>true</c> if the value is allowed; otherwise, <c>false</c>.</returns>
    bool AllowValue(string fieldName, T value);
}

/// <summary>
/// Attribute to specify permitted enum values.
/// </summary>
/// <typeparam name="T">The enum type.</typeparam>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
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
    /// <param name="fieldName">Name of the filed.</param>
    /// <param name="value">The enum value to check.</param>
    /// <returns><c>true</c> if the value is allowed; otherwise, <c>false</c>.</returns>
    public virtual bool AllowValue(string fieldName, T value) => PermitValues.Contains(value);

    bool IEnumValueAuthorizeAttribute.AllowValue(Type enumType, string fieldName, object fieldValue)
    {
        Debug.Assert(enumType == typeof(T));

        return fieldValue is T enumValue && AllowValue(fieldName, enumValue);
    }
}

/// <summary>
/// Class EnumPermitNamesAttribute.
/// Implements the <see cref="Attribute" />
/// Implements the <see cref="PropertyModels.ComponentModel.DataAnnotations.IEnumValueAuthorizeAttribute" />
/// </summary>
/// <seealso cref="Attribute" />
/// <seealso cref="PropertyModels.ComponentModel.DataAnnotations.IEnumValueAuthorizeAttribute" />
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
public class EnumPermitNamesAttribute : Attribute, IEnumValueAuthorizeAttribute
{
    /// <summary>
    /// The permit names
    /// </summary>
    public readonly HashSet<string> PermitNames;

    /// <summary>
    /// Initializes a new instance of the <see cref="EnumPermitNamesAttribute"/> class.
    /// </summary>
    /// <param name="permitNames">The permit names.</param>
    public EnumPermitNamesAttribute(params string[] permitNames)
    {
        PermitNames = new HashSet<string>(permitNames);
    }

    /// <summary>
    /// Determines whether the specified value is allowed.
    /// </summary>
    /// <param name="enumType">Type of the enum.</param>
    /// <param name="fieldName">Name of the filed.</param>
    /// <param name="fieldValue">The field value.</param>
    /// <returns><c>true</c> if the value is allowed; otherwise, <c>false</c>.</returns>
    public virtual bool AllowValue(Type enumType, string fieldName, object fieldValue) => PermitNames.Contains(fieldName);
}



/// <summary>
/// Attribute to specify prohibited enum values.
/// </summary>
/// <typeparam name="T">The enum type.</typeparam>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
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
    /// <param name="fieldName">Name of the filed.</param>
    /// <param name="value">The enum value to check.</param>
    /// <returns><c>true</c> if the value is allowed; otherwise, <c>false</c>.</returns>
    public virtual bool AllowValue(string fieldName, T value) => !ProhibitValues.Contains(value);

    bool IEnumValueAuthorizeAttribute.AllowValue(Type enumType, string fieldName, object fieldValue)
    {
        Debug.Assert(enumType == typeof(T));

        return fieldValue is T enumValue && AllowValue(fieldName, enumValue);
    }
}

/// <summary>
/// Class EnumProhibitNamesAttribute.
/// Implements the <see cref="Attribute" />
/// Implements the <see cref="PropertyModels.ComponentModel.DataAnnotations.IEnumValueAuthorizeAttribute" />
/// </summary>
/// <seealso cref="Attribute" />
/// <seealso cref="PropertyModels.ComponentModel.DataAnnotations.IEnumValueAuthorizeAttribute" />
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
public class EnumProhibitNamesAttribute : Attribute, IEnumValueAuthorizeAttribute
{
    /// <summary>
    /// The prohibit names
    /// </summary>
    public readonly HashSet<string> ProhibitNames;

    /// <summary>
    /// Initializes a new instance of the <see cref="EnumProhibitNamesAttribute"/> class.
    /// </summary>
    /// <param name="prohibitNames">The prohibit names.</param>
    public EnumProhibitNamesAttribute(params string[] prohibitNames)
    {
        ProhibitNames = new HashSet<string>(prohibitNames);
    }

    /// <summary>
    /// Determines whether the specified value is allowed.
    /// </summary>
    /// <param name="enumType">Type of the enum.</param>
    /// <param name="fieldName">Name of the filed.</param>
    /// <param name="fieldValue">The field value.</param>
    /// <returns><c>true</c> if the value is allowed; otherwise, <c>false</c>.</returns>
    public virtual bool AllowValue(Type enumType, string fieldName, object fieldValue) => !ProhibitNames.Contains(fieldName);
}
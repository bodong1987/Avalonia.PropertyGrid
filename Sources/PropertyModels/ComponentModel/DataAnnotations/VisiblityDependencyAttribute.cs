using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace PropertyModels.ComponentModel.DataAnnotations;

/// <summary>
/// Class ConditionTargetAttribute.
/// Implements the <see cref="Attribute" />
/// </summary>
/// <seealso cref="Attribute" />
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class ConditionTargetAttribute : Attribute;

/// <summary>
/// base class for property, used to set property's visibility in PropertyGrid
/// Implements the <see cref="Attribute" />
/// </summary>
/// <seealso cref="Attribute" />
[AttributeUsage(AttributeTargets.Property)]
public abstract class AbstractVisibilityConditionAttribute : Attribute
{
    /// <summary>
    /// Checks the visibility.
    /// </summary>
    /// <param name="component">The component.</param>
    /// <returns><c>true</c> if visible, <c>false</c> otherwise.</returns>
    public abstract bool CheckVisibility(object? component);
}

/// <summary>
/// Enum ConditionLogicType
/// </summary>
public enum ConditionLogicType
{
    /// <summary>
    /// default
    /// </summary>
    Default,
    /// <summary>
    /// not
    /// </summary>
    Not,
    /// <summary>
    /// and
    /// </summary>
    And,
    /// <summary>
    /// or
    /// </summary>
    Or
}


/// <summary>
/// Class PropertyVisibilityConditionAttribute.
/// Implements the <see cref="PropertyModels.ComponentModel.DataAnnotations.AbstractVisibilityConditionAttribute" />
/// </summary>
/// <seealso cref="PropertyModels.ComponentModel.DataAnnotations.AbstractVisibilityConditionAttribute" />
public class PropertyVisibilityConditionAttribute : AbstractVisibilityConditionAttribute
{
    /// <summary>
    /// The property name
    /// </summary>
    public readonly string PropertyName;

    /// <summary>
    /// The visible value
    /// </summary>
    public readonly object VisibleValue;

    /// <summary>
    /// Gets or sets the type of the logic.
    /// </summary>
    /// <value>The type of the logic.</value>
    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    public ConditionLogicType LogicType { get; set; } = ConditionLogicType.Default;

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyVisibilityConditionAttribute"/> class.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="visibleValue">The visible value.</param>
    public PropertyVisibilityConditionAttribute(string propertyName, object visibleValue)
    {
        PropertyName = propertyName;
        VisibleValue = visibleValue;
    }

    /// <summary>
    /// Checks the visibility.
    /// </summary>
    /// <param name="component">The component.</param>
    /// <returns><c>true</c> if visible, <c>false</c> otherwise.</returns>
    public override bool CheckVisibility(object? component)
    {
        if (component == null)
        {
            return false;
        }

        if (component is ICustomTypeDescriptor ctd)
        {
            var pd = ctd.GetProperties().Find(PropertyName, true);

            return IsVisible(pd?.GetValue(component));
        }

        var property = component.GetType().GetProperty(PropertyName, BindingFlags.Public|BindingFlags.Instance|BindingFlags.NonPublic);

        return IsVisible(property?.GetValue(component));
    }

    private bool IsVisible(object? value)
    {
        var isEqual = EqualityComparer<object>.Default.Equals(VisibleValue, value!);

        return LogicType == ConditionLogicType.Default ? isEqual : !isEqual;
    }
}
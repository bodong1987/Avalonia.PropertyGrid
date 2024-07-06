using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace PropertyModels.ComponentModel.DataAnnotations;

/// <summary>
/// Class ConditionTargetAttribute.
/// Implements the <see cref="Attribute" />
/// </summary>
/// <seealso cref="Attribute" />
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class ConditionTargetAttribute : Attribute
{
}

/// <summary>
/// base class for property, used to set property's visibility in PropertyGrid
/// Implements the <see cref="Attribute" />
/// </summary>
/// <seealso cref="Attribute" />
[AttributeUsage(AttributeTargets.Property)]
public abstract class AbstractVisiblityConditionAttribute : Attribute
{
    /// <summary>
    /// Checks the visibility.
    /// </summary>
    /// <param name="component">The component.</param>
    /// <returns><c>true</c> if success, <c>false</c> otherwise.</returns>
    public abstract bool CheckVisibility(object component);
}

/// <summary>
/// Enum ConditionLogicType
/// </summary>
public enum ConditionLogicType
{
    /// <summary>
    /// The default
    /// </summary>
    Default,
    /// <summary>
    /// The not
    /// </summary>
    Not,
    /// <summary>
    /// The and
    /// </summary>
    And,
    /// <summary>
    /// The or
    /// </summary>
    Or
}


/// <summary>
/// Class VisibilityPropertyConditionAttribute.
/// Implements the <see cref="PropertyModels.ComponentModel.DataAnnotations.AbstractVisiblityConditionAttribute" />
/// </summary>
/// <seealso cref="PropertyModels.ComponentModel.DataAnnotations.AbstractVisiblityConditionAttribute" />
public class VisibilityPropertyConditionAttribute : AbstractVisiblityConditionAttribute
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
    public ConditionLogicType LogicType { get; set; } = ConditionLogicType.Default;

    /// <summary>
    /// Initializes a new instance of the <see cref="VisibilityPropertyConditionAttribute"/> class.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="visibleValue">The visible value.</param>
    public VisibilityPropertyConditionAttribute(string propertyName, object visibleValue) 
    {
        PropertyName = propertyName;
        VisibleValue = visibleValue;
    }

    /// <summary>
    /// Checks the visibility.
    /// </summary>
    /// <param name="component">The component.</param>
    /// <returns><c>true</c> if success, <c>false</c> otherwise.</returns>
    public override bool CheckVisibility(object component)
    {
        if(component == null)
        {
            return false;
        }

        if(component is ICustomTypeDescriptor ctd)
        {
            var pd = ctd.GetProperties().Find(PropertyName, true);

            return IsVisible(pd?.GetValue(component));
        }
        else
        {
            var property = component.GetType().GetProperty(PropertyName);

            return IsVisible(property?.GetValue(component));
        }
    }

    private bool IsVisible(object value)
    {
        bool isEqual = EqualityComparer<object>.Default.Equals(VisibleValue, value);

        return LogicType == ConditionLogicType.Default ? isEqual : !isEqual;
    }
}
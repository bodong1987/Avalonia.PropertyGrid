using System;

namespace PropertyModels.ComponentModel;

/// <summary>
/// set property extra operation visibility
/// </summary>
public enum PropertyOperationVisibility
{
    /// <summary>
    /// Default
    /// </summary>
    Default,
    
    /// <summary>
    /// All Visible
    /// </summary>
    Visible,
        
    /// <summary>
    /// All Hidden
    /// </summary>
    Hidden
}

/// <summary>
/// set property operation visibility
/// </summary>
[AttributeUsage(AttributeTargets.Field|AttributeTargets.Property)]
public class PropertyOperationVisibilityAttribute : Attribute
{
    /// <summary>
    /// visibility
    /// </summary>
    public readonly PropertyOperationVisibility Visibility;
    
    /// <summary>
    /// constructor 
    /// </summary>
    /// <param name="visibility"></param>
    public PropertyOperationVisibilityAttribute(PropertyOperationVisibility visibility = PropertyOperationVisibility.Default)
    {
        Visibility = visibility;    
    }
}
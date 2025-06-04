using System;

namespace PropertyModels.ComponentModel;

/// <summary>
/// instead of bool? in attribute
/// </summary>
public enum NullableBooleanType
{
    /// <summary>
    /// bool?
    /// </summary>
    Undefined = 0,
    /// <summary>
    /// Yes
    /// </summary>
    Yes = 1,
    /// <summary>
    /// No
    /// </summary>
    No = -1
}

/// <summary>
/// used to custom expandable object display mode
/// </summary>
[AttributeUsage(AttributeTargets.All)]
public class ExpandableObjectDisplayModeAttribute : Attribute
{
    /// <summary>
    /// use category mode ?
    /// null means use root config
    /// </summary>
    public NullableBooleanType IsCategoryVisible { get; set; }
    
    /// <summary>
    /// is in tree view mode
    /// null means use root config
    /// </summary>
    public NullableBooleanType IsTreeMode { get; set; }
    
    /// <summary>
    /// if category use builtin order/ only valid when IsCategoryVisible
    /// null means use root config
    /// </summary>
    public NullableBooleanType IsCategoryBuiltinOrder { get; set; }
    
    /// <summary>
    /// if property use builtin order
    /// null means use root config
    /// </summary>
    public NullableBooleanType IsPropertyBuiltinOrder { get; set; }
}
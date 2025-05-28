using System;

namespace PropertyModels.ComponentModel;

/// <summary>
/// checked list display mode
/// specify the show mode of the elements in items  
/// </summary>
public enum CheckedListDisplayMode
{
    /// <summary>
    /// same as AutoWrap/ like grid layout/ WrapPanel
    /// </summary>
    Default,
    /// <summary>
    /// one line first, auto start new line
    /// </summary>
    AutoWrap,
    
    /// <summary>
    /// one element in a row
    /// </summary>
    Vertical,
    
    /// <summary>
    /// always in one row
    /// </summary>
    Horizontal
}

/// <summary>
/// set property display mode
/// </summary>
[AttributeUsage(AttributeTargets.Property|AttributeTargets.Field)]
public class CheckedListDisplayModeAttribute : Attribute
{
    /// <summary>
    /// display mode for checked list
    /// </summary>
    public readonly CheckedListDisplayMode DisplayMode;

    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="displayMode"></param>
    public CheckedListDisplayModeAttribute(CheckedListDisplayMode displayMode)
    {
        DisplayMode = displayMode;
    }
}
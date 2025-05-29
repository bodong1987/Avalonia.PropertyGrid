using System;

namespace PropertyModels.ComponentModel;

/// <summary>
/// selectable list display mode
/// specify the show mode of the elements in items  
/// </summary>
public enum SelectableListDisplayMode
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
public class SelectableListDisplayModeAttribute : Attribute
{
    /// <summary>
    /// display mode for checked list
    /// </summary>
    public readonly SelectableListDisplayMode DisplayMode;

    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="displayMode"></param>
    public SelectableListDisplayModeAttribute(SelectableListDisplayMode displayMode)
    {
        DisplayMode = displayMode;
    }
}
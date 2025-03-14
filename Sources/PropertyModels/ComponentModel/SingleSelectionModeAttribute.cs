using System;

namespace PropertyModels.ComponentModel;

/// <summary>
/// selection mode, used for normal enum type or ISelectableList
/// </summary>
public enum SingleSelectionMode
{
    /// <summary>
    /// default
    /// </summary>
    Default,
    /// <summary>
    /// default/combobox
    /// </summary>
    ComboBox = Default,
    /// <summary>
    /// radio 
    /// </summary>
    RadioButton,
    /// <summary>
    /// toggle button group
    /// </summary>
    ToggleButtonGroup,
}

/// <summary>
/// set select mode
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class SingleSelectionModeAttribute : Attribute
{
    /// <summary>
    /// Mode
    /// </summary>
    public readonly SingleSelectionMode Mode;

    /// <summary>
    /// construct this instance
    /// </summary>
    /// <param name="mode"></param>
    public SingleSelectionModeAttribute(SingleSelectionMode mode)
    {
        Mode = mode;
    }
}
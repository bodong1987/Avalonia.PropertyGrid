using System;
using Avalonia.PropertyGrid.ViewModels;
using PropertyModels.ComponentModel;

namespace Avalonia.PropertyGrid.Utils;

/// <summary>
/// Helper class for providing enum values related to PropertyGrid.
/// </summary>
public static class PropertyGridEnumValuesHelper
{
    /// <summary>
    /// All values of the PropertyOperationVisibility enum.
    /// </summary>
    public static readonly Array PropertyOperationVisibilityValues = Enum.GetValues(typeof(PropertyOperationVisibility));

    /// <summary>
    /// All values of the CellEditAlignmentType enum.
    /// </summary>
    public static readonly Array CellEditAlignmentTypeValues = Enum.GetValues(typeof(CellEditAlignmentType));

    /// <summary>
    /// All values of the PropertyGridLayoutStyle enum.
    /// </summary>
    public static readonly Array PropertyGridLayoutStyleValues = Enum.GetValues(typeof(PropertyGridLayoutStyle));

    /// <summary>
    /// All values of the PropertyGridOrderStyle enum.
    /// </summary>
    public static readonly Array PropertyGridOrderStyleValues = Enum.GetValues(typeof(PropertyGridOrderStyle));
}
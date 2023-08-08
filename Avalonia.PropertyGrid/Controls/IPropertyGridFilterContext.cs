using PropertyModels.ComponentModel;
using Avalonia.PropertyGrid.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avalonia.PropertyGrid.Controls
{
    /// <summary>
    /// Enum FilterCategory
    /// </summary>
    [Flags]
    public enum FilterCategory
    {
        /// <summary>
        /// The none
        /// </summary>
        None = 0,
        /// <summary>
        /// The property condition
        /// </summary>
        PropertyCondition = 1 << 0,
        /// <summary>
        /// The category
        /// </summary>
        Category = 1 << 1,
        /// <summary>
        /// The filter
        /// </summary>
        Filter = 1 << 2,
        /// <summary>
        /// The factory
        /// </summary>
        Factory = 1 << 4,

        /// <summary>
        /// The default
        /// </summary>
        Default = PropertyCondition | Category | Filter | Factory
    }


    /// <summary>
    /// Interface IPropertyGridFilterContext
    /// </summary>
    public interface IPropertyGridFilterContext
    {
        /// <summary>
        /// Gets the filter pattern.
        /// </summary>
        /// <value>The filter pattern.</value>
        IFilterPattern FilterPattern { get; }

        /// <summary>
        /// Gets the fast filter pattern.
        /// </summary>
        /// <value>The fast filter pattern.</value>
        ICheckedMaskModel FastFilterPattern { get; }

        /// <summary>
        /// Propagates the visibility.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="category">The category.</param>
        /// <returns>PropertyVisibility.</returns>
        PropertyVisibility PropagateVisibility(IPropertyGridCellInfo info, FilterCategory category = FilterCategory.Default);
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Avalonia.PropertyGrid.Model.ComponentModel
{
    /// <summary>
    /// Interface IFilterPattern
    /// Implements the <see cref="Avalonia.PropertyGrid.Model.ComponentModel.IReactiveObject" />
    /// </summary>
    /// <seealso cref="Avalonia.PropertyGrid.Model.ComponentModel.IReactiveObject" />
    public interface IFilterPattern : IReactiveObject
    {
        /// <summary>
        /// Gets or sets the filter text.
        /// </summary>
        /// <value>The filter text.</value>
        string FilterText { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [use regex].
        /// </summary>
        /// <value><c>true</c> if [use regex]; otherwise, <c>false</c>.</value>
        bool UseRegex { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [ignore case].
        /// </summary>
        /// <value><c>true</c> if [ignore case]; otherwise, <c>false</c>.</value>
        bool IgnoreCase { get; set; }

        /// <summary>
        /// Gets the quick filter.
        /// </summary>
        /// <value>The quick filter.</value>
        ICheckedMaskModel QuickFilter { get; }

        /// <summary>
        /// Matches the specified property descriptor.
        /// </summary>
        /// <param name="propertyDescriptor">The property descriptor.</param>
        /// <param name="context">The context.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool Match(PropertyDescriptor propertyDescriptor, object context);
    }
}

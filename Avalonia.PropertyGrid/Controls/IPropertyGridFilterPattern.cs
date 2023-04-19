using Avalonia.PropertyGrid.Model.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.PropertyGrid.Controls
{
    /// <summary>
    /// Interface IPropertyGridFilterPattern
    /// </summary>
    public interface IPropertyGridFilterPattern : IReactiveObject
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
        /// Matches the specified property descriptor.
        /// </summary>
        /// <param name="propertyDescriptor">The property descriptor.</param>
        /// <param name="context">The context.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool Match(PropertyDescriptor propertyDescriptor, object context);
    }
}

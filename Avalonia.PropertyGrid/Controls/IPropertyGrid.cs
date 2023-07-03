using System;
using System.Collections.Generic;
using System.Text;

namespace Avalonia.PropertyGrid.Controls
{
    /// <summary>
    /// Interface IPropertyGrid
    /// </summary>
    public interface IPropertyGrid
    {
        /// <summary>
        /// Gets the cell edit factory collection.
        /// </summary>
        /// <returns>ICellEditFactoryCollection.</returns>
        ICellEditFactoryCollection GetCellEditFactoryCollection();
    }
}

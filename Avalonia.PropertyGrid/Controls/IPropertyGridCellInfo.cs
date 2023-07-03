using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Avalonia.PropertyGrid.Controls
{
    /// <summary>
    /// Interface IPropertyGridCellInfoContainer
    /// </summary>
    public interface IPropertyGridCellInfoContainer
    {
        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <value>The children.</value>
        IPropertyGridCellInfo[] Children { get; }

        /// <summary>
        /// Adds the specified cell information.
        /// </summary>
        /// <param name="cellInfo">The cell information.</param>
        void Add(IPropertyGridCellInfo cellInfo);

        /// <summary>
        /// Removes the specified cell information.
        /// </summary>
        /// <param name="cellInfo">The cell information.</param>
        void Remove(IPropertyGridCellInfo cellInfo);

        /// <summary>
        /// Clears this instance.
        /// </summary>
        void Clear();
    }

    /// <summary>
    /// Enum PropertyGridCellType
    /// </summary>
    public enum PropertyGridCellType
    {
        /// <summary>
        /// The category
        /// </summary>
        Category,
        /// <summary>
        /// The cell
        /// </summary>
        Cell
    }

    /// <summary>
    /// Interface IPropertyGridCellInfo
    /// </summary>
    public interface IPropertyGridCellInfo : IPropertyGridCellInfoContainer
    {
        /// <summary>
        /// Gets the reference path.
        /// </summary>
        /// <value>The reference path.</value>
        string ReferencePath { get; }

        /// <summary>
        /// Gets the type of the cell.
        /// </summary>
        /// <value>The type of the cell.</value>
        PropertyGridCellType CellType { get; }

        /// <summary>
        /// Gets the name control.
        /// </summary>
        /// <value>The name control.</value>
        Control NameControl { get; }

        /// <summary>
        /// Gets the cell edit.
        /// </summary>
        /// <value>The cell edit.</value>
        Control CellEdit { get; }

        /// <summary>
        /// Gets the property.
        /// </summary>
        /// <value>The property.</value>
        PropertyDescriptor Property { get; }

        /// <summary>
        /// Gets the category.
        /// </summary>
        /// <value>The category.</value>
        string Category { get; }

        /// <summary>
        /// Gets the owner object.
        /// </summary>
        /// <value>The owner object.</value>
        object OwnerObject { get; }

        /// <summary>
        /// Gets the target.
        /// </summary>
        /// <value>The target.</value>
        object Target { get; }

        /// <summary>
        /// Gets the container.
        /// </summary>
        /// <value>The container.</value>
        Expander Container { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is visible.
        /// </summary>
        /// <value><c>true</c> if this instance is visible; otherwise, <c>false</c>.</value>
        bool IsVisible { get; set; }
    }

    /// <summary>
    /// Interface IPropertyGridCellInfoCache
    /// </summary>
    public interface IPropertyGridCellInfoCache : IPropertyGridCellInfoContainer
    {        
    }
}

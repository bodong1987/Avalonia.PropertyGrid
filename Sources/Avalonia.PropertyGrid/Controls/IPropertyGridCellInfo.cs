using System;
using Avalonia.Controls;

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
        /// Gets the context.
        /// </summary>
        /// <value>The context.</value>
        PropertyCellContext? Context { get; }

        /// <summary>
        /// Gets the reference path.
        /// </summary>
        /// <value>The reference path.</value>
        string? ReferencePath { get; }

        /// <summary>
        /// Gets the type of the cell.
        /// </summary>
        /// <value>The type of the cell.</value>
        PropertyGridCellType CellType { get; }

        /// <summary>
        /// Gets the name control.
        /// </summary>
        /// <value>The name control.</value>
        Control? NameControl { get; }

        /// <summary>
        /// Gets the category.
        /// </summary>
        /// <value>The category.</value>
        string? Category { get; }

        /// <summary>
        /// Gets the owner object.
        /// </summary>
        /// <value>The owner object.</value>
        object? OwnerObject { get; }

        /// <summary>
        /// Gets the target.
        /// </summary>
        /// <value>The target.</value>
        object? Target { get; }

        /// <summary>
        /// Gets the container.
        /// </summary>
        /// <value>The container.</value>
        Expander? Container { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is visible.
        /// </summary>
        /// <value><c>true</c> if this instance is visible; otherwise, <c>false</c>.</value>
        bool IsVisible { get; set; }

        /// <summary>
        /// Occurs when [cell property changed].
        /// </summary>
        event EventHandler<CellPropertyChangedEventArgs>? CellPropertyChanged;

        /// <summary>
        /// Binds this instance.
        /// </summary>
        void AddPropertyChangedObserver();

        /// <summary>
        /// Removes the property changed observer.
        /// </summary>
        void RemovePropertyChangedObserver();
    }

    /// <summary>
    /// Class CellPropertyChangedEventArgs.
    /// Implements the <see cref="EventArgs" />
    /// </summary>
    /// <seealso cref="EventArgs" />
    public class CellPropertyChangedEventArgs : EventArgs
    {
        /// <summary>
        /// The cell
        /// </summary>
        public readonly IPropertyGridCellInfo Cell;

        /// <summary>
        /// Initializes a new instance of the <see cref="CellPropertyChangedEventArgs"/> class.
        /// </summary>
        /// <param name="cell">The cell.</param>
        public CellPropertyChangedEventArgs(IPropertyGridCellInfo cell) => Cell = cell;
    }

    /// <summary>
    /// Interface IPropertyGridCellInfoCache
    /// </summary>
    public interface IPropertyGridCellInfoCache : IPropertyGridCellInfoContainer;
}

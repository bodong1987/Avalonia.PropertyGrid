using System;
using System.ComponentModel;
using Avalonia.Interactivity;
using Avalonia.PropertyGrid.ViewModels;
using PropertyModels.ComponentModel;
using INotifyPropertyChanged = System.ComponentModel.INotifyPropertyChanged;

namespace Avalonia.PropertyGrid.Controls
{
    /// <summary>
    /// Interface IPropertyGrid
    /// </summary>
    public interface IPropertyGrid: INotifyPropertyChanged, IDisposable
    {
        /// <summary>
        /// Gets the cell edit factory collection.
        /// </summary>
        /// <returns>ICellEditFactoryCollection.</returns>
        ICellEditFactoryCollection GetCellEditFactoryCollection();

        /// <summary>
        /// Clones the property grid.
        /// </summary>
        /// <returns>IPropertyGrid.</returns>
        IPropertyGrid ClonePropertyGrid();

        /// <summary>
        /// Gets the expandable object cache.
        /// </summary>
        /// <returns>IExpandableObjectCache.</returns>
        IExpandableObjectCache GetExpandableObjectCache();

        /// <summary>
        /// Gets the cell information cache.
        /// </summary>
        /// <returns>IPropertyGridCellInfoCache.</returns>
        IPropertyGridCellInfoCache GetCellInfoCache();
        
        /// <summary>
        /// Expand all categories
        /// </summary>
        void ExpandAllCategories();
        
        /// <summary>
        /// collapse all categories
        /// </summary>
        void CollapseAllCategories();

        /// <summary>
        /// Gets or sets a value indicating whether [show title].
        /// </summary>
        /// <value><c>true</c> if [show title]; otherwise, <c>false</c>.</value>
        bool IsTitleVisible { get; set; }

        /// <summary>
        /// Gets or sets Is Readonly flag
        /// </summary>
        /// <value><c>true</c> if [readonly]; otherwise, <c>false</c>.</value>
        bool IsReadOnly { get; set; }

        /// <summary>
        /// Gets or sets the display mode.
        /// </summary>
        /// <value>The display mode.</value>
        PropertyGridDisplayMode DisplayMode { get; set; }
        
        /// <summary>
        /// is header visible, include search box, options, quick filter panel
        /// </summary>
        bool IsHeaderVisible { get; set; }

        /// <summary>
        /// Gets or sets is category visible.
        /// </summary>
        /// <value>is category visible.</value>
        bool IsCategoryVisible { get; set; }
        
        /// <summary>
        /// Gets or sets the order of category
        /// </summary>
        PropertyGridOrderStyle CategoryOrderStyle { get; set; }

        /// <summary>
        /// Gets or sets the order of properties
        /// </summary>
        PropertyGridOrderStyle PropertyOrderStyle { get; set; }

        /// <summary>
        /// Gets or sets the width of the name.
        /// </summary>
        /// <value>The width of the name.</value>
        double NameWidth { get; set; }

        /// <summary>
        /// Gets or sets the root property grid.
        /// </summary>
        /// <value>The root property grid.</value>
        IPropertyGrid? RootPropertyGrid { get; set; }

        /// <summary>
        /// Occurs when [command executing].
        /// </summary>
        event EventHandler<RoutedCommandExecutingEventArgs> CommandExecuting;

        /// <summary>
        /// Occurs when [command executed].
        /// </summary>
        event EventHandler<RoutedCommandExecutedEventArgs> CommandExecuted;
    }

    /// <summary>
    /// Interface IExpandableObjectCache
    /// </summary>
    public interface IExpandableObjectCache
    {
        /// <summary>
        /// Adds the specified target.
        /// </summary>
        /// <param name="target">The target.</param>
        void Add(object? target);

        /// <summary>
        /// Removes the specified target.
        /// </summary>
        /// <param name="target">The target.</param>
        void Remove(object? target);

        /// <summary>
        /// Clears this instance.
        /// </summary>
        void Clear();

        /// <summary>
        /// Determines whether the specified target is exists.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns><c>true</c> if the specified target is exists; otherwise, <c>false</c>.</returns>
        bool IsExists(object? target);

        /// <summary>
        /// Merges the specified cache.
        /// </summary>
        /// <param name="cache">The cache.</param>
        void Merge(IExpandableObjectCache cache);
    }

    /// <summary>
    /// Class RoutedCommandExecutedEventArgs.
    /// Implements the <see cref="RoutedEventArgs" />
    /// </summary>
    /// <seealso cref="RoutedEventArgs" />
    public class RoutedCommandExecutedEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// The command
        /// </summary>
        public readonly ICancelableCommand Command;

        /// <summary>
        /// The target
        /// </summary>
        public readonly object Target;

        /// <summary>
        /// The property
        /// </summary>
        public readonly PropertyDescriptor Property;

        /// <summary>
        /// The old value
        /// </summary>
        public readonly object? OldValue;

        /// <summary>
        /// Creates new value.
        /// </summary>
        public readonly object? NewValue;

        /// <summary>
        /// The context
        /// </summary>
        public readonly object? Context;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoutedCommandExecutedEventArgs"/> class.
        /// </summary>
        /// <param name="routedEvent">The routed event.</param>
        /// <param name="command">The command.</param>
        /// <param name="target">The target.</param>
        /// <param name="property">The property.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        /// <param name="context">The context.</param>
        public RoutedCommandExecutedEventArgs(
            RoutedEvent routedEvent,
            ICancelableCommand command,
            object target,
            PropertyDescriptor property,
            object? oldValue,
            object? newValue,
            object? context)
            : base(routedEvent)
        {
            Command = command;
            Target = target;
            Property = property;
            OldValue = oldValue;
            NewValue = newValue;
            Context = context;
        }
    }

    /// <summary>
    /// Class RoutedCommandExecutingEventArgs.
    /// Implements the <see cref="Avalonia.PropertyGrid.Controls.RoutedCommandExecutedEventArgs" />
    /// </summary>
    /// <seealso cref="Avalonia.PropertyGrid.Controls.RoutedCommandExecutedEventArgs" />
    public class RoutedCommandExecutingEventArgs : RoutedCommandExecutedEventArgs
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="RoutedCommandExecutingEventArgs"/> is canceled.
        /// </summary>
        /// <value><c>true</c> if canceled; otherwise, <c>false</c>.</value>
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        public bool Canceled { get; set; } = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoutedCommandExecutingEventArgs"/> class.
        /// </summary>
        /// <param name="routedEvent">The routed event.</param>
        /// <param name="command">The command.</param>
        /// <param name="target">The target.</param>
        /// <param name="property">The property.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        /// <param name="context">The context.</param>
        public RoutedCommandExecutingEventArgs(
            RoutedEvent routedEvent,
            ICancelableCommand command,
            object target,
            PropertyDescriptor property,
            object? oldValue,
            object? newValue,
            object? context)
            : base(routedEvent, command, target, property, oldValue, newValue, context)
        {
        }
    }
}

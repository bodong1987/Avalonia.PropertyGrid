using Avalonia.PropertyGrid.ViewModels;
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
        /// Gets or sets a value indicating whether [show title].
        /// </summary>
        /// <value><c>true</c> if [show title]; otherwise, <c>false</c>.</value>
        bool ShowTitle { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [allow filter].
        /// </summary>
        /// <value><c>true</c> if [allow filter]; otherwise, <c>false</c>.</value>
        bool AllowFilter { get; set; }

        /// <summary>
        /// Gets or sets the show style.
        /// </summary>
        /// <value>The show style.</value>
        PropertyGridShowStyle ShowStyle { get; set; }

        /// <summary>
        /// Gets or sets the selected object.
        /// </summary>
        /// <value>The selected object.</value>
        object SelectedObject { get; set; }
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
        void Add(object target);

        /// <summary>
        /// Removes the specified target.
        /// </summary>
        /// <param name="target">The target.</param>
        void Remove(object target);

        /// <summary>
        /// Clears this instance.
        /// </summary>
        void Clear();

        /// <summary>
        /// Determines whether the specified target is exists.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns><c>true</c> if the specified target is exists; otherwise, <c>false</c>.</returns>
        bool IsExists(object target);
    }
}

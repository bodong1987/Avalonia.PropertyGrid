using Avalonia.Controls;
using Avalonia.PropertyGrid.Model.ComponentModel;
using Avalonia.PropertyGrid.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.PropertyGrid.Controls
{
    /// <summary>
    /// Interface ICellEditFactory
    /// </summary>
    public interface ICellEditFactory
    {
        /// <summary>
        /// Gets the import priority.
        /// The larger the value, the earlier the object will be processed
        /// </summary>
        /// <value>The import priority.</value>
        [Browsable(false)]
        int ImportPriority { get; }

        /// <summary>
        /// Gets the collection.
        /// </summary>
        /// <value>The collection.</value>
        [Browsable(false)]
        ICellEditFactoryCollection Collection { get; internal set; }

        /// <summary>
        /// Check available for target
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool Accept(object accessToken);

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>ICellEditFactory.</returns>
        ICellEditFactory Clone();

        /// <summary>
        /// Handles the new property.
        /// </summary>
        /// <param name="rootPropertyGrid">The root property grid.</param>
        /// <param name="target">The target.</param>
        /// <param name="propertyDescriptor">The property descriptor.</param>
        /// <returns>Control.</returns>
        Control HandleNewProperty(IPropertyGrid rootPropertyGrid, object target, PropertyDescriptor propertyDescriptor);

        /// <summary>
        /// Handles the property changed.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="propertyDescriptor">The property descriptor.</param>
        /// <param name="control">The control.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool HandlePropertyChanged(object target, PropertyDescriptor propertyDescriptor, Control control);

        /// <summary>
        /// Handles the property visibility.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="propertyDescriptor">The property descriptor.</param>
        /// <param name="control">The control.</param>
        /// <param name="filterPattern">The filter pattern.</param>
        /// <param name="categoryFilter">The category filter.</param>
        /// <returns>PropertyVisibility.</returns>
        PropertyVisibility? HandlePropertyVisibility(object target, PropertyDescriptor propertyDescriptor, Control control, IFilterPattern filterPattern, ICheckedMaskModel categoryFilter);
    }
}

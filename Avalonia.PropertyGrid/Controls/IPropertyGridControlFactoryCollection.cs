using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.PropertyGrid.Controls
{
    /// <summary>
    /// Interface IPropertyGridControlFactoryCollection
    /// </summary>
    public interface IPropertyGridControlFactoryCollection
    {
        /// <summary>
        /// Gets the factories.
        /// </summary>
        /// <value>The factories.</value>
        IEnumerable<IPropertyGridControlFactory> Factories { get; }

        /// <summary>
        /// Clones the factories.
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        /// <returns>IEnumerable&lt;IPropertyGridControlFactory&gt;.</returns>
        IEnumerable<IPropertyGridControlFactory> CloneFactories(object accessToken);

        /// <summary>
        /// Adds the factory.
        /// </summary>
        /// <param name="factory">The factory.</param>
        void AddFactory(IPropertyGridControlFactory factory);

        /// <summary>
        /// Removes the factory.
        /// </summary>
        /// <param name="factory">The factory.</param>
        void RemoveFactory(IPropertyGridControlFactory factory);

        /// <summary>
        /// Builds the property control.
        /// </summary>
        /// <param name="component">The component.</param>
        /// <param name="propertyDescriptor">The property descriptor.</param>
        /// <param name="factory">The factory.</param>
        /// <returns>Control.</returns>
        Control BuildPropertyControl(object component, PropertyDescriptor propertyDescriptor, out IPropertyGridControlFactory factory);
    }
}

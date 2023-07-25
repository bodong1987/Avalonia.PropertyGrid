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
    /// Interface ICellEditFactoryCollection
    /// </summary>
    public interface ICellEditFactoryCollection
    {
        /// <summary>
        /// Gets the factories.
        /// </summary>
        /// <value>The factories.</value>
        IEnumerable<ICellEditFactory> Factories { get; }

        /// <summary>
        /// Clones the factories.
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        /// <returns>IEnumerable&lt;ICellEditFactory&gt;.</returns>
        IEnumerable<ICellEditFactory> CloneFactories(object accessToken);

        /// <summary>
        /// Adds the factory.
        /// </summary>
        /// <param name="factory">The factory.</param>
        void AddFactory(ICellEditFactory factory);

        /// <summary>
        /// Removes the factory.
        /// </summary>
        /// <param name="factory">The factory.</param>
        void RemoveFactory(ICellEditFactory factory);

        /// <summary>
        /// Builds the property control.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>Control.</returns>
        Control BuildPropertyControl(PropertyCellContext context);
    }
}

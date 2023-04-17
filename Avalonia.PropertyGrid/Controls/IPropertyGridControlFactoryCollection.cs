using System;
using System.Collections.Generic;
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
        /// Gets the factories.
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        /// <returns>IEnumerable&lt;IPropertyGridControlFactory&gt;.</returns>
        IEnumerable<IPropertyGridControlFactory> GetFactories(object accessToken);

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
    }
}

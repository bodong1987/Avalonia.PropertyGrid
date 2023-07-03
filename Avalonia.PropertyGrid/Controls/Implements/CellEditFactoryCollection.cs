using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.PropertyGrid.Controls.Implements
{
    /// <summary>
    /// Class CellEditFactoryCollection.
    /// Implements the <see cref="Avalonia.PropertyGrid.Controls.ICellEditFactoryCollection" />
    /// </summary>
    /// <seealso cref="Avalonia.PropertyGrid.Controls.ICellEditFactoryCollection" />
    internal class CellEditFactoryCollection : ICellEditFactoryCollection
    {
        /// <summary>
        /// The factories
        /// </summary>
        readonly List<ICellEditFactory> _Factories = new List<ICellEditFactory>();

        /// <summary>
        /// Gets the factories.
        /// </summary>
        /// <value>The factories.</value>
        public IEnumerable<ICellEditFactory> Factories => _Factories.ToArray();

        /// <summary>
        /// Initializes a new instance of the <see cref="CellEditFactoryCollection"/> class.
        /// </summary>
        public CellEditFactoryCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CellEditFactoryCollection"/> class.
        /// </summary>
        /// <param name="factories">The factories.</param>
        public CellEditFactoryCollection(IEnumerable<ICellEditFactory> factories)
        {            
            _Factories.AddRange(factories);
            _Factories.Sort((x, y) =>
            {
                return Comparer<int>.Default.Compare(y.ImportPriority, x.ImportPriority);
            });

            foreach (var factory in _Factories)
            {
                factory.Collection = this;
            }

        }

        /// <summary>
        /// Clones the factories.
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        /// <returns>IEnumerable&lt;ICellEditFactory&gt;.</returns>
        public IEnumerable<ICellEditFactory> CloneFactories(object accessToken)
        {
            return _Factories.FindAll(x=>x.Accept(accessToken)).Select(x=>x.Clone());
        }

        /// <summary>
        /// Adds the factory.
        /// </summary>
        /// <param name="factory">The factory.</param>
        public void AddFactory(ICellEditFactory factory)
        {
            factory.Collection = this;
            _Factories.Add(factory);
            _Factories.Sort((x, y) =>
            {
                return Comparer<int>.Default.Compare(y.ImportPriority, x.ImportPriority);
            });
        }

        /// <summary>
        /// Removes the factory.
        /// </summary>
        /// <param name="factory">The factory.</param>
        public void RemoveFactory(ICellEditFactory factory)
        {            
            _Factories.Remove(factory);
        }

        /// <summary>
        /// Builds the property control.
        /// </summary>
        /// <param name="rootPropertyGrid">The root property grid.</param>
        /// <param name="component">The component.</param>
        /// <param name="propertyDescriptor">The property descriptor.</param>
        /// <param name="factory">The factory.</param>
        /// <returns>Control.</returns>
        public Control BuildPropertyControl(IPropertyGrid rootPropertyGrid, object component, PropertyDescriptor propertyDescriptor, out ICellEditFactory factory)
        {
            foreach (var Factory in _Factories)
            {
                var control = Factory.HandleNewProperty(rootPropertyGrid, component, propertyDescriptor);

                if (control != null)
                {
                    factory = Factory;
                    return control;
                }
            }

            factory = null;

            return null;
        }
    }
}

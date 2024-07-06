using Avalonia.Controls;
using System.Collections.Generic;
using System.Linq;
using PropertyModels.ComponentModel;
using PropertyModels.Extensions;

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
        private readonly List<ICellEditFactory> _factories = [];

        /// <summary>
        /// Gets the factories.
        /// </summary>
        /// <value>The factories.</value>
        public IEnumerable<ICellEditFactory> Factories => _factories.ToArray();

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
            _factories.AddRange(factories);
            _factories.Sort((x, y) => Comparer<int>.Default.Compare(y.ImportPriority, x.ImportPriority));

            foreach (var factory in _factories)
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
            return _factories.FindAll(x=>x.Accept(accessToken)).Select(x=>x.Clone());
        }

        /// <summary>
        /// Adds the factory.
        /// </summary>
        /// <param name="factory">The factory.</param>
        public void AddFactory(ICellEditFactory factory)
        {
            factory.Collection = this;
            _factories.Add(factory);
            _factories.Sort((x, y) => Comparer<int>.Default.Compare(y.ImportPriority, x.ImportPriority));
        }

        /// <summary>
        /// Removes the factory.
        /// </summary>
        /// <param name="factory">The factory.</param>
        public void RemoveFactory(ICellEditFactory factory)
        {            
            _factories.Remove(factory);
        }

        /// <summary>
        /// Builds the property control.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>Control.</returns>
        public Control BuildPropertyControl(PropertyCellContext context)
        {
            foreach (var factory in _factories)
            {
                var control = factory.HandleNewProperty(context);
                var classesAttributes = context.Property.GetCustomAttributes<ControlClassesAttribute>();
                if (classesAttributes?.Length > 0)
                {
                    var classes = classesAttributes.SelectMany(a => a.Classes).Distinct();
                    control?.Classes.AddRange(classes);
                }
                if (control != null)
                {
                    context.CellEdit = control;
                    context.Factory = factory;

                    return control;
                }
            }

            return null;
        }
    }
}

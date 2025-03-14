using System.ComponentModel;
using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.PropertyGrid.ViewModels;

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
        ICellEditFactoryCollection? Collection { get; internal set; }

        /// <summary>
        /// Check available for target
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        /// <returns><c>true</c> if success, <c>false</c> otherwise.</returns>
        bool Accept(object accessToken);

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>ICellEditFactory.</returns>
        ICellEditFactory? Clone();

        /// <summary>
        /// Handles the new property.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>Control.</returns>
        Control? HandleNewProperty(PropertyCellContext context);

        /// <summary>
        /// Handles the property changed.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns><c>true</c> if success, <c>false</c> otherwise.</returns>
        bool HandlePropertyChanged(PropertyCellContext context);

        /// <summary>
        /// Handles the propagate visibility.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="context">The context.</param>
        /// <param name="filterContext">The filter context.</param>
        /// <returns>System.Nullable&lt;PropertyVisibility&gt;.</returns>
        PropertyVisibility? HandlePropagateVisibility(object? target, PropertyCellContext context, IPropertyGridFilterContext filterContext);

        /// <summary>
        /// Handles readonly flag changed
        /// </summary>
        /// <param name="control">control.</param>
        /// <param name="readOnly">readonly flag</param>
        /// <returns>Control.</returns>
        void HandleReadOnlyStateChanged(Control control, bool readOnly);
    }

    /// <summary>
    /// Class PropertyCellContext.
    /// </summary>
    public class PropertyCellContext
    {
        /// <summary>
        /// The root
        /// </summary>
        public readonly IPropertyGrid? Root;

        /// <summary>
        /// The owner
        /// </summary>
        public readonly IPropertyGrid Owner;

        /// <summary>
        /// The target
        /// </summary>
        public readonly object Target;

        /// <summary>
        /// The property
        /// </summary>
        public readonly PropertyDescriptor Property;

        /// <summary>
        /// Gets the display name.
        /// </summary>
        /// <value>The display name.</value>
        public string DisplayName => Property.DisplayName;

        /// <summary>
        /// The cell edit
        /// </summary>
        public Control? CellEdit { get; set; }

        /// <summary>
        /// Gets or sets the factory.
        /// </summary>
        /// <value>The factory.</value>
        public ICellEditFactory? Factory { get; set; }

        /// <summary>
        /// The parent
        /// </summary>
        public readonly PropertyCellContext? ParentContext;

        /// <summary>
        /// If this property should be readonly
        /// </summary>
        public bool IsReadOnly => Property.IsReadOnly || Owner.IsReadOnly || ParentContext is { IsReadOnly: true };

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns>System.Object.</returns>
        public object? GetValue() => Property.GetValue(Target);

        /// <summary>
        /// Gets the cell edit factory collection.
        /// </summary>
        /// <returns>ICellEditFactoryCollection.</returns>
        public ICellEditFactoryCollection GetCellEditFactoryCollection()
        {
            Debug.Assert(Root != null);

            return Root.GetCellEditFactoryCollection();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyCellContext" /> class.
        /// </summary>
        /// <param name="parentContext">The parent context.</param>
        /// <param name="root">The root.</param>
        /// <param name="owner">The owner.</param>
        /// <param name="target">The target.</param>
        /// <param name="property">The property.</param>
        /// <param name="cellEdit">The cell edit.</param>
        public PropertyCellContext(PropertyCellContext? parentContext, IPropertyGrid root, IPropertyGrid owner, object target, PropertyDescriptor property, Control? cellEdit = null)
        {
            ParentContext = parentContext;
            Root = root;
            Owner = owner;
            Target = target;
            Property = property;

            CellEdit = cellEdit;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyCellContext" /> class.
        /// </summary>
        /// <param name="parentContext">The parent context.</param>        
        /// <param name="target">The target.</param>
        /// <param name="property">The property.</param>
        public PropertyCellContext(PropertyCellContext parentContext, object target, PropertyDescriptor property)
        {
            ParentContext = parentContext;
            Root = parentContext.Root;
            Owner = parentContext.Owner;
            Target = target;
            Property = property;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString() => DisplayName;
    }
}

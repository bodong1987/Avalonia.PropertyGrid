using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.PropertyGrid.ViewModels;
using PropertyModels.ComponentModel;
using PropertyModels.Extensions;

namespace Avalonia.PropertyGrid.Controls.Factories.Builtins
{
    /// <summary>
    /// Class ExpandableCellEditFactory.
    /// Implements the <see cref="Avalonia.PropertyGrid.Controls.Factories.AbstractCellEditFactory" />
    /// </summary>
    /// <seealso cref="Avalonia.PropertyGrid.Controls.Factories.AbstractCellEditFactory" />
    public class ExpandableCellEditFactory : AbstractCellEditFactory
    {
        /// <summary>
        /// Gets the import priority.
        /// The larger the value, the earlier the object will be processed
        /// </summary>
        /// <value>The import priority.</value>
        public override int ImportPriority => int.MinValue + 10000;

        /// <summary>
        /// Handles the new property.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>Control.</returns>
        public override Control? HandleNewProperty(PropertyCellContext context)
        {
            var propertyDescriptor = context.Property;
            var target = context.Target;

            if (propertyDescriptor is MultiObjectPropertyDescriptor)
            {
                return null;
            }

            if (!propertyDescriptor.IsExpandableType())
            {
                return null;
            }

            // avoid recursive expansion
            Debug.Assert(context.Root != null);

            var value = propertyDescriptor.GetValue(target);
            if (context.Root.GetExpandableObjectCache().IsExists(value))
            {
                return null;
            }

            var border = new Border
            {
                BorderBrush = Brushes.Gray,
                BorderThickness = new Thickness(context.Root.DisplayMode == PropertyGridDisplayMode.Inline ? 0 : 0.5),
                CornerRadius = new CornerRadius(0, 0, 5, 5),
                Margin = new Thickness(0)
            };

            var propertyGrid = context.Root.ClonePropertyGrid() as PropertyGrid;
            Debug.Assert(propertyGrid != null);

            propertyGrid.RootPropertyGrid = context.Root;

            Debug.Assert(propertyGrid.RootPropertyGrid != null);
            Debug.Assert(!Equals(propertyGrid.RootPropertyGrid, propertyGrid));

            propertyGrid.DisplayMode = context.Root.DisplayMode;
            propertyGrid.ShowStyle = context.Root.ShowStyle;
            propertyGrid.NameWidth = context.Root.NameWidth;
            propertyGrid.AllowFilter = false;
            propertyGrid.AllowQuickFilter = false;
            propertyGrid.ShowTitle = false;
            propertyGrid.DataContext = null;

            border.Child = propertyGrid;

            // avoid recursive expansion
            propertyGrid.GetExpandableObjectCache().Merge(context.Root.GetExpandableObjectCache());
            propertyGrid.GetExpandableObjectCache().Add(value);

            return border;
        }

        /// <summary>
        /// Handles the property changed.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns><c>true</c> if success, <c>false</c> otherwise.</returns>
        public override bool HandlePropertyChanged(PropertyCellContext context)
        {
            var propertyDescriptor = context.Property;
            var target = context.Target;
            var control = context.CellEdit;

            if (propertyDescriptor is MultiObjectPropertyDescriptor)
            {
                return false;
            }

            if (!propertyDescriptor.IsExpandableType())
            {
                return false;
            }

            if (control is Border { Child: PropertyGrid pg })
            {
                var value = propertyDescriptor.GetValue(target);

                pg.DataContext = value;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Handles the propagate visibility.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="context">The context.</param>
        /// <param name="filterContext">The filter context.</param>
        /// <param name="filterText">The filter text.</param>
        /// <param name="filterMatchesParentCategory">Indicates whether the filter matches the parent category.</param>
        /// <returns>System.Nullable&lt;PropertyVisibility&gt;.</returns>
        public override PropertyVisibility? HandlePropagateVisibility(
            object? target, 
            PropertyCellContext context, 
            IPropertyGridFilterContext filterContext,
            string? filterText = null,
            bool filterMatchesParentCategory = false)
        {
            if (!context.Property.IsExpandableType())
            {
                return null;
            }

            if (context.CellEdit is Border { Child: PropertyGrid pg })
            {
                var category = FilterCategory.Default;
                category &= ~FilterCategory.Category;

                return pg.FilterCells(filterContext, category, filterText, filterMatchesParentCategory);
            }

            return null;
        }

        /// <summary>
        /// Handles readonly flag changed
        /// </summary>
        /// <param name="control">control.</param>
        /// <param name="readOnly">readonly flag</param>
        /// <returns>Control.</returns>
        public override void HandleReadOnlyStateChanged(Control control, bool readOnly)
        {
            if (control is Border { Child: PropertyGrid pg })
            {
                pg.IsReadOnly = readOnly;
            }
            else
            {
                base.HandleReadOnlyStateChanged(control, readOnly);
            }
        }
    }
}

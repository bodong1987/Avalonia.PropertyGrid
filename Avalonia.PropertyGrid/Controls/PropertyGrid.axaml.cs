using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.PropertyGrid.Controls.Implements;
using Avalonia.PropertyGrid.Localization;
using Avalonia.PropertyGrid.Services;
using Avalonia.PropertyGrid.ViewModels;
using Avalonia.Reactive;
using PropertyModels.ComponentModel.DataAnnotations;
using PropertyModels.Extensions;

namespace Avalonia.PropertyGrid.Controls
{
    /// <summary>
    /// Class PropertyGrid.
    /// Implements the <see cref="UserControl" />
    /// </summary>
    /// <seealso cref="UserControl" />
    public partial class PropertyGrid : UserControl, IPropertyGrid
    {
        #region Common Properties
        /// <summary>
        /// The view model
        /// </summary>
        [Browsable(false)]
        internal PropertyGridViewModel ViewModel { get; } = new();

        /// <summary>
        /// get current property grid view
        /// </summary>
        [Browsable(false)]
        public IPropertyGridView? View { get; private set; }

        /// <summary>
        /// The factories
        /// </summary>
        public readonly ICellEditFactoryCollection Factories;

        /// <summary>
        /// Gets or sets the root property grid.
        /// </summary>
        /// <value>The root property grid.</value>
        [Browsable(false)]
        public IPropertyGrid? RootPropertyGrid { get; set; }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyGrid" /> class.
        /// </summary>
        public PropertyGrid()
        {
            Factories = new CellEditFactoryCollection(CellEditFactoryService.Default.CloneFactories(this));

            ViewModel.PropertyDescriptorChanged += OnPropertyDescriptorChanged;
            ViewModel.FilterChanged += OnFilterChanged;
            ViewModel.PropertyChanged += OnViewModelPropertyChanged;
            ViewModel.CustomPropertyDescriptorFilter += OnCustomPropertyDescriptorFilter;

            InitializeComponent();

            ToggleView(ViewType, true);
        }

        private void OnCustomPropertyDescriptorFilter(object? sender, CustomPropertyDescriptorFilterEventArgs e)
        {
            if (RootPropertyGrid is PropertyGrid pg)
            {
                pg.BroadcastCustomPropertyDescriptorFilterEvent(sender, e);
            }
            else
            {
                BroadcastCustomPropertyDescriptorFilterEvent(sender, e);
            }
        }

        private void BroadcastCustomPropertyDescriptorFilterEvent(object? sender, CustomPropertyDescriptorFilterEventArgs e)
        {
            RaiseEvent(e);
        }

#if DEBUG
        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return $"[{GetHashCode()}]{base.ToString()}";
        }
#endif

        /// <summary>
        /// Handles the <see cref="E:ViewModelPropertyChanged" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ViewModel.ShowStyle))
            {
                ShowStyle = ViewModel.ShowStyle;
                View?.Refresh();
            }
            else if(e.PropertyName == nameof(ViewModel.CategoryOrderStyle))
            {
                CategoryOrderStyle = ViewModel.CategoryOrderStyle;
                View?.Refresh();
            }
            else if(e.PropertyName == nameof(ViewModel.PropertyOrderStyle))
            {
                PropertyOrderStyle = ViewModel.PropertyOrderStyle;
                View?.Refresh();
            }
            else if(e.PropertyName == nameof(ViewModel.IsReadOnly))
            {
                IsReadOnly = ViewModel.IsReadOnly;
                View?.Refresh();
            }
        }

        /// <summary>
        /// Called when the <see cref="P:Avalonia.StyledElement.DataContext" /> finishes updating.
        /// </summary>
        protected override void OnDataContextEndUpdate()
        {
            base.OnDataContextEndUpdate();

            if (ViewModel.Context != DataContext)
            {
                ViewModel.Context = DataContext;
            }
        }

        /// <summary>
        /// Gets the cell edit factory collection.
        /// </summary>
        /// <returns>ICellEditFactoryCollection.</returns>
        public ICellEditFactoryCollection GetCellEditFactoryCollection()
        {
            return Factories;
        }

        /// <summary>
        /// Gets the expandable object cache.
        /// </summary>
        /// <returns>IExpandableObjectCache.</returns>
        public IExpandableObjectCache GetExpandableObjectCache()
        {
            return View!.GetExpandableObjectCache();
        }

        /// <summary>
        /// Clones the property grid.
        /// </summary>
        /// <returns>IPropertyGrid.</returns>
        public virtual IPropertyGrid ClonePropertyGrid()
        {
            return (Activator.CreateInstance(GetType()) as IPropertyGrid)!;
        }

        /// <summary>
        /// Gets the cell information cache.
        /// </summary>
        /// <returns>IPropertyGridCellInfoCache.</returns>
        public IPropertyGridCellInfoCache GetCellInfoCache()
        {
            return View!.GetCellInfoCache();
        }
                
        /// <summary>
        /// Gets the property grid view.
        /// </summary>
        /// <param name="viewType">Type of the view.</param>
        /// <returns>System.Nullable&lt;IPropertyGridView&gt;.</returns>
        protected virtual IPropertyGridView? CreatePropertyGridView(PropertyGridViewType viewType)
        {
            switch (viewType)
            {
                case PropertyGridViewType.TiledView:
                    return new PropertyGridTiledView()
                    {
                        Owner = this
                    };
                default:
                    break;
            }

            return null;
        }

        /// <summary>
        /// Handles the <see cref="E:PropertyDescriptorChanged" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void OnPropertyDescriptorChanged(object? sender, EventArgs e)
        {
            View?.Refresh();
        }

        /// <summary>
        /// Handles the <see cref="E:FilterChanged" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void OnFilterChanged(object? sender, EventArgs e)
        {
            RefreshVisibilities();
        }

        #region Visibilities
        /// <summary>
        /// Refreshes the visibilities.
        /// </summary>
        public void RefreshVisibilities()
        {
            FilterCells(ViewModel);
        }

        /// <summary>
        /// Filters the cells.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="category">The category.</param>
        /// <returns>PropertyVisibility.</returns>
        public PropertyVisibility FilterCells(IPropertyGridFilterContext context, FilterCategory category = FilterCategory.Default)
        {
            var atLeastOneVisible = false;

            foreach (var info in GetCellInfoCache().Children)
            {
                atLeastOneVisible |= context.PropagateVisibility(info, category) == PropertyVisibility.AlwaysVisible;
            }

            return atLeastOneVisible ? PropertyVisibility.AlwaysVisible : PropertyVisibility.HiddenByNoVisibleChildren;
        }
        #endregion
    }

    /// <summary>
    /// Class CustomPropertyDescriptorFilterEventArgs.
    /// Implements the <see cref="RoutedEventArgs" />
    /// </summary>
    /// <seealso cref="RoutedEventArgs" />
    public class CustomPropertyDescriptorFilterEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// The selected object
        /// </summary>
        public readonly object TargetObject;

        /// <summary>
        /// The property descriptor
        /// </summary>
        public readonly PropertyDescriptor PropertyDescriptor;

        /// <summary>
        /// Gets or sets the is visible.
        /// </summary>
        /// <value>The is visible.</value>
        public bool IsVisible { get; set; } = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomPropertyDescriptorFilterEventArgs" /> class.
        /// </summary>
        /// <param name="routedEvent">The routed event.</param>
        /// <param name="targetObject">The target object.</param>
        /// <param name="propertyDescriptor">The property descriptor.</param>
        public CustomPropertyDescriptorFilterEventArgs(RoutedEvent routedEvent, object targetObject, PropertyDescriptor propertyDescriptor) :
            base(routedEvent)
        {
            TargetObject = targetObject;
            PropertyDescriptor = propertyDescriptor;
        }
    }
}

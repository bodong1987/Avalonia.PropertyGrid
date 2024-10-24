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
        #region Properties
        /// <summary>
        /// The allow filter property
        /// </summary>
        public static readonly StyledProperty<bool> AllowFilterProperty = AvaloniaProperty.Register<PropertyGrid, bool>(nameof(AllowFilter), true);

        /// <summary>
        /// Gets or sets a value indicating whether [allow filter].
        /// </summary>
        /// <value><c>true</c> if [allow filter]; otherwise, <c>false</c>.</value>
        [Category("Views")]
        public bool AllowFilter
        {
            get => GetValue(AllowFilterProperty); set => SetValue(AllowFilterProperty, value);
        }

        /// <summary>
        /// The allow toggle view property
        /// </summary>
        public static readonly StyledProperty<bool> AllowToggleViewProperty = AvaloniaProperty.Register<PropertyGrid, bool>(nameof(AllowToggleView), true);

        /// <summary>
        /// Gets or sets a value indicating whether [allow toggle view].
        /// </summary>
        /// <value><c>true</c> if [allow toggle view]; otherwise, <c>false</c>.</value>
        [Category("Views")]
        public bool AllowToggleView
        {
            get => GetValue(AllowToggleViewProperty); set => SetValue(AllowToggleViewProperty, value);
        }

        /// <summary>
        /// The allow quick filter property
        /// </summary>
        public static readonly StyledProperty<bool> AllowQuickFilterProperty = AvaloniaProperty.Register<PropertyGrid, bool>(nameof(AllowQuickFilter), true);

        /// <summary>
        /// Gets or sets a value indicating whether [allow quick filter].
        /// </summary>
        /// <value><c>true</c> if [allow quick filter]; otherwise, <c>false</c>.</value>
        [Category("Views")]
        public bool AllowQuickFilter
        {
            get => GetValue(AllowQuickFilterProperty); set => SetValue(AllowQuickFilterProperty, value);
        }

        /// <summary>
        /// The show title property
        /// </summary>
        public static readonly StyledProperty<bool> ShowTitleProperty = AvaloniaProperty.Register<PropertyGrid, bool>(nameof(ShowTitle), true);

        /// <summary>
        /// Gets or sets a value indicating whether [show title].
        /// </summary>
        /// <value><c>true</c> if [show title]; otherwise, <c>false</c>.</value>
        [Category("Views")]
        public bool ShowTitle
        {
            get => GetValue(ShowTitleProperty); set => SetValue(ShowTitleProperty, value);
        }

        /// <summary>
        /// The show style property
        /// </summary>
        public static readonly StyledProperty<PropertyGridShowStyle> ShowStyleProperty = AvaloniaProperty.Register<PropertyGrid, PropertyGridShowStyle>(nameof(ShowStyle));

        /// <summary>
        /// Gets or sets the show style.
        /// </summary>
        /// <value>The show style.</value>
        [Category("Views")]
        public PropertyGridShowStyle ShowStyle
        {
            get => GetValue(ShowStyleProperty);
            set => SetValue(ShowStyleProperty, value);
        }

        /// <summary>
        /// The order style property
        /// control category sort algorithm
        /// </summary>
        public static readonly StyledProperty<PropertyGridOrderStyle> CategoryOrderStyleProperty = AvaloniaProperty.Register<PropertyGrid, PropertyGridOrderStyle>(nameof(CategoryOrderStyle));

        /// <summary>
        /// Gets or sets the category order style.
        /// </summary>
        /// <value>The order style.</value>
        [Category("Views")]
        public PropertyGridOrderStyle CategoryOrderStyle
        {
            get => GetValue(CategoryOrderStyleProperty);
            set => SetValue(CategoryOrderStyleProperty, value);
        }


        /// <summary>
        /// The order style property
        /// control property sort algorithm
        /// </summary>
        public static readonly StyledProperty<PropertyGridOrderStyle> PropertyOrderStyleProperty = AvaloniaProperty.Register<PropertyGrid, PropertyGridOrderStyle>(nameof(PropertyOrderStyle));

        /// <summary>
        /// Gets or sets the order style.
        /// </summary>
        /// <value>The order style.</value>
        [Category("Views")]
        public PropertyGridOrderStyle PropertyOrderStyle
        {
            get => GetValue(PropertyOrderStyleProperty);
            set => SetValue(PropertyOrderStyleProperty, value);
        }

        /// <summary>
        /// The name width property
        /// </summary>
        public static readonly StyledProperty<double> NameWidthProperty = AvaloniaProperty.Register<PropertyGrid, double>(nameof(NameWidth), 180);

        /// <summary>
        /// Gets or sets the width of the name.
        /// </summary>
        /// <value>The width of the name.</value>
        [Category("Views")]
        public double NameWidth
        {
            get => GetValue(NameWidthProperty);
            set => SetValue(NameWidthProperty, value);
        }

        /// <summary>
        /// The IsReadOnly property
        /// </summary>
        public static readonly StyledProperty<bool> IsReadOnlyProperty = AvaloniaProperty.Register<PropertyGrid, bool>(nameof(IsReadOnly));
                
        /// <summary>
        /// Gets or sets Is Readonly flag
        /// </summary>
        /// <value><c>true</c> if [readonly]; otherwise, <c>false</c>.</value>
        public bool IsReadOnly 
        {
            get => GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, value);
        }

        /// <summary>
        /// View type property
        /// </summary>
        public static readonly StyledProperty<PropertyGridViewType> ViewTypeProperty = AvaloniaProperty.Register<PropertyGrid, PropertyGridViewType>(nameof(ViewType));

        /// <summary>
        /// get or set current view type
        /// </summary>
        public PropertyGridViewType ViewType
        {
            get => GetValue(ViewTypeProperty);
            set => SetValue(ViewTypeProperty, value);
        }
        
        /// <summary>
        /// The view model
        /// </summary>
        internal PropertyGridViewModel ViewModel { get; } = new();

        /// <summary>
        /// get current property grid view
        /// </summary>
        public IPropertyGridView? View { get; private set; }
        
        /// <summary>
        /// The factories
        /// </summary>
        public readonly ICellEditFactoryCollection Factories;

        /// <summary>
        /// Gets or sets the root property grid.
        /// </summary>
        /// <value>The root property grid.</value>
        public IPropertyGrid? RootPropertyGrid { get; set; }

        /// <summary>
        /// The custom property descriptor filter event
        /// </summary>
        public static readonly RoutedEvent<CustomPropertyDescriptorFilterEventArgs> CustomPropertyDescriptorFilterEvent =
            RoutedEvent.Register<PropertyGrid, CustomPropertyDescriptorFilterEventArgs>(nameof(CustomPropertyDescriptorFilter), RoutingStrategies.Bubble);

        /// <summary>
        /// Occurs when [custom property descriptor filter].
        /// </summary>
        public event EventHandler<CustomPropertyDescriptorFilterEventArgs> CustomPropertyDescriptorFilter
        {
            add => AddHandler(CustomPropertyDescriptorFilterEvent, value);
            remove => RemoveHandler(CustomPropertyDescriptorFilterEvent, value);
        }
        #endregion

        #region Events
        /// <summary>
        /// The command executing event
        /// </summary>
        public static readonly RoutedEvent<RoutedCommandExecutingEventArgs> CommandExecutingEvent =
            RoutedEvent.Register<PropertyGrid, RoutedCommandExecutingEventArgs>(nameof(CommandExecuting), RoutingStrategies.Bubble);

        /// <summary>
        /// Occurs when [command executing].
        /// </summary>
        public event EventHandler<RoutedCommandExecutingEventArgs> CommandExecuting
        {
            add => AddHandler(CommandExecutingEvent, value);
            remove => RemoveHandler(CommandExecutingEvent, value);
        }

        /// <summary>
        /// The command executed event
        /// </summary>
        public static readonly RoutedEvent<RoutedCommandExecutedEventArgs> CommandExecutedEvent =
            RoutedEvent.Register<PropertyGrid, RoutedCommandExecutedEventArgs>(nameof(CommandExecuted), RoutingStrategies.Bubble);

        /// <summary>
        /// Occurs when [command executed].
        /// </summary>
        public event EventHandler<RoutedCommandExecutedEventArgs> CommandExecuted
        {
            add => AddHandler(CommandExecutedEvent, value);
            remove => RemoveHandler(CommandExecutedEvent, value);
        }
        #endregion

        /// <summary>
        /// Initializes static members of the <see cref="PropertyGrid"/> class.
        /// </summary>
        static PropertyGrid()
        {
            AllowFilterProperty.Changed.Subscribe(new AnonymousObserver<AvaloniaPropertyChangedEventArgs<bool>>(OnAllowFilterChanged));
            AllowQuickFilterProperty.Changed.Subscribe(new AnonymousObserver<AvaloniaPropertyChangedEventArgs<bool>>(OnAllowQuickFilterChanged));
            ShowStyleProperty.Changed.Subscribe(new AnonymousObserver<AvaloniaPropertyChangedEventArgs<PropertyGridShowStyle>>(OnShowStyleChanged));
            CategoryOrderStyleProperty.Changed.Subscribe(new AnonymousObserver<AvaloniaPropertyChangedEventArgs<PropertyGridOrderStyle>>(OnCategoryOrderStyleChanged));
            PropertyOrderStyleProperty.Changed.Subscribe(new AnonymousObserver<AvaloniaPropertyChangedEventArgs<PropertyGridOrderStyle>>(OnPropertyOrderStyleChanged));
            ShowTitleProperty.Changed.Subscribe(new AnonymousObserver<AvaloniaPropertyChangedEventArgs<bool>>(OnShowTitleChanged));
            NameWidthProperty.Changed.Subscribe(new AnonymousObserver<AvaloniaPropertyChangedEventArgs<double>>(OnNameWidthChanged));
            IsReadOnlyProperty.Changed.Subscribe(new AnonymousObserver<AvaloniaPropertyChangedEventArgs<bool>>(OnIsReadOnyPropertyChanged));
            ViewTypeProperty.Changed.Subscribe(
                new AnonymousObserver<AvaloniaPropertyChangedEventArgs<PropertyGridViewType>>(
                    OnViewTypePropertyChanged));
        }

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

            TiledViewControl.Owner = this;
            View = TiledViewControl;
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

        #region Styled Properties Handler
        /// <summary>
        /// Handles the <see cref="E:AllowFilterChanged" /> event.
        /// </summary>
        /// <param name="e">The <see cref="AvaloniaPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnAllowFilterChanged(AvaloniaPropertyChangedEventArgs e)
        {
            if (e.Sender is PropertyGrid sender)
            {
                sender.OnAllowFilterChanged(e.OldValue, e.NewValue);
            }
        }

        /// <summary>
        /// Called when [allow filter changed].
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnAllowFilterChanged(object? oldValue, object? newValue)
        {
            HeaderGrid.IsVisible = (bool)newValue!;
        }

        private static void OnNameWidthChanged(AvaloniaPropertyChangedEventArgs e)
        {
            if (e.Sender is PropertyGrid sender)
            {
                sender.OnNameWidthChanged(e.OldValue, e.NewValue);
            }
        }

        private void OnNameWidthChanged(object? oldValue, object? newValue)
        {
            if (View != null)
            {
                View.NameWidth = (double)newValue!;
            }
        }

        /// <summary>
        /// Called when [show style changed].
        /// </summary>
        /// <param name="e">The e.</param>
        private static void OnShowStyleChanged(AvaloniaPropertyChangedEventArgs<PropertyGridShowStyle> e)
        {
            if (e.Sender is PropertyGrid sender)
            {
                sender.OnShowStyleChanged(e.OldValue, e.NewValue);
            }
        }

        /// <summary>
        /// Called when [show style changed].
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnShowStyleChanged(Optional<PropertyGridShowStyle> oldValue, BindingValue<PropertyGridShowStyle> newValue)
        {
            ViewModel.ShowStyle = newValue.Value;
        }

        private static void OnCategoryOrderStyleChanged(AvaloniaPropertyChangedEventArgs<PropertyGridOrderStyle> e)
        {
            if (e.Sender is PropertyGrid sender)
            {
                sender.OnCategoryOrderStyleChanged(e.OldValue, e.NewValue);
            }
        }

        private void OnCategoryOrderStyleChanged(Optional<PropertyGridOrderStyle> oldValue, BindingValue<PropertyGridOrderStyle> newValue)
        {
            ViewModel.CategoryOrderStyle = newValue.Value;
        }

        private static void OnPropertyOrderStyleChanged(AvaloniaPropertyChangedEventArgs<PropertyGridOrderStyle> e)
        {
            if (e.Sender is PropertyGrid sender)
            {
                sender.OnPropertyOrderStyleChanged(e.OldValue, e.NewValue);
            }
        }

        private void OnPropertyOrderStyleChanged(Optional<PropertyGridOrderStyle> oldValue, BindingValue<PropertyGridOrderStyle> newValue)
        {
            ViewModel.PropertyOrderStyle = newValue.Value;
        }

        private static void OnShowTitleChanged(AvaloniaPropertyChangedEventArgs<bool> e)
        {
            if (e.Sender is PropertyGrid sender)
            {
                sender.OnShowTitleChanged(e.OldValue.Value, e.NewValue.Value);
            }
        }

        private void OnShowTitleChanged(bool oldValue, bool newValue)
        {
            if (View != null)
            {
                View.ShowTitle = newValue;
            }
        }

        private static void OnIsReadOnyPropertyChanged(AvaloniaPropertyChangedEventArgs<bool> e)
        {
            if(e.Sender is PropertyGrid sender)
            {
                sender.OnIsReadOnyPropertyChanged(e.OldValue.Value, e.NewValue.Value);
            }
        }

        private void OnIsReadOnyPropertyChanged(bool oldValue, bool newValue)
        {
            ViewModel.IsReadOnly = newValue;
        }
        
        private static void OnViewTypePropertyChanged(AvaloniaPropertyChangedEventArgs<PropertyGridViewType> e)
        {
            if (e.Sender is PropertyGrid sender)
            {
                sender.OnViewTypePropertyChanged(e.OldValue.Value, e.NewValue.Value);
            }
        }

        private void OnViewTypePropertyChanged(PropertyGridViewType oldValue, PropertyGridViewType newValue)
        {
            if (View != null && View.ViewType == newValue)
            {
                return;
            }
            
            var view = GetPropertyGridView(newValue);

            if (view != null)
            {
                View?.OnLeaveState();
                View = view;
                View.OnEnterState();
            }
        }

        /// <summary>
        /// Gets the property grid view.
        /// </summary>
        /// <param name="viewType">Type of the view.</param>
        /// <returns>System.Nullable&lt;IPropertyGridView&gt;.</returns>
        protected virtual IPropertyGridView? GetPropertyGridView(PropertyGridViewType viewType)
        {
            switch (viewType)
            {
                case PropertyGridViewType.TiledView:
                    return TiledViewControl;
                default:
                    break;
            }

            return null;
        }

        private static void OnAllowQuickFilterChanged(AvaloniaPropertyChangedEventArgs<bool> e)
        {
            if (e.Sender is PropertyGrid sender)
            {
                sender.OnAllowQuickFilterChanged(e.OldValue.Value, e.NewValue.Value);
            }
        }
        
        private void OnAllowQuickFilterChanged(bool oldValue, bool newValue)
        {
            FastFilterBox.IsVisible = newValue;
        }

        #endregion

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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.PropertyGrid.Controls.Implements;
using Avalonia.PropertyGrid.Localization;
using Avalonia.PropertyGrid.Services;
using Avalonia.PropertyGrid.Utils;
using Avalonia.PropertyGrid.ViewModels;
using Avalonia.Reactive;
using Avalonia.VisualTree;
using PropertyModels.ComponentModel;
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
        #region Visible Properties
        /// <summary>
        /// The is header visible property
        /// </summary>
        public static readonly StyledProperty<bool> IsHeaderVisibleProperty =
            AvaloniaProperty.Register<PropertyGrid, bool>(nameof(IsHeaderVisible), true);

        /// <summary>
        /// Gets or sets is header visible.
        /// </summary>
        /// <value><c>true</c> if [header visible]; otherwise, <c>false</c>.</value>
        [Category("Views")]
        public bool IsHeaderVisible
        {
            get => GetValue(IsHeaderVisibleProperty); 
            set => SetValue(IsHeaderVisibleProperty, value);
        }
        
        /// <summary>
        /// The allow quick filter property
        /// </summary>
        public static readonly StyledProperty<bool> IsQuickFilterVisibleProperty =
            AvaloniaProperty.Register<PropertyGrid, bool>(nameof(IsQuickFilterVisible), true);

        /// <summary>
        /// Gets or sets a value indicating whether [allow quick filter].
        /// </summary>
        /// <value><c>true</c> if [allow quick filter]; otherwise, <c>false</c>.</value>
        [Category("Views")]
        public bool IsQuickFilterVisible
        {
            get => GetValue(IsQuickFilterVisibleProperty); 
            set => SetValue(IsQuickFilterVisibleProperty, value);
        }

        /// <summary>
        /// The show title property
        /// </summary>
        public static readonly StyledProperty<bool> IsTitleVisibleProperty =
            AvaloniaProperty.Register<PropertyGrid, bool>(nameof(IsTitleVisible), true);

        /// <summary>
        /// Gets or sets a value indicating whether [show title].
        /// </summary>
        /// <value><c>true</c> if [show title]; otherwise, <c>false</c>.</value>
        [Category("Views")]
        public bool IsTitleVisible
        {
            get => GetValue(IsTitleVisibleProperty); 
            set => SetValue(IsTitleVisibleProperty, value);
        }
        
        /// <summary>
        /// The is category visible property
        /// </summary>
        public static readonly StyledProperty<bool> IsCategoryVisibleProperty =
            AvaloniaProperty.Register<PropertyGrid, bool>(nameof(IsCategoryVisible));

        /// <summary>
        /// Gets or sets is category visible.
        /// </summary>
        /// <value>is category visible.</value>
        [Category("Views")]
        public bool IsCategoryVisible
        {
            get => GetValue(IsCategoryVisibleProperty);
            set => SetValue(IsCategoryVisibleProperty, value);
        }
        
        /// <summary>
        /// global option for extra property button
        /// </summary>
        public static readonly StyledProperty<PropertyOperationVisibility> PropertyOperationVisibilityProperty =
            AvaloniaProperty.Register<PropertyGrid, PropertyOperationVisibility>(nameof(PropertyOperationVisibility), 
                defaultValue: PropertyOperationVisibility.Default, // default is hidden
                inherits: true);

        /// <summary>
        /// global option for extra property button
        /// </summary>
        public PropertyOperationVisibility PropertyOperationVisibility
        {
            get => GetValue(PropertyOperationVisibilityProperty);
            set => SetValue(PropertyOperationVisibilityProperty, value);
        }
        #endregion

        #region Appearance Properties

        /// <summary>
        /// cell edit alignment type
        /// </summary>
        public static readonly StyledProperty<CellEditAlignmentType> CellEditAlignmentProperty =
            AvaloniaProperty.Register<PropertyGrid, CellEditAlignmentType>(nameof(CellEditAlignment));
       
        /// <summary>
        /// cell edit alignment
        /// </summary>
        [Category("Views")]
        public CellEditAlignmentType CellEditAlignment
        {
            get => GetValue(CellEditAlignmentProperty); 
            set => SetValue(CellEditAlignmentProperty, value);
        }

        /// <summary>
        /// The layout style property
        /// </summary>
        public static readonly StyledProperty<PropertyGridLayoutStyle> LayoutStyleProperty =
            AvaloniaProperty.Register<PropertyGrid, PropertyGridLayoutStyle>(nameof(LayoutStyle));

        /// <summary>
        /// Gets or sets the layout style
        /// </summary>
        /// <value>The layout style.</value>
        [Category("Views")]
        public PropertyGridLayoutStyle LayoutStyle
        {
            get => GetValue(LayoutStyleProperty);
            set => SetValue(LayoutStyleProperty, value);
        }

        /// <summary>
        /// The order style property
        /// control category sort algorithm
        /// </summary>
        public static readonly StyledProperty<PropertyGridOrderStyle> CategoryOrderStyleProperty =
            AvaloniaProperty.Register<PropertyGrid, PropertyGridOrderStyle>(nameof(CategoryOrderStyle));

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
        public static readonly StyledProperty<PropertyGridOrderStyle> PropertyOrderStyleProperty =
            AvaloniaProperty.Register<PropertyGrid, PropertyGridOrderStyle>(nameof(PropertyOrderStyle));

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
        public static readonly StyledProperty<double> NameWidthProperty =
            AvaloniaProperty.Register<PropertyGrid, double>(nameof(NameWidth), 200);

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
        #endregion

        #region Behavior Properties
        /// <summary>
        /// The IsReadOnly property
        /// </summary>
        public static readonly StyledProperty<bool> IsReadOnlyProperty =
            AvaloniaProperty.Register<PropertyGrid, bool>(nameof(IsReadOnly));

        /// <summary>
        /// Gets or sets Is Readonly flag
        /// </summary>
        /// <value><c>true</c> if [readonly]; otherwise, <c>false</c>.</value>
        public bool IsReadOnly
        {
            get => GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, value);
        }
        
        private readonly List<Expander> _categoryExpanders = [];
        
        /// <summary>
        /// is all categories expanded property
        /// </summary>
        public static readonly StyledProperty<bool> AllCategoriesExpandedProperty = 
            AvaloniaProperty.Register<PropertyGrid, bool>(nameof(AllCategoriesExpanded), 
                defaultValue: true,
                inherits: true); 

        /// <summary>
        /// is all categories expanded
        /// </summary>
        public bool AllCategoriesExpanded
        {
            get => GetValue(AllCategoriesExpandedProperty);
            set => SetValue(AllCategoriesExpandedProperty, value);
        }

        #endregion

        #region Extends Properties
        /// <summary>
        /// Top header content
        /// allow user custom this area
        /// </summary>
        public static readonly StyledProperty<object> TopHeaderContentProperty =
            AvaloniaProperty.Register<PropertyGrid, object>(nameof(TopHeaderContent));
        
        /// <summary>
        /// Top header content
        /// </summary>
        public object TopHeaderContent
        {
            get => GetValue(TopHeaderContentProperty);
            set => SetValue(TopHeaderContentProperty, value);
        }
        
        /// <summary>
        /// middle area
        /// allow user custom this area
        /// </summary>
        public static readonly StyledProperty<object> MiddleContentProperty =
            AvaloniaProperty.Register<PropertyGrid, object>(nameof(MiddleContent));
        
        /// <summary>
        /// middle area
        /// </summary>
        public object MiddleContent
        {
            get => GetValue(MiddleContentProperty);
            set => SetValue(MiddleContentProperty, value);
        }
        
        /// <summary>
        /// bottom content area, allow user custom this area
        /// </summary>
        public static readonly StyledProperty<object> BottomContentProperty =
            AvaloniaProperty.Register<PropertyGrid, object>(nameof(BottomContent));
        
        /// <summary>
        /// bottom content area
        /// </summary>
        public object BottomContent
        {
            get => GetValue(BottomContentProperty);
            set => SetValue(BottomContentProperty, value);
        }
        
        /// <summary>
        /// export this property
        /// so user can redefine the style of it
        /// </summary>
        public Button DefaultOptionsButton => OptionsButton;
        
        /// <summary>
        /// export default options context menu
        /// so user can add more menu item to it
        /// </summary>
        public ContextMenu DefaultOptionsContextMenu => OptionsContextMenu;

        /// <summary>
        /// default header grid
        /// user can append more elements in it
        /// </summary>
        public Grid DefaultHeaderGrid => InternalHeaderGrid;
        #endregion

        #region Base Properties
        /// <summary>
        /// The view model
        /// </summary>
        internal PropertyGridViewModel ViewModel { get; } = new();

        /// <summary>
        /// The factories
        /// </summary>
        public readonly ICellEditFactoryCollection Factories;

        private readonly PropertyGridExpandableCache _expandableObjectCache = new();
        private readonly PropertyGridCellInfoCache _cellInfoCache = new();

        /// <summary>
        /// Gets or sets the root property grid.
        /// </summary>
        /// <value>The root property grid.</value>
        public IPropertyGrid? RootPropertyGrid
        {
            get => _rootPropertyGrid;
            set
            {
                if (_rootPropertyGrid != null)
                {
                    _rootPropertyGrid.PropertyChanged -= OnRootPropertyGridPropertyChanged;
                }

                _rootPropertyGrid = value;

                if (_rootPropertyGrid != null)
                {
                    _rootPropertyGrid.PropertyChanged += OnRootPropertyGridPropertyChanged;
                }
            }
        }

        private IPropertyGrid? _rootPropertyGrid;
        #endregion
        #endregion

        #region Events
        /// <summary>
        /// The custom property descriptor filter event
        /// </summary>
        public static readonly RoutedEvent<RoutedEventArgs> CustomPropertyDescriptorFilterEvent =
            RoutedEvent.Register<PropertyGrid, RoutedEventArgs>(
                nameof(CustomPropertyDescriptorFilter), RoutingStrategies.Bubble);

        /// <summary>
        /// Occurs when [custom property descriptor filter].
        /// </summary>
        public event EventHandler<RoutedEventArgs> CustomPropertyDescriptorFilter
        {
            add => AddHandler(CustomPropertyDescriptorFilterEvent, value);
            remove => RemoveHandler(CustomPropertyDescriptorFilterEvent, value);
        }
        
        /// <summary>
        /// The custom name block event
        /// </summary>
        public static readonly RoutedEvent<RoutedEventArgs> CustomNameBlockEvent = 
            RoutedEvent.Register<PropertyGrid, RoutedEventArgs>(nameof(CustomNameBlock),
                RoutingStrategies.Bubble);

        /// <summary>
        /// Occurs when [custom name block].
        /// </summary>
        public event EventHandler<RoutedEventArgs> CustomNameBlock
        {
            add => AddHandler(CustomNameBlockEvent, value);
            remove => RemoveHandler(CustomNameBlockEvent, value);
        }
        
        /// <summary>
        /// custom property operation control event
        /// you can use this to custom your controls.
        /// assign your control to e.CustomControl
        /// </summary>
        public static readonly RoutedEvent<RoutedEventArgs> CustomPropertyOperationControlEvent = 
            RoutedEvent.Register<PropertyGrid, RoutedEventArgs>(
                nameof(CustomPropertyOperationControl), RoutingStrategies.Bubble);

        /// <summary>
        /// Occurs when [custom name block].
        /// </summary>
        public event EventHandler<RoutedEventArgs> CustomPropertyOperationControl
        {
            add => AddHandler(CustomPropertyOperationControlEvent, value);
            remove => RemoveHandler(CustomPropertyOperationControlEvent, value);
        }

        /// <summary>
        /// custom menu items
        /// </summary>
        public static readonly RoutedEvent<RoutedEventArgs> CustomPropertyOperationMenuOpeningEvent = 
            RoutedEvent.Register<PropertyGrid, RoutedEventArgs>(
                nameof(CustomPropertyOperationMenuOpening), 
                RoutingStrategies.Bubble
            );

        /// <summary>
        /// custom menu items
        /// </summary>
        public event EventHandler<RoutedEventArgs> CustomPropertyOperationMenuOpening
        {
            add => AddHandler(CustomPropertyOperationMenuOpeningEvent, value);
            remove => RemoveHandler(CustomPropertyOperationMenuOpeningEvent, value);
        }

        
        /// <summary>
        /// The command executing event
        /// </summary>
        public static readonly RoutedEvent<RoutedEventArgs> CommandExecutingEvent =
            RoutedEvent.Register<PropertyGrid, RoutedEventArgs>(nameof(CommandExecuting), RoutingStrategies.Bubble);

        /// <summary>
        /// Occurs when [command executing].
        /// </summary>
        public event EventHandler<RoutedEventArgs> CommandExecuting
        {
            add => AddHandler(CommandExecutingEvent, value);
            remove => RemoveHandler(CommandExecutingEvent, value);
        }

        /// <summary>
        /// The command executed event
        /// </summary>
        public static readonly RoutedEvent<RoutedEventArgs> CommandExecutedEvent =
            RoutedEvent.Register<PropertyGrid, RoutedEventArgs>(nameof(CommandExecuted), RoutingStrategies.Bubble);

        /// <summary>
        /// Occurs when [command executed].
        /// </summary>
        public event EventHandler<RoutedEventArgs> CommandExecuted
        {
            add => AddHandler(CommandExecutedEvent, value);
            remove => RemoveHandler(CommandExecutedEvent, value);
        }
        
        /// <summary>
        /// Property got focus event
        /// </summary>
        public static readonly RoutedEvent<RoutedEventArgs> PropertyGotFocusEvent =
            RoutedEvent.Register<PropertyGrid, RoutedEventArgs>(
                nameof(PropertyGotFocus), 
                RoutingStrategies.Bubble
            );

        /// <summary>
        /// property got focus event handler
        /// </summary>
        public event EventHandler<RoutedEventArgs> PropertyGotFocus
        {
            add => AddHandler(PropertyGotFocusEvent, value);
            remove => RemoveHandler(PropertyGotFocusEvent, value);
        }
        
        /// <summary>
        /// Property got focus event
        /// </summary>
        public static readonly RoutedEvent<RoutedEventArgs> PropertyLostFocusEvent =
            RoutedEvent.Register<PropertyGrid, RoutedEventArgs>(
                nameof(PropertyLostFocus), 
                RoutingStrategies.Bubble
            );

        /// <summary>
        /// property got focus event handler
        /// </summary>
        public event EventHandler<RoutedEventArgs> PropertyLostFocus
        {
            add => AddHandler(PropertyLostFocusEvent, value);
            remove => RemoveHandler(PropertyLostFocusEvent, value);
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes static members of the <see cref="PropertyGrid"/> class.
        /// </summary>
        static PropertyGrid()
        {
            _ = IsHeaderVisibleProperty.Changed.Subscribe(new AnonymousObserver<AvaloniaPropertyChangedEventArgs<bool>>(IsHeaderVisibleChanged));
            _ = IsQuickFilterVisibleProperty.Changed.Subscribe(new AnonymousObserver<AvaloniaPropertyChangedEventArgs<bool>>(IsQuickFilterVisibleChanged));
            _ = LayoutStyleProperty.Changed.Subscribe(new AnonymousObserver<AvaloniaPropertyChangedEventArgs<PropertyGridLayoutStyle>>(OnDisplayModeChanged));
            _ = IsCategoryVisibleProperty.Changed.Subscribe(new AnonymousObserver<AvaloniaPropertyChangedEventArgs<bool>>(IsCategoryVisibleChanged));
            _ = CategoryOrderStyleProperty.Changed.Subscribe(new AnonymousObserver<AvaloniaPropertyChangedEventArgs<PropertyGridOrderStyle>>(OnCategoryOrderStyleChanged));
            _ = PropertyOrderStyleProperty.Changed.Subscribe(new AnonymousObserver<AvaloniaPropertyChangedEventArgs<PropertyGridOrderStyle>>(OnPropertyOrderStyleChanged));
            _ = IsTitleVisibleProperty.Changed.Subscribe(new AnonymousObserver<AvaloniaPropertyChangedEventArgs<bool>>(OnShowTitleChanged));
            _ = NameWidthProperty.Changed.Subscribe(new AnonymousObserver<AvaloniaPropertyChangedEventArgs<double>>(OnNameWidthChanged));
            _ = IsReadOnlyProperty.Changed.Subscribe(new AnonymousObserver<AvaloniaPropertyChangedEventArgs<bool>>(OnIsReadOnlyPropertyChanged));
            _ = AllCategoriesExpandedProperty.Changed.Subscribe(new AnonymousObserver<AvaloniaPropertyChangedEventArgs<bool>>(e => 
            {
                if (e.Sender is PropertyGrid pg && pg._categoryExpanders.Any())
                {
                    var expanded = e.NewValue.Value;
                    pg._categoryExpanders.ForEach(ex => ex.IsExpanded = expanded);
                }
            }));
            _ = PropertyOperationVisibilityProperty.Changed.Subscribe(
                new AnonymousObserver<AvaloniaPropertyChangedEventArgs<PropertyOperationVisibility>>(OnPropertyOperationVisibilityPropertyChanged));
            _ = CellEditAlignmentProperty.Changed.Subscribe(
                new AnonymousObserver<AvaloniaPropertyChangedEventArgs<CellEditAlignmentType>>(
                    OnCellEditAlignmentPropertyChanged)
            );
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

            ColumnName.PropertyChanged += OnColumnNamePropertyChanged;
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
        #endregion

        #region Base PropertyChanged Handlers
        private void OnCustomPropertyDescriptorFilter(object? sender, RoutedEventArgs e)
        {
            if (RootPropertyGrid is PropertyGrid pg)
            {
                pg.BroadcastCustomPropertyDescriptorFilterEvent(sender, (e as CustomPropertyDescriptorFilterEventArgs)!);
            }
            else
            {
                BroadcastCustomPropertyDescriptorFilterEvent(sender, (e as CustomPropertyDescriptorFilterEventArgs)!);
            }
        }

        private void BroadcastCustomPropertyDescriptorFilterEvent(object? sender, CustomPropertyDescriptorFilterEventArgs e) => RaiseEvent(e);
        
        /// <summary>
        /// Handles the <see cref="E:ViewModelPropertyChanged" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ViewModel.DisplayMode))
            {
                LayoutStyle = ViewModel.DisplayMode;
                BuildPropertiesView();
            }
            else if (e.PropertyName == nameof(ViewModel.IsCategoryVisible))
            {
                IsCategoryVisible = ViewModel.IsCategoryVisible;
                BuildPropertiesView();
            }
            else if (e.PropertyName == nameof(ViewModel.CategoryOrderStyle))
            {
                CategoryOrderStyle = ViewModel.CategoryOrderStyle;
                BuildPropertiesView();
            }
            else if (e.PropertyName == nameof(ViewModel.PropertyOrderStyle))
            {
                PropertyOrderStyle = ViewModel.PropertyOrderStyle;
                BuildPropertiesView();
            }
            else if (e.PropertyName == nameof(ViewModel.IsReadOnly))
            {
                IsReadOnly = ViewModel.IsReadOnly;
                BuildPropertiesView();
            }
        }

        /// <summary>
        /// Handles the <see cref="E:ViewModelPropertyChanged" /> event of the root property grid.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnRootPropertyGridPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender is IPropertyGrid propertyGrid)
            {
                // ReSharper disable once ConvertSwitchStatementToSwitchExpression
                switch (e.PropertyName)
                {
                    case nameof(IPropertyGrid.NameWidth):
                        NameWidth = propertyGrid.NameWidth;
                        break;
                }
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
        #endregion

        #region Interface Methods
        /// <summary>
        /// Gets the cell edit factory collection.
        /// </summary>
        /// <returns>ICellEditFactoryCollection.</returns>
        public ICellEditFactoryCollection GetCellEditFactoryCollection() => Factories;

        /// <summary>
        /// Gets the expandable object cache.
        /// </summary>
        /// <returns>IExpandableObjectCache.</returns>
        public IExpandableObjectCache GetExpandableObjectCache() => _expandableObjectCache;

        /// <summary>
        /// Clones the property grid.
        /// </summary>
        /// <returns>IPropertyGrid.</returns>
        public virtual IPropertyGrid ClonePropertyGrid() => (Activator.CreateInstance(GetType()) as IPropertyGrid)!;

        /// <summary>
        /// Gets the cell information cache.
        /// </summary>
        /// <returns>IPropertyGridCellInfoCache.</returns>
        public IPropertyGridCellInfoCache GetCellInfoCache() => _cellInfoCache;
        #endregion

        #region Styled Properties Handler
        private static void IsHeaderVisibleChanged(AvaloniaPropertyChangedEventArgs e)
        {
            if (e.Sender is PropertyGrid sender)
            {
                sender.IsHeaderVisibleChanged(e.OldValue, e.NewValue);
            }
        }

        /// <summary>
        /// Called when [allow filter changed].
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void IsHeaderVisibleChanged(object? oldValue, object? newValue) => InternalHeaderGrid.IsVisible = (bool)newValue!;

        private static void OnNameWidthChanged(AvaloniaPropertyChangedEventArgs e)
        {
            if (e.Sender is PropertyGrid sender)
            {
                sender.OnNameWidthChanged(e.OldValue, e.NewValue);
            }
        }

        private void OnNameWidthChanged(object? oldValue, object? newValue) => SplitterGrid.ColumnDefinitions[0].Width = new GridLength((double)newValue!);

        /// <summary>
        /// Called when [display mode changed].
        /// </summary>
        /// <param name="e">The e.</param>
        private static void OnDisplayModeChanged(AvaloniaPropertyChangedEventArgs<PropertyGridLayoutStyle> e)
        {
            if (e.Sender is PropertyGrid sender)
            {
                sender.OnDisplayModeChanged(e.OldValue, e.NewValue);
            }
        }

        /// <summary>
        /// Called when [display mode changed].
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnDisplayModeChanged(Optional<PropertyGridLayoutStyle> oldValue, BindingValue<PropertyGridLayoutStyle> newValue) => ViewModel.DisplayMode = newValue.Value;

        /// <summary>
        /// Called when [show style changed].
        /// </summary>
        /// <param name="e">The e.</param>
        private static void IsCategoryVisibleChanged(AvaloniaPropertyChangedEventArgs<bool> e)
        {
            if (e.Sender is PropertyGrid sender)
            {
                sender.IsCategoryVisibleChanged(e.OldValue, e.NewValue);
            }
        }

        /// <summary>
        /// Called when [show style changed].
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void IsCategoryVisibleChanged(Optional<bool> oldValue, BindingValue<bool> newValue) => ViewModel.IsCategoryVisible = newValue.Value;

        private static void OnCategoryOrderStyleChanged(AvaloniaPropertyChangedEventArgs<PropertyGridOrderStyle> e)
        {
            if (e.Sender is PropertyGrid sender)
            {
                sender.OnCategoryOrderStyleChanged(e.OldValue, e.NewValue);
            }
        }

        private void OnCategoryOrderStyleChanged(Optional<PropertyGridOrderStyle> oldValue, BindingValue<PropertyGridOrderStyle> newValue) => ViewModel.CategoryOrderStyle = newValue.Value;

        private static void OnPropertyOrderStyleChanged(AvaloniaPropertyChangedEventArgs<PropertyGridOrderStyle> e)
        {
            if (e.Sender is PropertyGrid sender)
            {
                sender.OnPropertyOrderStyleChanged(e.OldValue, e.NewValue);
            }
        }

        private void OnPropertyOrderStyleChanged(Optional<PropertyGridOrderStyle> oldValue, BindingValue<PropertyGridOrderStyle> newValue) => ViewModel.PropertyOrderStyle = newValue.Value;

        private static void OnShowTitleChanged(AvaloniaPropertyChangedEventArgs<bool> e)
        {
            if (e.Sender is PropertyGrid sender)
            {
                sender.OnShowTitleChanged(e.OldValue.Value, e.NewValue.Value);
            }
        }

        private void OnShowTitleChanged(bool oldValue, bool newValue)
        {
            SplitterGrid.Opacity = newValue ? 1 : 0;
            SplitterGrid.Height = newValue ? double.NaN : 0;
        }

        private static void OnIsReadOnlyPropertyChanged(AvaloniaPropertyChangedEventArgs<bool> e)
        {
            if (e.Sender is PropertyGrid sender)
            {
                sender.OnIsReadOnlyPropertyChanged(e.OldValue.Value, e.NewValue.Value);
            }
        }

        private void OnIsReadOnlyPropertyChanged(bool oldValue, bool newValue) => ViewModel.IsReadOnly = newValue;

        private static void IsQuickFilterVisibleChanged(AvaloniaPropertyChangedEventArgs<bool> e)
        {
            if (e.Sender is PropertyGrid sender)
            {
                sender.IsQuickFilterVisibleChanged(e.OldValue.Value, e.NewValue.Value);
            }
        }

        private void IsQuickFilterVisibleChanged(bool oldValue, bool newValue) => FastFilterBox.IsVisible = newValue;

        private static void OnPropertyOperationVisibilityPropertyChanged(AvaloniaPropertyChangedEventArgs<PropertyOperationVisibility> e)
        {
            if (e.Sender is PropertyGrid sender)
            {
                sender.OnPropertyOperationVisibilityChanged(e.OldValue.Value, e.NewValue.Value);
            }
        }

        private void OnPropertyOperationVisibilityChanged(PropertyOperationVisibility oldValueValue, PropertyOperationVisibility newValueValue)
        {
            BuildPropertiesView();
        }
        
        private static void OnCellEditAlignmentPropertyChanged(AvaloniaPropertyChangedEventArgs<CellEditAlignmentType> e)
        {
            if (e.Sender is PropertyGrid sender)
            {
                sender.OnCellEditAlignmentChanged(e.OldValue.Value, e.NewValue.Value);
            }
        }

        private void OnCellEditAlignmentChanged(CellEditAlignmentType oldValueValue, CellEditAlignmentType newValueValue)
        {
            BuildPropertiesView();
        }
        
        /// <summary>
        /// Handles the <see cref="E:PropertyDescriptorChanged" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void OnPropertyDescriptorChanged(object? sender, EventArgs e) => BuildPropertiesView();
        
        /// <summary>
        /// Handles the <see cref="E:FilterChanged" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="FilterChangedEventArgs"/> instance containing the event data.</param>
        private void OnFilterChanged(object? sender, FilterChangedEventArgs e) => RefreshVisibilities(e.FilterText);

        #endregion

        #region Open Methods
        /// <inheritdoc />
        public void ExpandAllCategories() => 
            _categoryExpanders.ForEach(e => e.IsExpanded = true);

        /// <inheritdoc />
        public void CollapseAllCategories() => 
            _categoryExpanders.ForEach(e => e.IsExpanded = false);

        #endregion

        #region Cell Builders
        /// <summary>
        /// Builds the properties view.
        /// </summary>
        private void BuildPropertiesView()
        {
            PropertiesGrid.RowDefinitions.Clear();
            PropertiesGrid.Children.Clear();
            _expandableObjectCache.Clear();

            ClearPropertyChangedObservers(_cellInfoCache.Children);
            _cellInfoCache.Clear();
            _categoryExpanders.Clear();

            var target = ViewModel.Context;

            if (target == null)
            {
                return;
            }

            var referencePath = new ReferencePath();

            try
            {
                _expandableObjectCache.Add(target);

                referencePath.BeginScope(target.GetType().Name);

                if (ViewModel.IsCategoryVisible)
                {
                    BuildCategoryPropertiesView(target, referencePath, ViewModel.CategoryOrderStyle, ViewModel.PropertyOrderStyle);
                }
                else if (ViewModel.PropertyOrderStyle == PropertyGridOrderStyle.Alphabetic)
                {
                    BuildAlphabeticPropertiesView(target, referencePath);
                }
                else
                {
                    BuildBuiltinPropertiesView(target, referencePath);
                }
            }
            finally
            {
                referencePath.EndScope();
            }

            AddPropertyChangedObservers(_cellInfoCache.Children);

            RefreshVisibilities(ViewModel.FilterPattern.FilterText);

            var width = ColumnName.Bounds.Width;

            SyncNameWidth(width, false);
        }
        #endregion

        #region Categories
        /// <summary>
        /// Builds the category properties view.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="referencePath">The reference path.</param>
        /// <param name="categoryStyle">category style.</param>
        /// <param name="propertyOrderStyle">property style.</param>
        protected virtual void BuildCategoryPropertiesView(
            object target, 
            ReferencePath referencePath, 
            PropertyGridOrderStyle categoryStyle, 
            PropertyGridOrderStyle propertyOrderStyle)
        {
            PropertiesGrid.ColumnDefinitions.Clear();

            var categories = ViewModel.Categories;

            if (categoryStyle == PropertyGridOrderStyle.Alphabetic)
            {
                categories = [.. categories.OrderBy(x => x.Key)];
            }

            var autoCollapseCategoriesAttribute = target.GetType().GetAnyCustomAttribute<AutoCollapseCategoriesAttribute>();

            foreach (var categoryInfo in categories)
            {
                PropertiesGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));

                var expander = new Expander
                {
                    ExpandDirection = ExpandDirection.Down,
                    HeaderTemplate = new FuncDataTemplate<object>((_, _) =>
                    {
                        var tb = new TextBlock();
                        tb.SetInlinesBinding(categoryInfo.Key);
                        tb.FontWeight = FontWeight.Bold;

                        return tb;
                    })
                };
                _ = expander.SetValue(Grid.RowProperty, PropertiesGrid.RowDefinitions.Count - 1);
                expander.IsExpanded = AllCategoriesExpanded && autoCollapseCategoriesAttribute?.ShouldAutoCollapse(categoryInfo.Key) != true;
                expander.HorizontalContentAlignment = HorizontalAlignment.Stretch;
                expander.HorizontalAlignment = HorizontalAlignment.Stretch;
                expander.Margin = new Thickness(LayoutStyle == PropertyGridLayoutStyle.Inline ? 0 : 2);
                expander.Padding = new Thickness(LayoutStyle == PropertyGridLayoutStyle.Inline ? 0 : 2);
                expander.Background = null;
                expander.SetLocalizeBinding(HeaderedContentControl.HeaderProperty, categoryInfo.Key);

                expander.TemplateApplied += (_, _) =>
                {
                    var contentPresenter = expander.GetTemplateChildren()
                        .OfType<ContentPresenter>()
                        .FirstOrDefault(c => c.Name == "PART_ContentPresenter");
    
                    if (contentPresenter != null)
                    {
                        contentPresenter.Margin = new Thickness(
                            8, 
                            contentPresenter.Margin.Top,
                            contentPresenter.Margin.Right,
                            contentPresenter.Margin.Bottom
                        );
                    }
                };
                
                
                _categoryExpanders.Add(expander);

                var grid = new Grid();
                grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
                grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
                grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));

                var cellInfo = new PropertyGridCellInfo(null)
                {
                    ReferencePath = $"{referencePath}[{categoryInfo.Key}]",
                    Category = categoryInfo.Key,
                    OwnerObject = target,
                    Container = expander,
                    CellType = PropertyGridCellType.Category
                };

                _cellInfoCache.Add(cellInfo);

                var properties = propertyOrderStyle == PropertyGridOrderStyle.Builtin ? categoryInfo.Value : [.. categoryInfo.Value.OrderBy(x => x.DisplayName)];

                BuildPropertiesCellEdit(target, referencePath, properties, grid, cellInfo);

                expander.Content = grid;

                PropertiesGrid.Children.Add(expander);
            }

            PropertiesGrid.RowDefinitions.Add(new RowDefinition(GridLength.Star));
        }

        /// <summary>
        /// Builds the properties cell edit.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="referencePath">The reference path.</param>
        /// <param name="properties">The properties.</param>
        /// <param name="grid">The grid.</param>
        /// <param name="container">The container.</param>
        private void BuildPropertiesCellEdit(
            object target,
            ReferencePath referencePath,
            IEnumerable<PropertyDescriptor> properties,
            Grid grid,
            IPropertyGridCellInfoContainer container)
        {
            var atLeastOnePropertyOperation = false;
            var list = new List<BuildPropertyCellEditResult>();
            
            foreach (var property in properties)
            {
                referencePath.BeginScope(property.Name);
                try
                {
                    var result = BuildPropertyCellEdit(target, referencePath, property, grid, container);
                    atLeastOnePropertyOperation |= result.ResultType.HasFlag(BuildPropertyCellEditResultType.PropertyOperationVisible);
                    list.Add(result);
                }
                finally
                {
                    referencePath.EndScope();
                }
            }

            if (atLeastOnePropertyOperation && CellEditAlignment != CellEditAlignmentType.Compact)
            {
                foreach (var result in list)
                {
                    if (!result.ResultType.HasFlag(BuildPropertyCellEditResultType.PropertyOperationVisible))
                    {
                        result.Context.CellEdit?.SetValue(Grid.ColumnSpanProperty,
                            result.ResultType.HasFlag(BuildPropertyCellEditResultType.InlineMode) ? 3 : 2);    
                    }
                }
            }
        }

        [Flags]
        private enum BuildPropertyCellEditResultType
        {
            // ReSharper disable once UnusedMember.Local
            Failure = 0,
            // ReSharper disable once UnusedMember.Local
            Success = 1<<0,
            InlineMode = 1<<1,
            PropertyOperationVisible = 1<<2
        }

        private struct BuildPropertyCellEditResult
        {
            public BuildPropertyCellEditResultType ResultType;
            public PropertyCellContext Context;
        }

        /// <summary>
        /// Builds the property cell edit.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="referencePath">The reference path.</param>
        /// <param name="propertyDescriptor">The property descriptor.</param>
        /// <param name="grid">The grid.</param>
        /// <param name="container">The container.</param>
        private BuildPropertyCellEditResult BuildPropertyCellEdit(
            object target,
            ReferencePath referencePath,
            PropertyDescriptor propertyDescriptor,
            Grid grid,
            IPropertyGridCellInfoContainer container)
        {
            var property = propertyDescriptor;
            
            var context = new PropertyCellContext(null, RootPropertyGrid ?? this, this, target, propertyDescriptor);
            var result = new BuildPropertyCellEditResult
            {
                Context = context
            };

            var control = Factories.BuildPropertyControl(context);

            if (control == null)
            {
#if DEBUG
                Debug.WriteLine($"Warning: Failed build property control for property:{property.Name}({property.PropertyType})(IsMultipleObject={context.Property is MultiObjectPropertyDescriptor})");
#endif
                return result;
            }

            Debug.Assert(context.Factory != null);
            Debug.Assert(context.CellEdit != null);
            Debug.Assert(context.CellEdit == control);
            
            var factory = context.Factory;
            factory.HandlePropertyChanged(context);
            grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));

            Control? nameControl = null;
            var shouldUseInlineMode = LayoutStyle == PropertyGridLayoutStyle.Inline && property.IsExpandableType();
            if (shouldUseInlineMode)
            {
                var propertyGrid = control.GetVisualChildren().OfType<PropertyGrid>().FirstOrDefault();
                var innerGrid = propertyGrid?.LogicalChildren.OfType<Grid>().FirstOrDefault();
                var innerPropertyGrid = innerGrid?.GetVisualChildren().OfType<Grid>()
                    .FirstOrDefault(item => item.Name == "PropertiesGrid");
                var innerExpander = innerPropertyGrid?.Children.OfType<Expander>().FirstOrDefault();
                innerExpander?.SetLocalizeBinding(HeaderedContentControl.HeaderProperty, property.DisplayName);

                result.ResultType |= BuildPropertyCellEditResultType.InlineMode;
            }
            else
            {
                var nameTextBlock = new TextBlock
                {
                    Margin = new Thickness(4),
                    VerticalAlignment = VerticalAlignment.Center
                };

                var args = new CustomNameBlockEventArgs(context, nameTextBlock);
                RaiseEvent(args);

                nameControl = args.CustomNameBlock ?? nameTextBlock;

                if (nameControl == nameTextBlock)
                {
                    nameTextBlock.SetInlinesBinding(propertyDescriptor.DisplayName, null, propertyDescriptor.GetCustomAttribute<UnitAttribute>());

                    if (property.GetCustomAttribute<DescriptionAttribute>() is { } descriptionAttribute && 
                        descriptionAttribute.Description.IsNotNullOrEmpty())
                    {
                        nameTextBlock.SetLocalizeBinding(ToolTip.TipProperty, descriptionAttribute.Description);
                    }
                }
                
                nameControl.SetValue(Grid.RowProperty, grid.RowDefinitions.Count - 1);
                nameControl.SetValue(Grid.ColumnProperty, 0);

                grid.Children.Add(nameControl);
            }

            control.SetValue(Grid.RowProperty, grid.RowDefinitions.Count - 1);
            control.SetValue(Grid.ColumnProperty, shouldUseInlineMode ? 0 : 1);
            control.SetValue(Grid.ColumnSpanProperty, shouldUseInlineMode ? 2 : 1);
            control.Margin = new Thickness(shouldUseInlineMode ? 0 : 4);
            factory.HandleReadOnlyStateChanged(control, context.IsReadOnly);

            control.GotFocus += (ss, ee) =>
            {
                var args = new PropertyGotFocusEventArgs(context);
                RaiseEvent(args);
            };
            
            control.LostFocus += (ss, ee) =>
            {
                var args = new PropertyLostFocusEventArgs(context);
                RaiseEvent(args);
            };

            grid.Children.Add(control);

            var cellInfo = new PropertyGridCellInfo(context)
            {
                ReferencePath = referencePath.ToString(),
                NameControl = nameControl,
                Category = (container as IPropertyGridCellInfo)?.Category ?? propertyDescriptor.Category,
                OwnerObject = target,
                Target = target,
                Container = (container as IPropertyGridCellInfo)?.Container,
                CellType = PropertyGridCellType.Cell
            };

            container.Add(cellInfo);

            if (AppendPropertyOperationUiIfNeed(context, grid, shouldUseInlineMode))
            {
                result.ResultType |= BuildPropertyCellEditResultType.PropertyOperationVisible;
            }
            
            return result;
        }

        private bool AppendPropertyOperationUiIfNeed(PropertyCellContext context, Grid grid, bool shouldUseInlineMode)
        {
            // Default = Hidden
            // so, you can force show operations for one property
            //
            if (IsPropertyOperationVisible(context.Property))
            {
                var args = new CustomPropertyOperationControlEventArgs(context);
                RaiseEvent(args);
                
                var operationControl = args.CustomControl ?? new Button
                {
                    Content = "?",
                    Margin = new Thickness(4, 0, 0, 0),
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                }; 
                
                operationControl.SetValue(Grid.ColumnProperty, shouldUseInlineMode ? 1: 2);
                operationControl.SetValue(Grid.RowProperty, grid.RowDefinitions.Count - 1);
                grid.Children.Add(operationControl);

                if (args.CustomControl == null && operationControl is Button button)
                {
                    var contextMenu = new ContextMenu();
                    button.ContextMenu = contextMenu;
                    
                    var initMenuArgs =
                        new CustomPropertyDefaultOperationEventArgs(context, PropertyDefaultOperationStageType.Init, button, contextMenu);
                    RaiseEvent(initMenuArgs);

                    button.Click += (s, e) =>
                    {
                        var customMenuArgs =
                            new CustomPropertyDefaultOperationEventArgs(context, PropertyDefaultOperationStageType.MenuOpening, button, contextMenu);
                        RaiseEvent(customMenuArgs);

                        if (contextMenu is { IsOpen: false, Items.Count: > 0 })
                        {
                            contextMenu.Open(button);
                        }
                    };
                }

                return true;
            }

            return false;
        }

        private bool IsPropertyOperationVisible(PropertyDescriptor propertyDescriptor)
        {
            var settings = PropertyOperationVisibility;
            
            if (settings == PropertyOperationVisibility.Hidden)
            {
                return false;
            }
            
            if (settings == PropertyOperationVisibility.Visible)
            {
                return true;
            }
            
            var propertyOperationVisibilityAttribute = propertyDescriptor.GetCustomAttribute<PropertyOperationVisibilityAttribute>();

            return propertyOperationVisibilityAttribute != null && propertyOperationVisibilityAttribute.Visibility != PropertyOperationVisibility.Hidden;
        }
        
        #endregion

        #region Alpha
        /// <summary>
        /// Builds the alphabetic properties view.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="referencePath">The reference path.</param>
        protected virtual void BuildAlphabeticPropertiesView(object target, ReferencePath referencePath)
        {
            PropertiesGrid.ColumnDefinitions.Clear();
            PropertiesGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
            PropertiesGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
            PropertiesGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));

            BuildPropertiesCellEdit(target, referencePath, ViewModel.AllProperties.OrderBy(x => x.DisplayName),  PropertiesGrid, _cellInfoCache);
        }

        /// <summary>
        /// Builds the builtin properties view.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="referencePath">The reference path.</param>
        protected virtual void BuildBuiltinPropertiesView(object target, ReferencePath referencePath)
        {
            PropertiesGrid.ColumnDefinitions.Clear();
            PropertiesGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
            PropertiesGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
            PropertiesGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));

            BuildPropertiesCellEdit(target, referencePath, ViewModel.AllProperties,  PropertiesGrid, _cellInfoCache);
        }
        #endregion

        #region Process Widths
        /// <summary>
        /// Synchronizes the width of the name.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="syncToTitle">if set to <c>true</c> [synchronize to title].</param>
        private void SyncNameWidth(double width, bool syncToTitle)
        {
            PropagateCellNameWidth(_cellInfoCache.Children, width);

            if (syncToTitle)
            {
                ColumnName.Width = width;
            }
        }

        private static void PropagateCellNameWidth(IEnumerable<IPropertyGridCellInfo> cells, double width)
        {
            foreach (var i in cells)
            {
                PropagateCellNameWidth(i.Children, width);

                if (i.NameControl != null)
                {
                    i.NameControl.Width = width;
                }
            }
        }

        /// <summary>
        /// Handles the <see cref="E:ColumnNamePropertyChanged" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="AvaloniaPropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnColumnNamePropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.Property == BoundsProperty)
            {
                var width = (sender as TextBlock)!.Bounds.Width;

                SyncNameWidth(width, false);
            }
        }
        #endregion

        #region Visibilities
        /// <summary>
        /// Refreshes the visibilities.
        /// </summary>
        /// <param name="filterText">The filter text.</param>
        private void RefreshVisibilities(string? filterText) => FilterCells(ViewModel, FilterCategory.Default, filterText);

        /// <summary>
        /// Filters the cells.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="category">The category.</param>
        /// <param name="filterText">The filter text.</param>
        /// <param name="filterMatchesParentCategory">Indicates whether the filter matches the parent category.</param>
        /// <returns>PropertyVisibility.</returns>
        public PropertyVisibility FilterCells(
            IPropertyGridFilterContext context, 
            FilterCategory category = FilterCategory.Default, 
            string? filterText = null,
            bool filterMatchesParentCategory = false)
        {
            var atLeastOneVisible = false;

            foreach (var info in _cellInfoCache.Children)
            {
                atLeastOneVisible |= context.PropagateVisibility(info, category, filterText, filterMatchesParentCategory) == PropertyVisibility.AlwaysVisible;
            }

            return atLeastOneVisible ? PropertyVisibility.AlwaysVisible : PropertyVisibility.HiddenByNoVisibleChildren;
        }
        #endregion

        #region Property Changed
        private void ClearPropertyChangedObservers(IEnumerable<IPropertyGridCellInfo> cells)
        {
            foreach (var i in cells)
            {
                i.CellPropertyChanged -= OnCellPropertyChanged;

                ClearPropertyChangedObservers(i.Children);
            }
        }

        private void AddPropertyChangedObservers(IEnumerable<IPropertyGridCellInfo> cells)
        {
            foreach (var i in cells)
            {
                i.CellPropertyChanged += OnCellPropertyChanged;

                AddPropertyChangedObservers(i.Children);
            }
        }

        private void OnCellPropertyChanged(object? sender, CellPropertyChangedEventArgs e)
        {
            if (e.Cell.Context?.Property != null && e.Cell.Context.Property.IsDefined<ConditionTargetAttribute>())
            {
                RefreshVisibilities(ViewModel.FilterPattern.FilterText);
            }
        }
        #endregion

        #region Common UI Event Handlers
        private void OnOptionsButtonClicked(object? sender, RoutedEventArgs e)
        {
            OptionsContextMenu.Open(sender as Control);
        }
        
        private void OnExpandAllClicked(object? sender, RoutedEventArgs e)
        {
            ExpandAllCategories();
        }

        private void OnCollapseAllClicked(object? sender, RoutedEventArgs e)
        {
            CollapseAllCategories();
        }
        #endregion

        #region IDisposable
        /// <summary>
        /// dispose object
        /// </summary>
#pragma warning disable CA1816
        public void Dispose()
#pragma warning restore CA1816
        {
            if (_rootPropertyGrid != null)
            {
                _rootPropertyGrid.PropertyChanged -= OnRootPropertyGridPropertyChanged;
            }
        }

        #endregion
    }

    #region Event Args
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

    /// <summary>
    /// allow use custom property operation control
    /// </summary>
    public class CustomPropertyOperationControlEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// input context
        /// </summary>
        public readonly PropertyCellContext Context;
        
        /// <summary>
        /// custom control
        /// </summary>
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public Control? CustomControl { get; set; }

        /// <summary>
        /// construct this event args
        /// </summary>
        public CustomPropertyOperationControlEventArgs(PropertyCellContext context) :
            base(PropertyGrid.CustomPropertyOperationControlEvent)
        {
            Context = context;
        }
    }

    /// <summary>
    /// custom default operation stage type
    /// </summary>
    public enum PropertyDefaultOperationStageType
    {
        /// <summary>
        /// initialize
        /// </summary>
        Init,
        
        /// <summary>
        /// menu opening
        /// </summary>
        MenuOpening
    }
    /// <summary>
    /// if you use default button for property operation
    /// you can add your custom menu items and default button
    /// </summary>
    public class CustomPropertyDefaultOperationEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// Context
        /// </summary>
        public readonly PropertyCellContext Context;
        
        /// <summary>
        /// stage type
        /// </summary>
        public readonly PropertyDefaultOperationStageType StageType;
        
        /// <summary>
        /// local menu
        /// </summary>
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public ContextMenu Menu { get; }
        
        /// <summary>
        /// internal button
        /// </summary>
        public Button DefaultButton { get; }

        /// <summary>
        /// constructor
        /// </summary>
        public CustomPropertyDefaultOperationEventArgs(
            PropertyCellContext context,
            PropertyDefaultOperationStageType stageType,
            Button defaultButton,
            ContextMenu menu
        ) : base(PropertyGrid.CustomPropertyOperationMenuOpeningEvent)
        {
            Context = context;
            StageType = stageType;
            DefaultButton = defaultButton;
            Menu = menu;
        }
    }
    
    /// <summary>
    /// Class CustomNameBlockEventArgs.
    /// Implements the <see cref="RoutedEventArgs" />
    /// </summary>
    /// <seealso cref="RoutedEventArgs" />
    public class CustomNameBlockEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// Context
        /// </summary>
        public readonly PropertyCellContext Context;

        /// <summary>
        /// Gets the default name block.
        /// </summary>
        /// <value>The default name block.</value>
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public Control DefaultNameBlock { get; }

        /// <summary>
        /// Gets or sets the custom name block.
        /// </summary>
        /// <value>The custom name block.</value>
        public Control? CustomNameBlock { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomNameBlockEventArgs"/> class.
        /// </summary>
        public CustomNameBlockEventArgs(
            PropertyCellContext context,
            Control defaultBlock
        ) : 
            base(PropertyGrid.CustomNameBlockEvent)
        {
            Context = context;
            DefaultNameBlock = defaultBlock;
            CustomNameBlock = defaultBlock;
        }
    }
    
    /// <summary>
    /// property got focus event args
    /// </summary>
    public class PropertyGotFocusEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// property cell context
        /// </summary>
        public readonly PropertyCellContext Context;
    
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="context"></param>
        public PropertyGotFocusEventArgs(PropertyCellContext context) : base(PropertyGrid.PropertyGotFocusEvent)
        {
            Context = context;
        }
    }
    
    /// <summary>
    /// property got focus event args
    /// </summary>
    public class PropertyLostFocusEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// property cell context
        /// </summary>
        public readonly PropertyCellContext Context;
    
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="context"></param>
        public PropertyLostFocusEventArgs(PropertyCellContext context) : base(PropertyGrid.PropertyLostFocusEvent)
        {
            Context = context;
        }
    }
    #endregion
}



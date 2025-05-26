using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Layout;
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
            get => GetValue(AllowFilterProperty); 
            set => SetValue(AllowFilterProperty, value);
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
            get => GetValue(AllowToggleViewProperty); 
            set => SetValue(AllowToggleViewProperty, value);
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
            get => GetValue(AllowQuickFilterProperty); 
            set => SetValue(AllowQuickFilterProperty, value);
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
            get => GetValue(ShowTitleProperty); 
            set => SetValue(ShowTitleProperty, value);
        }

        /// <summary>
        /// The display mode property
        /// </summary>
        public static readonly StyledProperty<PropertyGridDisplayMode> DisplayModeProperty = AvaloniaProperty.Register<PropertyGrid, PropertyGridDisplayMode>(nameof(DisplayMode));

        /// <summary>
        /// Gets or sets the display mode.
        /// </summary>
        /// <value>The display mode.</value>
        [Category("Views")]
        public PropertyGridDisplayMode DisplayMode
        {
            get => GetValue(DisplayModeProperty);
            set => SetValue(DisplayModeProperty, value);
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
        public static readonly StyledProperty<double> NameWidthProperty = AvaloniaProperty.Register<PropertyGrid, double>(nameof(NameWidth), 200);

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

        #region Events
        /// <summary>
        /// The custom name block event
        /// </summary>
        public static readonly RoutedEvent<CustomNameBlockEventArgs> CustomNameBlockEvent = 
            RoutedEvent.Register<PropertyGrid, CustomNameBlockEventArgs>(nameof(CustomNameBlock), RoutingStrategies.Bubble);

        /// <summary>
        /// Occurs when [custom name block].
        /// </summary>
        public event EventHandler<CustomNameBlockEventArgs> CustomNameBlock
        {
            add => AddHandler(CustomNameBlockEvent, value);
            remove => RemoveHandler(CustomNameBlockEvent, value);
        }
        
        /// <summary>
        /// custom property operation control event
        /// you can use this to custom your controls.
        /// assign your control to e.CustomControl
        /// </summary>
        public static readonly RoutedEvent<CustomPropertyOperationControlEventArgs> CustomPropertyOperationControlEvent = 
            RoutedEvent.Register<PropertyGrid, CustomPropertyOperationControlEventArgs>(nameof(CustomPropertyOperationControl), RoutingStrategies.Bubble);

        /// <summary>
        /// Occurs when [custom name block].
        /// </summary>
        public event EventHandler<CustomPropertyOperationControlEventArgs> CustomPropertyOperationControl
        {
            add => AddHandler(CustomPropertyOperationControlEvent, value);
            remove => RemoveHandler(CustomPropertyOperationControlEvent, value);
        }

        /// <summary>
        /// custom menu items
        /// </summary>
        public static readonly RoutedEvent<CustomPropertyOperationMenuEventArgs> CustomPropertyOperationMenuOpeningEvent = 
            RoutedEvent.Register<PropertyGrid, CustomPropertyOperationMenuEventArgs>(
                nameof(CustomPropertyOperationMenuOpening), 
                RoutingStrategies.Bubble
            );

        /// <summary>
        /// custom menu items
        /// </summary>
        public event EventHandler<CustomPropertyOperationMenuEventArgs> CustomPropertyOperationMenuOpening
        {
            add => AddHandler(CustomPropertyOperationMenuOpeningEvent, value);
            remove => RemoveHandler(CustomPropertyOperationMenuOpeningEvent, value);
        }

        
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
            _ = AllowFilterProperty.Changed.Subscribe(new AnonymousObserver<AvaloniaPropertyChangedEventArgs<bool>>(OnAllowFilterChanged));
            _ = AllowQuickFilterProperty.Changed.Subscribe(new AnonymousObserver<AvaloniaPropertyChangedEventArgs<bool>>(OnAllowQuickFilterChanged));
            _ = DisplayModeProperty.Changed.Subscribe(new AnonymousObserver<AvaloniaPropertyChangedEventArgs<PropertyGridDisplayMode>>(OnDisplayModeChanged));
            _ = ShowStyleProperty.Changed.Subscribe(new AnonymousObserver<AvaloniaPropertyChangedEventArgs<PropertyGridShowStyle>>(OnShowStyleChanged));
            _ = CategoryOrderStyleProperty.Changed.Subscribe(new AnonymousObserver<AvaloniaPropertyChangedEventArgs<PropertyGridOrderStyle>>(OnCategoryOrderStyleChanged));
            _ = PropertyOrderStyleProperty.Changed.Subscribe(new AnonymousObserver<AvaloniaPropertyChangedEventArgs<PropertyGridOrderStyle>>(OnPropertyOrderStyleChanged));
            _ = ShowTitleProperty.Changed.Subscribe(new AnonymousObserver<AvaloniaPropertyChangedEventArgs<bool>>(OnShowTitleChanged));
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

        private void BroadcastCustomPropertyDescriptorFilterEvent(object? sender, CustomPropertyDescriptorFilterEventArgs e) => RaiseEvent(e);

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
            if (e.PropertyName == nameof(ViewModel.DisplayMode))
            {
                DisplayMode = ViewModel.DisplayMode;
                BuildPropertiesView();
            }
            else if (e.PropertyName == nameof(ViewModel.ShowStyle))
            {
                ShowStyle = ViewModel.ShowStyle;
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
        private void OnAllowFilterChanged(object? oldValue, object? newValue) => HeaderGrid.IsVisible = (bool)newValue!;

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
        private static void OnDisplayModeChanged(AvaloniaPropertyChangedEventArgs<PropertyGridDisplayMode> e)
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
        private void OnDisplayModeChanged(Optional<PropertyGridDisplayMode> oldValue, BindingValue<PropertyGridDisplayMode> newValue) => ViewModel.DisplayMode = newValue.Value;

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
        private void OnShowStyleChanged(Optional<PropertyGridShowStyle> oldValue, BindingValue<PropertyGridShowStyle> newValue) => ViewModel.ShowStyle = newValue.Value;

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

        private static void OnAllowQuickFilterChanged(AvaloniaPropertyChangedEventArgs<bool> e)
        {
            if (e.Sender is PropertyGrid sender)
            {
                sender.OnAllowQuickFilterChanged(e.OldValue.Value, e.NewValue.Value);
            }
        }

        private void OnAllowQuickFilterChanged(bool oldValue, bool newValue) => FastFilterBox.IsVisible = newValue;

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
        #endregion

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


        /// <inheritdoc />
        public void ExpandAllCategories() => 
            _categoryExpanders.ForEach(e => e.IsExpanded = true);

        /// <inheritdoc />
        public void CollapseAllCategories() => 
            _categoryExpanders.ForEach(e => e.IsExpanded = false);

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

                if (ViewModel.ShowStyle == PropertyGridShowStyle.Category)
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

                        return tb;
                    })
                };
                _ = expander.SetValue(Grid.RowProperty, PropertiesGrid.RowDefinitions.Count - 1);
                expander.IsExpanded = AllCategoriesExpanded && autoCollapseCategoriesAttribute?.ShouldAutoCollapse(categoryInfo.Key) != true;
                expander.HorizontalContentAlignment = HorizontalAlignment.Stretch;
                expander.HorizontalAlignment = HorizontalAlignment.Stretch;
                expander.Margin = new Thickness(DisplayMode == PropertyGridDisplayMode.Inline ? 0 : 2);
                expander.Padding = new Thickness(DisplayMode == PropertyGridDisplayMode.Inline ? 0 : 2);

                // expander.Header = categoryInfo.Key;
                expander.SetLocalizeBinding(HeaderedContentControl.HeaderProperty, categoryInfo.Key);
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
            foreach (var property in properties)
            {
                referencePath.BeginScope(property.Name);
                try
                {
                    // var value = property.GetValue(target);
                    BuildPropertyCellEdit(target, referencePath, property, grid, container);
                }
                finally
                {
                    referencePath.EndScope();
                }
            }
        }

        /// <summary>
        /// Builds the property cell edit.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="referencePath">The reference path.</param>
        /// <param name="propertyDescriptor">The property descriptor.</param>
        /// <param name="grid">The grid.</param>
        /// <param name="container">The container.</param>
        private void BuildPropertyCellEdit(
            object target,
            ReferencePath referencePath,
            PropertyDescriptor propertyDescriptor,
            Grid grid,
            IPropertyGridCellInfoContainer container)
        {
            var property = propertyDescriptor;

            var context = new PropertyCellContext(null, RootPropertyGrid ?? this, this, target, propertyDescriptor);

            var control = Factories.BuildPropertyControl(context);

            if (control == null)
            {
#if DEBUG
                Debug.WriteLine($"Warning: Failed build property control for property:{property.Name}({property.PropertyType}");
#endif
                return;
            }

            Debug.Assert(context.Factory != null);
            Debug.Assert(context.CellEdit != null);
            Debug.Assert(context.CellEdit == control);

            var factory = context.Factory;
            factory.HandlePropertyChanged(context);
            grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));

            Control? nameControl = null;
            var shouldUseInlineMode = DisplayMode == PropertyGridDisplayMode.Inline && property.IsExpandableType();
            if (shouldUseInlineMode)
            {
                var propertyGrid = control.GetVisualChildren().OfType<PropertyGrid>().FirstOrDefault();
                var innerGrid = propertyGrid?.LogicalChildren.OfType<Grid>().FirstOrDefault();
                var innerPropertyGrid = innerGrid?.GetVisualChildren().OfType<Grid>()
                    .FirstOrDefault(item => item.Name == "PropertiesGrid");
                var innerExpander = innerPropertyGrid?.Children.OfType<Expander>().FirstOrDefault();
                innerExpander?.SetLocalizeBinding(HeaderedContentControl.HeaderProperty, property.DisplayName);
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

            AppendPropertyOperationUiIfNeed(context, grid, shouldUseInlineMode);
        }

        private void AppendPropertyOperationUiIfNeed(PropertyCellContext context, Grid grid, bool shouldUseInlineMode)
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

                    button.Click += (s, e) =>
                    {
                        contextMenu.Items.Clear();
                        
                        var customMenuArgs =
                            new CustomPropertyOperationMenuEventArgs(context, contextMenu);
                        RaiseEvent(customMenuArgs);

                        if (contextMenu is { IsOpen: false, Items.Count: > 0 })
                        {
                            contextMenu.Open(button);
                        }
                    };
                }
            }
        }

        private bool IsPropertyOperationVisible(PropertyDescriptor propertyDescriptor)
        {
            var settings = PropertyOperationVisibility;
            var propertyOperationVisibilityAttribute = propertyDescriptor.GetCustomAttribute<PropertyOperationVisibilityAttribute>();

            if (propertyOperationVisibilityAttribute == null)
            {
                return settings == PropertyOperationVisibility.Visible;
            }

            if (settings == PropertyOperationVisibility.Hidden)
            {
                return propertyOperationVisibilityAttribute.Visibility == PropertyOperationVisibility.Visible;
            }
            
            return propertyOperationVisibilityAttribute.Visibility != PropertyOperationVisibility.Hidden;
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
    /// if you use default button for property operation
    /// you can add your custom menu items
    /// </summary>
    public class CustomPropertyOperationMenuEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// Context
        /// </summary>
        public PropertyCellContext Context;
        
        /// <summary>
        /// local menu
        /// </summary>
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public ContextMenu Menu { get; }

        /// <summary>
        /// constructor
        /// </summary>
        public CustomPropertyOperationMenuEventArgs(
            PropertyCellContext context,
            ContextMenu menu
        ) : base(PropertyGrid.CustomPropertyOperationMenuOpeningEvent)
        {
            Context = context;
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
    #endregion
}



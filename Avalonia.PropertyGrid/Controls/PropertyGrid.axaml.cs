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
        /// The view model
        /// </summary>
        internal PropertyGridViewModel ViewModel { get; } = new();

        /// <summary>
        /// The factories
        /// </summary>
        public readonly ICellEditFactoryCollection Factories;

        private readonly PropertyGridExpandableCache _expandableObjectCache = new ();
        private readonly PropertyGridCellInfoCache _cellInfoCache = new ();

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
                BuildPropertiesView();
            }
            else if(e.PropertyName == nameof(ViewModel.CategoryOrderStyle))
            {
                CategoryOrderStyle = ViewModel.CategoryOrderStyle;
                BuildPropertiesView();
            }
            else if(e.PropertyName == nameof(ViewModel.PropertyOrderStyle))
            {
                PropertyOrderStyle = ViewModel.PropertyOrderStyle;
                BuildPropertiesView();
            }
            else if(e.PropertyName == nameof(ViewModel.IsReadOnly))
            {
                IsReadOnly = ViewModel.IsReadOnly;
                BuildPropertiesView();
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
            return _expandableObjectCache;
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
            return _cellInfoCache;
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
            SplitterGrid.ColumnDefinitions[0].Width = new GridLength((double)newValue!);
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
            SplitterGrid.IsVisible = newValue;
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
            BuildPropertiesView();
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
                else if(ViewModel.PropertyOrderStyle == PropertyGridOrderStyle.Alphabetic)
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

            RefreshVisibilities();

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
        protected virtual void BuildCategoryPropertiesView(object target, ReferencePath referencePath, PropertyGridOrderStyle categoryStyle, PropertyGridOrderStyle propertyOrderStyle)
        {
            PropertiesGrid.ColumnDefinitions.Clear();

            var categories = ViewModel.Categories;

            if (categoryStyle == PropertyGridOrderStyle.Alphabetic)
            {
                categories = categories.OrderBy(x => x.Key).ToList();
            }

            var autoCollapseCategoriesAttribute = target.GetType().GetAnyCustomAttribute<AutoCollapseCategoriesAttribute>();

            foreach (var categoryInfo in categories)
            {
                PropertiesGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));

                var expander = new Expander
                {
                    ExpandDirection = ExpandDirection.Down
                };
                expander.SetValue(Grid.RowProperty, PropertiesGrid.RowDefinitions.Count - 1);
                expander.IsExpanded = autoCollapseCategoriesAttribute == null || !autoCollapseCategoriesAttribute.ShouldAutoCollapse(categoryInfo.Key);
                expander.HorizontalContentAlignment = HorizontalAlignment.Stretch;
                expander.HorizontalAlignment = HorizontalAlignment.Stretch;
                expander.Margin = new Thickness(2);
                expander.Padding = new Thickness(2);

                // expander.Header = categoryInfo.Key;
                expander.SetLocalizeBinding(HeaderedContentControl.HeaderProperty, categoryInfo.Key);

                var grid = new Grid();
                grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
                grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));

                var cellInfo = new PropertyGridCellInfo(null)
                {
                    ReferencePath = $"{referencePath}[{categoryInfo.Key}]",
                    Category = categoryInfo.Key,
                    OwnerObject = target,
                    Container = expander,
                    CellType = PropertyGridCellType.Category
                };

                _cellInfoCache.Add(cellInfo);

                var properties = propertyOrderStyle == PropertyGridOrderStyle.Builtin ? categoryInfo.Value : categoryInfo.Value.OrderBy(x => x.DisplayName).ToList();

                BuildPropertiesCellEdit(target, referencePath, properties, expander, grid, cellInfo);

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
        /// <param name="expander">The expander.</param>
        /// <param name="grid">The grid.</param>
        /// <param name="container">The container.</param>
        private void BuildPropertiesCellEdit(
            object target,
            ReferencePath referencePath,
            IEnumerable<PropertyDescriptor> properties,
            Expander? expander,
            Grid grid,
            IPropertyGridCellInfoContainer container
            )
        {
            foreach (var property in properties)
            {
                referencePath.BeginScope(property.Name);
                try
                {
                    // var value = property.GetValue(target);

                    BuildPropertyCellEdit(target, referencePath, property, expander, grid, container);
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
        /// <param name="expander">The expander.</param>
        /// <param name="grid">The grid.</param>
        /// <param name="container">The container.</param>
        private void BuildPropertyCellEdit(
            object target,
            ReferencePath referencePath,
            PropertyDescriptor propertyDescriptor,
            Expander? expander,
            Grid grid,
            IPropertyGridCellInfoContainer container
            )
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

            grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));

            var nameBlock = new TextBlock();
            nameBlock.SetValue(Grid.RowProperty, grid.RowDefinitions.Count - 1);
            nameBlock.SetValue(Grid.ColumnProperty, 0);
            nameBlock.VerticalAlignment = VerticalAlignment.Center;
            nameBlock.Margin = new Thickness(4);

            // nameBlock.Text = LocalizationService.Default[property.DisplayName];
            nameBlock.SetLocalizeBinding(TextBlock.TextProperty, property.DisplayName);

            if (property.GetCustomAttribute<DescriptionAttribute>() is { } descriptionAttribute && descriptionAttribute.Description.IsNotNullOrEmpty())
            {
                // nameBlock.SetValue(ToolTip.TipProperty, LocalizationService.Default[descriptionAttribute.Description]);
                nameBlock.SetLocalizeBinding(ToolTip.TipProperty, descriptionAttribute.Description);
            }

            grid.Children.Add(nameBlock);

            control.SetValue(Grid.RowProperty, grid.RowDefinitions.Count - 1);
            control.SetValue(Grid.ColumnProperty, 1);
            control.Margin = new Thickness(4);
            factory.HandleReadOnlyStateChanged(control, context.IsReadOnly);

            grid.Children.Add(control);

            factory.HandlePropertyChanged(context);

            var cellInfo = new PropertyGridCellInfo(context)
            {
                ReferencePath = referencePath.ToString(),
                NameControl = nameBlock,
                Category = (container as IPropertyGridCellInfo)?.Category ?? propertyDescriptor.Category,
                OwnerObject = target,
                Target = target,
                Container = (container as IPropertyGridCellInfo)?.Container,
                CellType = PropertyGridCellType.Cell
            };

            container.Add(cellInfo);
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

            BuildPropertiesCellEdit(target, referencePath, ViewModel.AllProperties.OrderBy(x => x.DisplayName), null, PropertiesGrid, _cellInfoCache);
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

            BuildPropertiesCellEdit(target, referencePath, ViewModel.AllProperties, null, PropertiesGrid, _cellInfoCache);
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
            if (!ShowTitle)
            {
                return;
            }

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
        private void RefreshVisibilities()
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
            var atleastOneVisible = false;

            foreach (var info in _cellInfoCache.Children)
            {
                atleastOneVisible |= context.PropagateVisibility(info, category) == PropertyVisibility.AlwaysVisible;
            }

            return atleastOneVisible ? PropertyVisibility.AlwaysVisible : PropertyVisibility.HiddenByNoVisibleChildren;
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
                RefreshVisibilities();
            }
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

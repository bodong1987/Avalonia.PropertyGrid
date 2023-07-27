using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Dialogs.Internal;
using Avalonia.Interactivity;
using Avalonia.PropertyGrid.Controls.Implements;
using Avalonia.PropertyGrid.Model.ComponentModel;
using Avalonia.PropertyGrid.Model.ComponentModel.DataAnnotations;
using Avalonia.PropertyGrid.Model.Extensions;
using Avalonia.PropertyGrid.Model.Services;
using Avalonia.PropertyGrid.Model.Utils;
using Avalonia.PropertyGrid.Services;
using Avalonia.PropertyGrid.ViewModels;
using Avalonia.Threading;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

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
        public static readonly StyledProperty<PropertyGridShowStyle> ShowStyleProperty = AvaloniaProperty.Register<PropertyGrid, PropertyGridShowStyle>(nameof(ShowStyle), PropertyGridShowStyle.Category);
        /// <summary>
        /// Gets or sets the show style.
        /// </summary>
        /// <value>The show style.</value>
        [Category("Views")]
        public PropertyGridShowStyle ShowStyle
        {
            get => GetValue(ShowStyleProperty); 
            set 
            {
                SetValue(ShowStyleProperty, value);                
            }
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
        /// The selected object
        /// </summary>
        private object _SelectedObject;
        /// <summary>
        /// The selected object property
        /// </summary>
        public static readonly DirectProperty<PropertyGrid, object> SelectedObjectProperty = AvaloniaProperty.RegisterDirect<PropertyGrid, object>(
            nameof(SelectedObject),
            o => o._SelectedObject,
            (o,v)=> o.SetAndRaise(SelectedObjectProperty, ref o._SelectedObject, v)
            );
        /// <summary>
        /// Gets or sets the selected object.
        /// </summary>
        /// <value>The selected object.</value>
        [Browsable(false)]
        public object SelectedObject
        {
            get => _SelectedObject;
            set => SetAndRaise(SelectedObjectProperty, ref _SelectedObject, value);
        }

        /// <summary>
        /// The view model
        /// </summary>
        internal PropertyGridViewModel ViewModel { get; private set; } = new PropertyGridViewModel();

        /// <summary>
        /// The factories
        /// </summary>
        public readonly ICellEditFactoryCollection Factories;

        readonly IExpandableObjectCache ExpandableObjectCache = new PropertyGridExpandableCache();
        readonly IPropertyGridCellInfoCache CellInfoCache = new PropertyGridCellInfoCache();

        /// <summary>
        /// Gets or sets the root property grid.
        /// </summary>
        /// <value>The root property grid.</value>
        public IPropertyGrid RootPropertyGrid { get; set; }

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
            AllowFilterProperty.Changed.Subscribe(OnAllowFilterChanged);
            AllowQuickFilterProperty.Changed.Subscribe(OnAllowQuickFilterChanged);
            ShowStyleProperty.Changed.Subscribe(OnShowStyleChanged);
            ShowTitleProperty.Changed.Subscribe(OnShowTitleChanged);
            SelectedObjectProperty.Changed.Subscribe(OnSelectedObjectChanged);
            NameWidthProperty.Changed.Subscribe(OnNameWidthChanged);
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

            InitializeComponent();

            column_name.PropertyChanged += OnColumnNamePropertyChanged;
        }

        /// <summary>
        /// Handles the <see cref="E:ViewModelPropertyChanged" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(ViewModel.ShowCategory))
            {
                ShowStyle = ViewModel.ShowCategory ? PropertyGridShowStyle.Category : PropertyGridShowStyle.Alphabetic;
                BuildPropertiesView(ViewModel.SelectedObject, ViewModel.ShowCategory ? PropertyGridShowStyle.Category : PropertyGridShowStyle.Alphabetic);
            }
        }

        /// <summary>
        /// Called when [selected object changed].
        /// </summary>
        /// <param name="e">The e.</param>
        private static void OnSelectedObjectChanged(AvaloniaPropertyChangedEventArgs<object> e)
        {
            if(e.Sender is PropertyGrid pg)
            {
                pg.OnSelectedObjectChanged(e.OldValue.Value, e.NewValue.Value);
            }
        }

        /// <summary>
        /// Called when [selected object changed].
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnSelectedObjectChanged(object oldValue, object newValue)
        {
            ViewModel.SelectedObject = newValue;
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
            return ExpandableObjectCache;
        }

        /// <summary>
        /// Clones the property grid.
        /// </summary>
        /// <returns>IPropertyGrid.</returns>
        public virtual IPropertyGrid ClonePropertyGrid()
        {
            return Activator.CreateInstance(GetType()) as IPropertyGrid;
        }

        /// <summary>
        /// Gets the cell information cache.
        /// </summary>
        /// <returns>IPropertyGridCellInfoCache.</returns>
        public IPropertyGridCellInfoCache GetCellInfoCache()
        {
            return CellInfoCache;
        }

        #region Styled Properties Handler
        /// <summary>
        /// Handles the <see cref="E:AllowFilterChanged" /> event.
        /// </summary>
        /// <param name="e">The <see cref="AvaloniaPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnAllowFilterChanged(AvaloniaPropertyChangedEventArgs e)
        {
            if(e.Sender is PropertyGrid sender)
            {
                sender.OnAllowFilterChanged(e.OldValue, e.NewValue);
            }
        }

        /// <summary>
        /// Called when [allow filter changed].
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnAllowFilterChanged(object oldValue, object newValue)
        {            
            headerGrid.IsVisible = (bool)newValue;
        }

        private static void OnNameWidthChanged(AvaloniaPropertyChangedEventArgs e)
        {
            if(e.Sender is PropertyGrid sender)
            {
                sender.OnNameWidthChanged(e.OldValue, e.NewValue);
            }
        }

        private void OnNameWidthChanged(object oldValue, object newValue)
        {
            this.splitterGrid.ColumnDefinitions[0].Width = new GridLength((double)newValue);
        }

        /// <summary>
        /// Called when [show style changed].
        /// </summary>
        /// <param name="e">The e.</param>
        private static void OnShowStyleChanged(AvaloniaPropertyChangedEventArgs<PropertyGridShowStyle> e)
        {
            if(e.Sender is PropertyGrid sender)
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
            ViewModel.ShowCategory = newValue.Value == PropertyGridShowStyle.Category;
        }

        private static void OnShowTitleChanged(AvaloniaPropertyChangedEventArgs<bool> e)
        {
            if(e.Sender is PropertyGrid sender)
            {
                sender.OnShowTitleChanged(e.OldValue.Value, e.NewValue.Value);
            }
        }

        private void OnShowTitleChanged(bool oldValue, bool newValue)
        {
            splitterGrid.IsVisible = newValue;
        }


        private static void OnAllowQuickFilterChanged(AvaloniaPropertyChangedEventArgs<bool> e)
        {
            if(e.Sender is PropertyGrid sender)
            {
                sender.OnAllowQuickFilterChanged(e.OldValue.Value, e.NewValue.Value);
            }
        }

        private void OnAllowQuickFilterChanged(bool oldValue, bool newValue)
        {
            fastFilterBox.IsVisible = newValue;
        }

        #endregion

        /// <summary>
        /// Handles the <see cref="E:PropertyDescriptorChanged" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void OnPropertyDescriptorChanged(object sender, EventArgs e)
        {
            BuildPropertiesView(ViewModel.SelectedObject, ViewModel.ShowCategory ? PropertyGridShowStyle.Category : PropertyGridShowStyle.Alphabetic);
        }


        /// <summary>
        /// Handles the <see cref="E:FilterChanged" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void OnFilterChanged(object sender, EventArgs e)
        {
            RefreshVisibilities();
        }
               
        /// <summary>
        /// Builds the properties view.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="propertyGridShowStyle">The property grid show style.</param>
        private void BuildPropertiesView(object target, PropertyGridShowStyle propertyGridShowStyle)
        {
            propertiesGrid.RowDefinitions.Clear();
            propertiesGrid.Children.Clear();              
            ExpandableObjectCache.Clear();

            ClearPropertyChangedObservers(CellInfoCache.Children);
            CellInfoCache.Clear();

            if (target == null)
            {
                return;
            }

            ReferencePath referencePath = new ReferencePath();
            
            try
            {
                ExpandableObjectCache.Add(target);

                referencePath.BeginScope(target.GetType().Name);

                if (propertyGridShowStyle == PropertyGridShowStyle.Category)
                {
                    BuildCategoryPropertiesView(target, referencePath);
                }
                else if (propertyGridShowStyle == PropertyGridShowStyle.Alphabetic)
                {
                    BuildAlphabeticPropertiesView(target, referencePath);
                }
            }
            finally
            {
                referencePath.EndScope();
            }

            AddPropertyChangedObservers(CellInfoCache.Children);

            RefreshVisibilities();

            double width = column_name.Bounds.Width;

            SyncNameWidth(width, false);
        }

        #region Categories
        /// <summary>
        /// Builds the category properties view.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="referencePath">The reference path.</param>
        protected virtual void BuildCategoryPropertiesView(object target, ReferencePath referencePath)
        {
            propertiesGrid.ColumnDefinitions.Clear();

            foreach (var categoryInfo in ViewModel.Categories)
            {
                propertiesGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));

                Expander expander = new Expander();
                expander.ExpandDirection = ExpandDirection.Down;
                expander.Header = categoryInfo.Key;
                expander.SetValue(Grid.RowProperty, propertiesGrid.RowDefinitions.Count - 1);
                expander.IsExpanded = true;
                expander.HorizontalContentAlignment = Layout.HorizontalAlignment.Stretch;
                expander.HorizontalAlignment = Layout.HorizontalAlignment.Stretch;
                expander.Margin = new Thickness(2);
                expander.Padding = new Thickness(2);

                Grid grid = new Grid();
                grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
                grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));

                var cellInfo = new PropertyGridCellInfo(null)
                {
                    ReferencePath = $"{referencePath.ToString()}[{categoryInfo.Key}]",
                    Category = categoryInfo.Key,
                    OwnerObject = target,
                    Container = expander,
                    CellType = PropertyGridCellType.Category
                };

                CellInfoCache.Add(cellInfo);

                BuildPropertiesCellEdit(target, referencePath, categoryInfo.Value, expander, grid, cellInfo);

                expander.Content = grid;

                propertiesGrid.Children.Add(expander);
            }

            propertiesGrid.RowDefinitions.Add(new RowDefinition(GridLength.Star));
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
            Expander expander, 
            Grid grid,
            IPropertyGridCellInfoContainer container
            )
        {
            foreach(var property in properties)
            {
                referencePath.BeginScope(property.Name);
                try
                {
                    var value = property.GetValue(target);

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
            Expander expander, 
            Grid grid,
            IPropertyGridCellInfoContainer container
            )
        {
            var property = propertyDescriptor;
                        
            PropertyCellContext context = new PropertyCellContext(RootPropertyGrid ?? this, this, target, propertyDescriptor);

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

            ICellEditFactory factory = context.Factory;

            grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));

            TextBlock nameBlock = new TextBlock();
            nameBlock.Text = LocalizationService.Default[property.DisplayName];
            nameBlock.SetValue(Grid.RowProperty, grid.RowDefinitions.Count - 1);
            nameBlock.SetValue(Grid.ColumnProperty, 0);
            nameBlock.VerticalAlignment = Layout.VerticalAlignment.Center;
            nameBlock.Margin = new Thickness(4);

            if (property.GetCustomAttribute<DescriptionAttribute>() is DescriptionAttribute descriptionAttribute && descriptionAttribute.Description.IsNotNullOrEmpty())
            {
                nameBlock.SetValue(ToolTip.TipProperty, LocalizationService.Default[descriptionAttribute.Description]);
            }

            grid.Children.Add(nameBlock);

            control.SetValue(Grid.RowProperty, grid.RowDefinitions.Count - 1);
            control.SetValue(Grid.ColumnProperty, 1);
            control.IsEnabled = control.IsEnabled && !property.IsReadOnly;
            control.Margin = new Thickness(4);

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

            container?.Add(cellInfo);
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
            propertiesGrid.ColumnDefinitions.Clear();
            propertiesGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
            propertiesGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));

            BuildPropertiesCellEdit(target, referencePath, ViewModel.AllProperties, null, propertiesGrid, CellInfoCache);
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
            if(!ShowTitle)
            {
                return;
            }

            PropagateCellNameWidth(CellInfoCache.Children, width);

            if (syncToTitle)
            {
                column_name.Width = width;
            }
        }

        private void PropagateCellNameWidth(IEnumerable<IPropertyGridCellInfo> cells, double width)
        {
            foreach(var i in cells)
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
        private void OnColumnNamePropertyChanged(object sender, AvaloniaPropertyChangedEventArgs e)
        {
            if(e.Property == TextBlock.BoundsProperty)
            {
                double width = (sender as TextBlock).Bounds.Width;

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
            FilterCells(ViewModel, FilterCategory.Default);
        }

        /// <summary>
        /// Filters the cells.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="category">The category.</param>
        /// <returns>PropertyVisibility.</returns>
        public PropertyVisibility FilterCells(IPropertyGridFilterContext context, FilterCategory category = FilterCategory.Default)
        {
            bool AtleastOneVisible = false;

            foreach (var info in CellInfoCache.Children)
            {
                AtleastOneVisible |= context.PropagateVisibility(info, category) == PropertyVisibility.AlwaysVisible;
            }

            return AtleastOneVisible ? PropertyVisibility.AlwaysVisible : PropertyVisibility.HiddenByNoVisibleChidlren;
        }
        #endregion

        #region Property Changed
        private void ClearPropertyChangedObservers(IEnumerable<IPropertyGridCellInfo> cells)
        {
            foreach(var i in cells)
            {
                i.CellPropertyChanged -= OnCellPropertyChanged;

                ClearPropertyChangedObservers(i.Children);
            }
        }

        private void AddPropertyChangedObservers(IEnumerable<IPropertyGridCellInfo> cells)
        {
            foreach(var i in cells)
            {
                i.CellPropertyChanged += OnCellPropertyChanged;

                AddPropertyChangedObservers(i.Children);
            }
        }

        private void OnCellPropertyChanged(object sender, CellPropertyChangedEventArgs e)
        {
            if(e.Cell != null && e.Cell.Context.Property != null && e.Cell.Context.Property.IsDefined<ConditionTargetAttribute>())
            {
                RefreshVisibilities();
            }
        }

        #endregion
    }
}

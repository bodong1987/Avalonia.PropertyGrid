using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.PropertyGrid.Controls.Implements;
using Avalonia.PropertyGrid.Localization;
using Avalonia.PropertyGrid.ViewModels;
using PropertyModels.ComponentModel.DataAnnotations;
using PropertyModels.Extensions;

namespace Avalonia.PropertyGrid.Controls;

/// <summary>
/// Class PropertyGridTiledView.
/// Implements the <see cref="UserControl" />
/// Implements the <see cref="Avalonia.PropertyGrid.Controls.IPropertyGridView" />
/// </summary>
/// <seealso cref="UserControl" />
/// <seealso cref="Avalonia.PropertyGrid.Controls.IPropertyGridView" />
public partial class PropertyGridTiledView : UserControl, IPropertyGridView
{
    /// <summary>
    /// Gets the owner.
    /// </summary>
    /// <value>The owner.</value>
    public PropertyGrid? Owner { get; internal set; }
    /// <summary>
    /// Gets the view model.
    /// </summary>
    /// <value>The view model.</value>
    internal PropertyGridViewModel ViewModel => Owner!.ViewModel;
    /// <summary>
    /// The expandable object cache
    /// </summary>
    private readonly IExpandableObjectCache _expandableObjectCache = new PropertyGridExpandableCache();
    /// <summary>
    /// The cell information cache
    /// </summary>
    private readonly IPropertyGridCellInfoCache _cellInfoCache = new PropertyGridCellInfoCache();

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyGridTiledView"/> class.
    /// </summary>
    public PropertyGridTiledView()
    {
        InitializeComponent();
        
        ColumnName.PropertyChanged += OnColumnNamePropertyChanged;
    }

    /// <summary>
    /// get the view type of this instance
    /// </summary>
    /// <value>The type of the view.</value>
    public PropertyGridViewType ViewType => PropertyGridViewType.TiledView;


    /// <summary>
    /// is show title now
    /// </summary>
    /// <value><c>true</c> if [show title]; otherwise, <c>false</c>.</value>
    public bool ShowTitle
    {
        get => SplitterGrid.IsVisible;
        set => SplitterGrid.IsVisible = value;
    }

    /// <summary>
    /// Gets or sets the width of the name.
    /// </summary>
    /// <value>The width of the name.</value>
    public double NameWidth
    {
        get => SplitterGrid.ColumnDefinitions[0].Width.Value;
        set => SplitterGrid.ColumnDefinitions[0].Width = new GridLength(value);
    }

    /// <summary>
    /// call on enter this view state
    /// </summary>
    public void OnEnterState()
    {
        IsVisible = true;
    }

    /// <summary>
    /// call on leave this view state
    /// </summary>
    public void OnLeaveState()
    {
        IsVisible = false;
    }

    /// <summary>
    /// Refreshes this instance.
    /// </summary>
    public void Refresh()
    {
        BuildPropertiesView();
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
    /// Gets the cell information cache.
    /// </summary>
    /// <returns>IPropertyGridCellInfoCache.</returns>
    public IPropertyGridCellInfoCache GetCellInfoCache()
    {
        return _cellInfoCache;
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

        Owner!.RefreshVisibilities();

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

            foreach (var categoryInfo in categories)
            {
                PropertiesGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));

                var expander = new Expander
                {
                    ExpandDirection = ExpandDirection.Down
                };
                expander.SetValue(Grid.RowProperty, PropertiesGrid.RowDefinitions.Count - 1);
                expander.IsExpanded = true;
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

            var context = new PropertyCellContext(null, Owner!.RootPropertyGrid ?? Owner, Owner, target, propertyDescriptor);

            var control = Owner!.Factories.BuildPropertyControl(context);

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

    /// <summary>
    /// Propagates the width of the cell name.
    /// </summary>
    /// <param name="cells">The cells.</param>
    /// <param name="width">The width.</param>
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
    /// <param name="e">The <see cref="AvaloniaPropertyChangedEventArgs" /> instance containing the event data.</param>
    private void OnColumnNamePropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.Property == BoundsProperty)
            {
                var width = (sender as TextBlock)!.Bounds.Width;

                SyncNameWidth(width, false);
            }
        }
    #endregion

    #region Property Changed
    /// <summary>
    /// Clears the property changed observers.
    /// </summary>
    /// <param name="cells">The cells.</param>
    private void ClearPropertyChangedObservers(IEnumerable<IPropertyGridCellInfo> cells)
        {
            foreach (var i in cells)
            {
                i.CellPropertyChanged -= OnCellPropertyChanged;

                ClearPropertyChangedObservers(i.Children);
            }
        }

    /// <summary>
    /// Adds the property changed observers.
    /// </summary>
    /// <param name="cells">The cells.</param>
    private void AddPropertyChangedObservers(IEnumerable<IPropertyGridCellInfo> cells)
        {
            foreach (var i in cells)
            {
                i.CellPropertyChanged += OnCellPropertyChanged;

                AddPropertyChangedObservers(i.Children);
            }
        }

    /// <summary>
    /// Handles the <see cref="E:CellPropertyChanged" /> event.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="CellPropertyChangedEventArgs"/> instance containing the event data.</param>
    private void OnCellPropertyChanged(object? sender, CellPropertyChangedEventArgs e)
        {
            if (e.Cell.Context?.Property != null && e.Cell.Context.Property.IsDefined<ConditionTargetAttribute>())
            {
                Owner!.RefreshVisibilities();
            }
        }

        #endregion
}
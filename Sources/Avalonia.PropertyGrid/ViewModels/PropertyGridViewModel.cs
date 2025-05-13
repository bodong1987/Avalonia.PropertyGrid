using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.PropertyGrid.Controls;
using Avalonia.PropertyGrid.Controls.Implements;
using PropertyModels.ComponentModel;
using PropertyModels.ComponentModel.DataAnnotations;
using PropertyModels.Extensions;
using PropertyModels.Utils;

namespace Avalonia.PropertyGrid.ViewModels
{
    /// <summary>
    /// Enum PropertyGridDisplayMode
    /// </summary>
    public enum PropertyGridDisplayMode
    {
        /// <summary>
        /// Use tree.
        /// </summary>
        Tree,

        /// <summary>
        /// Use inline mode.
        /// </summary>
        Inline
    }

    /// <summary>
    /// Enum PropertyGridShowStyle
    /// </summary>
    public enum PropertyGridShowStyle
    {
        /// <summary>
        /// Use category.
        /// </summary>
        Category,

        /// <summary>
        /// Use tiled mode.
        /// </summary>
        Tiled
    }

    /// <summary>
    /// Enum PropertyGridOrderStyle
    /// </summary>
    public enum PropertyGridOrderStyle
    {
        /// <summary>
        /// Use internal order.
        /// </summary>
        Builtin,

        /// <summary>
        /// Use alphabetic order.
        /// </summary>
        Alphabetic
    }

    /// <summary>
    /// Enum PropertyVisibility
    /// </summary>
    [Flags]
    public enum PropertyVisibility
    {
        /// <summary>
        /// The always visible.
        /// </summary>
        AlwaysVisible = 0,

        /// <summary>
        /// The hidden by filter.
        /// </summary>
        HiddenByFilter = 1 << 0,

        /// <summary>
        /// The hidden by category filter.
        /// </summary>
        HiddenByCategoryFilter = 1 << 1,

        /// <summary>
        /// The hidden by no visible children.
        /// </summary>
        HiddenByNoVisibleChildren = 1 << 2,

        /// <summary>
        /// The hidden by condition.
        /// </summary>
        HiddenByCondition = 1 << 10
    }

    /// <summary>
    /// Class PropertyGridViewModel.
    /// Implements the <see cref="ReactiveObject" />
    /// </summary>
    /// <seealso cref="ReactiveObject" />
    internal class PropertyGridViewModel : ReactiveObject, IPropertyGridFilterContext
    {
        /// <summary>
        /// Gets or sets the filter pattern.
        /// </summary>
        /// <value>The filter pattern.</value>
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        public IFilterPattern FilterPattern { get; set; } = new PropertyGridFilterPattern();

        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        /// <value>The context.</value>
        public object? Context
        {
            get;
            set => this.RaiseAndSetIfChanged(ref field, value);
        }

        /// <summary>
        /// Gets or sets the display mode.
        /// </summary>
        /// <value>The display mode.</value>
        public PropertyGridDisplayMode DisplayMode
        {
            get;
            set
            {
                if (field != value)
                {
                    _ = this.RaiseAndSetIfChanged(ref field, value);
                }
            }
        } = PropertyGridDisplayMode.Tree;

        /// <summary>
        /// Gets or sets the show style.
        /// </summary>
        /// <value>The show style.</value>
        public PropertyGridShowStyle ShowStyle
        {
            get;
            set
            {
                if (field != value)
                {
                    _ = this.RaiseAndSetIfChanged(ref field, value);

                    RaisePropertyChanged(nameof(ShowStyleText));
                }
            }
        } = PropertyGridShowStyle.Category;

        /// <summary>
        /// Gets a value indicating whether [show style type].
        /// </summary>
        /// <value><c>null</c> if [show style type] contains no value, <c>true</c> if [show style type]; otherwise, <c>false</c>.</value>
        public bool IsCategoryView
        {
            get => ShowStyle == PropertyGridShowStyle.Category;
            set => ShowStyle = value ? PropertyGridShowStyle.Category : PropertyGridShowStyle.Tiled;
        }

        /// <summary>
        /// Gets the show style text.
        /// </summary>
        /// <value>The show style text.</value>
        public string ShowStyleText
        {
            get
            {
                return ShowStyle switch
                {
                    PropertyGridShowStyle.Tiled => "T",
                    _ => "C"
                };
            }
        }

        /// <summary>
        /// Gets the property order style
        /// </summary>
        /// <value>The show style </value>
        public PropertyGridOrderStyle PropertyOrderStyle
        {
            get;
            set
            {
                if (field != value)
                {
                    _ = this.RaiseAndSetIfChanged(ref field, value);
                }
            }
        } = PropertyGridOrderStyle.Builtin;

        /// <summary>
        /// Gets the category order style
        /// </summary>
        /// <value>The show style </value>
        public PropertyGridOrderStyle CategoryOrderStyle
        {
            get;
            set
            {
                if (field != value)
                {
                    _ = this.RaiseAndSetIfChanged(ref field, value);
                }
            }
        } = PropertyGridOrderStyle.Builtin;

        /// <summary>
        /// Gets the readonly flag
        /// </summary>
        /// <value>The readonly flag </value>
        public bool IsReadOnly
        {
            get;
            set
            {
                if (field != value)
                {
                    _ = this.RaiseAndSetIfChanged(ref field, value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the category filter.
        /// </summary>
        /// <value>The category filter.</value>
        public CheckedMaskModel? CategoryFilter
        {
            get;
            set => this.RaiseAndSetIfChanged(ref field, value);
        }

        /// <summary>
        /// Gets the fast filter pattern.
        /// </summary>
        /// <value>The fast filter pattern.</value>
        public ICheckedMaskModel? FastFilterPattern => CategoryFilter;

        /// <summary>
        /// Gets all properties.
        /// </summary>
        /// <value>All properties.</value>
        public List<PropertyDescriptor> AllProperties { get; } = [];

        /// <summary>
        /// Gets the categories.
        /// </summary>
        /// <value>The categories.</value>
        public List<KeyValuePair<string, List<PropertyDescriptor>>> Categories { get; } = [];

        /// <summary>
        /// Occurs when [filter changed].
        /// it means we need recreate all properties
        /// </summary>
        public event EventHandler? PropertyDescriptorChanged;

        /// <summary>
        /// Occurs when [filter changed].
        /// it means we need show or hide some property
        /// </summary>
        public event EventHandler<FilterChangedEventArgs>? FilterChanged;

        /// <summary>
        /// Occurs when [custom property descriptor filter].
        /// </summary>
        public event EventHandler<CustomPropertyDescriptorFilterEventArgs>? CustomPropertyDescriptorFilter;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyGridViewModel" /> class.
        /// </summary>
        public PropertyGridViewModel()
        {
            FilterPattern.PropertyChanged += OnPropertyChanged;
            PropertyChanged += OnPropertyChanged;
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            AllProperties.Clear();
            Categories.Clear();
        }

        /// <summary>
        /// Handles the <see cref="E:PropertyChanged" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs" /> instance containing the event data.</param>
        private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Context))
            {
                RefreshProperties();
            }
            else if (sender == FilterPattern)
            {
                FilterChanged?.Invoke(this, new FilterChangedEventArgs(FilterPattern.FilterText));
            }
        }

        /// <summary>
        /// Gets the category.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>System.String.</returns>
        public static string GetCategory(PropertyDescriptor property)
        {
            var category = string.IsNullOrEmpty(property.Category) ? "Misc" : property.Category;

            return category;
        }

        /// <summary>
        /// Propagates the visibility.
        /// </summary>
        /// <param name="cellInfo">The information.</param>
        /// <param name="category">The category.</param>
        /// <param name="filterText">The filter text.</param>
        /// <returns>
        /// The <see cref="PropertyVisibility"/> containing the result.
        /// </returns>
        public PropertyVisibility PropagateVisibility(
            IPropertyGridCellInfo cellInfo, 
            FilterCategory category = FilterCategory.Default, 
            string? filterText = null)
        {
            if (cellInfo.CellType == PropertyGridCellType.Category)
            {
                bool filterMatchesCategory = false;
                if (filterText.IsNotNullOrEmpty())
                {
                    filterMatchesCategory = cellInfo.Category?.Contains(filterText, StringComparison.CurrentCultureIgnoreCase) ?? false;
                }

                var atLeastOneVisible = false;
                foreach (var child in cellInfo.Children)
                {
                    var v = PropagateVisibility(child, child.Target, category, filterText);
                    switch (filterMatchesCategory && !v.HasFlag(PropertyVisibility.HiddenByCategoryFilter))
                    {
                        case true:
                            child.IsVisible = true;
                            atLeastOneVisible = true;
                            break;
                        case false:
                            atLeastOneVisible |= v == PropertyVisibility.AlwaysVisible;
                            break;
                    }
                }

                if (cellInfo.Container is { Presenter.Parent.Parent: DockPanel dockPanel } && 
                    dockPanel.Children.FirstOrDefault(item => item is ToggleButton) is 
                        ToggleButton { Presenter.Child: HighlightedTextBlock textBlock })
                {
                    textBlock.HighlightedText = filterText;
                }

                cellInfo.IsVisible = atLeastOneVisible;

                return atLeastOneVisible ? PropertyVisibility.AlwaysVisible : PropertyVisibility.HiddenByNoVisibleChildren;
            }

            return PropagateVisibility(cellInfo, cellInfo.Target, category);
        }

        /// <summary>
        /// Propagates the visibility.
        /// </summary>
        /// <param name="cellInfo">The information.</param>
        /// <param name="target">The target.</param>
        /// <param name="category">The category.</param>
        /// <param name="filterText">The filter text.</param>
        /// <returns>
        /// The <see cref="PropertyVisibility"/> containing the result.
        /// </returns>
        private PropertyVisibility PropagateVisibility(
            IPropertyGridCellInfo cellInfo, 
            object? target, 
            FilterCategory category = FilterCategory.Default,
            string? filterText = null)
        {
            var visibility = PropertyVisibility.AlwaysVisible;

            if (cellInfo.CellType == PropertyGridCellType.Cell)
            {
                Debug.Assert(cellInfo.Context != null);

                var property = cellInfo.Context.Property;

                Debug.Assert(property != null);

                PropertyVisibility? childrenVisibility = null;

                if (category.HasFlag(FilterCategory.Factory))
                {
                    childrenVisibility = cellInfo.Context.Factory?.HandlePropagateVisibility(target, cellInfo.Context, this);
                }

                if (category.HasFlag(FilterCategory.PropertyCondition))
                {
                    if (property.GetCustomAttribute<AbstractVisibilityConditionAttribute>() is { } attr)
                    {
                        if (!attr.CheckVisibility(target))
                        {
                            visibility |= PropertyVisibility.HiddenByCondition;
                        }
                    }
                }

                if (category.HasFlag(FilterCategory.Filter))
                {
                    if (!FilterPattern.Match(property, target))
                    {
                        if (childrenVisibility is not PropertyVisibility.AlwaysVisible)
                        {
                            visibility |= PropertyVisibility.HiddenByFilter;
                        }
                        else
                        {
                            visibility = PropertyVisibility.AlwaysVisible;
                        }
                    }
                }

                if (category.HasFlag(FilterCategory.Category))
                {
                    if (CategoryFilter != null && cellInfo.Category.IsNotNullOrEmpty() && !CategoryFilter.IsChecked(cellInfo.Category))
                    {
                        visibility |= PropertyVisibility.HiddenByCategoryFilter;
                    }
                }
            }
            else if (cellInfo.CellType == PropertyGridCellType.Category)
            {
                foreach (var child in cellInfo.Children)
                {
                    visibility |= PropagateVisibility(child, child.Target, category);
                }
            }

            cellInfo.IsVisible = visibility == PropertyVisibility.AlwaysVisible;

            if (cellInfo.NameControl is HighlightedTextBlock textBlock)
            {
                textBlock.HighlightedText = filterText;
            }

            return visibility;
        }

        /// <summary>
        /// Refreshes the properties.
        /// </summary>
        public void RefreshProperties()
        {
            Clear();

            if (Context == null)
            {
                CategoryFilter = null;
                PropertyDescriptorChanged?.Invoke(this, EventArgs.Empty);
                return;
            }

            var builder = new PropertyDescriptorBuilder(Context);
            AllProperties.AddRange(builder.GetProperties().Cast<PropertyDescriptor>().ToList().FindAll(
                x =>
                {
                    if (CustomPropertyDescriptorFilter != null)
                    {
                        var args = new CustomPropertyDescriptorFilterEventArgs(Controls.PropertyGrid.CustomPropertyDescriptorFilterEvent, Context, x);

                        CustomPropertyDescriptorFilter(this, args);

                        if (args.Handled)
                        {
                            return args.IsVisible;
                        }
                    }

                    return x.IsBrowsable && !x.IsDefined<IgnoreDataMemberAttribute>()/* compatible with ReactiveUI */;
                }
                )
            );

            var categories = new HashSet<string>();
            foreach (var property in AllProperties)
            {
                var category = GetCategory(property);

                _ = categories.Add(category);
            }

            CategoryFilter = new CheckedMaskModel(categories.OrderBy(x => x), "All");
            CategoryFilter.CheckChanged += OnCategoryFilterChanged;

            FilterProperties();

            PropertyDescriptorChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Handles the <see cref="E:CategoryFilterChanged" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void OnCategoryFilterChanged(object? sender, EventArgs e)
        {
            FilterProperties();

            FilterChanged?.Invoke(this, new FilterChangedEventArgs(FilterPattern.FilterText));
        }

        /// <summary>
        /// Filters the properties.
        /// </summary>
        public void FilterProperties()
        {
            Categories.Clear();

            foreach (var property in AllProperties)
            {
                var category = GetCategory(property);

                var index = Categories.IndexOf(x => x.Key == category);
                if (index == -1)
                {
                    var list = new List<PropertyDescriptor> { property };
                    Categories.Add(new KeyValuePair<string, List<PropertyDescriptor>>(category, list));
                }
                else
                {
                    Categories[index].Value.Add(property);
                }
            }
        }
    }
    
    /// <summary>
    /// Class FilterChangedEventArgs.
    /// </summary>
    public class FilterChangedEventArgs : EventArgs
    {
        /// <summary>
        /// The filter text.
        /// </summary>
        public readonly string? FilterText;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterChangedEventArgs" /> class.
        /// </summary>
        /// <param name="filterText">The filter text.</param>
        public FilterChangedEventArgs(string? filterText) :
            base()
        {
            FilterText = filterText;
        }
    }
}

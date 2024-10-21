using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using Avalonia.PropertyGrid.Controls;
using Avalonia.PropertyGrid.Controls.Implements;
using PropertyModels.ComponentModel;
using PropertyModels.ComponentModel.DataAnnotations;
using PropertyModels.Extensions;
using PropertyModels.Utils;

namespace Avalonia.PropertyGrid.ViewModels
{
    /// <summary>
    /// Enum PropertyGridShowStyle
    /// </summary>
    public enum PropertyGridShowStyle
    {
        /// <summary>
        /// use category
        /// </summary>
        Category,

        /// <summary>
        /// use tiled mode
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
        /// The always visible
        /// </summary>
        AlwaysVisible = 0,
        /// <summary>
        /// The hidden by filter
        /// </summary>
        HiddenByFilter = 1<<0,
        /// <summary>
        /// The hidden by category filter
        /// </summary>
        HiddenByCategoryFilter = 1<<1,
        /// <summary>
        /// The hidden by no visible children
        /// </summary>
        HiddenByNoVisibleChildren = 1<<2,
        /// <summary>
        /// The hidden by condition
        /// </summary>
        HiddenByCondition = 1<<10
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

        private object? _context;

        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        /// <value>The context.</value>
        public object? Context
        {
            get => _context;
            set => this.RaiseAndSetIfChanged(ref _context, value);
        }

        private PropertyGridShowStyle _showStyle = PropertyGridShowStyle.Category;

        /// <summary>
        /// Gets or sets the show style.
        /// </summary>
        /// <value>The show style.</value>
        public PropertyGridShowStyle ShowStyle
        {
            get => _showStyle;
            set
            {
                if(_showStyle != value)
                {
                    this.RaiseAndSetIfChanged(ref _showStyle, value);

                    RaisePropertyChanged(nameof(ShowStyleText));
                }                
            }
        }

        /// <summary>
        /// Gets a value indicating whether [show style type].
        /// </summary>
        /// <value><c>null</c> if [show style type] contains no value, <c>true</c> if [show style type]; otherwise, <c>false</c>.</value>
        public bool ShowStyleType
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


        private PropertyGridOrderStyle _propertyOrderStyle = PropertyGridOrderStyle.Builtin;

        /// <summary>
        /// Gets the property order style
        /// </summary>
        /// <value>The show style </value>
        public PropertyGridOrderStyle PropertyOrderStyle
        {
            get => _propertyOrderStyle;
            set
            {
                if (_propertyOrderStyle != value)
                {
                    this.RaiseAndSetIfChanged(ref _propertyOrderStyle, value);
                }
            }
        }

        private PropertyGridOrderStyle _categoryOrderStyle = PropertyGridOrderStyle.Builtin;

        /// <summary>
        /// Gets the category order style
        /// </summary>
        /// <value>The show style </value>
        public PropertyGridOrderStyle CategoryOrderStyle
        {
            get => _categoryOrderStyle;
            set
            {
                if(_categoryOrderStyle != value)
                {
                    this.RaiseAndSetIfChanged(ref _categoryOrderStyle, value);
                }
            }
        }

        private bool _isReadOnly;

        /// <summary>
        /// Gets the readonly flag
        /// </summary>
        /// <value>The readonly flag </value>
        public bool IsReadOnly
        {
            get => _isReadOnly;
            set
            {
                if(_isReadOnly != value)
                {
                    this.RaiseAndSetIfChanged(ref _isReadOnly, value);
                }
            }
        }

        /// <summary>
        /// The category filter
        /// </summary>
        private CheckedMaskModel? _categoryFilter;

        /// <summary>
        /// Gets or sets the category filter.
        /// </summary>
        /// <value>The category filter.</value>
        public CheckedMaskModel? CategoryFilter
        {
            get => _categoryFilter;
            set => this.RaiseAndSetIfChanged(ref _categoryFilter, value);
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
        public event EventHandler? FilterChanged;

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
                FilterChanged?.Invoke(this, EventArgs.Empty);
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
        /// <param name="info">The information.</param>
        /// <param name="category">The category.</param>
        public PropertyVisibility PropagateVisibility(IPropertyGridCellInfo info, FilterCategory category = FilterCategory.Default)
        {
            if (info.CellType == PropertyGridCellType.Category)
            {                
                var atLeastOneVisible = false;

                foreach (var child in info.Children)
                {
                    var v = PropagateVisibility(child, child.Target, category);

                    atLeastOneVisible |= (v == PropertyVisibility.AlwaysVisible);
                }

                info.IsVisible = atLeastOneVisible;

                return atLeastOneVisible ? PropertyVisibility.AlwaysVisible : PropertyVisibility.HiddenByNoVisibleChildren;
            }

            return PropagateVisibility(info, info.Target, category);
        }

        private PropertyVisibility PropagateVisibility(IPropertyGridCellInfo cellInfo, object? target, FilterCategory category = FilterCategory.Default)
        {
            var visibility = PropertyVisibility.AlwaysVisible;

            if(cellInfo.CellType == PropertyGridCellType.Cell)
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
                    
                if(category.HasFlag(FilterCategory.Filter))
                {
                    if (!FilterPattern.Match(property, target))
                    {
                        if(childrenVisibility is not PropertyVisibility.AlwaysVisible)
                        {
                            visibility |= PropertyVisibility.HiddenByFilter;
                        }
                        else
                        {
                            visibility = PropertyVisibility.AlwaysVisible;
                        }
                    }
                }                    

                if(category.HasFlag(FilterCategory.Category))
                {
                    if (CategoryFilter != null && cellInfo.Category.IsNotNullOrEmpty() && !CategoryFilter.IsChecked(cellInfo.Category))
                    {
                        visibility |= PropertyVisibility.HiddenByCategoryFilter;
                    }
                }        
            }
            else if(cellInfo.CellType == PropertyGridCellType.Category)
            {
                foreach(var child in cellInfo.Children)
                {
                    visibility |= PropagateVisibility(child, child.Target, category);
                }
            }

            cellInfo.IsVisible = visibility == PropertyVisibility.AlwaysVisible;

            return visibility;
        }

        /// <summary>
        /// Refreshes the properties.
        /// </summary>
        public void RefreshProperties()
        {
            Clear();

            if(_context == null)
            {
                CategoryFilter = null;
                PropertyDescriptorChanged?.Invoke(this, EventArgs.Empty);
                return;
            }

            var builder = new PropertyDescriptorBuilder(_context);
            AllProperties.AddRange(builder.GetProperties().Cast<PropertyDescriptor>().ToList().FindAll(
                x =>
                {
                    if(CustomPropertyDescriptorFilter != null)
                    {
                        var args = new CustomPropertyDescriptorFilterEventArgs(Controls.PropertyGrid.CustomPropertyDescriptorFilterEvent, _context, x);

                        CustomPropertyDescriptorFilter(this, args);

                        if(args.Handled)
                        {
                            return args.IsVisible;
                        }
					}

                    return x.IsBrowsable && !x.IsDefined<IgnoreDataMemberAttribute>()/* compatible with ReactiveUI */;
				}
                )
            );

            var categories = new HashSet<string>();
            foreach(var property in AllProperties)
            {
                var category = GetCategory(property);

                categories.Add(category);
            }

            CategoryFilter = new CheckedMaskModel(categories.OrderBy(x=>x), "All");
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

            FilterChanged?.Invoke(this, EventArgs.Empty);
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

                var index = Categories.IndexOf(x=> x.Key == category);
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
}

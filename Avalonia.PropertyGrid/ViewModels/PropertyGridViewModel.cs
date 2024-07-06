using Avalonia.PropertyGrid.Controls;
using Avalonia.PropertyGrid.Controls.Implements;
using PropertyModels.ComponentModel;
using PropertyModels.ComponentModel.DataAnnotations;
using PropertyModels.Extensions;
using PropertyModels.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;

namespace Avalonia.PropertyGrid.ViewModels
{
    /// <summary>
    /// Enum PropertyGridShowStyle
    /// </summary>
    public enum PropertyGridShowStyle
    {
        /// <summary>
        /// The category
        /// </summary>
        Category,

        /// <summary>
        /// The alphabetic
        /// </summary>
        Alphabetic,

        /// <summary>
        /// use internal order
        /// </summary>
        Builtin
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
        HiddenByCondition = 1<<10,
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
        public IFilterPattern FilterPattern { get; set; } = new PropertyGridFilterPattern();

        private object _context;

        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        /// <value>The context.</value>
        public object Context
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

                    RaisePropertyChanged(nameof(ShowStyleType));
                    RaisePropertyChanged(nameof(ShowStyleText));
                }                
            }
        }

        /// <summary>
        /// Gets a value indicating whether [show style type].
        /// </summary>
        /// <value><c>null</c> if [show style type] contains no value, <c>true</c> if [show style type]; otherwise, <c>false</c>.</value>
        public bool? ShowStyleType
        {
            get
            {
                return ShowStyle switch
                {
                    PropertyGridShowStyle.Category => true,
                    PropertyGridShowStyle.Alphabetic => false,
                    _ => null
                };
            }
            set
            {
                if(value == null)
                {
                    ShowStyle = PropertyGridShowStyle.Builtin;
                    return;
                }

                ShowStyle = (bool)value ? PropertyGridShowStyle.Category : PropertyGridShowStyle.Alphabetic;
            }
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
                    PropertyGridShowStyle.Category => "C",
                    PropertyGridShowStyle.Alphabetic => "A",
                    _ => "B"
                };
            }
        }

        /// <summary>
        /// The category filter
        /// </summary>
        private CheckedMaskModel _categoryFilter;
        /// <summary>
        /// Gets or sets the category filter.
        /// </summary>
        /// <value>The category filter.</value>
        public CheckedMaskModel CategoryFilter
        {
            get => _categoryFilter;
            set => this.RaiseAndSetIfChanged(ref _categoryFilter, value);
        }

        /// <summary>
        /// Gets the fast filter pattern.
        /// </summary>
        /// <value>The fast filter pattern.</value>
        public ICheckedMaskModel FastFilterPattern => CategoryFilter;

        /// <summary>
        /// Gets all properties.
        /// </summary>
        /// <value>All properties.</value>
        public List<PropertyDescriptor> AllProperties { get; private set; } = new();

        /// <summary>
        /// Gets the categories.
        /// </summary>
        /// <value>The categories.</value>
        public SortedList<string, List<PropertyDescriptor>> Categories { get; private set; } = new();

        /// <summary>
        /// Occurs when [filter changed].
        /// it means we need recreate all properties
        /// </summary>
        public event EventHandler PropertyDescriptorChanged;

        /// <summary>
        /// Occurs when [filter changed].
        /// it means we need show or hide some property
        /// </summary>
        public event EventHandler FilterChanged;

		/// <summary>
		/// Occurs when [custom property descriptor filter].
		/// </summary>
		public event EventHandler<CustomPropertyDescriptorFilterEventArgs> CustomPropertyDescriptorFilter;

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
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
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
                var atLeastOne = false;

                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (var child in info.Children)
                {
                    var v = PropagateVisibility(child, child.Target, category);

                    atLeastOne |= (v == PropertyVisibility.AlwaysVisible);
                }

                info.IsVisible = atLeastOne;

                return atLeastOne ? PropertyVisibility.AlwaysVisible : PropertyVisibility.HiddenByNoVisibleChildren;
            }
            else
            {
                return PropagateVisibility(info, info.Target, category);
            }
        }

        private PropertyVisibility PropagateVisibility(IPropertyGridCellInfo cellInfo, object target, FilterCategory category = FilterCategory.Default)
        {
            var visibility = PropertyVisibility.AlwaysVisible;

            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if(cellInfo.CellType == PropertyGridCellType.Cell)
            {
                var property = cellInfo.Context.Property;

                Debug.Assert(property != null);

                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (property != null)
                {
                    PropertyVisibility? propertyVisibility = null;

                    if (category.HasFlag(FilterCategory.Factory))
                    {
                        propertyVisibility = cellInfo.Context.Factory?.HandlePropagateVisibility(target, cellInfo.Context, this);
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
                            if(propertyVisibility is not PropertyVisibility.AlwaysVisible)
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
            }
            else if(cellInfo.CellType == PropertyGridCellType.Category)
            {
                // ReSharper disable once LoopCanBeConvertedToQuery
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
                return;
            }

            var builder = new PropertyDescriptorBuilder(_context);
            AllProperties.AddRange(builder.GetProperties().Cast<PropertyDescriptor>().ToList().FindAll(
                x =>
                {
                    if(CustomPropertyDescriptorFilter != null)
                    {
                        var args = new CustomPropertyDescriptorFilterEventArgs(PropertyGrid.Controls.PropertyGrid.CustomPropertyDescriptorFilterEvent, _context, x);

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
            
            foreach (var category in AllProperties.Select(GetCategory))
            {
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
        private void OnCategoryFilterChanged(object sender, EventArgs e)
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

                if (!Categories.TryGetValue(category, out var list))
                {
                    list = [property];
                    Categories.Add(category, list);
                }
                else
                {
                    list.Add(property);
                }
            }
        }
    }
}

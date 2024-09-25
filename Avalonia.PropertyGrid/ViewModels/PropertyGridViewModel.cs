using Avalonia.PropertyGrid.Controls;
using Avalonia.PropertyGrid.Controls.Implements;
using PropertyModels.ComponentModel;
using PropertyModels.ComponentModel.DataAnnotations;
using PropertyModels.Extensions;
using PropertyModels.Utils;
using Avalonia.PropertyGrid.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        /// Use category internal order.
        /// </summary>
        CategoryBuiltin,

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
        /// The hidden by no visible chidlren
        /// </summary>
        HiddenByNoVisibleChidlren = 1<<2,
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
                
        object _Context;

        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        /// <value>The context.</value>
        public object Context
        {
            get => _Context;
            set => this.RaiseAndSetIfChanged(ref _Context, value);
        }

        PropertyGridShowStyle _ShowStyle = PropertyGridShowStyle.Category;

        /// <summary>
        /// Gets or sets the show style.
        /// </summary>
        /// <value>The show style.</value>
        public PropertyGridShowStyle ShowStyle
        {
            get => _ShowStyle;
            set
            {
                if(_ShowStyle != value)
                {
                    this.RaiseAndSetIfChanged(ref _ShowStyle, value);

                    this.RaisePropertyChanged(nameof(ShowStyleType));
                    this.RaisePropertyChanged(nameof(ShowStyleText));
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
                switch (ShowStyle)
                {
                    case PropertyGridShowStyle.Category:
                        return true;
                    case PropertyGridShowStyle.Alphabetic:
                        return false;

                    default:
                        return null;
                }
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
                switch (ShowStyle)
                {
                    case PropertyGridShowStyle.Category:
                        return "C";
                    case PropertyGridShowStyle.Alphabetic:
                        return "A";

                    default:
                        return "B";
                }
            }
        }

        /// <summary>
        /// The category filter
        /// </summary>
        CheckedMaskModel _CategoryFilter;
        /// <summary>
        /// Gets or sets the category filter.
        /// </summary>
        /// <value>The category filter.</value>
        public CheckedMaskModel CategoryFilter
        {
            get => _CategoryFilter;
            set => this.RaiseAndSetIfChanged(ref _CategoryFilter, value);
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
        public List<PropertyDescriptor> AllProperties { get; private set; } = new List<PropertyDescriptor>();

        /// <summary>
        /// Gets the categories.
        /// </summary>
        /// <value>The categories.</value>
        public Dictionary<string, List<PropertyDescriptor>> Categories { get; private set; } = new Dictionary<string, List<PropertyDescriptor>>();

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
            string category = string.IsNullOrEmpty(property.Category) ? "Misc" : property.Category;

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
                bool AtleastOneVisible = false;

                foreach (var child in info.Children)
                {
                    var v = PropagateVisibility(child, child.Target, category);

                    AtleastOneVisible |= (v == PropertyVisibility.AlwaysVisible);
                }

                info.IsVisible = AtleastOneVisible;

                return AtleastOneVisible ? PropertyVisibility.AlwaysVisible : PropertyVisibility.HiddenByNoVisibleChidlren;
            }
            else
            {
                return PropagateVisibility(info, info.Target, category);
            }
        }

        PropertyVisibility PropagateVisibility(IPropertyGridCellInfo cellInfo, object target, FilterCategory category = FilterCategory.Default)
        {
            PropertyVisibility visibility = PropertyVisibility.AlwaysVisible;

            if(cellInfo.CellType == PropertyGridCellType.Cell)
            {
                var property = cellInfo.Context.Property;

                Debug.Assert(property != null);

                if (property != null)
                {
                    PropertyVisibility? childrenVisibilty = null;

                    if (category.HasFlag(FilterCategory.Factory))
                    {
                        childrenVisibilty = cellInfo.Context.Factory?.HandlePropagateVisibility(target, cellInfo.Context, this);
                    }

                    if (category.HasFlag(FilterCategory.PropertyCondition))
                    {
                        if (property.GetCustomAttribute<AbstractVisiblityConditionAttribute>() is AbstractVisiblityConditionAttribute attr)
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
                            if(childrenVisibilty == null || childrenVisibilty != PropertyVisibility.AlwaysVisible)
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

            if(_Context == null)
            {
                CategoryFilter = null;
                PropertyDescriptorChanged?.Invoke(this, EventArgs.Empty);
                return;
            }

            PropertyDescriptorBuilder builder = new PropertyDescriptorBuilder(_Context);
            AllProperties.AddRange(builder.GetProperties().Cast<PropertyDescriptor>().ToList().FindAll(
                x =>
                {
                    if(CustomPropertyDescriptorFilter != null)
                    {
                        CustomPropertyDescriptorFilterEventArgs args = new CustomPropertyDescriptorFilterEventArgs(PropertyGrid.Controls.PropertyGrid.CustomPropertyDescriptorFilterEvent, _Context, x);

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

            HashSet<string> categories = new HashSet<string>();
            foreach(var property in AllProperties)
            {
                string category = GetCategory(property);

                if(!categories.Contains(category))
                {
                    categories.Add(category);
                }
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
                string category = GetCategory(property);

                if (!Categories.TryGetValue(category, out var list))
                {
                    list = new List<PropertyDescriptor> { property };
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

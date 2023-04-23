using Avalonia.PropertyGrid.Controls;
using Avalonia.PropertyGrid.Controls.Implements;
using Avalonia.PropertyGrid.Model.ComponentModel;
using Avalonia.PropertyGrid.Model.ComponentModel.DataAnnotations;
using Avalonia.PropertyGrid.Model.Extensions;
using Avalonia.PropertyGrid.Model.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    internal class PropertyGridViewModel : ReactiveObject
    {
        /// <summary>
        /// Gets or sets the filter pattern.
        /// </summary>
        /// <value>The filter pattern.</value>
        public IPropertyGridFilterPattern FilterPattern { get; set; } = new PropertyGridFilterPattern();

        /// <summary>
        /// The selected object
        /// </summary>
        object _SelectedObject;
        /// <summary>
        /// Gets or sets the selected object.
        /// </summary>
        /// <value>The selected object.</value>
        public object SelectedObject
        {
            get => _SelectedObject;
            set => this.RaiseAndSetIfChanged(ref _SelectedObject, value);
        }

        /// <summary>
        /// The show category
        /// </summary>
        bool _ShowCategory = true;

        /// <summary>
        /// Gets or sets a value indicating whether [show category].
        /// </summary>
        /// <value><c>true</c> if [show category]; otherwise, <c>false</c>.</value>
        public bool ShowCategory
        {
            get => _ShowCategory;
            set => this.RaiseAndSetIfChanged(ref _ShowCategory, value);
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
        /// Gets all properties.
        /// </summary>
        /// <value>All properties.</value>
        public List<PropertyDescriptor> AllProperties { get; private set; } = new List<PropertyDescriptor>();

        /// <summary>
        /// Gets the categories.
        /// </summary>
        /// <value>The categories.</value>
        public SortedList<string, List<PropertyDescriptor>> Categories { get; private set; } = new SortedList<string, List<PropertyDescriptor>>();

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
            if(e.PropertyName == nameof(SelectedObject))
            {
                RefreshProperties();
            }
            else if(sender == FilterPattern)
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
            string category = string.IsNullOrEmpty(property.Category) ? Controls.PropertyGrid.LocalizationService["Misc"] : Controls.PropertyGrid.LocalizationService[property.Category];

            return category;
        }

        /// <summary>
        /// Checks the visibility.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="category">The category.</param>
        /// <param name="context">The context.</param>
        /// <returns>PropertyVisibility.</returns>
        public PropertyVisibility CheckVisibility(PropertyDescriptor property, string category, object context)
        {
            PropertyVisibility visiblity = PropertyVisibility.AlwaysVisible;

            if (property.GetCustomAttribute<AbstractVisiblityConditionAttribute>() is AbstractVisiblityConditionAttribute attr)
            {
                if (!attr.CheckVisibility(context))
                {
                    visiblity |= PropertyVisibility.HiddenByCondition;
                }
            }

            if(!FilterPattern.Match(property, context))
            {
                visiblity |= PropertyVisibility.HiddenByFilter;
            }

            if(CategoryFilter != null && category.IsNotNullOrEmpty() && !CategoryFilter.IsChecked(category))
            {
                visiblity |= PropertyVisibility.HiddenByCategoryFilter;
            }

            return visiblity;
        }

        /// <summary>
        /// Refreshes the properties.
        /// </summary>
        public void RefreshProperties()
        {
            Clear();

            if(_SelectedObject == null)
            {
                return;
            }

            PropertyDescriptorBuilder builder = new PropertyDescriptorBuilder(_SelectedObject);
            AllProperties.AddRange(builder.GetProperties().Cast<PropertyDescriptor>().ToList().FindAll(x => x.IsBrowsable));

            HashSet<string> categories = new HashSet<string>();
            foreach(var property in AllProperties)
            {
                string category = GetCategory(property);

                if(!categories.Contains(category))
                {
                    categories.Add(category);
                }
            }

            CategoryFilter = new CheckedMaskModel(categories.OrderBy(x=>x), Controls.PropertyGrid.LocalizationService["All"]);
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

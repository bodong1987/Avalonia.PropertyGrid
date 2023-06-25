using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.PropertyGrid.Controls.Implements;
using Avalonia.PropertyGrid.Localization;
using Avalonia.PropertyGrid.Model.ComponentModel;
using Avalonia.PropertyGrid.Model.ComponentModel.DataAnnotations;
using Avalonia.PropertyGrid.Model.Extensions;
using Avalonia.PropertyGrid.Model.Services;
using Avalonia.PropertyGrid.Model.Utils;
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
    public partial class PropertyGrid : UserControl
    {
        #region Factories
        /// <summary>
        /// The factories
        /// You can use this fields to extend ability of PropertyGrid
        /// </summary>
        public readonly static ICellEditFactoryCollection FactoryTemplates = new CellEditFactoryCollection();

        /// <summary>
        /// Gets or sets the localization service.
        /// </summary>
        /// <value>The localization service.</value>
        public static readonly ILocalizationService LocalizationService = new InternalLocalizationService();
        #endregion

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

        /// <summary>
        /// The bindings
        /// </summary>
        readonly PropertyGridBindingCache Bindings = new PropertyGridBindingCache();

        #endregion

        /// <summary>
        /// Initializes static members of the <see cref="PropertyGrid"/> class.
        /// </summary>
        static PropertyGrid()
        {
            // register builtin factories
            foreach(var type in typeof(PropertyGrid).Assembly.GetTypes())
            {
                if(type.IsClass && !type.IsAbstract && type.IsImplementFrom<ICellEditFactory>())
                {
                    FactoryTemplates.AddFactory(Activator.CreateInstance(type) as ICellEditFactory);
                }
            }

            AllowFilterProperty.Changed.Subscribe(OnAllowFilterChanged);
            ShowStyleProperty.Changed.Subscribe(OnShowStyleChanged);
            SelectedObjectProperty.Changed.Subscribe(OnSelectedObjectChanged);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyGrid" /> class.
        /// </summary>
        public PropertyGrid()
        {
            Factories = new CellEditFactoryCollection(FactoryTemplates.CloneFactories(this));

            ViewModel.PropertyDescriptorChanged += OnPropertyDescriptorChanged;
            ViewModel.FilterChanged += OnFilterChanged;
            ViewModel.PropertyChanged += OnViewModelPropertyChanged;

            InitializeComponent();

            column_name.PropertyChanged += OnColumnNamePropertyChanged;

            Bindings.PropertyChangedEvent += OnBindingPropertyChanged;
        }

        /// <summary>
        /// Handles the <see cref="E:BindingPropertyChanged" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="BindingPropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnBindingPropertyChanged(object sender, BindingPropertyChangedEventArgs e)
        {
            if(e.Binding != null && e.Binding.Property != null && e.Binding.Property.IsDefined<ConditionTargetAttribute>())
            {
                SetVisiblity(Bindings);
            }
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
            fastFilterBox.IsVisible = (bool)newValue;
            headerGrid.IsVisible = (bool)newValue;
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
        /// Sets the visiblity.
        /// </summary>
        /// <param name="bindings">The bindings.</param>
        private void SetVisiblity(IEnumerable<PropertyBinding> bindings)
        {
            // first pass check all direct property binding visibility
            foreach (var binding in bindings)
            {
                if(binding.Property != null && binding.Target != null)
                {
                    binding.Visibility = ViewModel.CheckVisibility(
                        binding.Property, 
                        binding.RootCategory,
                        binding.Target
                        );
                }
            }

            // second pass, populate indirect property binding visibility

            if(ViewModel.ShowCategory)
            {
                foreach (var info in bindings)
                {
                    if (info is IndirectPropertyBinding binding && binding.IsCategoryBinding)
                    {
                        binding.PropagateVisiblityState();
                    }
                }
            }
            else
            {
                foreach (var info in bindings)
                {
                    if (info is IndirectPropertyBinding binding)
                    {
                        binding.PropagateVisiblityState();
                    }
                }
            }
        }

        /// <summary>
        /// Handles the <see cref="E:FilterChanged" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void OnFilterChanged(object sender, EventArgs e)
        {
            SetVisiblity(Bindings);
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
            Bindings.Clear();

            if(target == null)
            {
                return;
            }

            ReferencePath referencePath = new ReferencePath();
            
            try
            {
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

                IndirectPropertyBinding binding = new IndirectPropertyBinding(
                    $"{referencePath.ToString()}[{categoryInfo.Key}]",
                    null,
                    expander,
                    target,
                    referencePath.Count,
                    categoryInfo.Key
                    );

                Bindings.AddBinding(binding);

                expander.IsVisible = BuildPropertiesCellEdit(target, referencePath, categoryInfo.Value, expander, grid, binding);

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
        /// <param name="parentBinding">The parent binding.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private bool BuildPropertiesCellEdit(
            object target, 
            ReferencePath referencePath, 
            IEnumerable<PropertyDescriptor> properties, 
            Expander expander, 
            Grid grid,
            IndirectPropertyBinding parentBinding
            )
        {
            bool AtLeastOneVisible = false;

            foreach(var property in properties)
            {
                referencePath.BeginScope(property.Name);
                try
                {
                    var value = property.GetValue(target);

                    // if is expand object, expand again will cause overflow exception.
                    if (value != null && !Bindings.IsBinding(value))
                    {
                        var attr = property.GetCustomAttribute<TypeConverterAttribute>();

                        if (attr == null)
                        {
                            attr = property.PropertyType.GetCustomAttribute<TypeConverterAttribute>();
                        }

                        if (attr != null && attr.GetConverterType().IsChildOf<ExpandableObjectConverter>())
                        {
                            AtLeastOneVisible |= BuildExpandableObjectPropertyCellEdit(target, value, referencePath, property, expander, grid, parentBinding);

                            continue;
                        }
                    }

                    AtLeastOneVisible |= BuildPropertyCellEdit(target, referencePath, property, expander, grid, parentBinding);
                }
                finally
                {
                    referencePath.EndScope();
                }
            }

            return AtLeastOneVisible;
        }

        /// <summary>
        /// Builds the expandable object property cell edit.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="value">The value.</param>
        /// <param name="referencePath">The reference path.</param>
        /// <param name="propertyDescriptor">The property descriptor.</param>
        /// <param name="expander">The expander.</param>
        /// <param name="grid">The grid.</param>
        /// <param name="parentBinding">The parent binding.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private bool BuildExpandableObjectPropertyCellEdit(object target, object value, ReferencePath referencePath, PropertyDescriptor propertyDescriptor, Expander expander, Grid grid, IndirectPropertyBinding parentBinding)
        {
            Debug.Assert(value != null);

            PropertyDescriptorBuilder builder = new PropertyDescriptorBuilder(value);

            var properties = builder.GetProperties();

            if(properties.Count == 0)
            {
                return false;
            }

            try
            {
                referencePath.BeginScope(value.GetType().Name);

                grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));

                Expander childExpander = new Expander();
                childExpander.ExpandDirection = ExpandDirection.Down;
                childExpander.Header = propertyDescriptor.DisplayName;
                childExpander.SetValue(Grid.RowProperty, grid.RowDefinitions.Count - 1);
                childExpander.SetValue(Grid.ColumnSpanProperty, 2);
                childExpander.IsExpanded = true;
                childExpander.HorizontalAlignment = Layout.HorizontalAlignment.Stretch;
                childExpander.HorizontalContentAlignment = Layout.HorizontalAlignment.Stretch;
                childExpander.Margin = new Thickness(6,2,6,2);
                childExpander.Padding = new Thickness(2);

                Grid childGrid = new Grid();
                childGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
                childGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));

                childExpander.Content = childGrid;
                grid.Children.Add(childExpander);

                IndirectPropertyBinding binding = new IndirectPropertyBinding(
                        referencePath.ToString(),
                        propertyDescriptor,
                        childExpander,
                        target,
                        referencePath.Count,
                        parentBinding?.RootCategory
                    );

                Bindings.AddBinding(binding);

                parentBinding?.AddBinding(binding);

                binding.Visibility = ViewModel.CheckVisibility(binding.Property, binding.RootCategory, target);

                BuildPropertiesCellEdit(value, referencePath, properties.Cast<PropertyDescriptor>(), childExpander, childGrid, binding);

                binding.PropagateVisiblityState();
            }
            finally
            {
                referencePath.EndScope();
            }
            

            return true;
        }

        /// <summary>
        /// Builds the property cell edit.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="referencePath">The reference path.</param>
        /// <param name="propertyDescriptor">The property descriptor.</param>
        /// <param name="expander">The expander.</param>
        /// <param name="grid">The grid.</param>
        /// <param name="parentBinding">The parent binding.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private bool BuildPropertyCellEdit(object target, ReferencePath referencePath, PropertyDescriptor propertyDescriptor, Expander expander, Grid grid, IndirectPropertyBinding parentBinding)
        {
            var property = propertyDescriptor;

            ICellEditFactory factory;
            var control = Factories.BuildPropertyControl(target, property, out factory);

            if (control == null)
            {
#if DEBUG
                Debug.WriteLine($"Failed build property control for property:{property.Name}({property.PropertyType}");
#endif
                return false;
            }

            grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));

            TextBlock nameBlock = new TextBlock();
            nameBlock.Text = LocalizationService[property.DisplayName];
            nameBlock.SetValue(Grid.RowProperty, grid.RowDefinitions.Count - 1);
            nameBlock.SetValue(Grid.ColumnProperty, 0);
            nameBlock.VerticalAlignment = Layout.VerticalAlignment.Center;
            nameBlock.Margin = new Thickness(4);

            if (property.GetCustomAttribute<DescriptionAttribute>() is DescriptionAttribute descriptionAttribute && descriptionAttribute.Description.IsNotNullOrEmpty())
            {
                nameBlock.SetValue(ToolTip.TipProperty, LocalizationService[descriptionAttribute.Description]);
            }

            grid.Children.Add(nameBlock);

            control.SetValue(Grid.RowProperty, grid.RowDefinitions.Count - 1);
            control.SetValue(Grid.ColumnProperty, 1);
            control.IsEnabled = !property.IsReadOnly;
            control.Margin = new Thickness(4);

            grid.Children.Add(control);

            factory.HandlePropertyChanged(target, property, control);

            var binding = new DirectPropertyBinding(
                referencePath.ToString(),
                property, 
                expander, 
                target, 
                referencePath.Count, 
                control, 
                nameBlock, 
                factory, 
                parentBinding?.RootCategory
                );

            Bindings.AddBinding(binding);          
            
            parentBinding?.AddBinding(binding);

            binding.Visibility = ViewModel.CheckVisibility(binding.Property, binding.RootCategory, target);

            return binding.Visibility == PropertyVisibility.AlwaysVisible;
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

            BuildPropertiesCellEdit(target, referencePath, ViewModel.AllProperties, null, propertiesGrid, null);
        }
        #endregion

        #region Process Widths
        /// <summary>
        /// Synchronizes the width of the with maximum property name.
        /// </summary>
        private void SyncWithMaxPropertyNameWidth()
        {
            double maxLength = Bindings.CalcBindingNameMaxLength();

            if(maxLength > 0)
            {
                SyncNameWidth(maxLength, true);
            }            
        }

        /// <summary>
        /// Synchronizes the width of the name.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="syncToTitle">if set to <c>true</c> [synchronize to title].</param>
        private void SyncNameWidth(double width, bool syncToTitle)
        {
            Bindings.SyncWidth(width);

            if (syncToTitle)
            {
                //splitterGrid.ColumnDefinitions[0].Width = new GridLength(width);
                column_name.Width = width;
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
    }

    
}

using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.PropertyGrid.Controls.Implements;
using Avalonia.PropertyGrid.Localization;
using Avalonia.PropertyGrid.Model.ComponentModel;
using Avalonia.PropertyGrid.Model.ComponentModel.DataAnnotations;
using Avalonia.PropertyGrid.Model.Extensions;
using Avalonia.PropertyGrid.Model.Services;
using Avalonia.PropertyGrid.ViewModels;
using Avalonia.Threading;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

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
        public readonly static IPropertyGridControlFactoryCollection FactoryTemplates = new PropertyGridControlFactoryCollection();

        /// <summary>
        /// Gets or sets the localization service.
        /// </summary>
        /// <value>The localization service.</value>
        public static ILocalizationService LocalizationService { get; set; } = new InternalLocalizationService();
        #endregion

        #region Properties
        public static readonly StyledProperty<bool> AllowSearchProperty = AvaloniaProperty.Register<PropertyGrid, bool>(nameof(AllowSearch), true);

        public bool AllowSearch 
        { 
            get => GetValue(AllowSearchProperty); set => SetValue(AllowSearchProperty, value);
        }

        public static readonly StyledProperty<PropertyGridShowStyle> ShowStyleProperty = AvaloniaProperty.Register<PropertyGrid, PropertyGridShowStyle>(nameof(ShowStyle), PropertyGridShowStyle.Category);
        public PropertyGridShowStyle ShowStyle
        {
            get => GetValue(ShowStyleProperty); 
            set 
            {
                SetValue(ShowStyleProperty, value);
                ViewModel.ShowCategory = value == PropertyGridShowStyle.Category;
            }
        }

        private object _SelectedObject;
        public static readonly DirectProperty<PropertyGrid, object> SelectedObjectProperty = AvaloniaProperty.RegisterDirect<PropertyGrid, object>(
            nameof(SelectedObject),
            o => o._SelectedObject,
            (o,v)=> o.SetAndRaise(SelectedObjectProperty, ref o._SelectedObject, v)
            );
        public object SelectedObject
        {
            get => _SelectedObject;
            set => SetAndRaise(SelectedObjectProperty, ref _SelectedObject, value);
        }

        PropertyGridViewModel ViewModel = new PropertyGridViewModel();

        public readonly IPropertyGridControlFactoryCollection Factories;

        private struct PropertyBinding
        {
            public PropertyDescriptor Property;
            public Control BindingControl;
            public TextBlock BindingNameControl;
            public IPropertyGridControlFactory Factory;
            public Expander BindingExpander;
        }

        Dictionary<string, PropertyBinding> PropertyBindingDict = new Dictionary<string, PropertyBinding>();

        #endregion

        static PropertyGrid()
        {
            // register builtin factories
            foreach(var type in typeof(PropertyGrid).Assembly.GetTypes())
            {
                if(type.IsClass && !type.IsAbstract && type.IsImplementFrom<IPropertyGridControlFactory>())
                {
                    FactoryTemplates.AddFactory(Activator.CreateInstance(type) as IPropertyGridControlFactory);
                }
            }

            AllowSearchProperty.Changed.Subscribe(OnAllowSearchChanged);
            ShowStyleProperty.Changed.Subscribe(OnShowStyleChanged);
            SelectedObjectProperty.Changed.Subscribe(OnSelectedObjectChanged);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyGrid"/> class.
        /// </summary>
        public PropertyGrid()
        {
            Factories = new PropertyGridControlFactoryCollection(FactoryTemplates.CloneFactories(this));

            this.DataContext = ViewModel;
            ViewModel.PropertyDescriptorChanged += OnPropertyDescriptorChanged;
            ViewModel.FilterChanged += OnFilterChanged;
            ViewModel.PropertyChanged += OnViewModelPropertyChanged;

            InitializeComponent();

            column_name.PropertyChanged += OnColumnNamePropertyChanged;
        }

        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(ViewModel.ShowCategory))
            {
                ShowStyle = ViewModel.ShowCategory ? PropertyGridShowStyle.Category : PropertyGridShowStyle.Alphabetic;
            }
        }

        private static void OnSelectedObjectChanged(AvaloniaPropertyChangedEventArgs<object> e)
        {
            if(e.Sender is PropertyGrid pg)
            {
                pg.OnSelectedObjectChanged(e.OldValue.Value, e.NewValue.Value);
            }
        }

        private void OnSelectedObjectChanged(object oldValue, object newValue)
        {
            if(oldValue is System.ComponentModel.INotifyPropertyChanged npc)
            {
                npc.PropertyChanged -= OnSelectedObjectPropertyChanged;
            }

            ViewModel.SelectedObject = newValue;

            if(newValue is System.ComponentModel.INotifyPropertyChanged nnpc)
            {
                nnpc.PropertyChanged += OnSelectedObjectPropertyChanged;
            }
        }

        private void OnSelectedObjectPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (PropertyBindingDict.TryGetValue(e.PropertyName, out PropertyBinding propertyBinding))
            {
                propertyBinding.Factory.HandlePropertyChanged(SelectedObject, propertyBinding.Property, propertyBinding.BindingControl);
            }
        }

        #region Styled Properties Handler
        private static void OnAllowSearchChanged(AvaloniaPropertyChangedEventArgs e)
        {
            if(e.Sender is PropertyGrid sender)
            {
                sender.OnAllowSearchChanged(e.OldValue, e.NewValue);
            }
        }

        private void OnAllowSearchChanged(object oldValue, object newValue)
        {
            fastFilterBox.IsVisible = (bool)newValue;
            headerGrid.IsVisible = (bool)newValue;
        }

        private static void OnShowStyleChanged(AvaloniaPropertyChangedEventArgs<PropertyGridShowStyle> e)
        {
            if(e.Sender is PropertyGrid sender)
            {
                sender.OnShowStyleChanged(e.OldValue, e.NewValue);
            }
        }

        private void OnShowStyleChanged(Optional<PropertyGridShowStyle> oldValue, BindingValue<PropertyGridShowStyle> newValue)
        {
            BuildPropertiesView(ViewModel.ShowCategory ? PropertyGridShowStyle.Category : PropertyGridShowStyle.Alphabetic);
        }

        #endregion

        private void OnPropertyDescriptorChanged(object sender, EventArgs e)
        {
            BuildPropertiesView(ViewModel.ShowCategory ? PropertyGridShowStyle.Category : PropertyGridShowStyle.Alphabetic);
        }

        private void OnFilterChanged(object sender, EventArgs e)
        {
            if(ViewModel.ShowCategory)
            {
                Dictionary<string, List<PropertyBinding>> caches = new Dictionary<string, List<PropertyBinding>>();

                foreach (var info in PropertyBindingDict)
                {
                    var category = PropertyGridViewModel.GetCategory(info.Value.Property);

                    if(caches.TryGetValue(category, out var list))
                    {
                        list.Add(info.Value);
                    }
                    else
                    {
                        list = new List<PropertyBinding>() { info.Value };
                        caches.Add(category, list);
                    }
                }

                foreach(var cache in caches)
                {
                    bool AtLeastOneVisible = false;
                    foreach (var info in cache.Value)
                    {
                        bool IsVisible = ViewModel.CheckVisible(info.Property);

                        info.BindingNameControl.IsVisible = IsVisible;
                        info.BindingControl.IsVisible = IsVisible;

                        AtLeastOneVisible |= IsVisible;
                    }

                    var anyBinding = cache.Value.FirstOrDefault();
                    if (anyBinding.BindingExpander != null)
                    {
                        anyBinding.BindingExpander.IsVisible = AtLeastOneVisible;
                    }
                }
            }         
            else
            {
                foreach(var info in PropertyBindingDict)
                {
                    bool IsVisible = ViewModel.CheckVisible(info.Value.Property);

                    info.Value.BindingNameControl.IsVisible = IsVisible;
                    info.Value.BindingControl.IsVisible = IsVisible;
                }
            }
        }

        private void BuildPropertiesView(PropertyGridShowStyle propertyGridShowStyle)
        {
            propertiesGrid.RowDefinitions.Clear();
            propertiesGrid.Children.Clear();
            PropertyBindingDict.Clear();

            if (propertyGridShowStyle == PropertyGridShowStyle.Category)
            {
                BuildCategoryPropertiesView();
            }
            else if(propertyGridShowStyle == PropertyGridShowStyle.Alphabetic)
            {
                BuildAlphabeticPropertiesView();
            }

            double width = column_name.Bounds.Width;

            SyncNameWidth(width, false);
        }

        #region Categories
        private void BuildCategoryPropertiesView()
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

                Grid grid = new Grid();
                grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
                grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));

                expander.IsVisible = BuildPropertiesGrid(categoryInfo.Value, expander, grid);

                expander.Content = grid;

                propertiesGrid.Children.Add(expander);
            }

            propertiesGrid.RowDefinitions.Add(new RowDefinition(GridLength.Star));
        }

        /// <summary>
        /// Builds the properties grid.
        /// </summary>
        /// <param name="properties">The properties.</param>
        /// <param name="expander">The expander.</param>
        /// <param name="grid">The grid.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private bool BuildPropertiesGrid(IEnumerable<PropertyDescriptor> properties, Expander expander, Grid grid)
        {
            bool AtLeastOneVisible = false;

            foreach(var property in properties)
            {
                IPropertyGridControlFactory factory;
                var control = Factories.BuildPropertyControl(SelectedObject, property, out factory);

                if(control == null)
                {
                    Debug.WriteLine($"Failed build property control for property:{property.Name}({property.PropertyType}");
                    continue;
                }

                bool IsVisible = ViewModel.CheckVisible(property);
                AtLeastOneVisible |= IsVisible;

                grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));

                TextBlock nameBlock = new TextBlock();
                nameBlock.Text = LocalizationService[property.DisplayName];
                nameBlock.SetValue(Grid.RowProperty, grid.RowDefinitions.Count - 1);
                nameBlock.SetValue(Grid.ColumnProperty, 0);
                nameBlock.VerticalAlignment = Layout.VerticalAlignment.Center;
                nameBlock.Margin = new Thickness(4);             
                
                if(property.GetCustomAttribute<DescriptionAttribute>() is DescriptionAttribute descriptionAttribute && descriptionAttribute.Description.IsNotNullOrEmpty())
                {
                    nameBlock.SetValue(ToolTip.TipProperty, LocalizationService[descriptionAttribute.Description]);
                }

                grid.Children.Add(nameBlock);

                control.SetValue(Grid.RowProperty, grid.RowDefinitions.Count - 1);
                control.SetValue(Grid.ColumnProperty, 1);
                control.IsEnabled = !property.IsReadOnly;
                control.HorizontalAlignment = Layout.HorizontalAlignment.Stretch;
                control.Margin = new Thickness(4);

                grid.Children.Add(control);

                factory.HandlePropertyChanged(ViewModel.SelectedObject, property, control);

                PropertyBindingDict.Add(property.Name, new PropertyBinding()
                {
                    Property = property,
                    BindingControl = control,
                    BindingNameControl = nameBlock,
                    Factory = factory,
                    BindingExpander = expander // expander can be null
                });

                nameBlock.IsVisible = IsVisible;
                control.IsVisible = IsVisible;
            }

            return AtLeastOneVisible;
        }
        #endregion

        #region Alpha
        private void BuildAlphabeticPropertiesView()
        {
            propertiesGrid.ColumnDefinitions.Clear();
            propertiesGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
            propertiesGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));

            BuildPropertiesGrid(ViewModel.AllProperties, null, propertiesGrid);
        }
        #endregion

        #region Process Widths
        private void SyncWithMaxPropertyNameWidth()
        {
            double maxLength = 0;
            foreach (var info in PropertyBindingDict)
            {
                if (info.Value.BindingNameControl != null)
                {
                    if(info.Value.BindingNameControl.Width >= maxLength)
                    {
                        maxLength = info.Value.BindingNameControl.Width;
                    }
                    else if(info.Value.BindingNameControl.DesiredSize.Width >= maxLength)
                    {
                        maxLength = info.Value.BindingNameControl.DesiredSize.Width;
                    }
                }
            }

            if(maxLength > 0)
            {
                SyncNameWidth(maxLength, true);
            }            
        }

        private void SyncNameWidth(double width, bool syncToTitle)
        {
            foreach (var info in PropertyBindingDict)
            {
                if (info.Value.BindingNameControl != null)
                {
                    info.Value.BindingNameControl.Width = width;
                }
            }

            if (syncToTitle)
            {
                //splitterGrid.ColumnDefinitions[0].Width = new GridLength(width);
                column_name.Width = width;
            }
        }

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

    public enum PropertyGridShowStyle
    {
        Category,
        Alphabetic
    }
}

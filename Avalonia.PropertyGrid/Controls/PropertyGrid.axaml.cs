using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.PropertyGrid.Controls.Implements;
using Avalonia.PropertyGrid.Localization;
using Avalonia.PropertyGrid.Model.Services;
using Avalonia.PropertyGrid.ViewModels;
using System;

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
        public readonly static IPropertyGridControlFactoryCollection Factories = new PropertyGridControlFactoryCollection();

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
            get => GetValue(ShowStyleProperty); set => SetValue(ShowStyleProperty, value);
        }

        private object _SelectedObject;
        public static readonly DirectProperty<PropertyGrid, object> SelectedObjectProperty = AvaloniaProperty.RegisterDirect<PropertyGrid, object>(
            nameof(SelectedObject),
            o => o._SelectedObject,
            (o,v)=> o._SelectedObject = v
            );
        public object SelectedObject
        {
            get => GetValue(SelectedObjectProperty);
            set => SetValue(SelectedObjectProperty, value);
        }

        PropertyGridViewModel ViewModel = new PropertyGridViewModel();

        #endregion

        static PropertyGrid()
        {
            AllowSearchProperty.Changed.Subscribe(OnAllowSearchChanged);
            ShowStyleProperty.Changed.Subscribe(OnShowStyleChanged);
            SelectedObjectProperty.Changed.Subscribe(OnSelectedObjectChanged);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyGrid"/> class.
        /// </summary>
        public PropertyGrid()
        {
            InitializeComponent();

            this.DataContext = ViewModel;
        }

        private static void OnSelectedObjectChanged(AvaloniaPropertyChangedEventArgs<object> e)
        {
            if(e.Sender is PropertyGrid pg)
            {
                pg.OnSelectedObjectChanged(e.NewValue.Value);
            }
        }

        private void OnSelectedObjectChanged(object newValue)
        {
            ViewModel.SelectedObject = newValue;
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
            
        }
        #endregion
    }

    public enum PropertyGridShowStyle
    {
        Category,
        Alphabetic
    }
}

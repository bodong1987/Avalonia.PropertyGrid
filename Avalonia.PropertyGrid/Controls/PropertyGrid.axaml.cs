using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.PropertyGrid.Controls.Implements;
using Avalonia.PropertyGrid.Controls.ViewInfos;
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
        #endregion

        #region Views
        public IPropertyGridViewInfo ViewInfo { get; private set; }
        #endregion

        static PropertyGrid()
        {
            AllowSearchProperty.Changed.Subscribe(OnAllowSearchChanged);
            ShowStyleProperty.Changed.Subscribe(OnShowStyleChanged);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyGrid"/> class.
        /// </summary>
        public PropertyGrid()
        {
            InitializeComponent();
        }

        protected override void OnDataContextChanged(EventArgs e)
        {
            base.OnDataContextChanged(e);
        }

        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
        {
            base.OnPropertyChanged(change);
        }

        protected virtual IPropertyGridViewInfo CreateViewInfo()
        {
            if(ShowStyle == PropertyGridShowStyle.Category)
            {
                return new PropertyGridCategoryViewInfo(this, Content);
            }
            else
            {
                return new PropertyGridAlphabeticViewInfo(this, Content);   
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

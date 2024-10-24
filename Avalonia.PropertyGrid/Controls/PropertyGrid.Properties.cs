using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.PropertyGrid.ViewModels;
using Avalonia.Reactive;
using System;
using System.ComponentModel;

namespace Avalonia.PropertyGrid.Controls;

partial class PropertyGrid
{
    #region Behaviors
    /// <summary>
    /// The IsReadOnly property
    /// </summary>
    public static readonly StyledProperty<bool> IsReadOnlyProperty = AvaloniaProperty.Register<PropertyGrid, bool>(nameof(IsReadOnly));

    /// <summary>
    /// Gets or sets Is Readonly flag
    /// </summary>
    /// <value><c>true</c> if [readonly]; otherwise, <c>false</c>.</value>
    [Category("Behaviors")]
    public bool IsReadOnly
    {
        get => GetValue(IsReadOnlyProperty);
        set => SetValue(IsReadOnlyProperty, value);
    }

    #endregion

    #region Views
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
    /// The allow toggle view property
    /// </summary>
    public static readonly StyledProperty<bool> AllowToggleViewProperty = AvaloniaProperty.Register<PropertyGrid, bool>(nameof(AllowToggleView), true);

    /// <summary>
    /// Gets or sets a value indicating whether [allow toggle view].
    /// </summary>
    /// <value><c>true</c> if [allow toggle view]; otherwise, <c>false</c>.</value>
    [Category("Views")]
    public bool AllowToggleView
    {
        get => GetValue(AllowToggleViewProperty); set => SetValue(AllowToggleViewProperty, value);
    }

    /// <summary>
    /// The allow quick filter property
    /// </summary>
    public static readonly StyledProperty<bool> AllowQuickFilterProperty = AvaloniaProperty.Register<PropertyGrid, bool>(nameof(AllowQuickFilter), true);

    /// <summary>
    /// Gets or sets a value indicating whether [allow quick filter].
    /// </summary>
    /// <value><c>true</c> if [allow quick filter]; otherwise, <c>false</c>.</value>
    [Category("Views")]
    public bool AllowQuickFilter
    {
        get => GetValue(AllowQuickFilterProperty); set => SetValue(AllowQuickFilterProperty, value);
    }

    /// <summary>
    /// The show title property
    /// </summary>
    public static readonly StyledProperty<bool> ShowTitleProperty = AvaloniaProperty.Register<PropertyGrid, bool>(nameof(ShowTitle), true);

    /// <summary>
    /// Gets or sets a value indicating whether [show title].
    /// </summary>
    /// <value><c>true</c> if [show title]; otherwise, <c>false</c>.</value>
    [Category("Views")]
    public bool ShowTitle
    {
        get => GetValue(ShowTitleProperty); set => SetValue(ShowTitleProperty, value);
    }

    /// <summary>
    /// The show style property
    /// </summary>
    public static readonly StyledProperty<PropertyGridShowStyle> ShowStyleProperty = AvaloniaProperty.Register<PropertyGrid, PropertyGridShowStyle>(nameof(ShowStyle));

    /// <summary>
    /// Gets or sets the show style.
    /// </summary>
    /// <value>The show style.</value>
    [Category("Views")]
    public PropertyGridShowStyle ShowStyle
    {
        get => GetValue(ShowStyleProperty);
        set => SetValue(ShowStyleProperty, value);
    }

    /// <summary>
    /// The order style property
    /// control category sort algorithm
    /// </summary>
    public static readonly StyledProperty<PropertyGridOrderStyle> CategoryOrderStyleProperty = AvaloniaProperty.Register<PropertyGrid, PropertyGridOrderStyle>(nameof(CategoryOrderStyle));

    /// <summary>
    /// Gets or sets the category order style.
    /// </summary>
    /// <value>The order style.</value>
    [Category("Views")]
    public PropertyGridOrderStyle CategoryOrderStyle
    {
        get => GetValue(CategoryOrderStyleProperty);
        set => SetValue(CategoryOrderStyleProperty, value);
    }


    /// <summary>
    /// The order style property
    /// control property sort algorithm
    /// </summary>
    public static readonly StyledProperty<PropertyGridOrderStyle> PropertyOrderStyleProperty = AvaloniaProperty.Register<PropertyGrid, PropertyGridOrderStyle>(nameof(PropertyOrderStyle));

    /// <summary>
    /// Gets or sets the order style.
    /// </summary>
    /// <value>The order style.</value>
    [Category("Views")]
    public PropertyGridOrderStyle PropertyOrderStyle
    {
        get => GetValue(PropertyOrderStyleProperty);
        set => SetValue(PropertyOrderStyleProperty, value);
    }

    /// <summary>
    /// The name width property
    /// </summary>
    public static readonly StyledProperty<double> NameWidthProperty = AvaloniaProperty.Register<PropertyGrid, double>(nameof(NameWidth), 180);

    /// <summary>
    /// Gets or sets the width of the name.
    /// </summary>
    /// <value>The width of the name.</value>
    [Category("Views")]
    public double NameWidth
    {
        get => GetValue(NameWidthProperty);
        set => SetValue(NameWidthProperty, value);
    }


    /// <summary>
    /// View type property
    /// </summary>
    public static readonly StyledProperty<PropertyGridViewType> ViewTypeProperty = AvaloniaProperty.Register<PropertyGrid, PropertyGridViewType>(nameof(ViewType));

    /// <summary>
    /// get or set current view type
    /// </summary>
    [Category("Views")]
    public PropertyGridViewType ViewType
    {
        get => GetValue(ViewTypeProperty);
        set => SetValue(ViewTypeProperty, value);
    }
    #endregion

    #region Static Constructor
    /// <summary>
    /// Initializes static members of the <see cref="PropertyGrid"/> class.
    /// </summary>
    static PropertyGrid()
    {
        AllowFilterProperty.Changed.Subscribe(new AnonymousObserver<AvaloniaPropertyChangedEventArgs<bool>>(OnAllowFilterChanged));
        AllowQuickFilterProperty.Changed.Subscribe(new AnonymousObserver<AvaloniaPropertyChangedEventArgs<bool>>(OnAllowQuickFilterChanged));
        ShowStyleProperty.Changed.Subscribe(new AnonymousObserver<AvaloniaPropertyChangedEventArgs<PropertyGridShowStyle>>(OnShowStyleChanged));
        CategoryOrderStyleProperty.Changed.Subscribe(new AnonymousObserver<AvaloniaPropertyChangedEventArgs<PropertyGridOrderStyle>>(OnCategoryOrderStyleChanged));
        PropertyOrderStyleProperty.Changed.Subscribe(new AnonymousObserver<AvaloniaPropertyChangedEventArgs<PropertyGridOrderStyle>>(OnPropertyOrderStyleChanged));
        ShowTitleProperty.Changed.Subscribe(new AnonymousObserver<AvaloniaPropertyChangedEventArgs<bool>>(OnShowTitleChanged));
        NameWidthProperty.Changed.Subscribe(new AnonymousObserver<AvaloniaPropertyChangedEventArgs<double>>(OnNameWidthChanged));
        IsReadOnlyProperty.Changed.Subscribe(new AnonymousObserver<AvaloniaPropertyChangedEventArgs<bool>>(OnIsReadOnyPropertyChanged));
        ViewTypeProperty.Changed.Subscribe(
            new AnonymousObserver<AvaloniaPropertyChangedEventArgs<PropertyGridViewType>>(
                OnViewTypePropertyChanged));
    }
    #endregion

    #region PropertyChanged Handlers
    /// <summary>
    /// Handles the <see cref="E:AllowFilterChanged" /> event.
    /// </summary>
    /// <param name="e">The <see cref="AvaloniaPropertyChangedEventArgs"/> instance containing the event data.</param>
    private static void OnAllowFilterChanged(AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Sender is PropertyGrid sender)
        {
            sender.OnAllowFilterChanged(e.OldValue, e.NewValue);
        }
    }

    /// <summary>
    /// Called when [allow filter changed].
    /// </summary>
    /// <param name="oldValue">The old value.</param>
    /// <param name="newValue">The new value.</param>
    private void OnAllowFilterChanged(object? oldValue, object? newValue)
    {
        HeaderGrid.IsVisible = (bool)newValue!;
    }

    private static void OnNameWidthChanged(AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Sender is PropertyGrid sender)
        {
            sender.OnNameWidthChanged(e.OldValue, e.NewValue);
        }
    }

    private void OnNameWidthChanged(object? oldValue, object? newValue)
    {
        if (View != null)
        {
            View.NameWidth = (double)newValue!;
        }
    }

    /// <summary>
    /// Called when [show style changed].
    /// </summary>
    /// <param name="e">The e.</param>
    private static void OnShowStyleChanged(AvaloniaPropertyChangedEventArgs<PropertyGridShowStyle> e)
    {
        if (e.Sender is PropertyGrid sender)
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
        ViewModel.ShowStyle = newValue.Value;
    }

    private static void OnCategoryOrderStyleChanged(AvaloniaPropertyChangedEventArgs<PropertyGridOrderStyle> e)
    {
        if (e.Sender is PropertyGrid sender)
        {
            sender.OnCategoryOrderStyleChanged(e.OldValue, e.NewValue);
        }
    }

    private void OnCategoryOrderStyleChanged(Optional<PropertyGridOrderStyle> oldValue, BindingValue<PropertyGridOrderStyle> newValue)
    {
        ViewModel.CategoryOrderStyle = newValue.Value;
    }

    private static void OnPropertyOrderStyleChanged(AvaloniaPropertyChangedEventArgs<PropertyGridOrderStyle> e)
    {
        if (e.Sender is PropertyGrid sender)
        {
            sender.OnPropertyOrderStyleChanged(e.OldValue, e.NewValue);
        }
    }

    private void OnPropertyOrderStyleChanged(Optional<PropertyGridOrderStyle> oldValue, BindingValue<PropertyGridOrderStyle> newValue)
    {
        ViewModel.PropertyOrderStyle = newValue.Value;
    }

    private static void OnShowTitleChanged(AvaloniaPropertyChangedEventArgs<bool> e)
    {
        if (e.Sender is PropertyGrid sender)
        {
            sender.OnShowTitleChanged(e.OldValue.Value, e.NewValue.Value);
        }
    }

    private void OnShowTitleChanged(bool oldValue, bool newValue)
    {
        if (View != null)
        {
            View.ShowTitle = newValue;
        }
    }

    private static void OnIsReadOnyPropertyChanged(AvaloniaPropertyChangedEventArgs<bool> e)
    {
        if (e.Sender is PropertyGrid sender)
        {
            sender.OnIsReadOnyPropertyChanged(e.OldValue.Value, e.NewValue.Value);
        }
    }

    private void OnIsReadOnyPropertyChanged(bool oldValue, bool newValue)
    {
        ViewModel.IsReadOnly = newValue;
    }

    private static void OnViewTypePropertyChanged(AvaloniaPropertyChangedEventArgs<PropertyGridViewType> e)
    {
        if (e.Sender is PropertyGrid sender)
        {
            sender.OnViewTypePropertyChanged(e.OldValue.Value, e.NewValue.Value);
        }
    }

    private void OnViewTypePropertyChanged(PropertyGridViewType oldValue, PropertyGridViewType newValue)
    {
        if (View != null && View.ViewType == newValue)
        {
            return;
        }

        var view = GetPropertyGridView(newValue);

        if (view != null)
        {
            View?.OnLeaveState();
            View = view;
            View.OnEnterState();
        }
    }

    private static void OnAllowQuickFilterChanged(AvaloniaPropertyChangedEventArgs<bool> e)
    {
        if (e.Sender is PropertyGrid sender)
        {
            sender.OnAllowQuickFilterChanged(e.OldValue.Value, e.NewValue.Value);
        }
    }

    private void OnAllowQuickFilterChanged(bool oldValue, bool newValue)
    {
        FastFilterBox.IsVisible = newValue;
    }

    #endregion
}
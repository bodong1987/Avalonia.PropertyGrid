﻿using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Markup.Xaml.Templates;
using PropertyModels.ComponentModel;
using PropertyModels.Extensions;

namespace Avalonia.PropertyGrid.Controls;

/// <summary>
/// Class CheckedListEdit.
/// Implements the <see cref="TemplatedControl" />
/// </summary>
/// <seealso cref="TemplatedControl" />
public class CheckedListEdit : TemplatedControl
{
    /// <summary>
    /// The selected items changed event
    /// </summary>
    public static readonly RoutedEvent<RoutedEventArgs> SelectedItemsChangedEvent =
        RoutedEvent.Register<CheckedListEdit, RoutedEventArgs>(nameof(SelectedItemsChanged), RoutingStrategies.Bubble);

    /// <summary>
    /// Occurs when [selected items changed].
    /// </summary>
    public event EventHandler<RoutedEventArgs> SelectedItemsChanged
    {
        add => AddHandler(SelectedItemsChangedEvent, value);
        remove => RemoveHandler(SelectedItemsChangedEvent, value);
    }

    /// <summary>
    /// The items property
    /// </summary>
    public static readonly DirectProperty<CheckedListEdit, object[]> ItemsProperty =
        AvaloniaProperty.RegisterDirect<CheckedListEdit, object[]>(
            nameof(Items),
            o => o.Items,
            (o, v) => o.Items = v
        );
        
    /// <summary>
    /// display mode property
    /// </summary>
    public static readonly StyledProperty<SelectableListDisplayMode> DisplayModeProperty =
        AvaloniaProperty.Register<CheckedListEdit, SelectableListDisplayMode>(
            nameof(DisplayMode),
            defaultValue: SelectableListDisplayMode.Default);

    /// <summary>
    /// get/set display mode
    /// </summary>
    public SelectableListDisplayMode DisplayMode
    {
        get => GetValue(DisplayModeProperty);
        set => SetValue(DisplayModeProperty, value);
    }

    /// <summary>
    /// when use stack model, get the orientation
    /// </summary>
    public Orientation StackViewOrientation => DisplayMode == SelectableListDisplayMode.Horizontal ? Orientation.Horizontal : Orientation.Vertical;

    /// <summary>
    /// Gets or sets the items.
    /// </summary>
    /// <value>The items.</value>
    public object[] Items
    {
        get => Model.Items.Select(x => x.Value!).ToArray();
        set
        {
            if (Model.Items != value)
            {
                Model.ResetItems(value);
                Model.RaisePropertyChanged(nameof(Items));
            }
        }
    }

    /// <summary>
    /// Gets or sets the selected items.
    /// </summary>
    /// <value>The selected items.</value>
    public object[] SelectedItems
    {
        get => Model.SelectedItems.Select(x => x.Value!).ToArray();
        set => Model.ResetSelectedItems(value);
    }

    /// <summary>
    /// Gets the model.
    /// </summary>
    /// <value>The model.</value>
    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local
    internal CheckedListViewModel Model { get; private set; } = new();

    /// <summary>
    /// Gets or sets a value indicating whether [enable raise selected items changed event].
    /// </summary>
    /// <value><c>true</c> if [enable raise selected items changed event]; otherwise, <c>false</c>.</value>
    public bool EnableRaiseSelectedItemsChangedEvent
    {
        get => Model.EnableRaiseSelectedItemsChangedEvent;
        set => Model.EnableRaiseSelectedItemsChangedEvent = value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CheckedListEdit"/> class.
    /// </summary>
    public CheckedListEdit() => Model.SelectedItemsChanged += OnSelectedItemChanged;

    /// <summary>
    /// Handles the <see cref="E:SelectedItemChanged" /> event.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    private void OnSelectedItemChanged(object? sender, EventArgs e)
    {
        var evt = new RoutedEventArgs(SelectedItemsChangedEvent);
        RaiseEvent(evt);
    }
        
    /// <inheritdoc />
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        ApplyDisplayModeUtils.Process(e, DisplayMode, StackViewOrientation);
    }
}

#region Help View Model
/// <summary>
/// Class CheckedListViewModel.
/// Implements the <see cref="MiniReactiveObject" />
/// </summary>
/// <seealso cref="MiniReactiveObject" />
internal class CheckedListViewModel : MiniReactiveObject
{
    /// <summary>
    /// The items
    /// </summary>
    private readonly List<CheckedListItemViewModel> _items = [];

    /// <summary>
    /// The selected items
    /// </summary>
    private readonly HashSet<CheckedListItemViewModel> _selectedItems = [];

    /// <summary>
    /// Gets the items.
    /// </summary>
    /// <value>The items.</value>
    public CheckedListItemViewModel[] Items => [.. _items];

    /// <summary>
    /// Gets the selected items.
    /// </summary>
    /// <value>The selected items.</value>
    public CheckedListItemViewModel[] SelectedItems => [.. _selectedItems];

    /// <summary>
    /// Occurs when [selected items changed].
    /// </summary>
    public event EventHandler? SelectedItemsChanged;

    /// <summary>
    /// Gets or sets a value indicating whether [enable raise selected items changed event].
    /// </summary>
    /// <value><c>true</c> if [enable raise selected items changed event]; otherwise, <c>false</c>.</value>
    internal bool EnableRaiseSelectedItemsChangedEvent { get; set; } = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="CheckedListViewModel"/> class.
    /// </summary>
    public CheckedListViewModel()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CheckedListViewModel"/> class.
    /// </summary>
    /// <param name="items">The items.</param>
    public CheckedListViewModel(IEnumerable<object> items) => AddRange(items);

    /// <summary>
    /// Initializes a new instance of the <see cref="CheckedListViewModel"/> class.
    /// </summary>
    /// <param name="items">The items.</param>
    /// <param name="selectedItems">The selected items.</param>
    public CheckedListViewModel(IEnumerable<object> items, IEnumerable<object> selectedItems)
    {
        _items.AddRange(items.Select(x => new CheckedListItemViewModel(this, x)));
        var enumerable = selectedItems as object[] ?? selectedItems.ToArray();

        foreach (var i in _items)
        {
            if (enumerable.Contains(x => i.IsValue(x)))
            {
                _ = _selectedItems.Add(i);
            }
        }
    }

    /// <summary>
    /// Resets the items.
    /// </summary>
    /// <param name="items">The items.</param>
    public void ResetItems(IEnumerable<object> items)
    {
        _items.Clear();
        _selectedItems.Clear();

        RaiseSelectedItemsChangedEvent();

        AddRange(items);

        RefreshItemsCheckStates();
    }

    /// <summary>
    /// Resets the selected items.
    /// </summary>
    /// <param name="items">The items.</param>
    public void ResetSelectedItems(IEnumerable<object> items)
    {
        _selectedItems.Clear();

        foreach (var i in items)
        {
            var obj = _items.Find(x => x.IsValue(i));

            if (obj != null)
            {
                _ = _selectedItems.Add(obj);
            }
        }

        RaiseSelectedItemsChangedEvent();

        RefreshItemsCheckStates();
    }

    /// <summary>
    /// Adds the range.
    /// </summary>
    /// <param name="items">The items.</param>
    public void AddRange(IEnumerable<object> items)
    {
        _items.AddRange(items.Select(x => new CheckedListItemViewModel(this, x)));

        RaisePropertyChanged(nameof(Items));

        RefreshItemsCheckStates();
    }

    /// <summary>
    /// Determines whether the specified item is checked.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns><c>true</c> if the specified item is checked; otherwise, <c>false</c>.</returns>
    public bool IsChecked(CheckedListItemViewModel item) => _selectedItems.Contains(item);

    /// <summary>
    /// Sets the checked.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="value">if set to <c>true</c> [value].</param>
    public void SetChecked(CheckedListItemViewModel item, bool value)
    {
        if (value)
        {
            if (_selectedItems.Add(item))
            {
                RaiseSelectedItemsChangedEvent();
                RefreshItemsCheckStates();
            }
        }
        else
        {
            if (_selectedItems.Contains(item))
            {
                _ = _selectedItems.Remove(item);

                RaiseSelectedItemsChangedEvent();
                RefreshItemsCheckStates();
            }
        }
    }

    /// <summary>
    /// Raises the selected items changed event.
    /// </summary>
    private void RaiseSelectedItemsChangedEvent()
    {
        if (EnableRaiseSelectedItemsChangedEvent)
        {
            SelectedItemsChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Refreshes the items check states.
    /// </summary>
    public void RefreshItemsCheckStates()
    {
        foreach (var item in _items)
        {
            item.RaisePropertyChanged(nameof(item.IsChecked));
        }
    }
}

/// <summary>
/// Class CheckedListItemViewModel.
/// Implements the <see cref="MiniReactiveObject" />
/// </summary>
/// <seealso cref="MiniReactiveObject" />
internal class CheckedListItemViewModel : MiniReactiveObject
{
    /// <summary>
    /// The parent
    /// </summary>
    public readonly CheckedListViewModel Parent;

    /// <summary>
    /// The value
    /// </summary>
    public readonly object? Value;

    /// <summary>
    /// Initializes a new instance of the <see cref="CheckedListItemViewModel"/> class.
    /// </summary>
    /// <param name="model">The model.</param>
    /// <param name="value">The value.</param>
    public CheckedListItemViewModel(CheckedListViewModel model, object? value)
    {
        Parent = model;
        Value = value;
    }

    /// <summary>
    /// Determines whether the specified value is value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns><c>true</c> if the specified value is value; otherwise, <c>false</c>.</returns>
    public bool IsValue(object value) => Value == value || (Value?.Equals(value) == true);

    /// <summary>
    /// Gets or sets a value indicating whether this instance is checked.
    /// </summary>
    /// <value><c>true</c> if this instance is checked; otherwise, <c>false</c>.</value>
    public bool IsChecked
    {
        get => Parent.IsChecked(this);
        set
        {
            if (IsChecked != value)
            {
                Parent.SetChecked(this, value);
                RaisePropertyChanged(nameof(IsChecked));
            }
        }
    }

    /// <summary>
    /// Gets the name.
    /// </summary>
    /// <value>The name.</value>
    public string Name => Value?.ToString() ?? string.Empty;
}
#endregion
    
internal class DeferredPanelTemplate : IDeferredContent
{
    private readonly Func<Panel> _panelFactory;

    public DeferredPanelTemplate(Func<Panel> panelFactory)
    {
        _panelFactory = panelFactory;
    }

    public object Build(IServiceProvider? serviceProvider)
    {
        var panel = _panelFactory();
        var nameScope = new NameScope(); 
        return new TemplateResult<Control>(panel, nameScope);
    }
}

internal static class ApplyDisplayModeUtils
{
    public static void Process(TemplateAppliedEventArgs e, SelectableListDisplayMode displayMode, Orientation orientation, string presenterName = "ItemsPresenter")
    {
        var itemsControl = e.NameScope.Find<ItemsControl>(presenterName);
        if (itemsControl != null)
        {
            var panelTemplate = new ItemsPanelTemplate
            {
                Content = new DeferredPanelTemplate(() =>
                {
                    if (displayMode is SelectableListDisplayMode.AutoWrap or SelectableListDisplayMode.Default)
                    {
                        return new WrapPanel { Orientation = Orientation.Horizontal, ItemSpacing = 4};
                    }

                    return new StackPanel { Orientation = orientation, Spacing = 4};
                })
            };

            itemsControl.ItemsPanel = panelTemplate;
        }
    }
}
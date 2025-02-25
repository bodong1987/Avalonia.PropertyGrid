using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using PropertyModels.ComponentModel;

namespace Avalonia.PropertyGrid.Controls
{
   
    /// <summary>
    /// Class RadioButtonListEdit.
    /// Implements the <see cref="TemplatedControl" />
    /// </summary>
    /// <seealso cref="TemplatedControl" />
    public class RadioButtonListEdit : TemplatedControl
    {
        /// <summary>
        /// The checked items changed event
        /// </summary>
        public static readonly RoutedEvent<RoutedEventArgs> CheckChangedEvent =
            RoutedEvent.Register<RadioButtonListEdit, RoutedEventArgs>(nameof(CheckChanged), RoutingStrategies.Bubble);

        /// <summary>
        /// Occurs when [checked items changed].
        /// </summary>
        public event EventHandler<RoutedEventArgs> CheckChanged
        {
            add => AddHandler(CheckChangedEvent, value);
            remove => RemoveHandler(CheckChangedEvent, value);
        }
        
        /// <summary>
        /// The items property
        /// </summary>
        public static readonly DirectProperty<RadioButtonListEdit, object[]> ItemsProperty =
            AvaloniaProperty.RegisterDirect<RadioButtonListEdit, object[]>(
                nameof(Items),
                o => o.Items,
                (o, v) => o.Items = v
                );

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>The items.</value>
        public object[] Items
        {
            get => Model.Items.Select(x=>x.Value!).ToArray();
            set
            {
                if(Model.Items != value)
                {
                    Model.ResetItems(value);
                    Model.RaisePropertyChanged(nameof(Items));
                }
            }
        }

        /// <summary>
        /// get or set checked item
        /// </summary>
        public object? CheckedItem
        {
            get => Model.CheckedItem?.Value;
            set => Model.ResetSelectedItems(value);
        }

        /// <summary>
        /// Gets the model.
        /// </summary>
        /// <value>The model.</value>
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local
        internal SingleSelectListViewModel Model { get; private set; } = new();

        /// <summary>
        /// Gets or sets a value indicating whether [enable raise checked item changed event].
        /// </summary>
        /// <value><c>true</c> if [enable raise checked item changed event]; otherwise, <c>false</c>.</value>
        public bool EnableRaiseCheckedItemChangedEvent
        {
            get => Model.EnableRaiseCheckedItemChangedEvent;
            set => Model.EnableRaiseCheckedItemChangedEvent = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RadioButtonListEdit"/> class.
        /// </summary>
        public RadioButtonListEdit()
        {
            Model.CheckChanged += OnCheckedItemChanged;
        }

        /// <summary>
        /// Handles the <see cref="E:SelectedItemChanged" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void OnCheckedItemChanged(object? sender, EventArgs e)
        {
            var evt = new RoutedEventArgs(CheckChangedEvent);
            RaiseEvent(evt);
        }
    }

    #region Help View Model
    /// <summary>
    /// Class SingleSelectListViewModel.
    /// Implements the <see cref="MiniReactiveObject" />
    /// </summary>
    /// <seealso cref="MiniReactiveObject" />
    internal class SingleSelectListViewModel : MiniReactiveObject
    {
        /// <summary>
        /// The items
        /// </summary>
        private readonly List<SingleSelectListItemViewModel> _items = [];

        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <value>The items.</value>
        public SingleSelectListItemViewModel[] Items => _items.ToArray();

        /// <summary>
        /// Gets the checked item.
        /// </summary>
        /// <value>The checked item.</value>
        public SingleSelectListItemViewModel? CheckedItem { get; private set; }

        /// <summary>
        /// Occurs when [checked items changed].
        /// </summary>
        public event EventHandler? CheckChanged;

        /// <summary>
        /// Gets or sets a value indicating whether [enable raise checked items changed event].
        /// </summary>
        /// <value><c>true</c> if [enable raise checked items changed event]; otherwise, <c>false</c>.</value>
        internal bool EnableRaiseCheckedItemChangedEvent { get; set; } = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="SingleSelectListViewModel"/> class.
        /// </summary>
        public SingleSelectListViewModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SingleSelectListViewModel"/> class.
        /// </summary>
        /// <param name="items">The items.</param>
        public SingleSelectListViewModel(IEnumerable<object> items)
        {
            AddRange(items);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SingleSelectListViewModel"/> class.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="checkedItem">The checked items.</param>
        public SingleSelectListViewModel(IEnumerable<object> items, object checkedItem)
        {
            _items.AddRange(items.Select(x => new SingleSelectListItemViewModel(this, x)));
            CheckedItem = _items.Find(x => x.IsValue(checkedItem));
        }

        /// <summary>
        /// Resets the items.
        /// </summary>
        /// <param name="items">The items.</param>
        public void ResetItems(IEnumerable<object> items)
        {
            _items.Clear();
            CheckedItem = null;

            RaiseSelectedItemsChangedEvent();

            AddRange(items);

            RefreshItemsCheckStates();
        }

        /// <summary>
        /// Resets the checked items.
        /// </summary>
        /// <param name="item">The items.</param>
        public void ResetSelectedItems(object? item)
        {
            CheckedItem = _items.Find(x => x.IsValue(item));

            RaiseSelectedItemsChangedEvent();

            RefreshItemsCheckStates();
        }

        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="items">The items.</param>
        public void AddRange(IEnumerable<object> items)
        {
            _items.AddRange(items.Select(x => new SingleSelectListItemViewModel(this, x)));

            RaisePropertyChanged(nameof(Items));

            RefreshItemsCheckStates();
        }

        /// <summary>
        /// Determines whether the specified item is checked.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns><c>true</c> if the specified item is checked; otherwise, <c>false</c>.</returns>
        public bool IsChecked(SingleSelectListItemViewModel item)
        {
            return CheckedItem == item;
        }

        /// <summary>
        /// Sets the checked.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public void SetChecked(SingleSelectListItemViewModel item, bool value)
        {
            CheckedItem = item;
            
            RaiseSelectedItemsChangedEvent();
            RefreshItemsCheckStates();
        }

        /// <summary>
        /// Raises the checked items changed event.
        /// </summary>
        private void RaiseSelectedItemsChangedEvent()
        {
            if(EnableRaiseCheckedItemChangedEvent)
            {
                CheckChanged?.Invoke(this, EventArgs.Empty);
            }            
        }

        /// <summary>
        /// Refreshes the items check states.
        /// </summary>
        public void RefreshItemsCheckStates()
        {
            foreach(var item in _items)
            {
                item.RaisePropertyChanged(nameof(item.IsChecked));
            }
        }
    }

    /// <summary>
    /// Class SingleSelectListItemViewModel.
    /// Implements the <see cref="MiniReactiveObject" />
    /// </summary>
    /// <seealso cref="MiniReactiveObject" />
    internal class SingleSelectListItemViewModel : MiniReactiveObject
    {
        /// <summary>
        /// The parent
        /// </summary>
        public readonly SingleSelectListViewModel Parent;

        /// <summary>
        /// The value
        /// </summary>
        public readonly object? Value;

        /// <summary>
        /// Initializes a new instance of the <see cref="SingleSelectListItemViewModel"/> class.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="value">The value.</param>
        public SingleSelectListItemViewModel(SingleSelectListViewModel model, object? value)
        {
            Parent = model;
            Value = value;
        }

        /// <summary>
        /// Determines whether the specified value is value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if the specified value is value; otherwise, <c>false</c>.</returns>
        public bool IsValue(object? value)
        {
            return Value == value || (Value != null && Value.Equals(value));
        }

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
}

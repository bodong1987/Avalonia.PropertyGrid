using System;
using System.Collections.Generic;
using System.Linq;
using PropertyModels.ComponentModel;

namespace Avalonia.PropertyGrid.ViewModels
{
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
		public SingleSelectListItemViewModel[] Items => [.. _items];

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
		public bool IsChecked(SingleSelectListItemViewModel item) => CheckedItem == item;

		/// <summary>
		/// Sets the checked.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <param name="value">if set to <c>true</c> [value].</param>
		public void SetChecked(SingleSelectListItemViewModel item, bool value)
		{
			CheckedItem = item;
			CheckedItem.IsChecked = value;

			RaiseSelectedItemsChangedEvent();
			RefreshItemsCheckStates();
		}

		/// <summary>
		/// Raises the checked items changed event.
		/// </summary>
		private void RaiseSelectedItemsChangedEvent()
		{
			if (EnableRaiseCheckedItemChangedEvent)
			{
				CheckChanged?.Invoke(this, EventArgs.Empty);
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
		public bool IsValue(object? value) => Value == value || (Value?.Equals(value) == true);

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
}
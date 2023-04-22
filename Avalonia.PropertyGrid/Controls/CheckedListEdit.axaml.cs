using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.PropertyGrid.Model.ComponentModel;
using Avalonia.PropertyGrid.Model.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Avalonia.PropertyGrid.Controls
{
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

        public static readonly DirectProperty<CheckedListEdit, object[]> ItemsProperty =
            AvaloniaProperty.RegisterDirect<CheckedListEdit, object[]>(
                nameof(Items),
                o => o.Items,
                (o, v) => o.Items = v
                );
        readonly CheckedListViewModel _Model = new CheckedListViewModel();

        public object[] Items
        {
            get => _Model.Items.Select(x=>x.Value).ToArray();
            set
            {
                if(_Model.Items != value)
                {
                    _Model.ResetItems(value);
                    _Model.RaisePropertyChanged(nameof(Items));
                }
            }
        }

        public object[] SelectedItems
        {
            get => _Model.SelectedItems.Select(x => x.Value).ToArray();
            set
            {
                _Model.ResetSelectedItems(value);                
            }
        }

        internal CheckedListViewModel Model => _Model;

        public bool EnableRaiseSelectedItemsChangedEvent
        {
            get => _Model.EnableRaiseSelectedItemsChangedEvent;
            set => _Model.EnableRaiseSelectedItemsChangedEvent = value;
        }

        public CheckedListEdit()
        {
            _Model.SelectedItemsChanged += OnSelectedItemChanged;
        }

        private void OnSelectedItemChanged(object sender, EventArgs e)
        {
            var evt = new RoutedEventArgs(SelectedItemsChangedEvent);
            RaiseEvent(evt);
        }
    }

    #region Help View Model
    internal class CheckedListViewModel : MiniReactiveObject
    {
        readonly List<CheckedListItemViewModel> _Items = new List<CheckedListItemViewModel>();

        readonly HashSet<CheckedListItemViewModel> _SelectedItems = new HashSet<CheckedListItemViewModel>();

        public CheckedListItemViewModel[] Items => _Items.ToArray();

        public CheckedListItemViewModel[] SelectedItems => _SelectedItems.ToArray();

        public event EventHandler SelectedItemsChanged;

        internal bool EnableRaiseSelectedItemsChangedEvent { get; set; } = true;

        public CheckedListViewModel()
        {
        }

        public CheckedListViewModel(IEnumerable<object> items)
        {
            AddRange(items);
        }

        public CheckedListViewModel(IEnumerable<object> items, IEnumerable<object> selectedItems)
        {
            _Items.AddRange(items.Select(x => new CheckedListItemViewModel(this, x)));

            foreach(var i in _Items)
            {
                if(selectedItems.Contains(x=> i.IsValue(x)))
                {
                    _SelectedItems.Add(i);
                }
            }
        }

        public void ResetItems(IEnumerable<object> items)
        {
            _Items.Clear();
            _SelectedItems.Clear();

            RaiseSelectedItemsChangedEvent();

            AddRange(items);

            RefreshItemsCheckStates();
        }

        public void ResetSelectedItems(IEnumerable<object> items)
        {
            _SelectedItems.Clear();

            foreach(var i in items)
            {
                var obj = _Items.Find(x => x.IsValue(i));

                if(obj != null)
                {
                    _SelectedItems.Add(obj);
                }
            }

            RaiseSelectedItemsChangedEvent();

            RefreshItemsCheckStates();
        }

        public void AddRange(IEnumerable<object> items)
        {
            _Items.AddRange(items.Select(x => new CheckedListItemViewModel(this, x)));

            RaisePropertyChanged(nameof(Items));

            RefreshItemsCheckStates();
        }

        public bool IsChecked(CheckedListItemViewModel item)
        {
            return _SelectedItems.Contains(item);
        }

        public void SetChecked(CheckedListItemViewModel item, bool value)
        {
            if (value)
            {
                if (!_SelectedItems.Contains(item))
                {
                    _SelectedItems.Add(item);

                    RaiseSelectedItemsChangedEvent();
                    RefreshItemsCheckStates();
                }
            }
            else
            {
                if(_SelectedItems.Contains(item))
                {
                    _SelectedItems.Remove(item);

                    RaiseSelectedItemsChangedEvent();
                    RefreshItemsCheckStates();
                }                
            }
        }

        private void RaiseSelectedItemsChangedEvent()
        {
            if(EnableRaiseSelectedItemsChangedEvent)
            {
                SelectedItemsChanged?.Invoke(this, EventArgs.Empty);
            }            
        }

        public void RefreshItemsCheckStates()
        {
            foreach(var item in _Items)
            {
                item.RaisePropertyChanged(nameof(item.IsChecked));
            }
        }
    }

    internal class CheckedListItemViewModel : MiniReactiveObject
    {
        public readonly CheckedListViewModel Parent;
        public readonly object Value;

        public CheckedListItemViewModel(CheckedListViewModel model, object value)
        {
            Parent = model;
            Value = value;
        }

        public bool IsValue(object value)
        {
            return Value.Equals(value);
        }

        public bool IsChecked
        {
            get
            {
                return Parent.IsChecked(this);
            }
            set
            {
                if (IsChecked != value)
                {
                    Parent.SetChecked(this, value);
                    this.RaisePropertyChanged(nameof(IsChecked));
                }
            }
        }

        public string Name => Value?.ToString();
    }
    #endregion
}

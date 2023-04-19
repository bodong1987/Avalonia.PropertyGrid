using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Avalonia.PropertyGrid.Controls
{
    public partial class CheckListEdit : UserControl
    {
        #region Events
        /// <summary>
        /// The selected items changed event
        /// </summary>
        public static readonly RoutedEvent<RoutedEventArgs> SelectedItemsChangedEvent =
            RoutedEvent.Register<CheckListEdit, RoutedEventArgs>(nameof(SelectedItemsChanged), RoutingStrategies.Bubble);

        /// <summary>
        /// Occurs when [selected items changed].
        /// </summary>
        public event EventHandler<RoutedEventArgs> SelectedItemsChanged
        {
            add => AddHandler(SelectedItemsChangedEvent, value);
            remove => RemoveHandler(SelectedItemsChangedEvent, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether [enable raise selected items changed event].
        /// </summary>
        /// <value><c>true</c> if [enable raise selected items changed event]; otherwise, <c>false</c>.</value>
        public bool EnableRaiseSelectedItemsChangedEvent { get; set; } = true;

        #endregion
        /// <summary>
        /// Initializes a new instance of the <see cref="CheckListEdit"/> class.
        /// </summary>
        public CheckListEdit()
        {
            InitializeComponent();
        }

        #region Data Sources
        private object[] ItemsCore;
        private int[] SelectedIndicesCore = new int[] { };

        /// <summary>
        /// Gets or sets the selected indices.
        /// </summary>
        /// <value>The selected indices.</value>
        public int[] SelectedIndices
        {
            get
            {
                return SelectedIndicesCore;
            }
            set
            {
                SelectedIndicesCore = value;

                OnDataSourceCheckChanged();
            }
        }

        /// <summary>
        /// Gets or sets the selected items.
        /// </summary>
        /// <value>The selected items.</value>
        public object[] SelectedItems
        {
            get
            {
                if (ItemsCore == null || SelectedIndicesCore == null)
                {
                    return new object[] { };
                }

                List<object> items = new List<object>();
                foreach (var i in SelectedIndicesCore)
                {
                    if (i >= 0 && i < ItemsCore.Length)
                    {
                        items.Add(ItemsCore[i]);
                    }
                }

                return items.ToArray();
            }
            set
            {
                if (ItemsCore != null && value != null)
                {
                    SortedSet<int> ints = new SortedSet<int>();

                    foreach (var item in value)
                    {
                        int index = Array.IndexOf(ItemsCore, item);

                        if (index != -1)
                        {
                            ints.Add(index);
                        }
                    }

                    SelectedIndices = ints.ToArray();
                }
                else
                {
                    SelectedIndices = null;
                }
            }
        }

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>The items.</value>
        public object[] Items
        {
            get
            {
                return ItemsCore;
            }
            set
            {
                if (ItemsCore != value)
                {
                    ItemsCore = value;

                    OnDataSourceChanged();
                }
            }
        }
        #endregion

        #region Data Cache
        private struct ItemInfo
        {
            public object Target;
            public CheckBox CheckBox;
            public TextBlock TextBlock;
        }

        Dictionary<object, ItemInfo> ItemControlCaches = new Dictionary<object, ItemInfo>();

        private void OnDataSourceChanged()
        {
            MainGrid.Children.Clear();
            MainGrid.RowDefinitions.Clear();
            ItemControlCaches.Clear();

            if (ItemsCore == null || ItemsCore.Length == 0)
            {
                return;
            }

            foreach (var item in ItemsCore)
            {
                if (item == null)
                {
                    throw new ArgumentNullException("CheckListEdit's Items can't contain null.");
                }

                MainGrid.RowDefinitions.Add(new RowDefinition(20, GridUnitType.Auto));

                CheckBox box = new CheckBox();
                box.SetValue(Grid.RowProperty, MainGrid.RowDefinitions.Count - 1);

                MainGrid.Children.Add(box);

                TextBlock textBlock = new TextBlock();
                textBlock.Text = item.ToString();
                textBlock.SetValue(Grid.RowProperty, MainGrid.RowDefinitions.Count - 1);
                textBlock.SetValue(Grid.ColumnProperty, 1);
                textBlock.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;

                MainGrid.Children.Add(textBlock);

                ItemControlCaches.Add(item, new ItemInfo()
                {
                    Target = item,
                    CheckBox = box,
                    TextBlock = textBlock
                });

                box.Checked += (s, e) => HandleCheckChanged(item, box);
                box.Unchecked += (s, e) => HandleCheckChanged(item, box);
            }
        }

        private void HandleCheckChanged(object item, CheckBox box)
        {
            List<int> indices = new List<int>();
            foreach (var info in ItemControlCaches.Values)
            {
                if ((bool)info.CheckBox.IsChecked)
                {
                    int index = Array.IndexOf(ItemsCore, info.Target);

                    if (index != -1)
                    {
                        indices.Add(index);
                    }
                }
            }

            SelectedIndicesCore = indices.ToArray();

            if (EnableRaiseSelectedItemsChangedEvent)
            {
                var et = new RoutedEventArgs(SelectedItemsChangedEvent);

                RaiseEvent(et);
            }
        }

        private void OnDataSourceCheckChanged()
        {
            if (SelectedIndicesCore == null)
            {
                return;
            }

            foreach (var i in ItemControlCaches.Values)
            {
                i.CheckBox.IsChecked = false;
            }

            foreach (var item in SelectedItems)
            {
                if (ItemControlCaches.TryGetValue(item, out var infoValue))
                {
                    infoValue.CheckBox.IsChecked = true;
                }
            }
        }
        #endregion
    }
}

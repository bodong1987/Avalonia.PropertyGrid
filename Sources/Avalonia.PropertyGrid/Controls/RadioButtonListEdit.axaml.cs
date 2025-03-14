using System;
using System.Linq;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.PropertyGrid.ViewModels;

namespace Avalonia.PropertyGrid.Controls
{
    /// <summary>
    /// base interface
    /// </summary>
    public interface ICheckableListEdit
    {
        /// <summary>
        /// get checked item
        /// </summary>
        object? CheckedItem { get; set; }

        /// <summary>
        /// get target items
        /// </summary>
        object[] Items { get; set; }

        /// <summary>
        /// used to disable event trigger
        /// </summary>
        bool EnableRaiseCheckedItemChangedEvent { get; set; }

        /// <summary>
        /// check changed event
        /// </summary>
        event EventHandler<RoutedEventArgs> CheckChanged;
    }

    /// <summary>
    /// Class RadioButtonListEdit.
    /// Implements the <see cref="TemplatedControl" />
    /// </summary>
    /// <seealso cref="TemplatedControl" />
    public class RadioButtonListEdit : TemplatedControl, ICheckableListEdit
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
        public RadioButtonListEdit() => Model.CheckChanged += OnCheckedItemChanged;

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
}

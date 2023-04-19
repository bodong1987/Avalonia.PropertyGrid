using Avalonia.Controls;
using Avalonia.Interactivity;
using System;

namespace Avalonia.PropertyGrid.Controls
{
    public partial class ButtonEdit : UserControl
    {
        #region Events
        /// <summary>
        /// The button click event
        /// </summary>
        public static readonly RoutedEvent<RoutedEventArgs> ButtonClickEvent =
            RoutedEvent.Register<ButtonEdit, RoutedEventArgs>(nameof(ButtonClick), RoutingStrategies.Bubble);

        /// <summary>
        /// Occurs when [button click].
        /// </summary>
        public event EventHandler<RoutedEventArgs> ButtonClick
        {
            add => AddHandler(ButtonClickEvent, value);
            remove => RemoveHandler(ButtonClickEvent, value);
        }

        /// <summary>
        /// The text changed event
        /// </summary>
        public static readonly RoutedEvent<RoutedEventArgs> TextChangedEvent =
            RoutedEvent.Register<ButtonEdit, RoutedEventArgs>(nameof(TextChanged), RoutingStrategies.Bubble);

        /// <summary>
        /// Occurs when [text changed].
        /// </summary>
        public event EventHandler<RoutedEventArgs> TextChanged
        {
            add => AddHandler(TextChangedEvent, value);
            remove => RemoveHandler(TextChangedEvent, value);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string Text
        {
            get
            {
                return TextEdit.Text;
            }
            set
            {
                TextEdit.Text = value;
            }
        }

        /// <summary>
        /// Gets or sets the button text.
        /// </summary>
        /// <value>The button text.</value>
        public string ButtonText
        {
            get
            {
                return BrowserButton.Content as string;
            }
            set
            {
                BrowserButton.Content = value;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ButtonEdit"/> class.
        /// </summary>
        public ButtonEdit()
        {
            InitializeComponent();

        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Handles the <see cref="E:ButtonClicked" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnButtonClicked(object sender, RoutedEventArgs e)
        {
            var evt = new RoutedEventArgs(ButtonClickEvent);
            RaiseEvent(evt);
        }

        /// <summary>
        /// Handles the <see cref="E:TextEditPropertyChanged" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="AvaloniaPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnTextEditPropertyChanged(object sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.Property == TextBox.TextProperty)
            {
                var evt = new RoutedEventArgs(TextChangedEvent);
                RaiseEvent(evt);
            }
        }
        #endregion
    }
}

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.PropertyGrid.Model.ComponentModel;
using System;
using System.Windows.Input;

namespace Avalonia.PropertyGrid.Controls
{
    public class ButtonEdit : TemplatedControl
    {
        public static readonly DirectProperty<ButtonEdit, ICommand> ButtonClickedCommandProperty =
            AvaloniaProperty.RegisterDirect<ButtonEdit, ICommand>(
                nameof(ButtonClickedCommand),
                o => o.ButtonClickedCommand,
                (o, v) => o.ButtonClickedCommand = v
                );

        private ICommand _ButtonClickedCommand;

        public ICommand ButtonClickedCommand
        {
            get => _ButtonClickedCommand;
            set => SetAndRaise(ButtonClickedCommandProperty, ref _ButtonClickedCommand, value);
        }

        public static readonly DirectProperty<ButtonEdit, string> TextProperty =
            AvaloniaProperty.RegisterDirect<ButtonEdit, string>(
                nameof(Text),
                o => o.Text,
                (o, v) => o.Text = v
                );

        string _Text;
        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string Text
        {
            get => _Text;
            set => SetAndRaise(TextProperty, ref _Text, value);
        }

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

        static ButtonEdit()
        {
            TextProperty.Changed.Subscribe(OnTextProperyChanged);
        }

        public ButtonEdit()
        {
            ButtonClickedCommand = ReactiveCommand.Create(OnButtonClicked);
        }

        private void OnButtonClicked(object sender)
        {
            var evt = new RoutedEventArgs(ButtonClickEvent);
            RaiseEvent(evt);
        }

        private static void OnTextProperyChanged(AvaloniaPropertyChangedEventArgs<string> e)
        {
            if(e.Sender is ButtonEdit be)
            {
                be.OntextPropertyChanged(e.NewValue.Value);
            }
        }

        private void OntextPropertyChanged(string value)
        {
            var evt = new RoutedEventArgs(TextChangedEvent);
            RaiseEvent(evt);
        }
    }
}

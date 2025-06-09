using System;
using System.Windows.Input;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Reactive;
using PropertyModels.ComponentModel;

namespace Avalonia.PropertyGrid.Controls;

/// <summary>
/// Class ButtonEdit.
/// Implements the <see cref="TemplatedControl" />
/// </summary>
/// <seealso cref="TemplatedControl" />
public class ButtonEdit : TemplatedControl
{
    /// <summary>
    /// The button clicked command property
    /// </summary>
    public static readonly DirectProperty<ButtonEdit, ICommand> ButtonClickedCommandProperty =
        AvaloniaProperty.RegisterDirect<ButtonEdit, ICommand>(
            nameof(ButtonClickedCommand),
            o => o.ButtonClickedCommand!,
            (o, v) => o.ButtonClickedCommand = v
        );

    /// <summary>
    /// Gets or sets the button clicked command.
    /// </summary>
    /// <value>The button clicked command.</value>
    public ICommand? ButtonClickedCommand
    {
        get;
        set => SetAndRaise(ButtonClickedCommandProperty!, ref field, value);
    }

    /// <summary>
    /// The IsReadOnly property
    /// </summary>
    public static readonly DirectProperty<ButtonEdit, bool> IsReadOnlyProperty =
        AvaloniaProperty.RegisterDirect<ButtonEdit, bool>(
            nameof(IsReadOnly),
            o => o.IsReadOnly,
            (o, v) => o.IsReadOnly = v
        );

    /// <summary>
    /// Gets or sets Is ReadOnly Flags.
    /// </summary>
    public bool IsReadOnly
    {
        get;
        set => SetAndRaise(IsReadOnlyProperty, ref field, value);
    }

    /// <summary>
    /// The text property
    /// </summary>
    public static readonly DirectProperty<ButtonEdit, string> TextProperty =
        AvaloniaProperty.RegisterDirect<ButtonEdit, string>(
            nameof(Text),
            o => o.Text ?? string.Empty,
            (o, v) => o.Text = v
        );

    /// <summary>
    /// Gets or sets the text.
    /// </summary>
    /// <value>The text.</value>
    public string? Text
    {
        get;
        set => SetAndRaise(TextProperty!, ref field, value);
    }

    /// <summary>
    /// The watermark property
    /// </summary>
    public static readonly DirectProperty<ButtonEdit, string> WatermarkProperty =
        AvaloniaProperty.RegisterDirect<ButtonEdit, string>(
            nameof(Watermark),
            o => o.Watermark ?? string.Empty,
            (o, v) => o.Watermark = v
        );

    /// <summary>
    /// Gets or sets the Watermark.
    /// </summary>
    /// <value>The Watermark.</value>
    public string? Watermark
    {
        get;
        set => SetAndRaise(WatermarkProperty!, ref field, value);
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

    /// <summary>
    /// Initializes static members of the <see cref="ButtonEdit"/> class.
    /// </summary>
    static ButtonEdit() => TextProperty.Changed.Subscribe(new AnonymousObserver<AvaloniaPropertyChangedEventArgs<string>>(OnTextPropertyChanged));

    /// <summary>
    /// Initializes a new instance of the <see cref="ButtonEdit"/> class.
    /// </summary>
    public ButtonEdit() => ButtonClickedCommand = ReactiveCommand.Create(OnButtonClicked);

    /// <summary>
    /// Called when [button clicked].
    /// </summary>
    /// <param name="sender">The sender.</param>
    private void OnButtonClicked(object? sender)
    {
        var evt = new RoutedEventArgs(ButtonClickEvent);
        RaiseEvent(evt);
    }

    /// <summary>
    /// Called when [text property changed].
    /// </summary>
    /// <param name="e">The e.</param>
    private static void OnTextPropertyChanged(AvaloniaPropertyChangedEventArgs<string> e)
    {
        if (e.Sender is ButtonEdit be)
        {
            be.OnTextPropertyChanged(e.NewValue.Value);
        }
    }

    /// <summary>
    /// Called when [property changed].
    /// </summary>
    /// <param name="value">The value.</param>
    private void OnTextPropertyChanged(string? value)
    {
        var evt = new RoutedEventArgs(TextChangedEvent);
        RaiseEvent(evt);
    }
}
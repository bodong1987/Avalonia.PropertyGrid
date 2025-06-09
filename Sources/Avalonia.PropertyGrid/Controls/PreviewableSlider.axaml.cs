using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;

namespace Avalonia.PropertyGrid.Controls;

/// <summary>
/// A slider control that provides preview and real value changed events.
/// </summary>
public class PreviewableSlider : TemplatedControl
{
    // Define dependency properties for Slider's bindable properties

    /// <summary>
    /// Defines the <see cref="Minimum"/> property.
    /// </summary>
    public static readonly StyledProperty<double> MinimumProperty =
        AvaloniaProperty.Register<PreviewableSlider, double>(nameof(Minimum));

    /// <summary>
    /// Gets or sets the minimum possible value.
    /// </summary>
    public double Minimum
    {
        get => GetValue(MinimumProperty);
        set => SetValue(MinimumProperty, value);
    }

    /// <summary>
    /// Defines the <see cref="Maximum"/> property.
    /// </summary>
    public static readonly StyledProperty<double> MaximumProperty =
        AvaloniaProperty.Register<PreviewableSlider, double>(nameof(Maximum), 100);

    /// <summary>
    /// Gets or sets the maximum possible value.
    /// </summary>
    public double Maximum
    {
        get => GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    /// <summary>
    /// Defines the <see cref="Value"/> property.
    /// </summary>
    public static readonly StyledProperty<double> ValueProperty =
        AvaloniaProperty.Register<PreviewableSlider, double>(nameof(Value));

    /// <summary>
    /// Gets or sets the current value.
    /// </summary>
    public double Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    /// <summary>
    /// Defines the <see cref="Orientation"/> property.
    /// </summary>
    public static readonly StyledProperty<Orientation> OrientationProperty =
        AvaloniaProperty.Register<PreviewableSlider, Orientation>(nameof(Orientation));

    /// <summary>
    /// Gets or sets the orientation of the slider.
    /// </summary>
    public Orientation Orientation
    {
        get => GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    /// <summary>
    /// Defines the <see cref="TickFrequency"/> property.
    /// </summary>
    public static readonly StyledProperty<double> TickFrequencyProperty =
        AvaloniaProperty.Register<PreviewableSlider, double>(nameof(TickFrequency), 1.0);

    /// <summary>
    /// Gets or sets the frequency of ticks on the slider.
    /// </summary>
    public double TickFrequency
    {
        get => GetValue(TickFrequencyProperty);
        set => SetValue(TickFrequencyProperty, value);
    }

    /// <summary>
    /// Defines the <see cref="IsSnapToTickEnabled"/> property.
    /// </summary>
    public static readonly StyledProperty<bool> IsSnapToTickEnabledProperty =
        AvaloniaProperty.Register<PreviewableSlider, bool>(nameof(IsSnapToTickEnabled));

    /// <summary>
    /// Gets or sets whether the slider snaps to tick marks.
    /// </summary>
    public bool IsSnapToTickEnabled
    {
        get => GetValue(IsSnapToTickEnabledProperty);
        set => SetValue(IsSnapToTickEnabledProperty, value);
    }

    /// <summary>
    /// Defines the PreviewValueChanged routed event.
    /// </summary>
    public static readonly RoutedEvent<RoutedEventArgs> PreviewValueChangedEvent =
        RoutedEvent.Register<PreviewableSlider, RoutedEventArgs>(nameof(PreviewValueChanged), RoutingStrategies.Bubble);

    /// <summary>
    /// Occurs when the thumb is dragged, providing a preview of the value change.
    /// </summary>
    public event EventHandler<RoutedEventArgs> PreviewValueChanged
    {
        add => AddHandler(PreviewValueChangedEvent, value);
        remove => RemoveHandler(PreviewValueChangedEvent, value);
    }

    /// <summary>
    /// Defines the RealValueChanged routed event.
    /// </summary>
    public static readonly RoutedEvent<RoutedEventArgs> RealValueChangedEvent =
        RoutedEvent.Register<PreviewableSlider, RoutedEventArgs>(nameof(RealValueChanged), RoutingStrategies.Bubble);

    /// <summary>
    /// Occurs when the value is changed by an operation other than dragging.
    /// </summary>
    public event EventHandler<RoutedEventArgs> RealValueChanged
    {
        add => AddHandler(RealValueChangedEvent, value);
        remove => RemoveHandler(RealValueChangedEvent, value);
    }

    private bool _isDragging;
    private double _preDragValue;
    private Slider? _slider;

    /// <inheritdoc />
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _slider = e.NameScope.Find<Slider>("slider");
        if (_slider != null)
        {
            _slider.AddHandler(Thumb.DragStartedEvent, OnDragStarted);
            _slider.AddHandler(Thumb.DragCompletedEvent, OnDragCompleted);
            _slider.ValueChanged += OnLocalValueChanged;
        }
    }

    private void OnLocalValueChanged(object? sender, RangeBaseValueChangedEventArgs e)
    {
        RaiseEvent(_isDragging
            ? new RoutedEventArgs(PreviewValueChangedEvent, this)
            : new RealValueChangedEventArgs(RealValueChangedReason.NotDragging, double.NaN, RealValueChangedEvent, this));

        e.Handled = true;
    }

    private void OnDragStarted(object? sender, VectorEventArgs e)
    {
        _isDragging = true;
        _preDragValue = Value;
        e.Handled = true;
    }

    private void OnDragCompleted(object? sender, VectorEventArgs e)
    {
        if (!_isDragging) return;

        _isDragging = false;

        if (Math.Abs(_preDragValue - Value) > double.Epsilon)
        {
            RaiseEvent(new RealValueChangedEventArgs(RealValueChangedReason.DragEnd, _preDragValue, RealValueChangedEvent, this));
        }

        e.Handled = true;
    }
}

/// <summary>
/// Enumerates the reasons for a real value change.
/// </summary>
public enum RealValueChangedReason
{
    /// <summary>
    /// The value changed by an operation other than dragging.
    /// </summary>
    NotDragging,

    /// <summary>
    /// The value changed due to the completion of a drag operation.
    /// </summary>
    DragEnd
}

/// <summary>
/// Provides data for the RealValueChanged event.
/// </summary>
public class RealValueChangedEventArgs : RoutedEventArgs
{
    /// <summary>
    /// Gets the reason for the value change.
    /// </summary>
    public readonly RealValueChangedReason Reason;

    /// <summary>
    /// Gets the value before the change.
    /// </summary>
    public readonly double OldValue;

    /// <summary>
    /// Initializes a new instance of the RealValueChangedEventArgs class.
    /// </summary>
    /// <param name="reason">The reason for the value change.</param>
    /// <param name="oldValue">The value before the change.</param>
    /// <param name="routedEvent">The routed event identifier for the event.</param>
    /// <param name="source">The source of the event.</param>
    public RealValueChangedEventArgs(RealValueChangedReason reason, double oldValue, RoutedEvent? routedEvent, object? source) :
        base(routedEvent, source)
    {
        Reason = reason;
        OldValue = oldValue;
    }
}
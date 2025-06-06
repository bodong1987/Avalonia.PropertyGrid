using System;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data.Converters;
using Avalonia.Input;
using Avalonia.Interactivity;
using PropertyModels.Utils;

namespace Avalonia.PropertyGrid.Controls
{
    /// <summary>
    /// Class TrackableEdit.
    /// Implements the <see cref="TemplatedControl" />
    /// </summary>
    /// <seealso cref="TemplatedControl" />
    public class TrackableEdit : RangeBase
    {
        /// <summary>
        /// The increment property
        /// </summary>
        public static readonly StyledProperty<double> IncrementProperty =
            AvaloniaProperty.Register<TrackableEdit, double>(nameof(Increment), 0.01);

        /// <summary>
        /// Gets or sets the increment.
        /// </summary>
        /// <value>The increment.</value>
        public double Increment
        {
            get => GetValue(IncrementProperty);
            set => SetValue(IncrementProperty, value);
        }

        /// <summary>
        /// The format string property
        /// </summary>
        public static readonly StyledProperty<string> FormatStringProperty =
            AvaloniaProperty.Register<TrackableEdit, string>(nameof(FormatString), "{0:0.00}");

        /// <summary>
        /// Gets or sets the format string.
        /// </summary>
        /// <value>The format string.</value>
        public string FormatString
        {
            get => GetValue(FormatStringProperty);
            set => SetValue(FormatStringProperty, value);
        }

        /// <summary>
        /// The Readonly property
        /// </summary>
        public static readonly StyledProperty<bool> IsReadOnlyProperty =
            AvaloniaProperty.Register<TrackableEdit, bool>(nameof(IsReadOnly));

        /// <summary>
        /// Gets or sets IsReadOnly state
        /// </summary>        
        public bool IsReadOnly
        {
            get => GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, value);
        }
        
        /// <summary>
        /// The show button spinner property
        /// </summary>
        public static readonly StyledProperty<bool> ShowButtonSpinnerProperty =
            AvaloniaProperty.Register<TrackableEdit, bool>(nameof(ShowButtonSpinner), true);

        /// <summary>
        /// Gets or sets ShowButton Spinner state
        /// </summary>        
        public bool ShowButtonSpinner
        {
            get => GetValue(ShowButtonSpinnerProperty);
            set => SetValue(ShowButtonSpinnerProperty, value);
        }
        
        /// <summary>
        /// allow spin
        /// </summary>
        public static readonly StyledProperty<bool> AllowSpinProperty =
            AvaloniaProperty.Register<TrackableEdit, bool>(nameof(AllowSpin), true);

        /// <summary>
        /// Gets or sets allow spin state
        /// </summary>        
        public bool AllowSpin
        {
            get => GetValue(AllowSpinProperty);
            set => SetValue(AllowSpinProperty, value);
        }
  
        /// <summary>
        /// when you drag with thumb, it will raise preview value changed event
        /// </summary>
        public static readonly RoutedEvent<RoutedEventArgs> PreviewValueChangedEvent =
            RoutedEvent.Register<TrackableEdit, RoutedEventArgs>(nameof(PreviewValueChanged), RoutingStrategies.Bubble);
        
        /// <summary>
        /// when you drag with thumb, it will raise preview value changed event
        /// </summary>
        public event EventHandler<RoutedEventArgs> PreviewValueChanged
        {
            add => AddHandler(PreviewValueChangedEvent, value);
            remove => RemoveHandler(PreviewValueChangedEvent, value);
        }
       
        /// <summary>
        /// not drag or change by other operation
        /// </summary>
        public static readonly RoutedEvent<RoutedEventArgs> RealValueChangedEvent =
            RoutedEvent.Register<TrackableEdit, RoutedEventArgs>(nameof(RealValueChanged), RoutingStrategies.Bubble);
        
        /// <summary>
        /// not drag or change by other operation
        /// </summary>
        public event EventHandler<RoutedEventArgs> RealValueChanged
        {
            add => AddHandler(RealValueChangedEvent, value);
            remove => RemoveHandler(RealValueChangedEvent, value);
        }
        
        private bool _isDragging;
        private double _preDragValue;

        /// <inheritdoc />
        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
            
            var slider = e.NameScope.Find<Slider>("slider");
            if (slider != null)
            {
                slider.AddHandler(Thumb.DragStartedEvent, OnDragStarted);
                slider.AddHandler(Thumb.DragCompletedEvent, OnDragCompleted);
            }

            ValueChanged += OnLocalValueChanged;
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
                RaiseEvent(new RealValueChangedEventArgs( RealValueChangedReason.DragEnd, _preDragValue, RealValueChangedEvent, this));
            }
        
            e.Handled = true;
        }
    }

    /// <summary>
    /// real value change reason
    /// </summary>
    public enum RealValueChangedReason
    {
        /// <summary>
        /// change by not drag
        /// </summary>
        NotDragging,
        
        /// <summary>
        /// finish drag
        /// </summary>
        DragEnd
    }
    
    /// <summary>
    /// Real value changed event args
    /// </summary>
    public class RealValueChangedEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// reason
        /// </summary>
        public readonly RealValueChangedReason Reason;

        /// <summary>
        /// start drag value 
        /// </summary>
        public readonly double OldValue;

        /// <summary>
        /// construct this event args
        /// </summary>
        /// <param name="reason"></param>
        /// <param name="oldValue">old value(optional)</param>
        /// <param name="routedEvent"></param>
        /// <param name="source"></param>
        public RealValueChangedEventArgs(RealValueChangedReason reason, double oldValue, RoutedEvent? routedEvent, object? source) :
            base(routedEvent, source)
        {
            Reason = reason;
            OldValue = oldValue;
        }
    }

    /// <summary>
    /// Class DoubleToDecimalConverter.
    /// Implements the <see cref="IValueConverter" />
    /// </summary>
    /// <seealso cref="IValueConverter" />
    public class DoubleToDecimalConverter : IValueConverter
    {
        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="targetType">The type of the target.</param>
        /// <param name="parameter">A user-defined parameter.</param>
        /// <param name="culture">The culture to use.</param>
        /// <returns>The converted value.</returns>
        /// <remarks>This method should not throw exceptions. If the value is not convertible, return
        /// a <see cref="T:Avalonia.Data.BindingNotification" /> in an error state. Any exceptions thrown will be
        /// treated as an application exception.</remarks>
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if ((targetType == typeof(decimal) || targetType == typeof(decimal?)) && value != null)
            {
                return DecimalConvertUtils.ConvertTo(value);
            }

            return value;
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="targetType">The type of the target.</param>
        /// <param name="parameter">A user-defined parameter.</param>
        /// <param name="culture">The culture to use.</param>
        /// <returns>The converted value.</returns>
        /// <remarks>This method should not throw exceptions. If the value is not convertible, return
        /// a <see cref="T:Avalonia.Data.BindingNotification" /> in an error state. Any exceptions thrown will be
        /// treated as an application exception.</remarks>
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value != null)
            {
                try
                {
                    var v = System.Convert.ChangeType(value, targetType);

                    // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                    if (v != null)
                    {
                        return v;
                    }
                }
                catch
                {
                    // ignored
                }
            }

            return value;
        }
    }
}

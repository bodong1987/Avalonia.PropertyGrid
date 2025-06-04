using System;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.PropertyGrid.Controls;
using Avalonia.PropertyGrid.Controls.Factories;

namespace Avalonia.PropertyGrid.Samples.PainterDemo.Views;

public class GradientStopView : TemplatedControl
{
    public EventHandler<AvaloniaPropertyChangedEventArgs>? GradientStopChanged;

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        if (change.Property == DataContextProperty)
        {
            if (change.OldValue is GradientStop oldValue)
                oldValue.PropertyChanged -= OnGradientStopValuePropertyChanged;

            if (change.NewValue is GradientStop newValue)
                newValue.PropertyChanged += OnGradientStopValuePropertyChanged;
        }

        base.OnPropertyChanged(change);
    }

    private void OnGradientStopValuePropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e) =>
        GradientStopChanged?.Invoke(sender, e);
}

internal class GradientStopCellEditFactory : AbstractCellEditFactory
{
    public override Control? HandleNewProperty(PropertyCellContext context)
    {
        if (context.Property.PropertyType != typeof(GradientStop)) return null;

        var value = (context.GetValue() as GradientStop)!;
        
        var control = new GradientStopView
        {
            DataContext = new GradientStop(value.Color, value.Offset)  // make a copy
        };

        control.GradientStopChanged += (s, e) =>
        {
            var currentValue = (control.DataContext as GradientStop)!;
            
            // make a copy, so it will save the changed command in command queue...
            SetAndRaise(context, control, new GradientStop(currentValue.Color, currentValue.Offset));
        };

        return control;
    }


    public override bool HandlePropertyChanged(PropertyCellContext context)
    {
        if (context?.Property.PropertyType != typeof(GradientStop)) return false;

        if (context.CellEdit is GradientStopView control)
        {
            var value = (context.GetValue() as GradientStop)!;
            control.DataContext = new GradientStop(value.Color, value.Offset);
            return true;
        }

        return false;
    }
}
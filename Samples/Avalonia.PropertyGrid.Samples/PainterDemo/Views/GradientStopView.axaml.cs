using System;
using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.PropertyGrid.Controls;
using Avalonia.PropertyGrid.Controls.Factories;

namespace Avalonia.PropertyGrid.Samples.PainterDemo.Views;

public class GradientStopView : TemplatedControl
{
    public EventHandler? PreviewColorChanged;
    public EventHandler? ColorChanged;
    public EventHandler? PreviewOffsetChanged;
    public EventHandler? OffsetChanged;

    public Color Color => _previewableColorPicker!.Color;
    public Color StartPreviewColor => _previewableColorPicker!.StartPreviewColor;

    public double Offset => _previewableSlider!.Value;
    public double StartPreviewOffset => _previewableSlider!.StartPreviewValue;
    
    private PreviewableColorPicker? _previewableColorPicker;
    private PreviewableSlider? _previewableSlider;
    
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _previewableColorPicker = e.NameScope.Find<PreviewableColorPicker>("Part_ColorPicker");
        _previewableSlider = e.NameScope.Find<PreviewableSlider>("Part_Slider");
        
        Debug.Assert(_previewableColorPicker != null);
        Debug.Assert(_previewableSlider != null);
        
        _previewableColorPicker.PreviewColorChanged += (s,ee)=> PreviewColorChanged?.Invoke(this, ee);
        _previewableColorPicker.ColorChanged += (s,ee)=> ColorChanged?.Invoke(this, ee);
        _previewableSlider.PreviewValueChanged += (s, ee)=> PreviewOffsetChanged?.Invoke(this, ee);
        _previewableSlider.RealValueChanged += (s, ee)=> OffsetChanged?.Invoke(this, ee);
    }
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

        control.PreviewColorChanged += (s, e) =>
        {
            var currentValue = control.Color;
            
            // only set value, don't generate command...
            HandleSetValue(context, control, new GradientStop(currentValue, control.Offset) );
        };

        control.ColorChanged += (s, e) =>
        {
            var currentValue = control.Color;
            var oldValue = control.StartPreviewColor;

            // set and generate command
            SetAndRaise(context, control, new GradientStop(currentValue, control.Offset), new GradientStop(oldValue, control.Offset));
        };

        control.PreviewOffsetChanged += (s, e) =>
        {
            var currentValue = control.Color;
            
            // only set value, don't generate command...
            HandleSetValue(context, control, new GradientStop(currentValue, control.StartPreviewOffset));
        };

        control.OffsetChanged += (s, e) =>
        {
            var currentValue = control.Color;
            
            // set and generate command
            SetAndRaise(context, control, new GradientStop(currentValue, control.Offset), new GradientStop(currentValue, control.StartPreviewOffset));
        };

        return control;
    }


    public override bool HandlePropertyChanged(PropertyCellContext context)
    {
        if (context.Property.PropertyType != typeof(GradientStop)) return false;

        if (context.CellEdit is GradientStopView control)
        {
            var value = (context.GetValue() as GradientStop)!;
            control.DataContext = new GradientStop(value.Color, value.Offset);
            return true;
        }

        return false;
    }
}
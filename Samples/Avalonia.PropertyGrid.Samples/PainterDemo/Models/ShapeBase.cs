using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using PropertyModels.ComponentModel;
// ReSharper disable CompareOfFloatsByEqualityOperator
// ReSharper disable PropertyCanBeMadeInitOnly.Global

namespace Avalonia.PropertyGrid.Samples.PainterDemo.Models;

public abstract class ShapeBase : MiniReactiveObject
{
    private double _x;
    
    [Category("Transform")]
    [FloatPrecision(0)]
    public double X
    {
        get => _x;
        set
        {
            if (_x != value)
            {
                _x = value;
                NotifyPropertyChanged();
            }
        }
    }
    private double _y;
    [Category("Transform")]
    [FloatPrecision(0)]
    public double Y
    {
        get => _y;
        set
        {
            if (_y != value)
            {
                _y = value;
                NotifyPropertyChanged();
            }
        }
    }

    private Color _fillColor;
    
    [Category("Appearance")]
    public Color FillColor
    {
        get => _fillColor;
        set
        {
            if (_fillColor != value)
            {
                _fillColor = value;
                NotifyPropertyChanged();
            }
        }
    }

    private double _opacity = 1.0;
    [Category("Appearance")]
    [Range(0.0, 1.0)]
    [Trackable(0.0, 1.0)]
    public double Opacity
    {
        get => _opacity;
        set
        {
            if (_opacity != value)
            {
                _opacity = value;
                NotifyPropertyChanged();
            }
        }
    }
    
    public abstract Shape CreateAvaloniaShape();

    public virtual bool UpdateProperties(Shape shape)
    {
        shape.Fill = new SolidColorBrush(FillColor);
        shape.Opacity = Opacity;
        Canvas.SetLeft(shape, X);
        Canvas.SetTop(shape, Y);
        
        return true;
    }

    protected void NotifyPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        System.Diagnostics.Debug.Assert(propertyName != null);
        // ReSharper disable once RedundantSuppressNullableWarningExpression
        RaisePropertyChanged(propertyName!);
    }
}
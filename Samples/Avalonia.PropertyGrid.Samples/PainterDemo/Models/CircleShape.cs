using System.ComponentModel;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using PropertyModels.ComponentModel;

// ReSharper disable CompareOfFloatsByEqualityOperator
// ReSharper disable PropertyCanBeMadeInitOnly.Global

namespace Avalonia.PropertyGrid.Samples.PainterDemo.Models;

public class CircleShape : ShapeBase
{
    private double _radius;
    [Category("Transform")]
    [FloatPrecision(0)]
    public double Radius
    {
        get => _radius;
        set
        {
            if (_radius != value)
            {
                _radius = value;
                NotifyPropertyChanged();
            }
        }
    }

    public override Shape CreateAvaloniaShape()
    {
        // Create an Ellipse for CircleShape
        return new Ellipse
        {
            Width = Radius * 2,
            Height = Radius * 2,
            Fill = new SolidColorBrush(FillColor),
            Opacity = Opacity
        };
    }

    public override bool UpdateProperties(Shape shape)
    {
        if (!base.UpdateProperties(shape))
        {
            return false;
        }
        
        if (shape is Ellipse ellipse)
        {
            ellipse.Width = Radius * 2;
            ellipse.Height = Radius * 2;
            return true;
        }
        
        return false;
    }
}

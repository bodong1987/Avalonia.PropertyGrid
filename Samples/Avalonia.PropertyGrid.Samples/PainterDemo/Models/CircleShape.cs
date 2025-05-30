using System.ComponentModel;
using Avalonia.Controls.Shapes;
using PropertyModels.ComponentModel;

// ReSharper disable CompareOfFloatsByEqualityOperator
// ReSharper disable PropertyCanBeMadeInitOnly.Global

namespace Avalonia.PropertyGrid.Samples.PainterDemo.Models;

public class CircleShape : ShapeGeneric<Ellipse>
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

    protected override void ApplyProperties(Ellipse shape)
    {
        shape.Width = Radius * 2;
        shape.Height = Radius * 2;
    }
}

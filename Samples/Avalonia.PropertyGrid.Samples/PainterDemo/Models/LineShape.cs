using System.ComponentModel;
using Avalonia.Controls.Shapes;
using PropertyModels.ComponentModel;
// ReSharper disable CompareOfFloatsByEqualityOperator
// ReSharper disable PropertyCanBeMadeInitOnly.Global

namespace Avalonia.PropertyGrid.Samples.PainterDemo.Models;

public class LineShape : ShapeGeneric<Line>
{
    private double _x2;
    private double _y2;

    [Category("Transform")]
    [FloatPrecision(0)]
    public double X2
    {
        get => _x2;
        set
        {
            if (_x2 != value)
            {
                _x2 = value;
                NotifyPropertyChanged();
            }
        }
    }

    [Category("Transform")]
    [FloatPrecision(0)]
    public double Y2
    {
        get => _y2;
        set
        {
            if (_y2 != value)
            {
                _y2 = value;
                NotifyPropertyChanged();
            }
        }
    }

    public LineShape()
    {
        IsFillModeVisible = false;
    }

    protected override void ApplyProperties(Line shape)
    {
        shape.StartPoint = new Point(0, 0);
        shape.EndPoint = new Point(X2, Y2);
    }
}
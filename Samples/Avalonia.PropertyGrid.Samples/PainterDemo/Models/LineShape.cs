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
        set => SetProperty(ref _x2, value);
    }

    [Category("Transform")]
    [FloatPrecision(0)]
    public double Y2
    {
        get => _y2;
        set => SetProperty(ref _y2, value);
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
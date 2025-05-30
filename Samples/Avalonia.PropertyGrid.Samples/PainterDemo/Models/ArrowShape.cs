using System.Collections.Generic;
using System.ComponentModel;
using PropertyModels.ComponentModel;
// ReSharper disable CompareOfFloatsByEqualityOperator
// ReSharper disable PropertyCanBeMadeInitOnly.Global

namespace Avalonia.PropertyGrid.Samples.PainterDemo.Models;

public class ArrowShape : ShapeGenericPolygon
{
    private double _length;
    private double _shaftWidth;
    private double _headWidth;
    private double _headHeight;

    [Category("Transform")]
    [FloatPrecision(0)]
    public double Length
    {
        get => _length;
        set => SetProperty(ref _length, value);
    }

    [Category("Transform")]
    [FloatPrecision(0)]
    public double ShaftWidth
    {
        get => _shaftWidth;
        set => SetProperty(ref _shaftWidth, value);
    }

    [Category("Transform")]
    [FloatPrecision(0)]
    public double HeadWidth
    {
        get => _headWidth;
        set => SetProperty(ref _headWidth, value);
    }

    [Category("Transform")]
    [FloatPrecision(0)]
    public double HeadHeight
    {
        get => _headHeight;
        set => SetProperty(ref _headHeight, value);
    }

    protected override List<Point> GeneratePoints() => CalculateArrowPoints();

    private List<Point> CalculateArrowPoints()
    {
        var points = new List<Point>
        {
            new Point(0, -ShaftWidth / 2), // Start of the shaft
            new Point(Length - HeadWidth, -ShaftWidth / 2), // End of the shaft
            new Point(Length - HeadWidth, -HeadHeight / 2), // Top of the arrowhead
            new Point(Length, 0), // Tip of the arrowhead
            new Point(Length - HeadWidth, HeadHeight / 2), // Bottom of the arrowhead
            new Point(Length - HeadWidth, ShaftWidth / 2), // End of the shaft
            new Point(0, ShaftWidth / 2), // Start of the shaft
            new Point(0, -ShaftWidth / 2) // Close the shape
        };

        return points;
    }
}
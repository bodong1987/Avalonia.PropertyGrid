using System.Collections.Generic;
using System.ComponentModel;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using PropertyModels.ComponentModel;
// ReSharper disable CompareOfFloatsByEqualityOperator
// ReSharper disable PropertyCanBeMadeInitOnly.Global

namespace Avalonia.PropertyGrid.Samples.PainterDemo.Models;

public class ArrowShape : ShapeBase
{
    private double _length;

    [Category("Transform")]
    [FloatPrecision(0)]
    public double Length
    {
        get => _length;
        set
        {
            if (_length != value)
            {
                _length = value;
                NotifyPropertyChanged();
            }
        }
    }

    public override Shape CreateAvaloniaShape()
    {
        var arrow = new Polyline
        {
            Points = CalculateArrowPoints(0, 0, Length),
            Stroke = new SolidColorBrush(FillColor),
            StrokeThickness = 2
        };
        return arrow;
    }

    public override bool UpdateProperties(Shape shape)
    {
        if (!base.UpdateProperties(shape))
        {
            return false;
        }

        if (shape is Polyline arrow)
        {
            arrow.Points = CalculateArrowPoints(0, 0, Length);
            return true;
        }

        return false;
    }

    private static IList<Point> CalculateArrowPoints(double startX, double startY, double length)
    {
        var points = new List<Point>
        {
            new Point(startX, startY),
            new Point(startX + length, startY),
            new Point(startX + length - 10, startY - 5),
            new Point(startX + length, startY),
            new Point(startX + length - 10, startY + 5)
        };
        return points;
    }
}
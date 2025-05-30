using System.Collections.Generic;
using System.ComponentModel;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using PropertyModels.ComponentModel;

// ReSharper disable CompareOfFloatsByEqualityOperator
// ReSharper disable PropertyCanBeMadeInitOnly.Global

namespace Avalonia.PropertyGrid.Samples.PainterDemo.Models;

public class HollowArrowShape : ShapeBase
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
        set
        {
            if (_length != value)
            {
                _length = value;
                NotifyPropertyChanged();
            }
        }
    }

    [Category("Transform")]
    [FloatPrecision(0)]
    public double ShaftWidth
    {
        get => _shaftWidth;
        set
        {
            if (_shaftWidth != value)
            {
                _shaftWidth = value;
                NotifyPropertyChanged();
            }
        }
    }

    [Category("Transform")]
    [FloatPrecision(0)]
    public double HeadWidth
    {
        get => _headWidth;
        set
        {
            if (_headWidth != value)
            {
                _headWidth = value;
                NotifyPropertyChanged();
            }
        }
    }

    [Category("Transform")]
    [FloatPrecision(0)]
    public double HeadHeight
    {
        get => _headHeight;
        set
        {
            if (_headHeight != value)
            {
                _headHeight = value;
                NotifyPropertyChanged();
            }
        }
    }

    public override Shape CreateAvaloniaShape()
    {
        var arrow = new Polyline();
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
            arrow.Points = CalculateArrowPoints();
            return true;
        }

        return false;
    }

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
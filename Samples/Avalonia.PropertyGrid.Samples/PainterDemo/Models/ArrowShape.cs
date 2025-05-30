using System;
using System.Collections.Generic;
using System.ComponentModel;
using Avalonia.PropertyGrid.Samples.PainterDemo.ViewModel;
using PropertyModels.ComponentModel;
// ReSharper disable CompareOfFloatsByEqualityOperator
// ReSharper disable PropertyCanBeMadeInitOnly.Global

namespace Avalonia.PropertyGrid.Samples.PainterDemo.Models;

[ShapeDescription(ToolMode.CreateArrow)]
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
            new(0, -ShaftWidth / 2), // Start of the shaft
            new(Length - HeadWidth, -ShaftWidth / 2), // End of the shaft
            new(Length - HeadWidth, -HeadHeight / 2), // Top of the arrowhead
            new(Length, 0), // Tip of the arrowhead
            new(Length - HeadWidth, HeadHeight / 2), // Bottom of the arrowhead
            new(Length - HeadWidth, ShaftWidth / 2), // End of the shaft
            new(0, ShaftWidth / 2), // Start of the shaft
            new(0, -ShaftWidth / 2) // Close the shape
        };

        return points;
    }

    protected override void OnFinishCreate(Point endPoint)
    {
        try
        {
            BeginBatchUpdate();

            Length = Math.Sqrt((endPoint.X - CreatingStartX) * (endPoint.X - CreatingStartX) + (endPoint.Y - CreatingStartY) * (endPoint.Y - CreatingStartY));
            HeadWidth = 30;
            HeadHeight = 30;
            ShaftWidth = 10;
        }
        finally
        {
            EndBatchUpdate();
            RaisePropertyChanged(nameof(Length));
        }
    }
}
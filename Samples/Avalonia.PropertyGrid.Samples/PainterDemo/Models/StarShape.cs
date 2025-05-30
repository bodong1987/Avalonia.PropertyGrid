using System;
using System.Collections.Generic;
using System.ComponentModel;
using PropertyModels.ComponentModel;
// ReSharper disable CompareOfFloatsByEqualityOperator
// ReSharper disable PropertyCanBeMadeInitOnly.Global

namespace Avalonia.PropertyGrid.Samples.PainterDemo.Models;

public class StarShape : ShapeGenericPolygon
{
    private double _radius;

    [Category("Transform")]
    [FloatPrecision(0)]
    public double Radius
    {
        get => _radius;
        set => SetProperty(ref _radius, value);
    }

    protected override List<Point> GeneratePoints() => CalculateStarPoints(0, 0, Radius);

    private static List<Point> CalculateStarPoints(double centerX, double centerY, double outerRadius)
    {
        var points = new List<Point>();
        const int numPoints = 5;
        var innerRadius = outerRadius * 0.5; // Inner radius for the star

        for (var i = 0; i < numPoints * 2; i++)
        {
            var angle = Math.PI / numPoints * i;
            var radius = (i % 2 == 0) ? outerRadius : innerRadius;
            points.Add(new Point(centerX + radius * Math.Cos(angle), centerY + radius * Math.Sin(angle)));
        }

        return points;
    }
}
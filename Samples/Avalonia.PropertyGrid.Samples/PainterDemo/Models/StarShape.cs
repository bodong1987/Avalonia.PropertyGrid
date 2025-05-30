using System;
using System.Collections.Generic;
using System.ComponentModel;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using PropertyModels.ComponentModel;
// ReSharper disable CompareOfFloatsByEqualityOperator
// ReSharper disable PropertyCanBeMadeInitOnly.Global

namespace Avalonia.PropertyGrid.Samples.PainterDemo.Models;

public class StarShape : ShapeBase
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
        var star = new Polyline();
        return star;
    }

    public override bool UpdateProperties(Shape shape)
    {
        if (!base.UpdateProperties(shape))
        {
            return false;
        }

        if (shape is Polyline star)
        {
            star.Points = CalculateStarPoints(0, 0, Radius);
            return true;
        }

        return false;
    }

    private static List<Point> CalculateStarPoints(double centerX, double centerY, double radius)
    {
        var points = new List<Point>();
        const int numPoints = 5;
        for (var i = 0; i < numPoints; i++)
        {
            var angle = i * 4 * Math.PI / numPoints;
            points.Add(new Point(centerX + radius * Math.Cos(angle), centerY + radius * Math.Sin(angle)));
        }
        
        points.Add(points[0]);
        
        return points;
    }
}
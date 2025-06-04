using System;
using System.ComponentModel;
using System.Diagnostics;
using Avalonia.Controls.Shapes;
using Avalonia.PropertyGrid.Samples.PainterDemo.ViewModel;
using PropertyModels.ComponentModel;
using PropertyModels.ComponentModel.DataAnnotations;

// ReSharper disable CompareOfFloatsByEqualityOperator
// ReSharper disable PropertyCanBeMadeInitOnly.Global

namespace Avalonia.PropertyGrid.Samples.PainterDemo.Models;

[ShapeDescription(ToolMode.CreateEllipse)]
public class EllipseShape : ShapeGeneric<Ellipse>
{
    private double _width;
    [Category("Transform")]
    [FloatPrecision(0)]
    public double Width
    {
        get => _width;
        set => SetProperty(ref _width, value);
    }

    private double _height;
    [Category("Transform")]
    [FloatPrecision(0)]
    public double Height
    {
        get => _height;
        set => SetProperty(ref _height, value);
    }

    protected override void ApplyProperties(Ellipse shape)
    {
        shape.Width = Width;
        shape.Height = Height;
    }

    protected override void OnFinishCreate(Point endPoint)
    {
        try
        {
            BeginBatchUpdate();

            X = Math.Min(endPoint.X, CreatingStartX);
            Y = Math.Min(endPoint.Y, CreatingStartY);
            
            _width = Math.Abs(endPoint.X - CreatingStartX);
            _height = Math.Abs(endPoint.Y - CreatingStartY);
        }
        finally
        {
            EndBatchUpdate();
            RaisePropertyChanged(nameof(Width));
        }
    }
}

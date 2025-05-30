using System;
using System.ComponentModel;
using Avalonia.Controls.Shapes;
using Avalonia.PropertyGrid.Samples.PainterDemo.ViewModel;
using PropertyModels.ComponentModel;

// ReSharper disable CompareOfFloatsByEqualityOperator
// ReSharper disable PropertyCanBeMadeInitOnly.Global

namespace Avalonia.PropertyGrid.Samples.PainterDemo.Models;

[ShapeDescription(ToolMode.CreateRectangle)]
public class RectangleShape : ShapeGeneric<Rectangle>
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

    protected override void ApplyProperties(Rectangle shape)
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
            
            Width = Math.Abs(endPoint.X - CreatingStartX);
            Height = Math.Abs(endPoint.Y - CreatingStartY);
        }
        finally
        {
            EndBatchUpdate();
            RaisePropertyChanged(nameof(Width));
        }
    }
}

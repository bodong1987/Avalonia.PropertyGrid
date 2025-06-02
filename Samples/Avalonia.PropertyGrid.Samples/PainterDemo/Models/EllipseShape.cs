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

public enum EllipseDetailType
{
    Ellipse,
    Circle
}

[ShapeDescription(ToolMode.CreateEllipse)]
public class EllipseShape : ShapeGeneric<Ellipse>
{
    private EllipseDetailType _ellipseDetailType;

    [ConditionTarget]
    public EllipseDetailType EllipseDetailType
    {
        get => _ellipseDetailType;
        set => SetProperty(ref _ellipseDetailType, value);
    }
    
    private double _radius;
    [Category("Transform")]
    [FloatPrecision(0)]
    [VisibilityPropertyCondition(nameof(EllipseDetailType), EllipseDetailType.Circle)]
    public double Radius
    {
        get => _radius;
        set
        {
            if (SetProperty(ref _radius, value))
            {
                _width = _radius * 2;
                _height = _radius * 2;
                
                RaisePropertyChanged(nameof(Width));
                RaisePropertyChanged(nameof(Height));
            }
        }
    }
    
    private double _width;
    [Category("Transform")]
    [FloatPrecision(0)]
    [VisibilityPropertyCondition(nameof(EllipseDetailType), EllipseDetailType.Ellipse)]
    public double Width
    {
        get => _width;
        set
        {
            if (SetProperty(ref _width, value))
            {
                _radius = _width / 2;
                
                RaisePropertyChanged(nameof(Radius));
            }
        }
    }

    private double _height;
    [Category("Transform")]
    [FloatPrecision(0)]
    [VisibilityPropertyCondition(nameof(EllipseDetailType), EllipseDetailType.Ellipse)]
    public double Height
    {
        get => _height;
        set
        {
            if (SetProperty(ref _height, value))
            {
                _radius = _height / 2;
                
                RaisePropertyChanged(nameof(Radius));
            }
        }
    }

    protected override void ApplyProperties(Ellipse shape)
    {
        if (EllipseDetailType == EllipseDetailType.Ellipse)
        {
            shape.Width = Width;
            shape.Height = Height;
        }
        else
        {
            shape.Width = Radius * 2;
            shape.Height = Radius * 2;    
        }
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
            
            // Debug.WriteLine($"{_width} {_height}");
        }
        finally
        {
            EndBatchUpdate();
            RaisePropertyChanged(nameof(Width));
        }
    }
}

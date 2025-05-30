using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.PropertyGrid.Services;
using PropertyModels.ComponentModel;
using PropertyModels.ComponentModel.DataAnnotations;

// ReSharper disable CompareOfFloatsByEqualityOperator
// ReSharper disable PropertyCanBeMadeInitOnly.Global

namespace Avalonia.PropertyGrid.Samples.PainterDemo.Models;

public enum ShapeFillMode
{
    Fill,
    Blank
}

public abstract class ShapeBase : MiniReactiveObject
{
    public string Type => LocalizationService.Default[GetType().Name];

    #region Transform
    private double _x;
    
    [Category("Transform")]
    [FloatPrecision(0)]
    public double X
    {
        get => _x;
        set => SetProperty(ref _x, value);
    }
    private double _y;
    [Category("Transform")]
    [FloatPrecision(0)]
    public double Y
    {
        get => _y;
        set => SetProperty(ref _y, value);
    }
    
    private double _rotation;

    [Category("Transform")]
    [Range(0.0, 360.0)]
    [Trackable(0, 360)]
    public double Rotation
    {
        get => _rotation;
        set => SetProperty(ref _rotation, value);
    }
    #endregion

    #region Appearance
    [Browsable(false)]
    [ConditionTarget]
    protected bool IsFillModeVisible { get; set; } = true;
    
    private ShapeFillMode _fillMode = ShapeFillMode.Fill;

    [Category("Appearance")]
    [ConditionTarget]
    [SingleSelectionMode(SingleSelectionMode.ToggleButtonGroup)]
    [VisibilityPropertyCondition(nameof(IsFillModeVisible), true)]
    public ShapeFillMode FillMode
    {
        get => _fillMode;
        set => SetProperty(ref _fillMode, value);
    }

    private Color _fillColor = Colors.Gray;
    
    [Category("Appearance")]
    [VisibilityPropertyCondition(nameof(FillMode), ShapeFillMode.Fill)]
    public Color FillColor
    {
        get => _fillColor;
        set => SetProperty(ref _fillColor, value);
    }
    
    private Color _strokeColor = Colors.DarkGray;
    
    [Category("Appearance")]
    public Color StrokeColor
    {
        get => _strokeColor;
        set => SetProperty(ref _strokeColor, value);
    }
    
    private double _strokeThickness = 1.0;
    [Category("Appearance")]
    [FloatPrecision(0)]
    public double StrokeThickness
    {
        get => _strokeThickness;
        set => SetProperty(ref _strokeThickness, value);
    }

    private double _opacity = 1.0;
    [Category("Appearance")]
    [Range(0.0, 1.0)]
    [Trackable(0.0, 1.0)]
    public double Opacity
    {
        get => _opacity;
        set => SetProperty(ref _opacity, value);
    }
    
    private PenLineCap _strokeLineCap = PenLineCap.Flat;
    [Category("Appearance")]
    public PenLineCap StrokeLineCap
    {
        get => _strokeLineCap;
        set => SetProperty(ref _strokeLineCap, value);
    }

    private PenLineJoin _strokeJoin = PenLineJoin.Miter;
    [Category("Appearance")]
    public PenLineJoin StrokeJoin
    {
        get => _strokeJoin;
        set => SetProperty(ref _strokeJoin, value);
    }

    private double _strokeDashOffset;
    [Category("Appearance")]
    [FloatPrecision(0)]
    public double StrokeDashOffset
    {
        get => _strokeDashOffset;
        set => SetProperty(ref _strokeDashOffset, value);
    }

    private BindingList<double> _strokeDashArray = [];
    
    [Category("Appearance")]
    public BindingList<double> StrokeDashArray
    {
        get => _strokeDashArray;
        set => SetProperty(ref _strokeDashArray, value);
    }

    private Stretch _stretch = Stretch.None;
    [Category("Appearance")]
    public Stretch Stretch
    {
        get => _stretch;
        set => SetProperty(ref _stretch, value);
    }
    
    protected readonly SolidColorBrush FillBrush = new (Colors.Black);
    protected  readonly SolidColorBrush StrokeBrush = new (Colors.White);
    #endregion

    #region Methods
    public abstract Shape CreateAvaloniaShape();

    public virtual bool UpdateProperties(Shape shape)
    {
        FillBrush.Color = FillColor;
        StrokeBrush.Color = StrokeColor;
        
        shape.Fill = FillMode == ShapeFillMode.Fill ? FillBrush : null;
        shape.Stroke = StrokeBrush;
        shape.Opacity = Opacity;
        shape.StrokeThickness = StrokeThickness;
        shape.StrokeLineCap = StrokeLineCap;
        shape.StrokeJoin = StrokeJoin;
        shape.StrokeDashOffset = StrokeDashOffset;
        shape.StrokeDashArray?.Clear();
        shape.Stretch = Stretch;
        
        if (StrokeDashArray.Count > 0)
        {
            if (shape.StrokeDashArray == null)
            {
                shape.StrokeDashArray = new AvaloniaList<double>(StrokeDashArray);
            }
            else
            {
                shape.StrokeDashArray.AddRange(StrokeDashArray);    
            }
        }
        
        Canvas.SetLeft(shape, X);
        Canvas.SetTop(shape, Y);
        
        var transformGroup = new TransformGroup();
        transformGroup.Children.Add(new RotateTransform(Rotation, shape.Bounds.Width / 2, shape.Bounds.Height / 2));
        shape.RenderTransform = transformGroup;

        
        return true;
    }
    #endregion
}

public abstract class ShapeGeneric<T> : ShapeBase where T : Shape, new()
{
    public override Shape CreateAvaloniaShape() => new T();

    public override bool UpdateProperties(Shape shape)
    {
        if (!base.UpdateProperties(shape))
        {
            return false;
        }

        if (shape is T tShape)
        {
            ApplyProperties(tShape);
            return true;
        }

        return false;
    }

    protected abstract void ApplyProperties(T shape);
}

public abstract class ShapeGenericPolygon : ShapeGeneric<Polygon>
{
    protected override void ApplyProperties(Polygon shape)
    {
        shape.Points = GeneratePoints();
    }
    
    protected abstract List<Point> GeneratePoints();
}
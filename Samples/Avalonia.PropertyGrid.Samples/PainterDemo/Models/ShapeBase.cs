using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.PropertyGrid.Samples.PainterDemo.ViewModel;
using Avalonia.PropertyGrid.Services;
using PropertyModels.ComponentModel;
using PropertyModels.ComponentModel.DataAnnotations;
using PropertyModels.Extensions;

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
    
    private double _scaleX = 1.0;

    [Category("Transform")]
    [Range(0.1, 10.0)]
    [Trackable(0.1, 10.0)]
    public double ScaleX
    {
        get => _scaleX;
        set => SetProperty(ref _scaleX, value);
    }
    
    private double _scaleY = 1.0;

    [Category("Transform")]
    [Range(0.1, 10.0)]
    [Trackable(0.1, 10.0)]
    public double ScaleY
    {
        get => _scaleY;
        set => SetProperty(ref _scaleY, value);
    }
    #endregion
    
    #region Appearance
    [Browsable(false)]
    [ConditionTarget]
    public bool IsFillModeVisible { get; set; } = true;
    
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

    private BrushConfiguration _fillBrush = new() { SolidColor = Colors.Gray };
    
    [Category("Appearance")]
    [VisibilityPropertyCondition(nameof(FillMode), ShapeFillMode.Fill)]
    public BrushConfiguration FillBrush
    {
        get => _fillBrush;
        set => SetProperty(ref _fillBrush, value);
    }
    
    private BrushConfiguration _strokeBrush = new () { SolidColor = Colors.DarkGray };
    
    [Category("Appearance")]
    public BrushConfiguration StrokeBrush
    {
        get => _strokeBrush;
        set => SetProperty(ref _strokeBrush, value);
    }
    
    private double _strokeThickness = 2.0;
    [Category("Appearance")]
    [FloatPrecision(0)]
    [Trackable(1.0, 50.0)]
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
    [SingleSelectionMode(SingleSelectionMode.ToggleButtonGroup)]
    public PenLineCap StrokeLineCap
    {
        get => _strokeLineCap;
        set => SetProperty(ref _strokeLineCap, value);
    }

    private PenLineJoin _strokeJoin = PenLineJoin.Miter;
    [Category("Appearance")]
    [SingleSelectionMode(SingleSelectionMode.ToggleButtonGroup)]
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
    [SingleSelectionMode(SingleSelectionMode.ToggleButtonGroup)]
    public Stretch Stretch
    {
        get => _stretch;
        set => SetProperty(ref _stretch, value);
    }

    protected double CreatingStartX;
    protected double CreatingStartY;
    
    #endregion

    #region Constructor
    protected ShapeBase()
    {
        FillBrush.PropertyChanged += (s, e) => RaisePropertyChanged(nameof(FillBrush));
        StrokeBrush.PropertyChanged += (s, e) => RaisePropertyChanged(nameof(StrokeBrush));
    }

    #endregion

    #region Methods
    protected bool SetProperty(ref double field, double value, [CallerMemberName] string? propertyName = null)
    {
        if (Math.Abs(field - value) < 0.0001)
            return false;
        field = value;
        RaisePropertyChanged(propertyName!);
        return true;
    }
    
    public abstract Shape CreateAvaloniaShape();

    public virtual bool UpdateProperties(Shape shape)
    {
        shape.Fill = FillMode == ShapeFillMode.Fill ? FillBrush.Brush : null;
        shape.Stroke = StrokeBrush.Brush;
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
        transformGroup.Children.Add(new ScaleTransform(ScaleX, ScaleY));
        transformGroup.Children.Add(new RotateTransform(Rotation, shape.Bounds.Width / 2, shape.Bounds.Height / 2));
        shape.RenderTransform = transformGroup;

        
        return true;
    }

    public virtual void OnMousePressed(Point startPoint)
    {
        CreatingStartX = X = startPoint.X;
        CreatingStartY = Y = startPoint.Y;
    }

    public virtual void OnMouseMove(Point point)
    {
        OnFinishCreate(point);
    }

    public virtual void OnMouseReleased(Point point)
    {
        OnFinishCreate(point);
    }

    protected abstract void OnFinishCreate(Point endPoint);
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

[AttributeUsage(AttributeTargets.Class)]
public class ShapeDescriptionAttribute : Attribute
{
    public readonly ToolMode BindingMode;

    public ShapeDescriptionAttribute(ToolMode bindingMode)
    {
        BindingMode = bindingMode;
    }
}

public class ShapeFactory
{
    private static readonly Dictionary<ToolMode, Type> Creators = new ();
    
    static ShapeFactory()
    {
        foreach (var type in typeof(ShapeFactory).Assembly.GetTypes().ToList().FindAll(t =>
                     t is { IsClass: true, IsAbstract: false } && t.IsSubclassOf(typeof(ShapeBase)) &&
                     t.IsDefined<ShapeDescriptionAttribute>()))
        {
            var attr = type.GetAnyCustomAttribute<ShapeDescriptionAttribute>()!;
            Creators[attr.BindingMode] = type;
        }
    }

    public static ShapeBase? NewShape(ToolMode bindingMode)
    {
        if (Creators.TryGetValue(bindingMode, out var type))
        {
            return Activator.CreateInstance(type) as ShapeBase;
        }

        return null;
    }
}
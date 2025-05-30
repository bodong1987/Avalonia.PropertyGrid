using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
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
    private double _x;
    
    [Category("Transform")]
    [FloatPrecision(0)]
    public double X
    {
        get => _x;
        set
        {
            if (_x != value)
            {
                _x = value;
                NotifyPropertyChanged();
            }
        }
    }
    private double _y;
    [Category("Transform")]
    [FloatPrecision(0)]
    public double Y
    {
        get => _y;
        set
        {
            if (_y != value)
            {
                _y = value;
                NotifyPropertyChanged();
            }
        }
    }
    
    private ShapeFillMode _fillMode = ShapeFillMode.Fill;

    [Category("Apperance")]
    [ConditionTarget]
    [SingleSelectionMode(SingleSelectionMode.ToggleButtonGroup)]
    public ShapeFillMode FillMode
    {
        get => _fillMode;
        set
        {
            if (_fillMode != value)
            {
                _fillMode = value;
                NotifyPropertyChanged();
            }
        }
    }

    private Color _fillColor = Colors.Gray;
    
    [Category("Appearance")]
    [VisibilityPropertyCondition(nameof(FillMode), ShapeFillMode.Fill)]
    public Color FillColor
    {
        get => _fillColor;
        set
        {
            if (_fillColor != value)
            {
                _fillColor = value;
                NotifyPropertyChanged();
            }
        }
    }
    
    private Color _strokeColor = Colors.DarkGray;
    
    [Category("Appearance")]
    public Color StrokeColor
    {
        get => _strokeColor;
        set
        {
            if (_strokeColor != value)
            {
                _strokeColor = value;
                NotifyPropertyChanged();
            }
        }
    }
    
    private double _strokeThickness = 1.0;
    [Category("Appearance")]
    [FloatPrecision(0)]
    public double StrokeThickness
    {
        get => _strokeThickness;
        set
        {
            if (_strokeThickness != value)
            {
                _strokeThickness = value;
                NotifyPropertyChanged();
            }
        }
    }

    private double _opacity = 1.0;
    [Category("Appearance")]
    [Range(0.0, 1.0)]
    [Trackable(0.0, 1.0)]
    public double Opacity
    {
        get => _opacity;
        set
        {
            if (_opacity != value)
            {
                _opacity = value;
                NotifyPropertyChanged();
            }
        }
    }
    
    private PenLineCap _strokeLineCap = PenLineCap.Flat;
    [Category("Appearance")]
    public PenLineCap StrokeLineCap
    {
        get => _strokeLineCap;
        set
        {
            if (_strokeLineCap != value)
            {
                _strokeLineCap = value;
                NotifyPropertyChanged();
            }
        }
    }

    private PenLineJoin _strokeJoin = PenLineJoin.Miter;
    [Category("Appearance")]
    public PenLineJoin StrokeJoin
    {
        get => _strokeJoin;
        set
        {
            if (_strokeJoin != value)
            {
                _strokeJoin = value;
                NotifyPropertyChanged();
            }
        }
    }

    private double _strokeDashOffset = 0.0;
    [Category("Appearance")]
    [FloatPrecision(0)]
    public double StrokeDashOffset
    {
        get => _strokeDashOffset;
        set
        {
            if (_strokeDashOffset != value)
            {
                _strokeDashOffset = value;
                NotifyPropertyChanged();
            }
        }
    }

    private BindingList<double> _strokeDashArray = [];
    
    [Category("Appearance")]
    public BindingList<double> StrokeDashArray
    {
        get => _strokeDashArray;
        set
        {
            if (_strokeDashArray != value)
            {
                _strokeDashArray = value;
                NotifyPropertyChanged();
            }
        }
    }

    private Stretch _stretch = Stretch.None;
    [Category("Appearance")]
    public Stretch Stretch
    {
        get => _stretch;
        set
        {
            if (_stretch != value)
            {
                _stretch = value;
                NotifyPropertyChanged();
            }
        }
    }
    
    protected readonly SolidColorBrush FillBrush = new (Colors.Black);
    protected  readonly SolidColorBrush StrokeBrush = new (Colors.White);
    
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
        
        return true;
    }

    protected void NotifyPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        System.Diagnostics.Debug.Assert(propertyName != null);
        // ReSharper disable once RedundantSuppressNullableWarningExpression
        RaisePropertyChanged(propertyName!);
    }
}
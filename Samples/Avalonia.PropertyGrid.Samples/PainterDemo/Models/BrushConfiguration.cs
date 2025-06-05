using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Avalonia.Media;
using PropertyModels.ComponentModel;
using PropertyModels.ComponentModel.DataAnnotations;

namespace Avalonia.PropertyGrid.Samples.PainterDemo.Models;

public enum BrushCategory
{
    Solid,
    LinearGradient,
    RadialGradient
}

[TypeConverter(typeof(ExpandableObjectConverter))]
public class BrushConfiguration : ReactiveObject
{
    private IBrush _brush = new SolidColorBrush(Colors.Gray);

    [Browsable(false)]
    public IBrush Brush
    {
        get
        {
            switch (_brushCategory)
            {
                case BrushCategory.Solid:
                {
                    var solidBrush = _brush as SolidColorBrush;
                    Debug.Assert(solidBrush != null);
                    solidBrush.Color = SolidColor;
                    
                    return solidBrush;
                }
                case BrushCategory.LinearGradient:
                {
                    var linearBrush = _brush as LinearGradientBrush;
                    Debug.Assert(linearBrush != null);
                    linearBrush.StartPoint = new RelativePoint(_linearGradientStartPointX, _linearGradientStartPointY, RelativeUnit.Relative);
                    linearBrush.EndPoint = new RelativePoint(_linearGradientEndPointX, _linearGradientEndPointY, RelativeUnit.Relative);
                    linearBrush.GradientStops = [.. GradientStops];

                    return linearBrush;
                }
                case BrushCategory.RadialGradient:
                {
                    var radialBrush = _brush as RadialGradientBrush;
                    Debug.Assert(radialBrush != null);
                    radialBrush.RadiusX = new RelativeScalar(_radialGradientRadiusX, RelativeUnit.Relative);
                    radialBrush.RadiusY = new RelativeScalar(_radialGradientRadiusY, RelativeUnit.Relative);
                    radialBrush.Center = new RelativePoint(_radialGradientCenterX, _radialGradientCenterY, RelativeUnit.Relative);
                    radialBrush.GradientStops = [.. GradientStops];
                    
                    return radialBrush;
                }
                
                default:
                    throw new NotImplementedException();
            }
        }
    }
    
    private BrushCategory _brushCategory;

    [ConditionTarget]
    public BrushCategory BrushCategory
    {
        get => _brushCategory;
        set
        {
            if (_brushCategory != value)
            {
                _brush = value switch
                {
                    BrushCategory.Solid => new SolidColorBrush(SolidColor),
                    BrushCategory.LinearGradient => new LinearGradientBrush(),
                    BrushCategory.RadialGradient => new RadialGradientBrush(),
                    _ => _brush
                };

                SetProperty(ref _brushCategory, value);
            }
        }
    }

    #region SolidColorBrush
    private Color _solidColor = Colors.Gray;

    [PropertyVisibilityCondition(nameof(BrushCategory), BrushCategory.Solid)]
    public Color SolidColor
    {
        get => _solidColor;
        set
        {
            if (SetProperty(ref _solidColor, value) && _brush is SolidColorBrush solidBrush)
            {
                solidBrush.Color = value;
            }
        }
    }
    #endregion

    #region GradientBrushes
    [Browsable(false)]
    [ConditionTarget]
    [DependsOnProperty(nameof(BrushCategory))]
    public bool IsGradientBrush => BrushCategory is BrushCategory.LinearGradient or BrushCategory.RadialGradient;

    private BindingList<GradientStop> _gradientStops = [
        new (){Color = Colors.Firebrick, Offset = 0},
        new (){Color = Colors.LightBlue, Offset = 1.0}
    ];

    [PropertyVisibilityCondition(nameof(IsGradientBrush), true)]
    public BindingList<GradientStop> GradientStops
    {
        get => _gradientStops;
        set => SetProperty(ref _gradientStops, value);
    }
    #endregion

    #region LinearGradientBrush
    private double _linearGradientStartPointX;

    [PropertyVisibilityCondition(nameof(BrushCategory), BrushCategory.LinearGradient)]
    [Trackable(0, 1)]
    [Range(0, 1)]
    public double LinearGradientStartPointX
    {
        get => _linearGradientStartPointX;
        set => SetProperty(ref _linearGradientStartPointX, value);
    }
    
    private double _linearGradientStartPointY;

    [PropertyVisibilityCondition(nameof(BrushCategory), BrushCategory.LinearGradient)]
    [Trackable(0, 1)]
    [Range(0, 1)]
    public double LinearGradientStartPointY
    {
        get => _linearGradientStartPointY;
        set => SetProperty(ref _linearGradientStartPointY, value);
    }
    
    private double _linearGradientEndPointX = 100;

    [PropertyVisibilityCondition(nameof(BrushCategory), BrushCategory.LinearGradient)]
    [Trackable(0, 1)]
    [Range(0, 1)]
    public double LinearGradientEndPointX
    {
        get => _linearGradientEndPointX;
        set => SetProperty(ref _linearGradientEndPointX, value);
    }
    
    private double _linearGradientEndPointY = 100;

    [PropertyVisibilityCondition(nameof(BrushCategory), BrushCategory.LinearGradient)]
    [Trackable(0, 1)]
    [Range(0, 1)]
    public double LinearGradientEndPointY
    {
        get => _linearGradientEndPointY;
        set => SetProperty(ref _linearGradientEndPointY, value);
    }
    #endregion

    #region RadialGradientBrush

    private double _radialGradientCenterX = 0.5;

    [PropertyVisibilityCondition(nameof(BrushCategory), BrushCategory.RadialGradient)]
    [Trackable(0, 1)]
    [Range(0, 1)]
    public double RadialGradientCenterX
    {
        get => _radialGradientCenterX;
        set => SetProperty(ref _radialGradientCenterX, value);
    }
    
    private double _radialGradientCenterY = 0.5;

    [PropertyVisibilityCondition(nameof(BrushCategory), BrushCategory.RadialGradient)]
    [Trackable(0, 1)]
    [Range(0, 1)]
    public double RadialGradientCenterY
    {
        get => _radialGradientCenterY;
        set => SetProperty(ref _radialGradientCenterY, value);
    }
    
    private double _radialGradientRadiusX = 0.5;

    [PropertyVisibilityCondition(nameof(BrushCategory), BrushCategory.RadialGradient)]
    [Trackable(0, 1)]
    [Range(0, 1)]
    public double RadialGradientRadiusX
    {
        get => _radialGradientRadiusX;
        set => SetProperty(ref _radialGradientRadiusX, value);
    }
    
    private double _radialGradientRadiusY = 0.5;

    [PropertyVisibilityCondition(nameof(BrushCategory), BrushCategory.RadialGradient)]
    [Trackable(0, 1)]
    [Range(0, 1)]
    public double RadialGradientRadiusY
    {
        get => _radialGradientRadiusY;
        set => SetProperty(ref _radialGradientRadiusY, value);
    }

    #endregion

}
using System.ComponentModel;
using Avalonia;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.PropertyGrid.Samples.PainterDemo.Models;
using PropertyModels.ComponentModel;
// ReSharper disable CompareOfFloatsByEqualityOperator
// ReSharper disable PropertyCanBeMadeInitOnly.Global

namespace Avalonia.PropertyGrid.Samples.PainterDemo.Models;

public class LineShape : ShapeBase
{
    private double _x2;
    private double _y2;

    [Category("Transform")]
    [FloatPrecision(0)]
    public double X2
    {
        get => _x2;
        set
        {
            if (_x2 != value)
            {
                _x2 = value;
                NotifyPropertyChanged();
            }
        }
    }

    [Category("Transform")]
    [FloatPrecision(0)]
    public double Y2
    {
        get => _y2;
        set
        {
            if (_y2 != value)
            {
                _y2 = value;
                NotifyPropertyChanged();
            }
        }
    }

    public override Shape CreateAvaloniaShape()
    {
        return new Line();
    }

    public override bool UpdateProperties(Shape shape)
    {
        if (!base.UpdateProperties(shape))
        {
            return false;
        }

        if (shape is Line line)
        {
            line.StartPoint = new Point(0, 0);
            line.EndPoint = new Point(X2, Y2);
            return true;
        }

        return false;
    }    
}
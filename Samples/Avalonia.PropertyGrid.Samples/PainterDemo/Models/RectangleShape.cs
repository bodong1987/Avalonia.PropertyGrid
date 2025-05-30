using System.ComponentModel;
using Avalonia.Controls.Shapes;
using PropertyModels.ComponentModel;

// ReSharper disable CompareOfFloatsByEqualityOperator
// ReSharper disable PropertyCanBeMadeInitOnly.Global

namespace Avalonia.PropertyGrid.Samples.PainterDemo.Models;

public class RectangleShape : ShapeGeneric<Rectangle>
{
    private double _width;
    [Category("Transform")]
    [FloatPrecision(0)]
    public double Width
    {
        get => _width;
        set
        {
            if (_width != value)
            {
                _width = value;
                NotifyPropertyChanged();
            }
        }
    }

    private double _height;
    [Category("Transform")]
    [FloatPrecision(0)]
    public double Height
    {
        get => _height;
        set
        {
            if (_height != value)
            {
                _height = value;
                NotifyPropertyChanged();
            }
        }
    }

    protected override void ApplyProperties(Rectangle shape)
    {
        shape.Width = Width;
        shape.Height = Height;
    }
}

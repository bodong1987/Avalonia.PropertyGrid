using System.ComponentModel;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
// ReSharper disable CompareOfFloatsByEqualityOperator
// ReSharper disable PropertyCanBeMadeInitOnly.Global

namespace Avalonia.PropertyGrid.Samples.PainterDemo.Models;

public class RectangleShape : ShapeBase
{
    private double _width;
    [Category("Transform")]
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
    
    public override Shape CreateAvaloniaShape()
    {
        return new Rectangle()
        {
            Width = Width,
            Height = Height,
            Fill = new SolidColorBrush(FillColor),
            Opacity = Opacity
        };
    }

    public override bool UpdateProperties(Shape shape)
    {
        if (!base.UpdateProperties(shape))
        {
            return false;
        }
        
        if (shape is Rectangle rectangle)
        {
            rectangle.Width = Width;
            rectangle.Height = Height;
            return true;
        }
        
        return false;
    }
}

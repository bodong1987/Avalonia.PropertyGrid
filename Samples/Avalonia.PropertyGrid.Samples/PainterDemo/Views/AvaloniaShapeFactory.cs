using Avalonia.Controls.Shapes;
using Avalonia.PropertyGrid.Samples.PainterDemo.Models;

namespace Avalonia.PropertyGrid.Samples.PainterDemo.Views;

public static class AvaloniaShapeFactory
{
    public static Shape CreateShape(ShapeBase baseShape)
    {
        return baseShape.CreateAvaloniaShape();
    }
}
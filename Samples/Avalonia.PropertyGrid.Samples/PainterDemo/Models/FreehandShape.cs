using System.ComponentModel;
using Avalonia.Collections;
using Avalonia.Controls.Shapes;

namespace Avalonia.PropertyGrid.Samples.PainterDemo.Models;

public class FreehandShape : ShapeGeneric<Polyline>
{
    private BindingList<Point> _points = [];

    [Category("Geometry")]
    [Browsable(false)]
    public BindingList<Point> Points
    {
        get => _points;
        set => SetProperty(ref _points, value);
    }

    public FreehandShape()
    {
        FillMode = ShapeFillMode.Blank;
    }

    protected override void ApplyProperties(Polyline shape)
    {
        shape.Points = new AvaloniaList<Point>(_points);
    }
}
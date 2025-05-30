using System.ComponentModel;
using Avalonia.Collections;
using Avalonia.Controls.Shapes;
using Avalonia.PropertyGrid.Samples.PainterDemo.ViewModel;

namespace Avalonia.PropertyGrid.Samples.PainterDemo.Models;

[ShapeDescription(ToolMode.Brush)]
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
    
    protected override void OnFinishCreate(Point endPoint)
    {
        Points.Add(endPoint);
        RaisePropertyChanged(nameof(Points));
    }
}
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.PropertyGrid.Samples.PainterDemo.Models;
using Avalonia.PropertyGrid.Samples.PainterDemo.ViewModel;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Avalonia.PropertyGrid.Samples.PainterDemo.Views;

using AvaloniaShape = Avalonia.Controls.Shapes.Shape;

public partial class PainterView : UserControl
{
    private readonly Dictionary<ShapeBase, AvaloniaShape> _shapesMapping = new ();
    private readonly Dictionary<AvaloniaShape, ShapeBase> _avaloniaShapesMapping = new ();
    
    private AvaloniaShape? _draggedShape;
    private Point _dragStartPoint;
    private Point _shapeStartPoint;
    
    public static readonly RoutedEvent<ShapeSelectedEventArgs> ShapeSelectedEvent =
        RoutedEvent.Register<PainterView, ShapeSelectedEventArgs>(
            nameof(ShapeSelected), RoutingStrategies.Bubble);

    private Point _contextMenuPosition;
    private FreehandShape? _currentFreehandShape;
    
    public event EventHandler<ShapeSelectedEventArgs> ShapeSelected
    {
        add => AddHandler(ShapeSelectedEvent, value);
        remove => RemoveHandler(ShapeSelectedEvent, value);
    }
    
    public PainterView()
    {
        InitializeComponent();

        var viewModel = new PainterViewModel();
        DataContext = viewModel;
        viewModel.Shapes.CollectionChanged += OnShapeCollectionChanged;
        
        // for demo
        viewModel.Shapes.Add(new EllipseShape { X = 50, Y = 50, Radius = 90, FillColor = Colors.Blue });
        viewModel.Shapes.Add(new RectangleShape { X = 350, Y = 200, Width = 260, Height = 240, FillColor = Colors.Green });
        viewModel.Shapes.Add(new StarShape{ X= 150, Y = 550, Radius = 100, FillColor = Colors.Red });
    }

    private void OnShapeCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        RefreshShapes();
    }

    private void RefreshShapes()
    {
        var viewModel = (DataContext as PainterViewModel)!;
        var shapes = viewModel.Shapes;
        foreach (var pair in _shapesMapping.ToArray())
        {
            if (!shapes.Contains(pair.Key))
            {
                // this is removed
                MainCanvas.Children.Remove(pair.Value);
                _shapesMapping.Remove(pair.Key);
                _avaloniaShapesMapping.Remove(pair.Value);

                if (viewModel.SelectedShape == pair.Key)
                {
                    viewModel.SelectedShape = null;
                }
            }
        }

        foreach (var shape in shapes)
        {
            if (!_shapesMapping.ContainsKey(shape))
            {
                var avaloniaShape = shape.CreateAvaloniaShape();
                
                avaloniaShape.PointerEntered += OnAvaloniaShapePointerEntered;
                avaloniaShape.PointerExited += OnAvaloniaShapePointerExited;
                avaloniaShape.PointerPressed += OnAvaloniaShapePointerPressed;

                shape.PropertyChanged += OnShapePropertyChanged;

                shape.UpdateProperties(avaloniaShape);

                _shapesMapping.Add(shape, avaloniaShape);
                _avaloniaShapesMapping.Add(avaloniaShape, shape);
                MainCanvas.Children.Add(avaloniaShape);
            }
        }
    }

    private void OnShapePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is ShapeBase shape && _shapesMapping.TryGetValue(shape, out var avaloniaShape))
        {
            shape.UpdateProperties(avaloniaShape);
        }
    }

    private void OnAvaloniaShapePointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is not AvaloniaShape shape)
        {
            return;
        }

        if (_avaloniaShapesMapping.TryGetValue(shape, out var baseShape))
        {
            // Raise the ShapeSelected routed event
            var args = new ShapeSelectedEventArgs(shape, baseShape)
            {
                RoutedEvent = ShapeSelectedEvent,
                Source = this
            };
            RaiseEvent(args);

            if (args.Handled)
            {
                return;
            }

            (DataContext as PainterViewModel)!.SelectedShape = baseShape;
        }
    }

    private static void OnAvaloniaShapePointerExited(object? sender, PointerEventArgs e)
    {
        if (sender is AvaloniaShape { Tag: { } tag } shape)
        {
            // Restore original stroke settings
            shape.Stroke = (tag as dynamic).OriginalStroke as IBrush;
            shape.StrokeThickness = (double)(tag as dynamic).OriginalStrokeThickness;
        }
    }

    private static void OnAvaloniaShapePointerEntered(object? sender, PointerEventArgs e)
    {
        if (sender is AvaloniaShape shape)
        {
            // Store original stroke settings if needed
            shape.Tag = new { OriginalStroke = shape.Stroke, OriginalStrokeThickness = shape.StrokeThickness };

            // Set highlight stroke
            shape.Stroke = new SolidColorBrush(Colors.OrangeRed); // Highlight color
            shape.StrokeThickness = 4; // Highlight thickness
        }
    }
    
    private void MainCanvas_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        var point = e.GetPosition(MainCanvas);
        var viewModel = (DataContext as PainterViewModel)!;
        
        if (e.GetCurrentPoint(MainCanvas).Properties.IsRightButtonPressed)
        {
            _contextMenuPosition = point; // Update the context menu position

            foreach (var shape in _avaloniaShapesMapping)
            {
                if (shape.Key.IsPointerOver)
                {
                    viewModel.SelectedShape = shape.Value;
                    break;
                }
            }
            return;
        }

        switch (viewModel.CurrentToolMode)
        {
            case ToolMode.Select:
                foreach (var shape in _avaloniaShapesMapping.Keys)
                {
                    if (shape.IsPointerOver)
                    {
                        _draggedShape = shape;
                        _dragStartPoint = point;
                        _shapeStartPoint = new Point(_avaloniaShapesMapping[shape].X, _avaloniaShapesMapping[shape].Y);
                        e.Pointer.Capture(MainCanvas);
                        
                        MainCanvas.Cursor = new Cursor(StandardCursorType.DragMove);
                        
                        break;
                    }
                }
                break;
            case ToolMode.Brush:
                _currentFreehandShape = new FreehandShape(){StrokeColor = Colors.Red, FillColor = Colors.Red, StrokeThickness = 2};
                _currentFreehandShape.Points.Add(point);
                viewModel.Shapes.Add(_currentFreehandShape);
                viewModel.SelectedShape = _currentFreehandShape;
                e.Pointer.Capture(MainCanvas);
                break;
        }
    }
    
    private void MainCanvas_PointerMoved(object? sender, PointerEventArgs e)
    {
        if (_draggedShape != null && e.Pointer.Captured != null)
        {
            var currentPoint = e.GetPosition(MainCanvas);
            var delta = currentPoint - _dragStartPoint;

            if (_avaloniaShapesMapping.TryGetValue(_draggedShape, out var baseShape))
            {
                baseShape.X = _shapeStartPoint.X + delta.X;
                baseShape.Y = _shapeStartPoint.Y + delta.Y;
            }
        }
        
        if (_currentFreehandShape != null && e.Pointer.Captured != null)
        {
            var currentPoint = e.GetPosition(MainCanvas);
            _currentFreehandShape.Points.Add(currentPoint);
            _currentFreehandShape.RaisePropertyChanged(nameof(_currentFreehandShape.Points));
        }
    }
    
    private void MainCanvas_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (_draggedShape != null && e.Pointer.Captured != null)
        {
            e.Pointer.Capture(null);
            _draggedShape = null;
            
            // Restore default cursor
            MainCanvas.Cursor = Cursor.Default;
        }
        
        if (_currentFreehandShape != null && e.Pointer.Captured != null)
        {
            e.Pointer.Capture(null);
            _currentFreehandShape = null;
        }
    }
    
    private void AddRectangle_Click(object? sender, RoutedEventArgs e)
    {
        var point = _contextMenuPosition;
        var rectangle = new RectangleShape
        {
            X = point.X,
            Y = point.Y,
            Width = 300,
            Height = 250,
            FillColor = Colors.Gray
        };
        (DataContext as PainterViewModel)!.Shapes.Add(rectangle);
        (DataContext as PainterViewModel)!.SelectedShape = rectangle;
    }

    private void AddEllipse_Click(object? sender, RoutedEventArgs e)
    {
        var point = _contextMenuPosition;
        var circle = new EllipseShape
        {
            X = point.X,
            Y = point.Y,
            Radius = 150,
            FillColor = Colors.Gray
        };
        (DataContext as PainterViewModel)!.Shapes.Add(circle);
        (DataContext as PainterViewModel)!.SelectedShape = circle;
    }

    private void AddLine_Click(object? sender, RoutedEventArgs e)
    {
        var point = _contextMenuPosition;
        var line = new LineShape
        {
            X = point.X,
            Y = point.Y,
            X2 = 100,
            Y2 = 100,
            FillColor = Colors.Gray
        };
        (DataContext as PainterViewModel)!.Shapes.Add(line);
        (DataContext as PainterViewModel)!.SelectedShape = line;
    }

    private void AddStar_Click(object? sender, RoutedEventArgs e)
    {
        var point = _contextMenuPosition;
        var star = new StarShape
        {
            X = point.X,
            Y = point.Y,
            Radius = 100,
            FillColor = Colors.Gray
        };
        (DataContext as PainterViewModel)!.Shapes.Add(star);
        (DataContext as PainterViewModel)!.SelectedShape = star;
    }

    private void AddArrow_Click(object? sender, RoutedEventArgs e)
    {
        var point = _contextMenuPosition;
        var arrow = new ArrowShape
        {
            X = point.X,
            Y = point.Y,
            Length = 100,
            HeadHeight = 30,
            HeadWidth = 30,
            ShaftWidth = 15,
            FillColor = Colors.Gray
        };
        (DataContext as PainterViewModel)!.Shapes.Add(arrow);
        (DataContext as PainterViewModel)!.SelectedShape = arrow;
    }

    private void DeleteShape_Click(object? sender, RoutedEventArgs e)
    {
        if ((DataContext as PainterViewModel)!.SelectedShape != null)
        {
            (DataContext as PainterViewModel)!.Shapes.Remove((DataContext as PainterViewModel)!.SelectedShape!);
        }
    }
}

#region Event Args
public class ShapeSelectedEventArgs : RoutedEventArgs
{
    public AvaloniaShape AvaloniaShape { get; }
    public ShapeBase ShapeBase { get; }

    public ShapeSelectedEventArgs(AvaloniaShape avaloniaShape, ShapeBase shapeBase) :
        base(PainterView.ShapeSelectedEvent)
    {
        AvaloniaShape = avaloniaShape;
        ShapeBase = shapeBase;
    }
}
#endregion
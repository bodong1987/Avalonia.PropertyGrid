using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.PropertyGrid.Controls;
using Avalonia.PropertyGrid.Samples.PainterDemo.Models;
using Avalonia.PropertyGrid.Samples.PainterDemo.ViewModel;
using PropertyModels.ComponentModel;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Avalonia.PropertyGrid.Samples.PainterDemo.Views;

using AvaloniaShape = Avalonia.Controls.Shapes.Shape;

public partial class PainterView : UserControl
{
    private readonly Dictionary<ShapeBase, AvaloniaShape> _shapesMapping = new ();
    private readonly Dictionary<AvaloniaShape, ShapeBase> _avaloniaShapesMapping = new ();
    
    private AvaloniaShape? _draggedShape;
    private ShapeBase? _draggedShapeBase;
    private Point _dragStartPoint;
    private Point _shapeStartPoint;
    private Point _shapeEndPoint;
    private bool _hasDragged;
    
    public static readonly RoutedEvent<ShapeSelectedEventArgs> ShapeSelectedEvent =
        RoutedEvent.Register<PainterView, ShapeSelectedEventArgs>(
            nameof(ShapeSelected), RoutingStrategies.Bubble);

    private ShapeBase? _creatingShape;
    private AvaloniaShape? _creatingAvaloniaShape;
    
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
        viewModel.Shapes.Add(new EllipseShape { X = 50, Y = 50, Width = 90, Height = 90, FillColor = Colors.Blue });
        viewModel.Shapes.Add(new RectangleShape { X = 350, Y = 200, Width = 260, Height = 240, FillColor = Colors.Green });
        viewModel.Shapes.Add(new StarShape{ X= 150, Y = 550, Radius = 100, FillColor = Colors.Red });
        viewModel.Shapes.Add(new ArrowShape(){ X= 150, Y = 680, Length = 300, ShaftWidth = 10, HeadHeight = 30, HeadWidth = 30, FillColor = Colors.YellowGreen});
        viewModel.Shapes.Add(new LineShape(){ X= 450, Y = 500, X2 = 250, Y2 = 100, StrokeColor = Colors.DarkRed, StrokeThickness = 4});
    }

    private void OnShapeCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        RefreshShapes();
    }

    public void RemoveShape(ShapeBase shape)
    {
        if (_shapesMapping.TryGetValue(shape, out var avaloniaShape))
        {
            MainCanvas.Children.Remove(avaloniaShape);
            _shapesMapping.Remove(shape);
            _avaloniaShapesMapping.Remove(avaloniaShape);
            
            var viewModel = (DataContext as PainterViewModel)!;
            if (shape == viewModel.SelectedShape)
            {
                viewModel.SelectedShape = null;
            }
        }
    }

    public void AddShape(ShapeBase shape)
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

    private void RefreshShapes()
    {
        var viewModel = (DataContext as PainterViewModel)!;
        var shapes = viewModel.Shapes;
        foreach (var pair in _shapesMapping.ToArray())
        {
            if (!shapes.Contains(pair.Key))
            {
                // this is removed
                RemoveShape(pair.Key);
            }
        }

        foreach (var shape in shapes)
        {
            if (!_shapesMapping.ContainsKey(shape))
            {
                AddShape(shape);
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
            shape.StrokeThickness = shape.StrokeThickness + 4; // Highlight thickness
        }
    }
    
    private void MainCanvas_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        var point = e.GetPosition(MainCanvas);
        var viewModel = (DataContext as PainterViewModel)!;
        
        // hook right button pressed, it always used to show context menu
        if (e.GetCurrentPoint(MainCanvas).Properties.IsRightButtonPressed)
        {
            foreach (var shape in _avaloniaShapesMapping)
            {
                if (shape.Key.IsPointerOver)
                {
                    viewModel.SelectedShape = shape.Value;
                    return;
                }
            }
            
            return;
        }

        switch (viewModel.CurrentToolMode)
        {
            case ToolMode.Select:
                foreach (var (shape, shapeBase) in _avaloniaShapesMapping)
                {
                    if (shape.IsPointerOver)
                    {
                        _draggedShape = shape;
                        _draggedShapeBase = shapeBase; 
                        _dragStartPoint = point;
                        _shapeStartPoint = new Point(shapeBase.X, shapeBase.Y);
                        e.Pointer.Capture(MainCanvas);
                        
                        MainCanvas.Cursor = new Cursor(StandardCursorType.DragMove);
                        
                        break;
                    }
                }
                break;
            default:
            {
                var newShape = ShapeFactory.NewShape(viewModel.CurrentToolMode);
                if (newShape == null)
                {
                    Debug.WriteLine($"Failed to create a new shape for mode: {viewModel.CurrentToolMode}");
                    return;
                }

                newShape.FillColor = viewModel.DefaultFillColor;
                newShape.StrokeColor = viewModel.DefaultBorderColor;
                newShape.StrokeThickness = viewModel.DefaultStrokeThickness;

                _creatingShape = newShape;
                
                var currentPoint = e.GetPosition(MainCanvas);
                _creatingShape.OnMousePressed(currentPoint);
            }
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
                _hasDragged = true;
                
                baseShape.X = _shapeStartPoint.X + delta.X;
                baseShape.Y = _shapeStartPoint.Y + delta.Y;
                _shapeEndPoint = new Point(baseShape.X, baseShape.Y);
            }
        }
        
        if (_creatingShape != null && e.Pointer.Captured != null)
        {
            var currentPoint = e.GetPosition(MainCanvas);

            _creatingShape.OnMouseMove(currentPoint);

            if (_creatingAvaloniaShape == null)
            {
                _creatingAvaloniaShape = _creatingShape.CreateAvaloniaShape();
                _creatingShape.UpdateProperties(_creatingAvaloniaShape);
                MainCanvas.Children.Add(_creatingAvaloniaShape);

                _creatingShape.PropertyChanged += OnCreatingShapePropertyChanged;
            }
        }
    }

    private void OnCreatingShapePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_creatingShape != null && _creatingAvaloniaShape != null)
        {
            _creatingShape.UpdateProperties(_creatingAvaloniaShape);
        }
    }

    private void MainCanvas_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (_draggedShape != null && e.Pointer.Captured != null)
        {
            if (_hasDragged)
            {
                _hasDragged = false;
                
                var startPos = _shapeStartPoint;
                var endPos = _shapeEndPoint;
                var shapeBase = _draggedShapeBase!;
            
                var command = new GenericCancelableCommand(
                    $"Drag {shapeBase.Type}",
                    () =>
                    {
                        shapeBase.X = endPos.X;
                        shapeBase.Y = endPos.Y;
                        return true;
                    },
                    () =>
                    {
                        shapeBase.X = startPos.X;
                        shapeBase.Y = startPos.Y;
                        return true;
                    });
                (DataContext as PainterViewModel)?.ExecuteCommand(command);    
            }
            
            e.Pointer.Capture(null);
            _draggedShape = null;
            _draggedShapeBase = null;
            
            // Restore default cursor
            MainCanvas.Cursor = Cursor.Default;
        }
        
        if (_creatingShape != null && e.Pointer.Captured != null)
        {
            var currentPoint = e.GetPosition(MainCanvas);
            _creatingShape.OnMouseReleased(currentPoint);
            
            if (_creatingAvaloniaShape != null)
            {
                MainCanvas.Children.Remove(_creatingAvaloniaShape);
                _creatingAvaloniaShape = null;
            }
            
            _creatingShape.PropertyChanged -= OnCreatingShapePropertyChanged;

            var capturedShape = _creatingShape;
            
            var command = new GenericCancelableCommand(
                $"Create {capturedShape.Type}",
                () =>
                {
                    (DataContext as PainterViewModel)!.Shapes.Add(capturedShape);
                    (DataContext as PainterViewModel)!.SelectedShape = capturedShape;
                    return true;
                },
                () =>
                {
                    (DataContext as PainterViewModel)!.Shapes.Remove(capturedShape);
                    if ((DataContext as PainterViewModel)!.SelectedShape == capturedShape)
                    {
                        (DataContext as PainterViewModel)!.SelectedShape = null;
                    }

                    return true;
                });
            (DataContext as PainterViewModel)?.ExecuteCommand(command);
            
            e.Pointer.Capture(null);
            _creatingShape = null;

            if ((DataContext as PainterViewModel)!.CurrentToolMode != ToolMode.Select &&
                (DataContext as PainterViewModel)!.CurrentToolMode != ToolMode.Brush)
            {
                (DataContext as PainterViewModel)!.CurrentToolMode = ToolMode.Select;
            }
        }
    }
  
    private void DeleteShape_Click(object? sender, RoutedEventArgs e)
    {
        var selectedShape = (DataContext as PainterViewModel)?.SelectedShape;
        if (selectedShape == null)
        {
            return;
        }
        
        var command = new GenericCancelableCommand(
            $"Remove {selectedShape.Type}",
            () =>
            {
                (DataContext as PainterViewModel)!.Shapes.Remove(selectedShape);
                if ((DataContext as PainterViewModel)!.SelectedShape == selectedShape)
                {
                    (DataContext as PainterViewModel)!.SelectedShape = null;
                }

                return true;
            },
            () =>
            {
                (DataContext as PainterViewModel)!.Shapes.Add(selectedShape);
                (DataContext as PainterViewModel)!.SelectedShape = selectedShape;
                return true;
            });
        (DataContext as PainterViewModel)?.ExecuteCommand(command);
    }

    private void OnPropertyGridCommandExecuted(object? sender, RoutedCommandExecutedEventArgs e)
    {
        (DataContext as PainterViewModel)?.OnCommandExecuted(sender, e);
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

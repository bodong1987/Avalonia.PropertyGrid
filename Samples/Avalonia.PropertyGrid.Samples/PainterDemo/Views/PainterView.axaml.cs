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
    
    public static readonly RoutedEvent<ShapeSelectedEventArgs> ShapeSelectedEvent =
        RoutedEvent.Register<PainterView, ShapeSelectedEventArgs>(
            nameof(ShapeSelected), RoutingStrategies.Bubble);

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
        viewModel.Shapes.Add(new CircleShape { X = 50, Y = 50, Radius = 90, FillColor = Colors.Blue });
        viewModel.Shapes.Add(new RectangleShape { X = 350, Y = 200, Width = 260, Height = 240, FillColor = Colors.Green });
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
            }
        }

        foreach (var shape in shapes)
        {
            if (!_shapesMapping.ContainsKey(shape))
            {
                var avaloniaShape = AvaloniaShapeFactory.CreateShape(shape);
                
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

            ShapePropertyGrid.DataContext = baseShape;
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
            shape.Stroke = new SolidColorBrush(Colors.Red); // Highlight color
            shape.StrokeThickness = 2; // Highlight thickness
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
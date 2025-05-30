using System;
using System.Collections.ObjectModel;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.PropertyGrid.Samples.PainterDemo.Models;
using PropertyModels.ComponentModel;

namespace Avalonia.PropertyGrid.Samples.PainterDemo.ViewModel;

public enum ToolMode
{
    Select,
    Brush 
}

public class EnumToBooleanConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null || parameter == null)
            return false;

        return value.Equals(parameter);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is true ? parameter : AvaloniaProperty.UnsetValue;
    }
}

public class PainterViewModel : ReactiveObject
{
    public ObservableCollection<ShapeBase> Shapes { get; } = [];

    private ShapeBase? _selectedShape;
    public ShapeBase? SelectedShape
    {
        get => _selectedShape;
        set
        {
            _selectedShape = value;
            RaisePropertyChanged(nameof(SelectedShape));
        }
    }
    
    private ToolMode _currentToolMode = ToolMode.Select;
    public ToolMode CurrentToolMode
    {
        get => _currentToolMode;
        set
        {
            _currentToolMode = value;
            RaisePropertyChanged(nameof(CurrentToolMode));
        }
    }
}
using System.Collections.ObjectModel;
using Avalonia.PropertyGrid.Samples.PainterDemo.Models;
using PropertyModels.ComponentModel;

namespace Avalonia.PropertyGrid.Samples.PainterDemo.ViewModel;

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
}
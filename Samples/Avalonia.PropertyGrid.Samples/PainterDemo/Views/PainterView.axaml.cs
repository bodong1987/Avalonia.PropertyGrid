using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Media;

namespace Avalonia.PropertyGrid.Samples.PainterDemo.Views;

public partial class PainterView : UserControl
{
    public PainterView()
    {
        InitializeComponent();
    }
    
    private void OnShapePointerPressed(object sender, PointerPressedEventArgs e)
    {
        if (sender is Shape shape)
        {
        }
    }
}
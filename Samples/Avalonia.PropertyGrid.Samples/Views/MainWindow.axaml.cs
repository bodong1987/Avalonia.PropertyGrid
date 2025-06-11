using System.Diagnostics;
using Avalonia.Controls;

namespace Avalonia.PropertyGrid.Samples.Views;

public partial class MainWindow : Window
{
    public MainWindow() => InitializeComponent();
    
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        if(change.Property == BackgroundProperty)
        {
            Debug.Assert(true);
        }
        base.OnPropertyChanged(change);
    }
}
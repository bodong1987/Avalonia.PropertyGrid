using Avalonia.Controls;
using Avalonia.PropertyGrid.Samples.SettingsDemo.ViewModels;

namespace Avalonia.PropertyGrid.Samples.SettingsDemo.Views;

public partial class SettingsView : UserControl
{
    public SettingsView()
    {
        InitializeComponent();

        DataContext = new SettingsViewModel();
    }
}
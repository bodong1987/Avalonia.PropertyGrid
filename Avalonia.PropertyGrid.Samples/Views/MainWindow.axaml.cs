using Avalonia.Controls;
using Avalonia.PropertyGrid.Samples.ViewModels;

namespace Avalonia.PropertyGrid.Samples.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            DataContext = new MainWindowViewModel();

            InitializeComponent();

            propertyGrid_ShowControlProperties.SelectedObject = propertyGrid_ShowControlProperties;
        }
    }
}
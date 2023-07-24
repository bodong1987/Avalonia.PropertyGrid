using Avalonia.Controls;
using Avalonia.PropertyGrid.Samples.ViewModels;
using Avalonia.Styling;
using Avalonia.PropertyGrid.Samples;

namespace Avalonia.PropertyGrid.Samples.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            DataContext = new MainWindowViewModel();

            InitializeComponent();

            propertyGrid_ShowControlProperties.SelectedObject = propertyGrid_ShowControlProperties;

            ThemeBox.SelectedItem = App.CurrentTheme;
            ThemeBox.SelectionChanged += (sender, e) =>
            {
                if (ThemeBox.SelectedItem is ThemeType theme)
                {
                    App.SetTheme(theme);
                }
            };

            //ThemeVariantsBox.SelectedItem = Application.Current!.RequestedThemeVariant;
            ThemeVariantsBox.SelectionChanged += (sender, e) =>
            {
                if (ThemeVariantsBox.SelectedItem is ThemeVariant themeVariant)
                {
                    Application.Current!.RequestedThemeVariant = themeVariant;
                }
            };
        }
    }

    public enum ThemeType
    {
        Fluent,
        Simple
    }
}
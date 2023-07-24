using Avalonia.Controls;
using Avalonia.PropertyGrid.Samples.ViewModels;
using Avalonia.Styling;
using Avalonia.Themes.Fluent;
using Avalonia.Themes.Simple;

namespace Avalonia.PropertyGrid.Samples.Views
{
    public partial class MainDemoView : UserControl
    {
        public MainDemoView()
        {
            DataContext = new MainDemoViewModel();

            InitializeComponent();

            propertyGrid_ShowControlProperties.SelectedObject = propertyGrid_ShowControlProperties;

            ThemeBox.SelectedItem = AppThemeUtils.CurrentTheme;
            ThemeBox.SelectionChanged += (sender, e) =>
            {
                if (ThemeBox.SelectedItem is ThemeType theme)
                {
                    AppThemeUtils.SetTheme(theme);
                }
            };

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

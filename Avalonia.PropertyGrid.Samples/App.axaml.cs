using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.PropertyGrid.Samples.ViewModels;
using Avalonia.PropertyGrid.Samples.Views;
using Avalonia.Styling;
using Avalonia.Themes.Fluent;
using Avalonia.Themes.Simple;

namespace Avalonia.PropertyGrid.Samples
{
    public partial class App : Application
    {
        private readonly Styles _themeStylesContainer = new();
        private FluentTheme _fluentTheme;
        private SimpleTheme _simpleTheme;
        private IStyle _colorPickerFluent, _colorPickerSimple;
        private IStyle _dataGridFluent, _dataGridSimple;

        public override void Initialize()
        {
            Styles.Add(_themeStylesContainer);

            AvaloniaXamlLoader.Load(this);

            _fluentTheme = (FluentTheme)Resources["FluentTheme"]!;
            _simpleTheme = (SimpleTheme)Resources["SimpleTheme"]!;
            _colorPickerFluent = (IStyle)Resources["ColorPickerFluent"]!;
            _colorPickerSimple = (IStyle)Resources["ColorPickerSimple"]!;
            _dataGridFluent = (IStyle)Resources["DataGridFluent"]!;
            _dataGridSimple = (IStyle)Resources["DataGridSimple"]!;

            SetTheme(ThemeType.Fluent);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(),
                };
            }

            base.OnFrameworkInitializationCompleted();
        }

        private ThemeType _prevTheme;
        public static ThemeType CurrentTheme => ((App)Current!)._prevTheme;
        public static void SetTheme(ThemeType theme)
        {
            var app = (App)Current!;
            var prevTheme = app._prevTheme;
            app._prevTheme = theme;
            var shouldReopenWindow = prevTheme != theme;

            if (app._themeStylesContainer.Count == 0)
            {
                app._themeStylesContainer.Add(new Style());
                app._themeStylesContainer.Add(new Style());
                app._themeStylesContainer.Add(new Style());
            }

            if (theme == ThemeType.Fluent)
            {
                app._themeStylesContainer[0] = app._fluentTheme!;
                app._themeStylesContainer[1] = app._colorPickerFluent!;
                app._themeStylesContainer[2] = app._dataGridFluent!;
            }
            else if (theme == ThemeType.Simple)
            {
                app._themeStylesContainer[0] = app._simpleTheme!;
                app._themeStylesContainer[1] = app._colorPickerSimple!;
                app._themeStylesContainer[2] = app._dataGridSimple!;
            }

            if (shouldReopenWindow)
            {
                if (app.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
                {
                    var oldWindow = desktopLifetime.MainWindow;
                    var newWindow = new MainWindow();
                    desktopLifetime.MainWindow = newWindow;
                    newWindow.Show();
                    oldWindow?.Close();
                }
//                 else if (app.ApplicationLifetime is ISingleViewApplicationLifetime singleViewLifetime)
//                 {
//                     singleViewLifetime.MainView = new MainView();
//                 }
            }
        }
    }
}
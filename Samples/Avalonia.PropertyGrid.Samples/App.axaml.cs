using System;
using System.Linq;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Avalonia.Platform;
using Avalonia.PropertyGrid.Samples.Views;
using Avalonia.Styling;
using Avalonia.Themes.Fluent;
using Avalonia.Themes.Simple;
using MainWindow = Avalonia.PropertyGrid.Samples.Views.MainWindow;

namespace Avalonia.PropertyGrid.Samples;

public class App : Application
{
    public override void Initialize()
    {
        ThemeUtils.BeforeInitialize();

        AvaloniaXamlLoader.Load(this);

        ThemeUtils.AfterInitialize();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();
            desktop.MainWindow = new MainWindow();
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView();
        }

        if (this.TryGetFeature<IActivatableLifetime>() is { } activatableApplicationLifetime)
        {
            activatableApplicationLifetime.Activated += (sender, args) =>
                Console.WriteLine($"App activated: {args.Kind}");
            activatableApplicationLifetime.Deactivated += (sender, args) =>
                Console.WriteLine($"App deactivated: {args.Kind}");
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}

internal static class ThemeUtils
{
    private static readonly Styles _themeStylesContainer = new();
    private static FluentTheme? _fluentTheme;
    private static SimpleTheme? _simpleTheme;
    private static IStyle? _colorPickerFluent, _colorPickerSimple;
    private static IStyle? _dataGridFluent, _dataGridSimple;

    public static void BeforeInitialize()
    {
        App.Current!.Styles.Add(_themeStylesContainer);
    }

    public static void AfterInitialize()
    {
        _fluentTheme = (FluentTheme)App.Current!.Resources["FluentTheme"]!;
        _simpleTheme = (SimpleTheme)App.Current.Resources["SimpleTheme"]!;
        _colorPickerFluent = (IStyle)App.Current.Resources["ColorPickerFluent"]!;
        _colorPickerSimple = (IStyle)App.Current.Resources["ColorPickerSimple"]!;
        _dataGridFluent = (IStyle)App.Current.Resources["DataGridFluent"]!;
        _dataGridSimple = (IStyle)App.Current.Resources["DataGridSimple"]!;

        SetTheme(ThemeType.Simple);
    }

    private static ThemeType _prevTheme = ThemeType.Simple;
    public static ThemeType CurrentTheme => _prevTheme;

    public static void SetTheme(ThemeType theme)
    {
        var app = (App)App.Current!;

        var prevTheme = _prevTheme;
        _prevTheme = theme;
        var shouldReopenWindow = prevTheme != theme;

        if (_themeStylesContainer.Count == 0)
        {
            _themeStylesContainer.Add(new Style());
            _themeStylesContainer.Add(new Style());
            _themeStylesContainer.Add(new Style());
        }

        if (theme == ThemeType.Fluent)
        {
            _themeStylesContainer[0] = _fluentTheme!;
            _themeStylesContainer[1] = _colorPickerFluent!;
            _themeStylesContainer[2] = _dataGridFluent!;
        }
        else if (theme == ThemeType.Simple)
        {
            _themeStylesContainer[0] = _simpleTheme!;
            _themeStylesContainer[1] = _colorPickerSimple!;
            _themeStylesContainer[2] = _dataGridSimple!;
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
            else if (app.ApplicationLifetime is ISingleViewApplicationLifetime singleViewLifetime)
            {
                singleViewLifetime.MainView = new MainView();
            }
        }
    }
}
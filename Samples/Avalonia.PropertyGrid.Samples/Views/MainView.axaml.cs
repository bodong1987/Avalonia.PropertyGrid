using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.PropertyGrid.Services;
using Avalonia.Reactive;
using Avalonia.Styling;
using PropertyModels.Localization;

namespace Avalonia.PropertyGrid.Samples.Views;

public partial class MainView : UserControl
{
    private readonly List<ICultureData> _allCultures = [];

    public ICultureData[] AllCultures => [.. _allCultures];
    
    public static readonly StyledProperty<ICultureData> CurrentCultureProperty =
        AvaloniaProperty.Register<MainView, ICultureData>(
            nameof(CurrentCulture),
            defaultValue: LocalizationService.Default.CultureData);

    public ICultureData CurrentCulture
    {
        get => GetValue(CurrentCultureProperty);
        set => SetValue(CurrentCultureProperty, value);
    }

    public static string Version => $"v{typeof(Utils.FontUtils).Assembly.GetName().Version?.ToString() ?? "Unknown Version"}";
    
    public MainView()
    {
        LocalizationService.Default.AddExtraService(new SampleLocalizationService());
        _allCultures.AddRange(LocalizationService.Default.GetCultures());
        
        InitializeComponent();
        
        this.GetObservable(CurrentCultureProperty).Subscribe(new AnonymousObserver<ICultureData>(OnCurrentCultureChanged));

        
        ThemeBox.SelectedItem = App.CurrentTheme;
        ThemeBox.SelectionChanged += (sender, e) =>
        {
            if (ThemeBox.SelectedItem is ThemeType theme)
            {
                App.SetThemes(theme);
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
    
    private static void OnCurrentCultureChanged(ICultureData? newCulture)
    {
        if (newCulture != null)
        {
            LocalizationService.Default.SelectCulture(newCulture.Culture.Name);
        }
    }
}

public enum ThemeType
{
    Fluent,
    Simple
}
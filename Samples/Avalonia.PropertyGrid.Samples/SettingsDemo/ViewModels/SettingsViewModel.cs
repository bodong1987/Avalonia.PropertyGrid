using Avalonia.Collections;
using Avalonia.PropertyGrid.Samples.SettingsDemo.Models;
using Avalonia.PropertyGrid.Services;
using PropertyModels.ComponentModel;

namespace Avalonia.PropertyGrid.Samples.SettingsDemo.ViewModels;

public class SettingsViewModel : ReactiveObject
{
    public AvaloniaList<FlatSettingCategory> FlatCategories { get; } = [];

    public SettingsViewModel()
    {
        InitializeCategories();

        LocalizationService.Default.OnCultureChanged += (sender, args) =>
        {
            foreach (var category in FlatCategories)
            {
                category.Translate();
            }
        };
    }

    private void InitializeCategories()
    {
        var categories = new[]
        {
            CreateCategory("General",  new GeneralSettings()),
            CreateCategory("Appearance", new AppearanceSettings()),
            CreateCategory("Themes",  new ThemeSettings()),
            CreateCategory("Accounts", new AccountSettings()),
            CreateCategory("Firewall", new FirewallSettings()),
            CreateCategory("About",  new AboutSettings())
        };

        FlatCategories.AddRange(categories);
    }

    private static FlatSettingCategory CreateCategory(string title, object settings)
    {
        return new FlatSettingCategory(title, settings);
    }
}

public class FlatSettingCategory : ReactiveObject
{
    public string Key { get; }
    public string Title { get; private set; }
    public object Settings { get; }

    public FlatSettingCategory(string key, object settings)
    {
        Key = key;
        Title = LocalizationService.Default[key];
        Settings = settings;
    }

    public void Translate()
    {
        Title = LocalizationService.Default[Key];
        this.RaisePropertyChanged(nameof(Title));
    }
}
using System.ComponentModel;
using Avalonia.Media;
using PropertyModels.ComponentModel.DataAnnotations;

namespace Avalonia.PropertyGrid.Samples.SettingsDemo.Models;

public class AppearanceSettings
{
    [Category("Theme")]
    [DisplayName("Accent Color")]
    public Color AccentColor { get; set; } = Colors.Blue;

    [Category("Theme")]
    [DisplayName("Dark Mode")]
    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    public bool UseDarkTheme { get; set; } = true;

    [Category("Fonts")]
    [DisplayName("System Font")]
    public FontFamily SystemFont { get; set; } = FontFamily.Default;

    [Category("Effects")]
    [DisplayName("Transparency Effects")]
    [DependsOnProperty(nameof(UseDarkTheme))]
    [Browsable(false)]
    public bool EnableTransparencyEffects => UseDarkTheme;
}
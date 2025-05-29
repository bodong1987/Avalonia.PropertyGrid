using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Avalonia.Media;
using PropertyModels.ComponentModel;
using PropertyModels.ComponentModel.DataAnnotations;

namespace Avalonia.PropertyGrid.Samples.SettingsDemo.Models;

public class AppearanceSettings : ReactiveObject
{
    [Category("Theme")]
    [DisplayName("Accent Color")]
    public Color AccentColor { get; set; } = Colors.Blue;

    [Category("Theme")]
    [DisplayName("Dark Mode")]
    [ConditionTarget]
    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    public bool UseDarkTheme { get; set; } = true;
    
    [Category("Theme")]
    [Range(0.3f, 1.0f)]
    [Trackable(0.3f,1.0f)]
    public float Transparency { get; set; } = 1.0f;

    [Category("Fonts")]
    [DisplayName("Custom Font")]
    [ConditionTarget]
    public bool UseCustomFont { get; set; } = false;
    
    [Category("Fonts")]
    [DisplayName("System Font")]
    [VisibilityPropertyCondition(nameof(UseCustomFont), true)]
    public FontFamily SystemFont { get; set; } = FontFamily.Default;

    [Category("Effects")]
    [DisplayName("Transparency Effects")]
    [DependsOnProperty(nameof(UseDarkTheme))]
    public bool EnableTransparencyEffects => UseDarkTheme;
}
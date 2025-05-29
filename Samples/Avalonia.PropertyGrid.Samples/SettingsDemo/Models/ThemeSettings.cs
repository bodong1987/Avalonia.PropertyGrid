using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Avalonia.Media;
using PropertyModels.Collections;
using PropertyModels.ComponentModel;
using PropertyModels.ComponentModel.DataAnnotations;

namespace Avalonia.PropertyGrid.Samples.SettingsDemo.Models;

public class ThemeSettings : ReactiveObject
{
    [Category("Color Scheme")]
    [DisplayName("Primary Color")]
    public Color PrimaryColor { get; set; } = Color.FromRgb(0, 120, 215);

    [Category("Color Scheme")]
    [DisplayName("Secondary Color")]
    public Color SecondaryColor { get; set; } = Color.FromRgb(153, 235, 255);

    [Category("Appearance")]
    [DisplayName("Theme Mode")]
    public SelectableList<string> ThemeMode { get; set; } = new (["Auto", "Light", "Dark"], "Auto");

    [Category("Advanced")]
    [DisplayName("Custom Font")]
    [VisibilityPropertyCondition(nameof(UseCustomFont), true)]
    public FontFamily CustomFont { get; set; } = FontFamily.Default;

    [Category("Advanced")]
    [DisplayName("Enable Custom Font")]
    public bool UseCustomFont { get; set; }

    [Category("Effects")]
    [DisplayName("Transparency Level")]
    [Range(0.3, 1.0)]
    [FloatPrecision(2)]
    [Trackable(0.3, 1)] 
    public double Transparency { get; set; } = 0.8;
}
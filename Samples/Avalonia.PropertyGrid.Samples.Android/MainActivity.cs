using Android.App;
using Android.Content.PM;
using Avalonia.Android;

namespace Avalonia.PropertyGrid.Samples.Android;

[Activity(
    Label = "Avalonia.PropertyGrid.Samples.Android",
    Theme = "@style/MyTheme.NoActionBar",
    Icon = "@drawable/icon",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
public class MainActivity : AvaloniaMainActivity
{
}

using System;
using Avalonia;
using Avalonia.Platform;

namespace Avalonia.PropertyGrid.Samples.Desktop;

internal sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            #if !DEBUG
            .AfterSetup(builder=>
            {
                builder.Instance!.AttachDevTools(new Avalonia.Diagnostics.DevToolsOptions()
                {
                    StartupScreenIndex = 1,
                });
            })
            #endif
            ;
}

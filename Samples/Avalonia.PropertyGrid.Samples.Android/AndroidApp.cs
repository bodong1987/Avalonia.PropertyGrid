using System;
using Android.App;
using Android.Runtime;
using Avalonia.Android;

namespace Avalonia.PropertyGrid.Samples.Android;

[Application]
public class AndroidApp : AvaloniaAndroidApplication<App>
{
    protected AndroidApp(IntPtr javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer)
    {
    }
}

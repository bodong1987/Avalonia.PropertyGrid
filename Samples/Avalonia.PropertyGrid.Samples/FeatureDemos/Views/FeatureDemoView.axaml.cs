using System.ComponentModel;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Interactivity;
using Avalonia.PropertyGrid.Controls;
using Avalonia.PropertyGrid.Samples.FeatureDemos.Models;
using Avalonia.PropertyGrid.Samples.FeatureDemos.ViewModels;
using Avalonia.PropertyGrid.Services;
using Avalonia.PropertyGrid.ViewModels;
using PropertyModels.ComponentModel;
using PropertyModels.Extensions;

namespace Avalonia.PropertyGrid.Samples.FeatureDemos.Views;

public partial class FeatureDemoView : UserControl
{
    public ICommand ShowManagedNotificationCommand { get; }
    
    public WindowNotificationManager? NotificationManager { get; set; }
    
    public FeatureDemoView()
    {
        var mainVm = new FeatureDemoViewModel();
        DataContext = mainVm;

        InitializeComponent();
        
        ShowManagedNotificationCommand = ReactiveCommand.Create(() =>
        {
            NotificationManager?.Show(new Notification(
                LocalizationService.Default["Welcome"], 
                LocalizationService.Default["Avalonia.PropertyGrid now supports custom areas."]
                ));
        });
        
        CurrentPropertyName = LocalizationService.Default["No Property Focused"];
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        
        NotificationManager = new WindowNotificationManager(TopLevel.GetTopLevel(this)!);
    } 

    private void OnCustomPropertyDescriptorFilter(object sender, RoutedEventArgs args)
    {
        if (args is CustomPropertyDescriptorFilterEventArgs { TargetObject: SimpleObject, PropertyDescriptor.Name: nameof(SimpleObject.ThreeStates2) } e)
        {
            e.IsVisible = true;
            e.Handled = true;
        }
    }

    private void OnCommandExecuted(object? sender, RoutedEventArgs e) => (DataContext as FeatureDemoViewModel)!.CancelableObject.OnCommandExecuted(sender, (e as RoutedCommandExecutedEventArgs)!);
    
    public static readonly StyledProperty<string> CurrentPropertyNameProperty =
        AvaloniaProperty.Register<FeatureDemoView, string>(nameof(CurrentPropertyName));

    public string CurrentPropertyName
    {
        get => GetValue(CurrentPropertyNameProperty);
        set => SetValue(CurrentPropertyNameProperty, value);
    }
    
    private void OnPropertyGotFocus(object? sender, RoutedEventArgs args)
    {
        var e = args as PropertyGotFocusEventArgs;
        
        CurrentPropertyName =
            string.Format(LocalizationService.Default["CurrentPropertyDescription"], 
                LocalizationService.Default[e!.Context.DisplayName],
                e!.Context.Property.Description.IsNotNullOrEmpty() ? (": " + LocalizationService.Default[e!.Context.Property.Description]) : "" 
                );
    }

    private void OnPropertyLostFocus(object? sender, RoutedEventArgs args)
    {
        CurrentPropertyName = LocalizationService.Default["No Property Focused"];
    }
}
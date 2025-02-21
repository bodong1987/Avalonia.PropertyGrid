using Avalonia.Controls;
using Avalonia.PropertyGrid.Controls;
using Avalonia.PropertyGrid.Samples.Models;
using Avalonia.PropertyGrid.Samples.ViewModels;
using Avalonia.PropertyGrid.ViewModels;
using Avalonia.Styling;
using System.ComponentModel;

namespace Avalonia.PropertyGrid.Samples.Views;

public partial class MainView : UserControl
{
    MainViewModel MainVM;

    public MainView()
    {
        MainVM = new MainViewModel();
        DataContext = MainVM;

        InitializeComponent();

        ThemeBox.SelectedItem = AppThemeUtils.CurrentTheme;
        ThemeBox.SelectionChanged += (sender, e) =>
        {
            if (ThemeBox.SelectedItem is ThemeType theme)
            {
                AppThemeUtils.SetTheme(theme);
            }
        };

        ThemeVariantsBox.SelectionChanged += (sender, e) =>
        {
            if (ThemeVariantsBox.SelectedItem is ThemeVariant themeVariant)
            {
                Application.Current!.RequestedThemeVariant = themeVariant;
            }
        };

        proeprtyGrid_RedoUndo.CommandExecuted += OnCommandExecuted;

        ((MainViewModel)DataContext).PropertyChanged += OnPropertyChanged;

        ShowStyleComboBox.SelectionChanged += (sender, e) =>
        {
            if (ShowStyleComboBox.SelectedItem is PropertyGridShowStyle showStyle)
            {
                propertyGrid_Styles.ShowStyle = showStyle;
            }
        };

        CategoryOrderComboBox.SelectionChanged += (sender, e) =>
        {
            if (CategoryOrderComboBox.SelectedItem is PropertyGridOrderStyle showStyle)
            {
                propertyGrid_Styles.CategoryOrderStyle = showStyle;
            }
        };

        PropertyOrderComboBox.SelectionChanged += (sender, e) =>
        {
            if (PropertyOrderComboBox.SelectedItem is PropertyGridOrderStyle showStyle)
            {
                propertyGrid_Styles.PropertyOrderStyle = showStyle;
            }
        };
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MainVM.ShowStyle))
        {
            propertyGrid_Styles.ShowStyle = MainVM.ShowStyle;
        }
        else if (e.PropertyName == nameof(MainVM.IsShowTitle))
        {
            propertyGrid_Styles.ShowTitle = MainVM.IsShowTitle;
        }
        else if (e.PropertyName == nameof(MainVM.AllowFilter))
        {
            propertyGrid_Styles.AllowFilter = MainVM.AllowFilter;
        }
        else if (e.PropertyName == nameof(MainVM.AllowQuickFilter))
        {
            propertyGrid_Styles.AllowQuickFilter = MainVM.AllowQuickFilter;
        }
        else if (e.PropertyName == nameof(MainVM.DefaultNameWidth))
        {
            propertyGrid_Styles.NameWidth = MainVM.DefaultNameWidth;
        }
        else if (e.PropertyName == nameof(MainVM.IsReadOnly))
        {
            propertyGrid_Styles.IsReadOnly = MainVM.IsReadOnly;
        }
    }

    private void OnCustomPropertyDescriptorFilter(object sender, CustomPropertyDescriptorFilterEventArgs e)
    {
        if (e.TargetObject is SimpleObject simpleObject && e.PropertyDescriptor.Name == "ThreeStates2")
        {
            e.IsVisible = true;
            e.Handled = true;
        }
    }

    private void OnCommandExecuted(object? sender, RoutedCommandExecutedEventArgs e)
    {
        (DataContext as MainViewModel)!.cancelableObject.OnCommandExecuted(sender, e);
    }

}
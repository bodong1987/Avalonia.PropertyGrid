using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.PropertyGrid.Controls;
using Avalonia.PropertyGrid.Samples.FeatureDemos.Models;
using Avalonia.PropertyGrid.Samples.FeatureDemos.ViewModels;
using Avalonia.PropertyGrid.ViewModels;
using PropertyModels.ComponentModel;

namespace Avalonia.PropertyGrid.Samples.FeatureDemos.Views;

public partial class FeatureDemoView : UserControl
{
    private readonly FeatureDemoViewModel _mainVm;

    public FeatureDemoView()
    {
        _mainVm = new FeatureDemoViewModel();
        DataContext = _mainVm;

        InitializeComponent();

        PropertyGridRedoUndo.CommandExecuted += OnCommandExecuted;

        ((FeatureDemoViewModel)DataContext).PropertyChanged += OnPropertyChanged;

        DisplayModeComboBox.SelectionChanged += (sender, e) =>
        {
            if (DisplayModeComboBox.SelectedItem is PropertyGridDisplayMode displayMode)
            {
                StylesPropertyGrid.DisplayMode = displayMode;
            }
        };

        ShowStyleComboBox.SelectionChanged += (sender, e) =>
        {
            if (ShowStyleComboBox.SelectedItem is PropertyGridShowStyle showStyle)
            {
                StylesPropertyGrid.ShowStyle = showStyle;
            }
        };

        CategoryOrderComboBox.SelectionChanged += (sender, e) =>
        {
            if (CategoryOrderComboBox.SelectedItem is PropertyGridOrderStyle showStyle)
            {
                StylesPropertyGrid.CategoryOrderStyle = showStyle;
            }
        };

        PropertyOrderComboBox.SelectionChanged += (sender, e) =>
        {
            if (PropertyOrderComboBox.SelectedItem is PropertyGridOrderStyle showStyle)
            {
                StylesPropertyGrid.PropertyOrderStyle = showStyle;
            }
        };
        
        PropertyOperationComboBox.SelectionChanged += (sender, e) =>
        {
            if (PropertyOperationComboBox.SelectedItem is PropertyOperationVisibility visibility)
            {
                StylesPropertyGrid.PropertyOperationVisibility = visibility;
            }
        };

        CellEditAlignmentComboBox.SelectionChanged += (sender, e) =>
        {
            if (CellEditAlignmentComboBox.SelectedItem is CellEditAlignmentType cellEditAlignment)
            {
                StylesPropertyGrid.CellEditAlignment = cellEditAlignment;
            }
        };
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(_mainVm.ShowStyle))
        {
            StylesPropertyGrid.ShowStyle = _mainVm.ShowStyle;
        }
        else if (e.PropertyName == nameof(_mainVm.IsShowTitle))
        {
            StylesPropertyGrid.ShowTitle = _mainVm.IsShowTitle;
        }
        else if (e.PropertyName == nameof(_mainVm.AllowFilter))
        {
            StylesPropertyGrid.AllowFilter = _mainVm.AllowFilter;
        }
        else if (e.PropertyName == nameof(_mainVm.AllowQuickFilter))
        {
            StylesPropertyGrid.AllowQuickFilter = _mainVm.AllowQuickFilter;
        }
        else if (e.PropertyName == nameof(_mainVm.DefaultNameWidth))
        {
            StylesPropertyGrid.NameWidth = _mainVm.DefaultNameWidth;
        }
        else if (e.PropertyName == nameof(_mainVm.IsReadOnly))
        {
            StylesPropertyGrid.IsReadOnly = _mainVm.IsReadOnly;
        }
    }

    private void OnCustomPropertyDescriptorFilter(object sender, CustomPropertyDescriptorFilterEventArgs e)
    {
        if (e is { TargetObject: SimpleObject, PropertyDescriptor.Name: "ThreeStates2" })
        {
            e.IsVisible = true;
            e.Handled = true;
        }
    }

    private void OnCommandExecuted(object? sender, RoutedCommandExecutedEventArgs e) => (DataContext as FeatureDemoViewModel)!.CancelableObject.OnCommandExecuted(sender, e);

}
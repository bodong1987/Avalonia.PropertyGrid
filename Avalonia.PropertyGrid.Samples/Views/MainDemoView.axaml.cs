using Avalonia.Controls;
using Avalonia.PropertyGrid.Controls;
using Avalonia.PropertyGrid.Samples.Models;
using Avalonia.PropertyGrid.Samples.ViewModels;
using Avalonia.PropertyGrid.ViewModels;
using Avalonia.Styling;
using Avalonia.Themes.Fluent;
using Avalonia.Themes.Simple;
using System;

namespace Avalonia.PropertyGrid.Samples.Views
{
    public partial class MainDemoView : UserControl
    {
        public MainDemoView()
        {
            DataContext = new MainDemoViewModel();

            InitializeComponent();

            propertyGrid_ShowControlProperties.DataContext = propertyGrid_ShowControlProperties;

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

            ShowStyleComboBox.SelectionChanged += (sender, e) =>
            {
                if(ShowStyleComboBox.SelectedItem is PropertyGridShowStyle showStyle)
                {
                    propertyGrid_Styles.ShowStyle = showStyle;
                }
            };

        }

		private void OnCustomPropertyDescriptorFilter(object sender, CustomPropertyDescriptorFilterEventArgs e)
		{
			if(e.SelectedObject is SimpleObject simpleObject&& e.PropertyDescriptor.Name == "ThreeStates2")
            {
                e.IsVisible = true;
                e.Handled = true;
            }
		}

		private void OnCommandExecuted(object sender, RoutedCommandExecutedEventArgs e)
        {
            (DataContext as MainDemoViewModel).cancelableObject.OnCommandExecuted(sender, e);
        }
    }

    public enum ThemeType
    {
        Fluent,
        Simple
    }
}

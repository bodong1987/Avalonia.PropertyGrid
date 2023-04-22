using Avalonia.Controls;
using Avalonia.PropertyGrid.NugetSamples.ViewModels;
using System;

namespace Avalonia.PropertyGrid.NugetSamples.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnDataContextChanged(EventArgs e)
        {
            base.OnDataContextChanged(e);
        }
    }
}
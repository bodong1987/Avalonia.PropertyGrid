using Avalonia.Controls;
using Avalonia.PropertyGrid.Model.ComponentModel;
using System.ComponentModel;
using System;

namespace Avalonia.PropertyGrid.Controls
{
    public partial class BindingListElementPlaceholderEdit : UserControl
    {
        public BindingListElementPlaceholderEdit()
        {
            InitializeComponent();
        }

        protected override void OnDataContextChanged(EventArgs e)
        {
            base.OnDataContextChanged(e);

            OnDataDescPropertyChanged(DataContext as BindingListElementDataDesc);
        }
     
        private void OnDataDescPropertyChanged(BindingListElementDataDesc value)
        {
            mainGrid.Children.Clear();

            if(value == null)
            {
                return;
            }

            IPropertyGridControlFactory factory;
            var control = value.Collection.BuildPropertyControl(value.List, value.Property, out factory);
            if (control != null)
            {
                control.Margin = new Thickness(10,2);
                mainGrid.Children.Add(control);

                factory.HandlePropertyChanged(value.List, value.Property, control);
            }
        }
    }
}

using Avalonia.Controls;
using Avalonia.PropertyGrid.Model.ComponentModel;
using System.ComponentModel;
using System;

namespace Avalonia.PropertyGrid.Controls
{
    public partial class BindingListElementPlaceholderEdit : UserControl
    {
        IPropertyGridControlFactory Factory;
        Control BindingControl;

        public BindingListElementPlaceholderEdit()
        {
            InitializeComponent();
        }

        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
        {
            base.OnPropertyChanged(change);

            if(change.Property == DataContextProperty)
            {
                OnDataDescPropertyChanged(change.OldValue.Value as BindingListElementDataDesc, change.NewValue.Value as BindingListElementDataDesc);
            }
        }

        protected override void OnDataContextChanged(EventArgs e)
        {
            base.OnDataContextChanged(e);
        }
     
        private void OnDataDescPropertyChanged(BindingListElementDataDesc oldValue, BindingListElementDataDesc value)
        {
            if(oldValue != null)
            {
                oldValue.ValueChanged -= OnElementValueChanged;
            }

            mainGrid.Children.Clear();

            if(value == null)
            {
                return;
            }

            value.ValueChanged += OnElementValueChanged;

            var control = value.Collection.BuildPropertyControl(value.List, value.Property, out Factory);
            if (control != null)
            {
                BindingControl = control;
                control.Margin = new Thickness(10,2);
                mainGrid.Children.Add(control);

                Factory.HandlePropertyChanged(value.List, value.Property, control);
            }
        }


        private void OnElementValueChanged(object sender, EventArgs e)
        {
            var value = DataContext as BindingListElementDataDesc;

            if(value == null)
            {
                return;
            }

            if(Factory!=null && BindingControl != null)
            {
                Factory.HandlePropertyChanged(value.List, value.Property, BindingControl);
            }
        }
    }
}

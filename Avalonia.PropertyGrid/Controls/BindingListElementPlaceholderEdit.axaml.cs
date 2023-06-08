using Avalonia.Controls;
using Avalonia.PropertyGrid.Model.ComponentModel;
using System.ComponentModel;
using System;
using System.Diagnostics;

namespace Avalonia.PropertyGrid.Controls
{
    /// <summary>
    /// Class BindingListElementPlaceholderEdit.
    /// Implements the <see cref="UserControl" />
    /// </summary>
    /// <seealso cref="UserControl" />
    public partial class BindingListElementPlaceholderEdit : UserControl
    {
        ICellEditFactory Factory;
        Control BindingControl;

        /// <summary>
        /// Initializes a new instance of the <see cref="BindingListElementPlaceholderEdit"/> class.
        /// </summary>
        public BindingListElementPlaceholderEdit()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="change">The change.</param>
        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {        
            base.OnPropertyChanged(change);

            if(change.Property == DataContextProperty)
            {
                OnDataDescPropertyChanged(change.OldValue as BindingListElementDataDesc, change.NewValue as BindingListElementDataDesc);
            }
        }

        /// <summary>
        /// Called when the <see cref="P:Avalonia.StyledElement.DataContext" /> property changes.
        /// </summary>
        /// <param name="e">The event args.</param>
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

            mainBorder.Child = null;

            if (value == null)
            {
                return;
            }

            value.ValueChanged += OnElementValueChanged;

            var control = value.Collection.BuildPropertyControl(value.List, value.Property, out Factory);
            if (control != null)
            {
                BindingControl = control;
                control.Margin = new Thickness(2,2);
                control.HorizontalAlignment = Layout.HorizontalAlignment.Stretch;

                mainBorder.Child = control;

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

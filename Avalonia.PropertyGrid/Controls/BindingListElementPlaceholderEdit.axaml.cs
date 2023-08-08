using Avalonia.Controls;
using PropertyModels.ComponentModel;
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
        PropertyCellContext Context;

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

            Debug.Assert(value.Context.Root!=null);

            Context = new PropertyCellContext(value.Context, value.List, value.Property);

            var control = Context.GetCellEditFactoryCollection().BuildPropertyControl(Context);
            if (control != null)
            {
                control.Margin = new Thickness(2,2);

                mainBorder.Child = control;

                Debug.Assert(Context.Factory != null);

                Context.Factory.HandlePropertyChanged(Context);
            }
        }


        private void OnElementValueChanged(object sender, EventArgs e)
        {
            var value = DataContext as BindingListElementDataDesc;

            if(value == null)
            {
                return;
            }

            if(Context != null && Context.Factory != null)
            {
                Debug.Assert(Context!=null);
                Debug.Assert(Context.CellEdit != null);

                Context.Factory.HandlePropertyChanged(Context);
            }
        }
    }
}

using System;
using System.Diagnostics;
using Avalonia.Controls;

namespace Avalonia.PropertyGrid.Controls
{
    /// <summary>
    /// Class ListElementPlaceholderEdit.
    /// Implements the <see cref="UserControl" />
    /// </summary>
    /// <seealso cref="UserControl" />
    public partial class ListElementPlaceholderEdit : UserControl
    {
        private PropertyCellContext? _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListElementPlaceholderEdit"/> class.
        /// </summary>
        public ListElementPlaceholderEdit()
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
                OnDataDescPropertyChanged(change.OldValue as ListElementDataDesc, change.NewValue as ListElementDataDesc);
            }
        }

        // ReSharper disable once RedundantOverriddenMember
        /// <summary>
        /// Called when the <see cref="P:Avalonia.StyledElement.DataContext" /> property changes.
        /// </summary>
        /// <param name="e">The event args.</param>
        protected override void OnDataContextChanged(EventArgs e)
        {
            base.OnDataContextChanged(e);
        }
     
        private void OnDataDescPropertyChanged(ListElementDataDesc? oldValue, ListElementDataDesc? value)
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

            Debug.Assert(value.Context != null);
            Debug.Assert(value.Context.Root!=null);

            _context = new PropertyCellContext(value.Context, value.List, value.Property);

            var control = _context.GetCellEditFactoryCollection().BuildPropertyControl(_context);
            if (control != null)
            {
                control.Margin = new Thickness(2,2);

                mainBorder.Child = control;

                Debug.Assert(_context.Factory != null);

                _context.Factory!.HandleReadOnlyStateChanged(control, _context.IsReadOnly || value.IsReadOnly);

                _context.Factory.HandlePropertyChanged(_context);
            }
        }


        private void OnElementValueChanged(object? sender, EventArgs e)
        {
            if(DataContext is not ListElementDataDesc)
            {
                return;
            }

            if(_context is { Factory: not null })
            {
                Debug.Assert(_context!=null);
                Debug.Assert(_context!.CellEdit != null);

                _context.Factory.HandlePropertyChanged(_context);
            }
        }
    }
}

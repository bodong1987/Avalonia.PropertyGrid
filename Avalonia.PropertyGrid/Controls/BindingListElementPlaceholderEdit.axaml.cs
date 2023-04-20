using Avalonia.Controls;
using Avalonia.PropertyGrid.Model.ComponentModel;
using System.ComponentModel;
using System;

namespace Avalonia.PropertyGrid.Controls
{
    public partial class BindingListElementPlaceholderEdit : UserControl
    {
        public static readonly DirectProperty<BindingListElementPlaceholderEdit, BindingListElementDataDesc> DataDescProperty =
            AvaloniaProperty.RegisterDirect<BindingListElementPlaceholderEdit, BindingListElementDataDesc>(
                nameof(DataDesc),
                o => o.DataDesc,
                (o, v) => o.DataDesc = v
                );

        BindingListElementDataDesc _DataDesc;

        public BindingListElementDataDesc DataDesc
        {
            get => _DataDesc;
            set => SetAndRaise(DataDescProperty, ref _DataDesc, value);
        }

        static BindingListElementPlaceholderEdit()
        {
            DataDescProperty.Changed?.Subscribe(OnDataDescPropertyChanged);
        }

        public BindingListElementPlaceholderEdit()
        {
            InitializeComponent();
        }

        private static void OnDataDescPropertyChanged(AvaloniaPropertyChangedEventArgs<BindingListElementDataDesc> e)
        {
            if(e.Sender is BindingListElementPlaceholderEdit sender)
            {
                sender.OnDataDescPropertyChanged(e.NewValue.Value);
            }
        }

        private void OnDataDescPropertyChanged(BindingListElementDataDesc value)
        {
            mainGrid.Children.Clear();

            if(value == null)
            {
                return;
            }

            IPropertyGridControlFactory factory;
            var control = value.Parent.BuildPropertyControl(value.Property, out factory);
            if (control != null)
            {
                mainGrid.Children.Add(control);

                factory.HandlePropertyChanged(value.List, value.Property, control);
            }
        }
    }

    public class BindingListElementDataDesc : ReactiveObject
    {
        public readonly IBindingList List;
        public readonly PropertyDescriptor Property;
        public readonly PropertyGrid Parent;

        public BindingListElementDataDesc(PropertyGrid parent,  IBindingList list, PropertyDescriptor property)
        {
            this.Parent = parent;
            this.List = list;
            this.Property = property;
        }
    }
}

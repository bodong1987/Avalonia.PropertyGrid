using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using System.ComponentModel;

namespace Avalonia.PropertyGrid.Controls
{
    public class BindingListEdit : TemplatedControl
    {
        public static readonly DirectProperty<BindingListEdit, IBindingList> DataListProperty =
            AvaloniaProperty.RegisterDirect<BindingListEdit, IBindingList>(
                nameof(DataList),
                o => o.DataList,
                (o, v) => o.DataList = v
                );

        IBindingList _DataList;
        public IBindingList DataList
        {
            get => _DataList;
            set => SetAndRaise(DataListProperty, ref _DataList, value);
        }


    }
}

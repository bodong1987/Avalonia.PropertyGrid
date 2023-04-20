using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Avalonia.PropertyGrid.Controls
{
    public class BindingListElementEdit : TemplatedControl
    {
        public readonly static DirectProperty<BindingListElementEdit, BindingListElementDataDesc> DescProperty =
            AvaloniaProperty.RegisterDirect<BindingListElementEdit, BindingListElementDataDesc>(
                nameof(Desc),
                o => o.Desc,
                (o, v) => o.Desc = v);

        BindingListElementDataDesc _Desc;
        public BindingListElementDataDesc Desc
        {
            get => _Desc;
            set => SetAndRaise(DescProperty, ref _Desc, value);
        }
    }
}

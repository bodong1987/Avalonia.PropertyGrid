using Avalonia;
using Avalonia.Controls;
using System;
using Avalonia.Controls.Primitives;
using Avalonia.PropertyGrid.Model.ComponentModel;
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

        internal BindingListViewModel Model { get; private set; }
        
        static BindingListEdit()
        {
            DataListProperty.Changed.Subscribe(OnDataListChanged);
        }

        public BindingListEdit()
        {
        }

        public BindingListEdit(PropertyGrid propertyGrid)
        {
        }

        private static void OnDataListChanged(AvaloniaPropertyChangedEventArgs<IBindingList> e)
        {
            if(e.Sender is BindingListEdit ble)
            {
                ble.OnDataListChanged(e.NewValue.Value);
            }
        }

        private void OnDataListChanged(IBindingList value)
        {
            Model.List = value;
        }
    }

    internal class BindingListViewModel : ReactiveObject
    {
        IBindingList _List;
        public IBindingList List
        {
            get => _List;
            set => this.RaiseAndSetIfChanged(ref _List, value);
        }

        public BindingListViewModel() :
            this(null)
        {
        }

        public BindingListViewModel(IBindingList list)
        {
            List = list;


        }
    }
}

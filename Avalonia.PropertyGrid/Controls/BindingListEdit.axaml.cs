using Avalonia;
using Avalonia.Controls;
using System;
using Avalonia.Controls.Primitives;
using Avalonia.PropertyGrid.Model.ComponentModel;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

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

        internal BindingListViewModel Model { get; private set; } = new BindingListViewModel();
        
        static BindingListEdit()
        {
            DataListProperty.Changed.Subscribe(OnDataListChanged);
        }

        public BindingListEdit()
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

    #region Models
    internal class BindingListViewModel : ReactiveObject
    {
        public IPropertyGridControlFactoryCollection Collection { get; set; }

        IBindingList _List;
        public IBindingList List
        {
            get => _List;
            set => this.RaiseAndSetIfChanged(ref _List, value);
        }

        BindingListDescription _Desc;
        public BindingListDescription Desc
        {
            get => _Desc;
            set => this.RaiseAndSetIfChanged(ref _Desc, value);
        }

        public string Title => string.Format(PropertyGrid.LocalizationService["{0} Elements"], _List != null ? _List.Count : 0);

        public BindingListViewModel() :
            this(null)
        {
        }

        public BindingListViewModel(IBindingList list)
        {
            List = list;

            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(List))
            {
                Debug.Assert(Collection != null);

                // create binding view
                Desc = new BindingListDescription(List, Collection);
            }
        }
    }

    internal class BindingListDescription : ReactiveObject
    {
        readonly List<BindingListElementDataDesc> _Desc = new List<BindingListElementDataDesc>();
        public BindingListElementDataDesc[] Descs => _Desc.ToArray();
        readonly IBindingList List;

        public BindingListDescription(IBindingList list, IPropertyGridControlFactoryCollection collection)
        {
            List = list;

            if(list == null)
            {
                return;
            }

            foreach(var index in Enumerable.Range(0, list.Count))
            {
                var pd = new BindingListElementPropertyDescriptor(index.ToString(), index, list[index]?.GetType() ?? list.GetType().GetGenericArguments()[0]);
                BindingListElementDataDesc desc = new BindingListElementDataDesc(list, pd, collection);

                _Desc.Add(desc);
            }
        }
    }
    #endregion
}

using Avalonia;
using Avalonia.Controls;
using System;
using Avalonia.Controls.Primitives;
using Avalonia.PropertyGrid.Model.ComponentModel;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Xml.Linq;
using Avalonia.PropertyGrid.Model.ComponentModel.DataAnnotations;
using System.Windows.Input;

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

        readonly List<BindingListElementDataDesc> _Elements = new List<BindingListElementDataDesc>();

        public BindingListElementDataDesc[] Elements => _Elements.ToArray();

        [DependsOnProperty(nameof(List))]
        public string Title => string.Format(PropertyGrid.LocalizationService["{0} Elements"], _List != null ? _List.Count : 0);

        public ICommand InsertCommand { get; set; }

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

                _Elements.Clear();

                if(List != null)
                {
                    var list = List;
                    foreach (var index in Enumerable.Range(0, list.Count))
                    {
                        var pd = new BindingListElementPropertyDescriptor(index.ToString(), index, list[index]?.GetType() ?? list.GetType().GetGenericArguments()[0]);
                        BindingListElementDataDesc desc = new BindingListElementDataDesc(this, list, pd, Collection);

                        _Elements.Add(desc);
                    }
                }

                RaisePropertyChanged(nameof(Elements));                
            }
        }
    }
    internal class BindingListElementDataDesc : ReactiveObject
    {
        public readonly IBindingList List;
        public readonly PropertyDescriptor Property;
        public readonly IPropertyGridControlFactoryCollection Collection;

        public ICommand InsertCommand { get; set; }

        public BindingListViewModel Model { get; set; }

        public BindingListElementDataDesc(BindingListViewModel model, IBindingList list, PropertyDescriptor property, IPropertyGridControlFactoryCollection collection)
        {
            Model = model;
            this.List = list;
            this.Property = property;
            Collection = collection;
        }
    }
    #endregion
}

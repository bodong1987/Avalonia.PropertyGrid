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
using Avalonia.Interactivity;

namespace Avalonia.PropertyGrid.Controls
{
    public class BindingListEdit : TemplatedControl
    {
        #region Data Properties
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
        #endregion

        #region Commands
        public static readonly DirectProperty<BindingListEdit, ICommand> NewElementCommandProperty =
            AvaloniaProperty.RegisterDirect<BindingListEdit, ICommand>(
                nameof(NewElementCommand),
                o => o.NewElementCommand,
                (o, v) => o.NewElementCommand = v
                );

        ICommand _NewElementCommand;
        public ICommand NewElementCommand
        {
            get => _NewElementCommand;
            set => SetAndRaise(NewElementCommandProperty, ref _NewElementCommand, value);
        }

        public static readonly DirectProperty<BindingListEdit, ICommand> ClearElementsCommandProperty =
            AvaloniaProperty.RegisterDirect<BindingListEdit, ICommand>(
                nameof(ClearElementsCommand),
                o => o.ClearElementsCommand,
                (o, v) => o.ClearElementsCommand = v
                );

        ICommand _ClearElementsCommand;
        public ICommand ClearElementsCommand
        {
            get => _ClearElementsCommand;
            set => SetAndRaise(ClearElementsCommandProperty, ref _ClearElementsCommand, value);
        }
        #endregion

        #region Events
        /// <summary>
        /// The selected items changed event
        /// </summary>
        public static readonly RoutedEvent<BindingListRoutedEventArgs> NewElementEvent =
            RoutedEvent.Register<BindingListEdit, BindingListRoutedEventArgs>(nameof(NewElement), RoutingStrategies.Bubble);

        /// <summary>
        /// Creates new element.
        /// </summary>
        public event EventHandler<BindingListRoutedEventArgs> NewElement
        {
            add => AddHandler(NewElementEvent, value);
            remove => RemoveHandler(NewElementEvent, value);
        }

        /// <summary>
        /// The clear elements event
        /// </summary>
        public static readonly RoutedEvent<BindingListRoutedEventArgs> ClearElementsEvent =
           RoutedEvent.Register<BindingListEdit, BindingListRoutedEventArgs>(nameof(ClearElements), RoutingStrategies.Bubble);

        /// <summary>
        /// Occurs when [clear elements].
        /// </summary>
        public event EventHandler<BindingListRoutedEventArgs> ClearElements
        {
            add => AddHandler(ClearElementsEvent, value);
            remove => RemoveHandler(ClearElementsEvent, value);
        }

        /// <summary>
        /// The insert element event
        /// </summary>
        public static readonly RoutedEvent<BindingListRoutedEventArgs> InsertElementEvent =
            RoutedEvent.Register<BindingListEdit, BindingListRoutedEventArgs>(nameof(InsertElement), RoutingStrategies.Bubble);

        /// <summary>
        /// Occurs when [insert element].
        /// </summary>
        public event EventHandler<BindingListRoutedEventArgs> InsertElement
        {
            add => AddHandler(InsertElementEvent, value);
            remove => RemoveHandler(InsertElementEvent, value);
        }

        /// <summary>
        /// The remove elements event
        /// </summary>
        public static readonly RoutedEvent<BindingListRoutedEventArgs> RemoveElementsEvent =
           RoutedEvent.Register<BindingListEdit, BindingListRoutedEventArgs>(nameof(RemoveElement), RoutingStrategies.Bubble);

        /// <summary>
        /// Occurs when [remove element].
        /// </summary>
        public event EventHandler<BindingListRoutedEventArgs> RemoveElement
        {
            add => AddHandler(RemoveElementsEvent, value);
            remove => RemoveHandler(RemoveElementsEvent, value);
        }

        /// <summary>
        /// The element value changed event
        /// </summary>
        public static readonly RoutedEvent<BindingListRoutedEventArgs> ElementValueChangedEvent =
            RoutedEvent.Register<BindingListEdit, BindingListRoutedEventArgs>(nameof(ElementValueChanged), RoutingStrategies.Bubble);

        /// <summary>
        /// Occurs when [element value changed].
        /// </summary>
        public event EventHandler<BindingListRoutedEventArgs> ElementValueChanged
        {
            add => AddHandler(ElementValueChangedEvent, value);
            remove => RemoveHandler(ElementValueChangedEvent, value);
        }
        #endregion

        internal BindingListViewModel Model { get; private set; }
        
        static BindingListEdit()
        {
            DataListProperty.Changed.Subscribe(OnDataListChanged);
        }

        public BindingListEdit()
        {
            NewElementCommand = ReactiveCommand.Create(HandleNewElement);
            ClearElementsCommand = ReactiveCommand.Create(HandleClearElements);
            
            Model = new BindingListViewModel(ReactiveCommand.Create(HandleInsertElement), ReactiveCommand.Create(HandleRemoveElement));
        }

        private static void OnDataListChanged(AvaloniaPropertyChangedEventArgs<IBindingList> e)
        {
            if(e.Sender is BindingListEdit ble)
            {
                ble.OnDataListChanged(e.OldValue.Value, e.NewValue.Value);
            }
        }

        private void OnDataListChanged(IBindingList previousValue, IBindingList value)
        {
            if(previousValue!=null)
            {
                previousValue.ListChanged -= OnListChanged;
            }

            Model.List = value;

            if(value != null)
            {
                value.ListChanged += OnListChanged;
            }
        }

        private void HandleNewElement(object parameter)
        {
            var et = new BindingListRoutedEventArgs(NewElementEvent, _DataList, -1);
            RaiseEvent(et);
        }

        private void HandleClearElements(object parameter)
        {
            var et = new BindingListRoutedEventArgs(ClearElementsEvent, _DataList, -1);
            RaiseEvent(et);
        }

        private void HandleInsertElement(object parameter)
        {
            if(parameter is BindingListElementDataDesc desc)
            {
                BindingListRoutedEventArgs et = new BindingListRoutedEventArgs(InsertElementEvent, desc.List, desc.Property.Index);
                RaiseEvent(et);
            }            
        }

        private void HandleRemoveElement(object parameter)
        {
            if (parameter is BindingListElementDataDesc desc)
            {
                BindingListRoutedEventArgs et = new BindingListRoutedEventArgs(RemoveElementsEvent, desc.List, desc.Property.Index);
                RaiseEvent(et);
            }                
        }

        private void OnListChanged(object sender, ListChangedEventArgs e)
        {
            if (e.ListChangedType == ListChangedType.Reset ||
                e.ListChangedType == ListChangedType.ItemDeleted ||
                e.ListChangedType == ListChangedType.ItemAdded ||
                e.ListChangedType == ListChangedType.ItemMoved
                )
            {
                // this will force refresh list elements
                Model.RaisePropertyChanged(nameof(Model.List));
            }
            else if (e.ListChangedType == ListChangedType.ItemChanged)
            {
                var et = new BindingListRoutedEventArgs(ElementValueChangedEvent, DataContext as IBindingList, e.NewIndex);
                RaiseEvent(et);
            }
        }
    }

    /// <summary>
    /// Class BindingListRoutedEventArgs.
    /// Implements the <see cref="RoutedEventArgs" />
    /// </summary>
    /// <seealso cref="RoutedEventArgs" />
    public class BindingListRoutedEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// Gets the index.
        /// </summary>
        /// <value>The index.</value>
        public int Index { get; internal set; }

        /// <summary>
        /// Gets the array.
        /// </summary>
        /// <value>The array.</value>
        public IBindingList List { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BindingListRoutedEventArgs" /> class.
        /// </summary>
        /// <param name="routedEvent">The routed event.</param>
        /// <param name="list">The list.</param>
        /// <param name="index">The index.</param>
        public BindingListRoutedEventArgs(RoutedEvent routedEvent, IBindingList list, int index) :
            base(routedEvent)
        {
            List = list;
            Index = index;
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

        public ICommand InsertCommand { get; private set; }
        public ICommand RemoveCommand { get; private set; }

        public BindingListViewModel(ICommand insertCommand, ICommand removeCommand) :
            this(null, insertCommand, removeCommand)
        {
        }

        public BindingListViewModel(IBindingList list, ICommand insertCommand, ICommand removeCommand)
        {            
            InsertCommand = insertCommand;
            RemoveCommand = removeCommand;

            PropertyChanged += OnPropertyChanged;
            List = list;
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
                        BindingListElementDataDesc desc = new BindingListElementDataDesc(
                            this, 
                            list, 
                            pd, 
                            Collection, 
                            InsertCommand, 
                            RemoveCommand
                            );

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
        public readonly BindingListElementPropertyDescriptor Property;
        public readonly IPropertyGridControlFactoryCollection Collection;

        public ICommand InsertCommand { get; set; }
        public ICommand RemoveCommand { get; set; }

        public BindingListViewModel Model { get; set; }

        public string DisplayName => Property.Index.ToString();

        public event EventHandler ValueChanged;

        public BindingListElementDataDesc(
            BindingListViewModel model, 
            IBindingList list,
            BindingListElementPropertyDescriptor property, 
            IPropertyGridControlFactoryCollection collection,
            ICommand insertCommand,
            ICommand removeCommand
            )
        {
            Model = model;
            this.List = list;
            this.Property = property;
            Collection = collection;
            InsertCommand = ReactiveCommand.Create(() => insertCommand.Execute(this));
            RemoveCommand = ReactiveCommand.Create(() => removeCommand.Execute(this));

            list.ListChanged += OnListChanged;
        }

        private void OnListChanged(object sender, ListChangedEventArgs e)
        {
            if(e.ListChangedType != ListChangedType.ItemChanged || e.NewIndex != Property.Index)
            {
                return;
            }

            ValueChanged?.Invoke(this, EventArgs.Empty);
        }
    }
    #endregion
}

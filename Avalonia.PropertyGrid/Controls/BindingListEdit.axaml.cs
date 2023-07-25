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
    /// <summary>
    /// Class BindingListEdit.
    /// Implements the <see cref="TemplatedControl" />
    /// </summary>
    /// <seealso cref="TemplatedControl" />
    public class BindingListEdit : TemplatedControl
    {
        #region Data Properties
        /// <summary>
        /// The data list property
        /// </summary>
        public static readonly DirectProperty<BindingListEdit, IBindingList> DataListProperty =
            AvaloniaProperty.RegisterDirect<BindingListEdit, IBindingList>(
                nameof(DataList),
                o => o.DataList,
                (o, v) => o.DataList = v
                );

        /// <summary>
        /// The data list
        /// </summary>
        IBindingList _DataList;
        /// <summary>
        /// Gets or sets the data list.
        /// </summary>
        /// <value>The data list.</value>
        public IBindingList DataList
        {
            get => _DataList;
            set => SetAndRaise(DataListProperty, ref _DataList, value);
        }
        #endregion

        #region Commands
        /// <summary>
        /// Creates new elementcommandproperty.
        /// </summary>
        public static readonly DirectProperty<BindingListEdit, ICommand> NewElementCommandProperty =
            AvaloniaProperty.RegisterDirect<BindingListEdit, ICommand>(
                nameof(NewElementCommand),
                o => o.NewElementCommand,
                (o, v) => o.NewElementCommand = v
                );

        /// <summary>
        /// The new element command
        /// </summary>
        ICommand _NewElementCommand;
        /// <summary>
        /// Creates new elementcommand.
        /// </summary>
        /// <value>The new element command.</value>
        public ICommand NewElementCommand
        {
            get => _NewElementCommand;
            set => SetAndRaise(NewElementCommandProperty, ref _NewElementCommand, value);
        }

        /// <summary>
        /// The clear elements command property
        /// </summary>
        public static readonly DirectProperty<BindingListEdit, ICommand> ClearElementsCommandProperty =
            AvaloniaProperty.RegisterDirect<BindingListEdit, ICommand>(
                nameof(ClearElementsCommand),
                o => o.ClearElementsCommand,
                (o, v) => o.ClearElementsCommand = v
                );

        /// <summary>
        /// The clear elements command
        /// </summary>
        ICommand _ClearElementsCommand;
        /// <summary>
        /// Gets or sets the clear elements command.
        /// </summary>
        /// <value>The clear elements command.</value>
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

        /// <summary>
        /// Gets the model.
        /// </summary>
        /// <value>The model.</value>
        internal BindingListViewModel Model { get; private set; }

        /// <summary>
        /// Initializes static members of the <see cref="BindingListEdit"/> class.
        /// </summary>
        static BindingListEdit()
        {
            DataListProperty.Changed.Subscribe(OnDataListChanged);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BindingListEdit"/> class.
        /// </summary>
        public BindingListEdit()
        {
            NewElementCommand = ReactiveCommand.Create(HandleNewElement);
            ClearElementsCommand = ReactiveCommand.Create(HandleClearElements);
            
            Model = new BindingListViewModel(ReactiveCommand.Create(HandleInsertElement), ReactiveCommand.Create(HandleRemoveElement));
        }

        /// <summary>
        /// Called when [data list changed].
        /// </summary>
        /// <param name="e">The e.</param>
        private static void OnDataListChanged(AvaloniaPropertyChangedEventArgs<IBindingList> e)
        {
            if(e.Sender is BindingListEdit ble)
            {
                ble.OnDataListChanged(e.OldValue.Value, e.NewValue.Value);
            }
        }

        /// <summary>
        /// Called when [data list changed].
        /// </summary>
        /// <param name="previousValue">The previous value.</param>
        /// <param name="value">The value.</param>
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

        /// <summary>
        /// Handles the new element.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        private void HandleNewElement(object parameter)
        {
            var et = new BindingListRoutedEventArgs(NewElementEvent, _DataList, -1);
            RaiseEvent(et);
        }

        /// <summary>
        /// Handles the clear elements.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        private void HandleClearElements(object parameter)
        {
            var et = new BindingListRoutedEventArgs(ClearElementsEvent, _DataList, -1);
            RaiseEvent(et);
        }

        /// <summary>
        /// Handles the insert element.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        private void HandleInsertElement(object parameter)
        {
            if(parameter is BindingListElementDataDesc desc)
            {
                BindingListRoutedEventArgs et = new BindingListRoutedEventArgs(InsertElementEvent, desc.List, desc.Property.Index);
                RaiseEvent(et);
            }            
        }

        /// <summary>
        /// Handles the remove element.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        private void HandleRemoveElement(object parameter)
        {
            if (parameter is BindingListElementDataDesc desc)
            {
                BindingListRoutedEventArgs et = new BindingListRoutedEventArgs(RemoveElementsEvent, desc.List, desc.Property.Index);
                RaiseEvent(et);
            }                
        }

        /// <summary>
        /// Handles the <see cref="E:ListChanged" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ListChangedEventArgs"/> instance containing the event data.</param>
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
                var et = new BindingListRoutedEventArgs(ElementValueChangedEvent, Model.List, e.NewIndex);
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
    /// <summary>
    /// Class BindingListViewModel.
    /// Implements the <see cref="ReactiveObject" />
    /// </summary>
    /// <seealso cref="ReactiveObject" />
    internal class BindingListViewModel : ReactiveObject
    {
        /// <summary>
        /// Gets or sets the collection.
        /// </summary>
        /// <value>The collection.</value>
        public ICellEditFactoryCollection Collection { get; set; }

        /// <summary>
        /// Gets or sets the root.
        /// </summary>
        /// <value>The root.</value>
        public IPropertyGrid Root { get; set; }

        /// <summary>
        /// The list
        /// </summary>
        IBindingList _List;
        /// <summary>
        /// Gets or sets the list.
        /// </summary>
        /// <value>The list.</value>
        public IBindingList List
        {
            get => _List;
            set => this.RaiseAndSetIfChanged(ref _List, value);
        }

        /// <summary>
        /// The elements
        /// </summary>
        readonly List<BindingListElementDataDesc> _Elements = new List<BindingListElementDataDesc>();

        /// <summary>
        /// Gets the elements.
        /// </summary>
        /// <value>The elements.</value>
        public BindingListElementDataDesc[] Elements => _Elements.ToArray();

        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>The title.</value>
        [DependsOnProperty(nameof(List))]
        public string Title => string.Format(PropertyGrid.LocalizationService["{0} Elements"], _List != null ? _List.Count : 0);

        /// <summary>
        /// Gets the insert command.
        /// </summary>
        /// <value>The insert command.</value>
        public ICommand InsertCommand { get; private set; }
        /// <summary>
        /// Gets the remove command.
        /// </summary>
        /// <value>The remove command.</value>
        public ICommand RemoveCommand { get; private set; }

        /// <summary>
        /// The is editable
        /// </summary>
        bool _IsEditable = true;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is editable.
        /// </summary>
        /// <value><c>true</c> if this instance is editable; otherwise, <c>false</c>.</value>
        public bool IsEditable
        {
            get => _IsEditable;
            set => this.RaiseAndSetIfChanged(ref _IsEditable, value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BindingListViewModel"/> class.
        /// </summary>
        /// <param name="insertCommand">The insert command.</param>
        /// <param name="removeCommand">The remove command.</param>
        public BindingListViewModel(ICommand insertCommand, ICommand removeCommand) :
            this(null, insertCommand, removeCommand)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BindingListViewModel"/> class.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="insertCommand">The insert command.</param>
        /// <param name="removeCommand">The remove command.</param>
        public BindingListViewModel(IBindingList list, ICommand insertCommand, ICommand removeCommand)
        {            
            InsertCommand = insertCommand;
            RemoveCommand = removeCommand;

            PropertyChanged += OnPropertyChanged;
            List = list;
        }

        /// <summary>
        /// Handles the <see cref="E:PropertyChanged" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
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
                            Root, 
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
    /// <summary>
    /// Class BindingListElementDataDesc.
    /// Implements the <see cref="MiniReactiveObject" />
    /// </summary>
    /// <seealso cref="MiniReactiveObject" />
    internal class BindingListElementDataDesc : MiniReactiveObject
    {
        /// <summary>
        /// The list
        /// </summary>
        public readonly IBindingList List;
        /// <summary>
        /// The property
        /// </summary>
        public readonly BindingListElementPropertyDescriptor Property;

        /// <summary>
        /// The root property grid
        /// </summary>
        public readonly IPropertyGrid RootPropertyGrid;

        /// <summary>
        /// Gets or sets the insert command.
        /// </summary>
        /// <value>The insert command.</value>
        public ICommand InsertCommand { get; set; }
        /// <summary>
        /// Gets or sets the remove command.
        /// </summary>
        /// <value>The remove command.</value>
        public ICommand RemoveCommand { get; set; }

        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        /// <value>The model.</value>
        public BindingListViewModel Model { get; set; }

        /// <summary>
        /// Gets the display name.
        /// </summary>
        /// <value>The display name.</value>
        public string DisplayName => Property.Index.ToString();

        /// <summary>
        /// Occurs when [value changed].
        /// </summary>
        public event EventHandler ValueChanged;

        /// <summary>
        /// Gets a value indicating whether this instance is editable.
        /// </summary>
        /// <value><c>true</c> if this instance is editable; otherwise, <c>false</c>.</value>
        public bool IsEditable => Model.IsEditable;

        /// <summary>
        /// Initializes a new instance of the <see cref="BindingListElementDataDesc" /> class.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="list">The list.</param>
        /// <param name="property">The property.</param>
        /// <param name="rootPropertyGrid">The root property grid.</param>
        /// <param name="insertCommand">The insert command.</param>
        /// <param name="removeCommand">The remove command.</param>
        public BindingListElementDataDesc(
            BindingListViewModel model, 
            IBindingList list,
            BindingListElementPropertyDescriptor property,
            IPropertyGrid rootPropertyGrid,
            ICommand insertCommand,
            ICommand removeCommand
            )
        {
            Model = model;
            this.List = list;
            this.Property = property;
            this.RootPropertyGrid = rootPropertyGrid;

            InsertCommand = ReactiveCommand.Create(() => insertCommand.Execute(this));
            RemoveCommand = ReactiveCommand.Create(() => removeCommand.Execute(this));

            list.ListChanged += OnListChanged;

            model.PropertyChanged += OnPropertyChanged;
        }

        /// <summary>
        /// Handles the <see cref="E:PropertyChanged" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(Model.IsEditable))
            {
                RaisePropertyChanged(nameof(IsEditable));
            }
        }

        /// <summary>
        /// Handles the <see cref="E:ListChanged" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ListChangedEventArgs"/> instance containing the event data.</param>
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

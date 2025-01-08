using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.PropertyGrid.Services;
using Avalonia.Reactive;
using PropertyModels.ComponentModel;
using PropertyModels.ComponentModel.DataAnnotations;

namespace Avalonia.PropertyGrid.Controls
{
    /// <summary>
    /// Class ListEdit.
    /// Implements the <see cref="TemplatedControl" />
    /// </summary>
    /// <seealso cref="TemplatedControl" />
    public class ListEdit : TemplatedControl
    {
        #region Data Properties
        /// <summary>
        /// The data list property
        /// </summary>
        public static readonly DirectProperty<ListEdit, IList> DataListProperty =
            AvaloniaProperty.RegisterDirect<ListEdit, IList>(
                nameof(DataList),
                o => o.DataList!,
                (o, v) => o.DataList = v
                );

        /// <summary>
        /// The data list
        /// </summary>
        private IList? _dataList;
        /// <summary>
        /// Gets or sets the data list.
        /// </summary>
        /// <value>The data list.</value>
        public IList? DataList
        {
            get => _dataList;
            set => SetAndRaise(DataListProperty!, ref _dataList, value);
        }
        #endregion

        #region Commands
        /// <summary>
        /// Creates new element command property.
        /// </summary>
        public static readonly DirectProperty<ListEdit, ICommand> NewElementCommandProperty =
            AvaloniaProperty.RegisterDirect<ListEdit, ICommand>(
                nameof(NewElementCommand),
                o => o.NewElementCommand!,
                (o, v) => o.NewElementCommand = v
                );

        /// <summary>
        /// The new element command
        /// </summary>
        private ICommand? _newElementCommand;
        /// <summary>
        /// Creates new element command.
        /// </summary>
        /// <value>The new element command.</value>
        public ICommand? NewElementCommand
        {
            get => _newElementCommand;
            set => SetAndRaise(NewElementCommandProperty!, ref _newElementCommand, value);
        }

        /// <summary>
        /// The clear elements command property
        /// </summary>
        public static readonly DirectProperty<ListEdit, ICommand> ClearElementsCommandProperty =
            AvaloniaProperty.RegisterDirect<ListEdit, ICommand>(
                nameof(ClearElementsCommand),
                o => o.ClearElementsCommand!,
                (o, v) => o.ClearElementsCommand = v
                );

        /// <summary>
        /// The clear elements command
        /// </summary>
        private ICommand? _clearElementsCommand;
        /// <summary>
        /// Gets or sets the clear elements command.
        /// </summary>
        /// <value>The clear elements command.</value>
        public ICommand? ClearElementsCommand
        {
            get => _clearElementsCommand;
            set => SetAndRaise(ClearElementsCommandProperty!, ref _clearElementsCommand, value);
        }
        #endregion

        #region Events
        /// <summary>
        /// The selected items changed event
        /// </summary>
        public static readonly RoutedEvent<ListRoutedEventArgs> NewElementEvent =
            RoutedEvent.Register<ListEdit, ListRoutedEventArgs>(nameof(NewElement), RoutingStrategies.Bubble);

        /// <summary>
        /// Creates new element.
        /// </summary>
        public event EventHandler<ListRoutedEventArgs> NewElement
        {
            add => AddHandler(NewElementEvent, value);
            remove => RemoveHandler(NewElementEvent, value);
        }

        /// <summary>
        /// The clear elements event
        /// </summary>
        public static readonly RoutedEvent<ListRoutedEventArgs> ClearElementsEvent =
           RoutedEvent.Register<ListEdit, ListRoutedEventArgs>(nameof(ClearElements), RoutingStrategies.Bubble);

        /// <summary>
        /// Occurs when [clear elements].
        /// </summary>
        public event EventHandler<ListRoutedEventArgs> ClearElements
        {
            add => AddHandler(ClearElementsEvent, value);
            remove => RemoveHandler(ClearElementsEvent, value);
        }

        /// <summary>
        /// The insert element event
        /// </summary>
        public static readonly RoutedEvent<ListRoutedEventArgs> InsertElementEvent =
            RoutedEvent.Register<ListEdit, ListRoutedEventArgs>(nameof(InsertElement), RoutingStrategies.Bubble);

        /// <summary>
        /// Occurs when [insert element].
        /// </summary>
        public event EventHandler<ListRoutedEventArgs> InsertElement
        {
            add => AddHandler(InsertElementEvent, value);
            remove => RemoveHandler(InsertElementEvent, value);
        }

        /// <summary>
        /// The remove elements event
        /// </summary>
        public static readonly RoutedEvent<ListRoutedEventArgs> RemoveElementsEvent =
           RoutedEvent.Register<ListEdit, ListRoutedEventArgs>(nameof(RemoveElement), RoutingStrategies.Bubble);

        /// <summary>
        /// Occurs when [remove element].
        /// </summary>
        public event EventHandler<ListRoutedEventArgs> RemoveElement
        {
            add => AddHandler(RemoveElementsEvent, value);
            remove => RemoveHandler(RemoveElementsEvent, value);
        }

        /// <summary>
        /// The element value changed event
        /// </summary>
        public static readonly RoutedEvent<ListRoutedEventArgs> ElementValueChangedEvent =
            RoutedEvent.Register<ListEdit, ListRoutedEventArgs>(nameof(ElementValueChanged), RoutingStrategies.Bubble);

        /// <summary>
        /// Occurs when [element value changed].
        /// </summary>
        public event EventHandler<ListRoutedEventArgs> ElementValueChanged
        {
            add => AddHandler(ElementValueChangedEvent, value);
            remove => RemoveHandler(ElementValueChangedEvent, value);
        }
        #endregion

        /// <summary>
        /// Gets the model.
        /// </summary>
        /// <value>The model.</value>
        internal ListViewModel Model { get; }

        /// <summary>
        /// Initializes static members of the <see cref="ListEdit"/> class.
        /// </summary>
        static ListEdit()
        {
            DataListProperty.Changed.Subscribe(new AnonymousObserver<AvaloniaPropertyChangedEventArgs<IList>>(OnDataListChanged));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListEdit"/> class.
        /// </summary>
        public ListEdit()
        {
            NewElementCommand = ReactiveCommand.Create(HandleNewElement);
            ClearElementsCommand = ReactiveCommand.Create(HandleClearElements);
            
            Model = new ListViewModel(ReactiveCommand.Create(HandleInsertElement), ReactiveCommand.Create(HandleRemoveElement));
        }

        /// <summary>
        /// Called when [data list changed].
        /// </summary>
        /// <param name="e">The e.</param>
        private static void OnDataListChanged(AvaloniaPropertyChangedEventArgs<IList> e)
        {
            if(e.Sender is ListEdit ble)
            {
                ble.OnDataListChanged(e.OldValue.Value, e.NewValue.Value);
            }
        }

        /// <summary>
        /// Called when [data list changed].
        /// </summary>
        /// <param name="previousValue">The previous value.</param>
        /// <param name="value">The value.</param>
        private void OnDataListChanged(IList previousValue, IList value)
        {
            if(previousValue is IBindingList pblist)
            {
                pblist.ListChanged -= OnListChanged;
            }

            Model.List = value;

            if(value is IBindingList blist)
            {
                blist.ListChanged += OnListChanged;
            }
        }

        /// <summary>
        /// Handles the new element.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        private void HandleNewElement(object? parameter)
        {
            var et = new ListRoutedEventArgs(NewElementEvent, _dataList, -1);
            RaiseEvent(et);
        }

        /// <summary>
        /// Handles the clear elements.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        private void HandleClearElements(object? parameter)
        {
            var et = new ListRoutedEventArgs(ClearElementsEvent, _dataList, -1);
            RaiseEvent(et);
        }

        /// <summary>
        /// Handles the insert element.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        private void HandleInsertElement(object? parameter)
        {
            if(parameter is ListElementDataDesc desc)
            {
                var et = new ListRoutedEventArgs(InsertElementEvent, desc.List, desc.Property.Index);
                RaiseEvent(et);
            }            
        }

        /// <summary>
        /// Handles the remove element.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        private void HandleRemoveElement(object? parameter)
        {
            if (parameter is ListElementDataDesc desc)
            {
                var et = new ListRoutedEventArgs(RemoveElementsEvent, desc.List, desc.Property.Index);
                RaiseEvent(et);
            }                
        }

        /// <summary>
        /// Handles the <see cref="E:ListChanged" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ListChangedEventArgs"/> instance containing the event data.</param>
        private void OnListChanged(object? sender, ListChangedEventArgs e)
        {
            // ReSharper disable once MergeIntoLogicalPattern
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
                var et = new ListRoutedEventArgs(ElementValueChangedEvent, Model.List, e.NewIndex);
                RaiseEvent(et);
            }
        }
    }

    /// <summary>
    /// Class ListRoutedEventArgs.
    /// Implements the <see cref="RoutedEventArgs" />
    /// </summary>
    /// <seealso cref="RoutedEventArgs" />
    public class ListRoutedEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// Gets the index.
        /// </summary>
        /// <value>The index.</value>
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        public int Index { get; internal set; }

        /// <summary>
        /// Gets the array.
        /// </summary>
        /// <value>The array.</value>
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public IList? List { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListRoutedEventArgs" /> class.
        /// </summary>
        /// <param name="routedEvent">The routed event.</param>
        /// <param name="list">The list.</param>
        /// <param name="index">The index.</param>
        public ListRoutedEventArgs(RoutedEvent routedEvent, IList? list, int index) :
            base(routedEvent)
        {
            List = list;
            Index = index;
        }
    }

    #region Models
    /// <summary>
    /// Class ListViewModel.
    /// Implements the <see cref="ReactiveObject" />
    /// </summary>
    /// <seealso cref="ReactiveObject" />
    internal class ListViewModel : ReactiveObject
    {
        /// <summary>
        /// Gets or sets the collection.
        /// </summary>
        /// <value>The collection.</value>
        public ICellEditFactoryCollection? Collection { get; set; }

        /// <summary>
        /// Gets or sets the property context.
        /// </summary>
        /// <value>The property context.</value>
        public PropertyCellContext? PropertyContext { get; set; }

        /// <summary>
        /// The list
        /// </summary>
        private IList? _list;
        /// <summary>
        /// Gets or sets the list.
        /// </summary>
        /// <value>The list.</value>
        public IList? List
        {
            get => _list;
            set => this.RaiseAndSetIfChanged(ref _list, value);
        }

        /// <summary>
        /// The elements
        /// </summary>
        private readonly List<ListElementDataDesc> _elements = [];

        /// <summary>
        /// Gets the elements.
        /// </summary>
        /// <value>The elements.</value>
        public ListElementDataDesc[] Elements => _elements.ToArray();

        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>The title.</value>
        [DependsOnProperty(nameof(List))]
        public string Title => string.Format(LocalizationService.Default["{0} Elements"], _list?.Count ?? 0);

        /// <summary>
        /// Gets the insert command.
        /// </summary>
        /// <value>The insert command.</value>
        public ICommand InsertCommand { get; }
        /// <summary>
        /// Gets the remove command.
        /// </summary>
        /// <value>The remove command.</value>
        public ICommand RemoveCommand { get; }

        /// <summary>
        /// The is editable
        /// </summary>
        private bool _isEditable = true;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is editable.
        /// </summary>
        /// <value><c>true</c> if this instance is editable; otherwise, <c>false</c>.</value>
        public bool IsEditable
        {
            get => _isEditable;
            set => this.RaiseAndSetIfChanged(ref _isEditable, value);
        }

        private bool _isReadOnly;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is readonly.
        /// </summary>
        /// <value><c>true</c> if this instance is readonly; otherwise, <c>false</c>.</value>
        public bool IsReadOnly
        {
            get => _isReadOnly;
            set => this.RaiseAndSetIfChanged(ref _isReadOnly, value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListViewModel"/> class.
        /// </summary>
        /// <param name="insertCommand">The insert command.</param>
        /// <param name="removeCommand">The remove command.</param>
        public ListViewModel(ICommand insertCommand, ICommand removeCommand) :
            this(null, insertCommand, removeCommand)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListViewModel"/> class.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="insertCommand">The insert command.</param>
        /// <param name="removeCommand">The remove command.</param>
        public ListViewModel(IList? list, ICommand insertCommand, ICommand removeCommand)
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
        private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(List))
            {
                Debug.Assert(Collection != null);

                _elements.Clear();

                if(List != null)
                {
                    var list = List;
                    foreach (var index in Enumerable.Range(0, list.Count))
                    {
                        var pd = new ListElementPropertyDescriptor(
                            index.ToString(), 
                            index, 
                            list[index]?.GetType() ?? list.GetType().GetGenericArguments()[0], 
                            PropertyContext?.Property.Attributes.OfType<Attribute>().ToArray()
                            );

                        var desc = new ListElementDataDesc(
                            this, 
                            list, 
                            pd, 
                            PropertyContext, 
                            InsertCommand, 
                            RemoveCommand
                            );

                        _elements.Add(desc);
                    }
                }

                RaisePropertyChanged(nameof(Elements));                
            }
        }
    }
    /// <summary>
    /// Class ListElementDataDesc.
    /// Implements the <see cref="MiniReactiveObject" />
    /// </summary>
    /// <seealso cref="MiniReactiveObject" />
    internal class ListElementDataDesc : MiniReactiveObject
    {
        /// <summary>
        /// The list
        /// </summary>
        public readonly IList List;

        /// <summary>
        /// The property
        /// </summary>
        public readonly ListElementPropertyDescriptor Property;

        /// <summary>
        /// The root property grid
        /// </summary>
        public readonly PropertyCellContext? Context;

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
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        public ListViewModel Model { get; set; }

        /// <summary>
        /// Gets the display name.
        /// </summary>
        /// <value>The display name.</value>
        public string DisplayName => Property.Index.ToString();

        /// <summary>
        /// Occurs when [value changed].
        /// </summary>
        public event EventHandler? ValueChanged;

        /// <summary>
        /// Gets a value indicating whether this instance is editable.
        /// </summary>
        /// <value><c>true</c> if this instance is editable; otherwise, <c>false</c>.</value>
        public bool IsEditable => Model.IsEditable;

        /// <summary>
        /// Gets a value indicating whether this instance is readonly.
        /// </summary>
        /// <value><c>true</c> if this instance is readonly; otherwise, <c>false</c>.</value>
        public bool IsReadOnly => Model.IsReadOnly;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListElementDataDesc" /> class.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="list">The list.</param>
        /// <param name="property">The property.</param>
        /// <param name="context">The context.</param>
        /// <param name="insertCommand">The insert command.</param>
        /// <param name="removeCommand">The remove command.</param>
        public ListElementDataDesc(
            ListViewModel model, 
            IList list,
            ListElementPropertyDescriptor property,
            PropertyCellContext? context,
            ICommand insertCommand,
            ICommand removeCommand
            )
        {
            Model = model;
            List = list;
            Property = property;
            Context = context;

            InsertCommand = ReactiveCommand.Create(() => insertCommand.Execute(this));
            RemoveCommand = ReactiveCommand.Create(() => removeCommand.Execute(this));

            if(list is IBindingList blist)
            {
                blist.ListChanged += OnListChanged;
            }            

            model.PropertyChanged += OnPropertyChanged;
        }

        /// <summary>
        /// Handles the <see cref="E:PropertyChanged" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(Model.IsEditable))
            {
                RaisePropertyChanged(nameof(IsEditable));
            }
            else if(e.PropertyName == nameof(Model.IsReadOnly))
            {
                RaisePropertyChanged(nameof(IsReadOnly));
            }
        }

        /// <summary>
        /// Handles the <see cref="E:ListChanged" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ListChangedEventArgs"/> instance containing the event data.</param>
        private void OnListChanged(object? sender, ListChangedEventArgs e)
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

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.PropertyGrid.Model.ComponentModel;
using System;
using System.Windows.Input;

namespace Avalonia.PropertyGrid.Controls
{
    public class BindingListElementEdit : TemplatedControl
    {
        public static readonly DirectProperty<BindingListElementEdit, ICommand> InsertCommandProperty =
            AvaloniaProperty.RegisterDirect<BindingListElementEdit, ICommand>(
                nameof(InsertCommand),
                o => o.InsertCommand,
                (o, v) => o.InsertCommand = v);

        ICommand _InsertCommand;
        public ICommand InsertCommand
        {
            get => _InsertCommand;
            set => SetAndRaise(InsertCommandProperty, ref _InsertCommand, value);
        }

        public static readonly DirectProperty<BindingListElementEdit, ICommand> RemoveCommandProperty =
            AvaloniaProperty.RegisterDirect<BindingListElementEdit, ICommand>(
                nameof(RemoveCommand),
                o => o.RemoveCommand,
                (o, v) => o.RemoveCommand = v);

        ICommand _RemoveCommand;
        public ICommand RemoveCommand
        {
            get => _RemoveCommand;
            set => SetAndRaise(RemoveCommandProperty, ref _RemoveCommand, value);
        }

        public BindingListElementEdit()
        {
            InsertCommand = ReactiveCommand.Create(OnInsert);
            RemoveCommand = ReactiveCommand.Create(OnRemove);
        }

        private void OnInsert()
        {
        }

        private void OnRemove()
        {

        }
    }
}

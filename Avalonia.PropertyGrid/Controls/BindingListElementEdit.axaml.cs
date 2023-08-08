using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using PropertyModels.ComponentModel;
using System;
using System.Windows.Input;

namespace Avalonia.PropertyGrid.Controls
{
    /// <summary>
    /// Class BindingListElementEdit.
    /// Implements the <see cref="TemplatedControl" />
    /// </summary>
    /// <seealso cref="TemplatedControl" />
    public class BindingListElementEdit : TemplatedControl
    {
        /// <summary>
        /// The insert command property
        /// </summary>
        public static readonly DirectProperty<BindingListElementEdit, ICommand> InsertCommandProperty =
            AvaloniaProperty.RegisterDirect<BindingListElementEdit, ICommand>(
                nameof(InsertCommand),
                o => o.InsertCommand,
                (o, v) => o.InsertCommand = v);

        ICommand _InsertCommand;
        /// <summary>
        /// Gets or sets the insert command.
        /// </summary>
        /// <value>The insert command.</value>
        public ICommand InsertCommand
        {
            get => _InsertCommand;
            set => SetAndRaise(InsertCommandProperty, ref _InsertCommand, value);
        }

        /// <summary>
        /// The remove command property
        /// </summary>
        public static readonly DirectProperty<BindingListElementEdit, ICommand> RemoveCommandProperty =
            AvaloniaProperty.RegisterDirect<BindingListElementEdit, ICommand>(
                nameof(RemoveCommand),
                o => o.RemoveCommand,
                (o, v) => o.RemoveCommand = v);

        ICommand _RemoveCommand;
        /// <summary>
        /// Gets or sets the remove command.
        /// </summary>
        /// <value>The remove command.</value>
        public ICommand RemoveCommand
        {
            get => _RemoveCommand;
            set => SetAndRaise(RemoveCommandProperty, ref _RemoveCommand, value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BindingListElementEdit"/> class.
        /// </summary>
        public BindingListElementEdit()
        {
        }
    }
}

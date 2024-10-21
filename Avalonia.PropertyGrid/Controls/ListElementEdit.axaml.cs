using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using PropertyModels.ComponentModel;
using System;
using System.Windows.Input;

namespace Avalonia.PropertyGrid.Controls
{
    /// <summary>
    /// Class ListElementEdit.
    /// Implements the <see cref="TemplatedControl" />
    /// </summary>
    /// <seealso cref="TemplatedControl" />
    public class ListElementEdit : TemplatedControl
    {
        /// <summary>
        /// The insert command property
        /// </summary>
        public static readonly DirectProperty<ListElementEdit, ICommand> InsertCommandProperty =
            AvaloniaProperty.RegisterDirect<ListElementEdit, ICommand>(
                nameof(InsertCommand),
                o => o.InsertCommand!,
                (o, v) => o.InsertCommand = v);

        ICommand? _InsertCommand;

        /// <summary>
        /// Gets or sets the insert command.
        /// </summary>
        /// <value>The insert command.</value>
        public ICommand? InsertCommand
        {
            get => _InsertCommand;
            set => SetAndRaise(InsertCommandProperty!, ref _InsertCommand, value);
        }

        /// <summary>
        /// The remove command property
        /// </summary>
        public static readonly DirectProperty<ListElementEdit, ICommand> RemoveCommandProperty =
            AvaloniaProperty.RegisterDirect<ListElementEdit, ICommand>(
                nameof(RemoveCommand),
                o => o.RemoveCommand!,
                (o, v) => o.RemoveCommand = v);

        ICommand? _RemoveCommand;

        /// <summary>
        /// Gets or sets the remove command.
        /// </summary>
        /// <value>The remove command.</value>
        public ICommand? RemoveCommand
        {
            get => _RemoveCommand;
            set => SetAndRaise(RemoveCommandProperty!, ref _RemoveCommand, value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListElementEdit"/> class.
        /// </summary>
        public ListElementEdit()
        {
        }
    }
}

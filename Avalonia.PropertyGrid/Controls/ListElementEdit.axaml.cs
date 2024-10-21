using Avalonia.Controls.Primitives;
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

        private ICommand? _insertCommand;

        /// <summary>
        /// Gets or sets the insert command.
        /// </summary>
        /// <value>The insert command.</value>
        public ICommand? InsertCommand
        {
            get => _insertCommand;
            set => SetAndRaise(InsertCommandProperty!, ref _insertCommand, value);
        }

        /// <summary>
        /// The remove command property
        /// </summary>
        public static readonly DirectProperty<ListElementEdit, ICommand> RemoveCommandProperty =
            AvaloniaProperty.RegisterDirect<ListElementEdit, ICommand>(
                nameof(RemoveCommand),
                o => o.RemoveCommand!,
                (o, v) => o.RemoveCommand = v);

        private ICommand? _removeCommand;

        /// <summary>
        /// Gets or sets the remove command.
        /// </summary>
        /// <value>The remove command.</value>
        public ICommand? RemoveCommand
        {
            get => _removeCommand;
            set => SetAndRaise(RemoveCommandProperty!, ref _removeCommand, value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListElementEdit"/> class.
        /// </summary>
        public ListElementEdit()
        {
        }
    }
}

using System.Windows.Input;
using Avalonia.Controls.Primitives;

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

        /// <summary>
        /// Gets or sets the insert command.
        /// </summary>
        /// <value>The insert command.</value>
        public ICommand? InsertCommand
        {
            get;
            set => SetAndRaise(InsertCommandProperty!, ref field, value);
        }

        /// <summary>
        /// The remove command property
        /// </summary>
        public static readonly DirectProperty<ListElementEdit, ICommand> RemoveCommandProperty =
            AvaloniaProperty.RegisterDirect<ListElementEdit, ICommand>(
                nameof(RemoveCommand),
                o => o.RemoveCommand!,
                (o, v) => o.RemoveCommand = v);

        /// <summary>
        /// Gets or sets the remove command.
        /// </summary>
        /// <value>The remove command.</value>
        public ICommand? RemoveCommand
        {
            get;
            set => SetAndRaise(RemoveCommandProperty!, ref field, value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListElementEdit"/> class.
        /// </summary>
        public ListElementEdit()
        {
        }
    }
}

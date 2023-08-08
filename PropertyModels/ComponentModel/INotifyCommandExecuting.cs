using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace PropertyModels.ComponentModel
{
    /// <summary>
    /// Interface INotifyCommandExecuting
    /// </summary>
    public interface INotifyCommandExecuting
    {
        /// <summary>
        /// Occurs when [command executing].
        /// </summary>
        event EventHandler<CommandExecutingEventArgs> CommandExecuting;

        /// <summary>
        /// Raises the command executing.
        /// </summary>
        /// <param name="eventArgs">The <see cref="CommandExecutingEventArgs"/> instance containing the event data.</param>
        void RaiseCommandExecuting(CommandExecutingEventArgs eventArgs);
    }

    /// <summary>
    /// Class CommandExecutingEventArgs.
    /// Implements the <see cref="CancelEventArgs" />
    /// </summary>
    /// <seealso cref="CancelEventArgs" />
    public class CommandExecutingEventArgs : CancelEventArgs
    {
        /// <summary>
        /// The command
        /// </summary>
        public readonly ICancelableCommand Command;

        /// <summary>
        /// The target
        /// </summary>
        public readonly Object Target;

        /// <summary>
        /// The property
        /// </summary>
        public readonly PropertyDescriptor Property;

        /// <summary>
        /// The old value
        /// </summary>
        public readonly object OldValue;

        /// <summary>
        /// Creates new value.
        /// </summary>
        public readonly object NewValue;

        /// <summary>
        /// The context
        /// </summary>
        public readonly object Context;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandExecutingEventArgs" /> class.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="target">The target.</param>
        /// <param name="property">The property.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        /// <param name="context">The context.</param>
        public CommandExecutingEventArgs(ICancelableCommand command, object target, PropertyDescriptor property, object oldValue, object newValue, object context)
        {
            Command = command;
            Target = target;
            Property = property;
            OldValue = oldValue;
            NewValue = newValue;
            Context = context;
        }
    }
}

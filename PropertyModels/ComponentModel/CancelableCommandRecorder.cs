using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace PropertyModels.ComponentModel
{
    /// <summary>
    /// Delegate OnCommandCanceledEventHandler
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="command">The in command.</param>
    public delegate void CommandCanceledEventHandler(object sender, ICancelableCommand command);
    /// <summary>
    /// Delegate OnCommandRedoEventHandler
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="command">The in command.</param>
    public delegate void CommandRedoEventHandler(object sender, ICancelableCommand command);
    /// <summary>
    /// Delegate OnNewCommandAddedEventHandler
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="command">The in command.</param>
    public delegate void NewCommandAddedEventHandler(object sender, ICancelableCommand command);


    /// <summary>
    /// Class CancelableCommandRecorder.
    /// </summary>
    public class CancelableCommandRecorder
    {
        /// <summary>
        /// Gets or sets the maximum command.
        /// </summary>
        /// <value>The maximum command.</value>
        public int MaxCommand { get; set; } = 20;

        /// <summary>
        /// The command queue
        /// </summary>
        protected readonly List<ICancelableCommand> CommandQueue = [];

        /// <summary>
        /// The canceled queue
        /// </summary>
        protected readonly List<ICancelableCommand> CanceledQueue = [];

        /// <summary>
        /// Occurs when [on command canceled].
        /// </summary>
        [Browsable(true)]
        public event CommandCanceledEventHandler? OnCommandCanceled;

        /// <summary>
        /// Occurs when [on command redo].
        /// </summary>
        [Browsable(true)]
        public event CommandRedoEventHandler? OnCommandRedo;

        /// <summary>
        /// Occurs when [on new command added].
        /// </summary>
        [Browsable(true)]
        public event NewCommandAddedEventHandler? OnNewCommandAdded;

        /// <summary>
        /// Occurs when [on command cleared].
        /// </summary>
        [Browsable(true)]
        public event EventHandler? OnCommandCleared;

        // just push
        /// <summary>
        /// Pushes the command.
        /// </summary>
        /// <param name="command">The in command.</param>
        /// <returns><c>true</c> if success, <c>false</c> otherwise.</returns>
        public virtual bool PushCommand(ICancelableCommand command)
        {
            // Debug.Assert(command != null);

            CommandQueue.Add(command);

            // clear redo queue
            CanceledQueue.Clear();

            OnNewCommandAdded?.Invoke(this, command);

            return true;
        }

        // push and execute
        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="command">The in command.</param>
        /// <returns><c>true</c> if success, <c>false</c> otherwise.</returns>
        public virtual bool ExecuteCommand(ICancelableCommand command)
        {
            // Debug.Assert(command != null);

            if (!command.Execute())
            {
                return false;
            }

            CommandQueue.Add(command);

            // clear redo queue
            CanceledQueue.Clear();

            OnNewCommandAdded?.Invoke(this, command);

            return true;
        }

        /// <summary>
        /// Undoes this instance.
        /// </summary>
        /// <returns><c>true</c> if success, <c>false</c> otherwise.</returns>
        public virtual bool Undo()
        {
            if (!CanUndo)
            {
                return false;
            }

            var command = CommandQueue.Last();

            command.Cancel();

            // remove last one...
            CommandQueue.RemoveAt(CommandQueue.Count - 1);

            CanceledQueue.Add(command);

            OnCommandCanceled?.Invoke(this, command);

            return true;
        }

        /// <summary>
        /// Gets the undo command description.
        /// </summary>
        /// <value>The undo command description.</value>
        public virtual string UndoCommandDescription => CommandQueue.Count > 0 ? CommandQueue.Last().Name : "No Command";

        /// <summary>
        /// Redoes this instance.
        /// </summary>
        /// <returns><c>true</c> if success, <c>false</c> otherwise.</returns>
        public virtual bool Redo()
        {
            if (!CanRedo)
            {
                return false;
            }

            var command = CanceledQueue.Last();

            CanceledQueue.RemoveAt(CanceledQueue.Count - 1);

            if (!command.Execute())
            {
                return false;
            }

            CommandQueue.Add(command);

            OnCommandRedo?.Invoke(this, command);

            return true;
        }

        /// <summary>
        /// Gets the redo command description.
        /// </summary>
        /// <value>The redo command description.</value>
        public virtual string RedoCommandDescription => CanceledQueue.Count > 0 ? CanceledQueue.Last().Name : "No Command";

        /// <summary>
        /// Gets a value indicating whether this <see cref="CancelableCommandRecorder"/> can redo.
        /// </summary>
        /// <value><c>true</c> can redo; otherwise, <c>false</c>.</value>
        public bool CanRedo => CanceledQueue.Count > 0 && CanceledQueue.Last().CanExecute();

        /// <summary>
        /// Gets a value indicating whether this <see cref="CancelableCommandRecorder"/> is undoable.
        /// </summary>
        /// <value><c>true</c> can undo; otherwise, <c>false</c>.</value>
        public bool CanUndo => CommandQueue.Count > 0 && CommandQueue.Last().CanCancel();

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public virtual void Clear()
        {
            CommandQueue.Clear();
            CanceledQueue.Clear();

            OnCommandCleared?.Invoke(this, EventArgs.Empty);
        }
    }
}

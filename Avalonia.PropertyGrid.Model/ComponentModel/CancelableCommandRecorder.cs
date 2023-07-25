using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Avalonia.PropertyGrid.Model.ComponentModel
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
        protected List<ICancelableCommand> CommandQueue = new List<ICancelableCommand>();
        /// <summary>
        /// The canceled queue
        /// </summary>
        protected List<ICancelableCommand> CanceledQueue = new List<ICancelableCommand>();

        /// <summary>
        /// Occurs when [on command canceled].
        /// </summary>
        [Browsable(true)]
        public event CommandCanceledEventHandler OnCommandCanceled;

        /// <summary>
        /// Occurs when [on command redo].
        /// </summary>
        [Browsable(true)]
        public event CommandRedoEventHandler OnCommandRedo;

        /// <summary>
        /// Occurs when [on new command added].
        /// </summary>
        [Browsable(true)]
        public event NewCommandAddedEventHandler OnNewCommandAdded;

        /// <summary>
        /// Occurs when [on command cleared].
        /// </summary>
        [Browsable(true)]
        public event EventHandler OnCommandCleared;

        // just push
        /// <summary>
        /// Pushes the command.
        /// </summary>
        /// <param name="command">The in command.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public virtual bool PushCommand(ICancelableCommand command)
        {
            Debug.Assert(command != null);

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
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public virtual bool ExecuteCommand(ICancelableCommand command)
        {
            Debug.Assert(command != null);

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
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public virtual bool Undo()
        {
            if (!Undoable)
            {
                return false;
            }

            ICancelableCommand Command = CommandQueue.Last();

            Command.Cancel();

            // remove last one...
            CommandQueue.RemoveAt(CommandQueue.Count - 1);

            CanceledQueue.Add(Command);

            OnCommandCanceled?.Invoke(this, Command);

            return true;
        }

        /// <summary>
        /// Gets the undo command description.
        /// </summary>
        /// <value>The undo command description.</value>
        public virtual string UndoCommandDescription
        {
            get
            {
                if (CommandQueue.Count > 0)
                {
                    return CommandQueue.Last().Name;
                }

                return "No Command";
            }
        }

        /// <summary>
        /// Redoes this instance.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public virtual bool Redo()
        {
            if (!Redoable)
            {
                return false;
            }

            ICancelableCommand Command = CanceledQueue.Last();

            CanceledQueue.RemoveAt(CanceledQueue.Count - 1);

            if (!Command.Execute())
            {
                return false;
            }

            CommandQueue.Add(Command);

            OnCommandRedo?.Invoke(this, Command);

            return true;
        }

        /// <summary>
        /// Gets the redo command description.
        /// </summary>
        /// <value>The redo command description.</value>
        public virtual string RedoCommandDescription
        {
            get
            {
                if (CanceledQueue.Count > 0)
                {
                    return CanceledQueue.Last().Name;
                }

                return "No Command";
            }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="CancelableCommandRecorder"/> is redoable.
        /// </summary>
        /// <value><c>true</c> if redoable; otherwise, <c>false</c>.</value>
        public bool Redoable
        {
            get
            {
                return CanceledQueue.Count > 0 && CanceledQueue.Last().CanExecute();
            }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="CancelableCommandRecorder"/> is undoable.
        /// </summary>
        /// <value><c>true</c> if undoable; otherwise, <c>false</c>.</value>
        public bool Undoable
        {
            get
            {
                return CommandQueue.Count > 0 && CommandQueue.Last().CanCancel();
            }
        }

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

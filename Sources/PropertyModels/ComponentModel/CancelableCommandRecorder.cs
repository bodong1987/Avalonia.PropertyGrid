using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace PropertyModels.ComponentModel;

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
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
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
        
    /// <summary>
    /// Gets the redo queue as a read-only collection.
    /// </summary>
    /// <returns>A read-only collection of redo commands.</returns>
    public ReadOnlyCollection<ICancelableCommand> GetRedoQueue()
    {
        return CommandQueue.AsReadOnly();
    }

    /// <summary>
    /// Gets the undo queue as a read-only collection.
    /// </summary>
    /// <returns>A read-only collection of undo commands.</returns>
    public ReadOnlyCollection<ICancelableCommand> GetUndoQueue()
    {
        return CanceledQueue.AsReadOnly();
    }

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

        _ = command.Cancel();

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

/// <summary>
/// ViewModel for managing command history with undo and redo functionality.
/// </summary>
public class CommandHistoryViewModel : MiniReactiveObject
{
    private readonly CancelableCommandRecorder _recorder = new();

    /// <summary>
    /// Gets a value indicating whether this instance can undo.
    /// </summary>
    [Browsable(false)]
    public bool CanUndo => _recorder.CanUndo;

    /// <summary>
    /// Gets a value indicating whether this instance can redo.
    /// </summary>
    [Browsable(false)]
    public bool CanRedo => _recorder.CanRedo;

    /// <summary>
    /// Gets the description of the undo command.
    /// </summary>
    [Browsable(false)]
    public string UndoDescription => _recorder.UndoCommandDescription;

    /// <summary>
    /// Gets the description of the redo command.
    /// </summary>
    [Browsable(false)]
    public string RedoDescription => _recorder.RedoCommandDescription;

    /// <summary>
    /// Gets or sets the undo command.
    /// </summary>
    [Browsable(false)]
    public ICommand UndoCommand { get; set; }

    /// <summary>
    /// Gets or sets the redo command.
    /// </summary>
    [Browsable(false)]
    public ICommand RedoCommand { get; set; }

    /// <summary>
    /// Gets or sets the clear command.
    /// </summary>
    [Browsable(false)]
    public ICommand ClearCommand { get; set; }

    /// <summary>
    /// Gets the undo queue as a read-only collection.
    /// </summary>
    [Browsable(false)] 
    public ReadOnlyCollection<ICancelableCommand> UndoQueue => _recorder.GetUndoQueue();
        
    /// <summary>
    /// Gets the redo queue as a read-only collection.
    /// </summary>
    [Browsable(false)]
    public ReadOnlyCollection<ICancelableCommand> RedoQueue => _recorder.GetRedoQueue();

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandHistoryViewModel"/> class.
    /// </summary>
    public CommandHistoryViewModel()
    {
        UndoCommand = ReactiveCommand.Create(() => _recorder.Undo());
        RedoCommand = ReactiveCommand.Create(() => _recorder.Redo());
        ClearCommand = ReactiveCommand.Create(_recorder.Clear);

        _recorder.OnNewCommandAdded += (s, e) => RefreshFlags();
        _recorder.OnCommandCanceled += (s, e) => RefreshFlags();
        _recorder.OnCommandCleared += (s, e) => RefreshFlags();
        _recorder.OnCommandRedo += (s, e) => RefreshFlags();
    }
        
    /// <summary>
    /// Refreshes the flags and raises property changed notifications.
    /// </summary>
    private void RefreshFlags()
    {
        RaisePropertyChanged(nameof(CanUndo));
        RaisePropertyChanged(nameof(CanRedo));
        RaisePropertyChanged(nameof(UndoDescription));
        RaisePropertyChanged(nameof(RedoDescription));
        RaisePropertyChanged(nameof(UndoQueue));
        RaisePropertyChanged(nameof(RedoQueue));
    }
}
using System;
using System.Windows.Input;

namespace PropertyModels.ComponentModel;

/// <summary>
/// Class ReactiveCommand.
/// Implements the <see cref="ICommand" />
/// </summary>
/// <seealso cref="ICommand" />
public class ReactiveCommand : ICommand
{
    /// <summary>
    /// Occurs when changes occur that can execute state changed.
    /// </summary>
    public event EventHandler? CanExecuteChanged;

    /// <summary>
    /// The can execute delegate
    /// </summary>
    public Func<object?, bool>? CanExecuteDelegate;

    /// <summary>
    /// The execute delegate
    /// </summary>
    public Action<object?>? ExecuteDelegate;

    /// <summary>
    /// The execute delegate no parameter
    /// </summary>
    public Action? ExecuteDelegateNoParam;

    // ReSharper disable once UnusedMember.Local
    private void OnCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

    /// <summary>
    /// Defines the method that determines whether the command can execute in its current state.
    /// </summary>
    /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to <see langword="null" />.</param>
    /// <returns><see langword="true" /> if this command can be executed; otherwise, <see langword="false" />.</returns>
    public bool CanExecute(object? parameter) => CanExecuteDelegate == null || CanExecuteDelegate(parameter);

    /// <summary>
    /// Defines the method to be called when the command is invoked.
    /// </summary>
    /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to <see langword="null" />.</param>
    public void Execute(object? parameter)
    {
        ExecuteDelegate?.Invoke(parameter);
        ExecuteDelegateNoParam?.Invoke();
    }

    #region Factory
    /// <summary>
    /// Creates the specified action.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <returns>ICommand.</returns>
    public static ICommand Create(Action<object?>? action)
    {
        return new ReactiveCommand
        {
            ExecuteDelegate = action
        };
    }

    /// <summary>
    /// Creates the specified action.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <returns>ICommand.</returns>
    public static ICommand Create(Action action)
    {
        return new ReactiveCommand
        {
            ExecuteDelegateNoParam = action
        };
    }

    /// <summary>
    /// Creates the specified action.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <param name="canExecute">The can execute.</param>
    /// <returns>ICommand.</returns>
    public static ICommand Create(Action<object?>? action, Func<object?, bool>? canExecute)
    {
        return new ReactiveCommand
        {
            ExecuteDelegate = action,
            CanExecuteDelegate = canExecute
        };
    }

    /// <summary>
    /// Creates the specified action.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <param name="canExecute">The can execute.</param>
    /// <returns>ICommand.</returns>
    public static ICommand Create(Action action, Func<object?, bool> canExecute)
    {
        return new ReactiveCommand
        {
            ExecuteDelegateNoParam = action,
            CanExecuteDelegate = canExecute
        };
    }
    #endregion
}
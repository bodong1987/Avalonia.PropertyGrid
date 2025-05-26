using Avalonia.Interactivity;
using Avalonia.PropertyGrid.Controls;
using PropertyModels.ComponentModel;

namespace Avalonia.PropertyGrid.Utils;

/// <summary>
/// execute command helper
/// </summary>
public static class CommandExecuteUtils
{
    /// <summary>
    /// Executes the command.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="propertyContext">The property context.</param>
    /// <param name="oldValue">The old value.</param>
    /// <param name="value">The value.</param>
    /// <param name="context">The context.</param>
    /// <returns><c>true</c> if success, <c>false</c> otherwise.</returns>
    public static bool ExecuteCommand(ICancelableCommand command, PropertyCellContext propertyContext, object? oldValue,
        object? value, object? context)
    {
        var evt = new RoutedCommandExecutingEventArgs(
            Controls.PropertyGrid.CommandExecutingEvent,
            command,
            propertyContext.Target,
            propertyContext.Property,
            oldValue,
            value,
            context);

        (propertyContext.Owner as Interactive)?.RaiseEvent(evt);

        if (evt.Canceled)
        {
            return false;
        }

        if (propertyContext.Target is INotifyCommandExecuting nce)
        {
            var args = new CommandExecutingEventArgs(command, propertyContext.Target, propertyContext.Property, oldValue, value, context);

            nce.RaiseCommandExecuting(args);

            if (args.Cancel)
            {
                return false;
            }
        }

        if (!command.Execute())
        {
            return false;
        }

        var evt2 = new RoutedCommandExecutedEventArgs(
            Controls.PropertyGrid.CommandExecutedEvent,
            command,
            propertyContext.Target,
            propertyContext.Property,
            oldValue,
            value,
            context);

        (propertyContext.Owner as Interactive)?.RaiseEvent(evt2);

        return true;
    }
}
using Avalonia.Interactivity;
using System;

namespace Avalonia.PropertyGrid.Controls;

partial class PropertyGrid
{
    /// <summary>
    /// The custom property descriptor filter event
    /// </summary>
    public static readonly RoutedEvent<CustomPropertyDescriptorFilterEventArgs> CustomPropertyDescriptorFilterEvent =
        RoutedEvent.Register<PropertyGrid, CustomPropertyDescriptorFilterEventArgs>(nameof(CustomPropertyDescriptorFilter), RoutingStrategies.Bubble);

    /// <summary>
    /// Occurs when [custom property descriptor filter].
    /// </summary>
    public event EventHandler<CustomPropertyDescriptorFilterEventArgs> CustomPropertyDescriptorFilter
    {
        add => AddHandler(CustomPropertyDescriptorFilterEvent, value);
        remove => RemoveHandler(CustomPropertyDescriptorFilterEvent, value);
    }

    /// <summary>
    /// The command executing event
    /// </summary>
    public static readonly RoutedEvent<RoutedCommandExecutingEventArgs> CommandExecutingEvent =
        RoutedEvent.Register<PropertyGrid, RoutedCommandExecutingEventArgs>(nameof(CommandExecuting), RoutingStrategies.Bubble);

    /// <summary>
    /// Occurs when [command executing].
    /// </summary>
    public event EventHandler<RoutedCommandExecutingEventArgs> CommandExecuting
    {
        add => AddHandler(CommandExecutingEvent, value);
        remove => RemoveHandler(CommandExecutingEvent, value);
    }

    /// <summary>
    /// The command executed event
    /// </summary>
    public static readonly RoutedEvent<RoutedCommandExecutedEventArgs> CommandExecutedEvent =
        RoutedEvent.Register<PropertyGrid, RoutedCommandExecutedEventArgs>(nameof(CommandExecuted), RoutingStrategies.Bubble);

    /// <summary>
    /// Occurs when [command executed].
    /// </summary>
    public event EventHandler<RoutedCommandExecutedEventArgs> CommandExecuted
    {
        add => AddHandler(CommandExecutedEvent, value);
        remove => RemoveHandler(CommandExecutedEvent, value);
    }
}
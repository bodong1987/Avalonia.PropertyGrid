using System;

namespace Avalonia.PropertyGrid.Utils;

/// <inheritDoc />
public class PropertyObserver<T> : IObserver<T>
{
    private readonly Action<T> _action;

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyObserver{T}"/> class.
    /// </summary>
    /// <param name="action">The <see cref="Action{T}"/> to perform.</param>
    public PropertyObserver(Action<T> action)
    {
        _action = action;
    }

    /// <inheritDoc />
    public void OnNext(T value) => _action(value);

    /// <inheritDoc />
    public void OnError(Exception error) { }

    /// <inheritDoc />
    public void OnCompleted() { }
}
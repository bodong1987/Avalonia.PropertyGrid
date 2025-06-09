using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace PropertyModels.ComponentModel;

/// <summary>
/// Class CheckedMaskModel.
/// </summary>
public class CheckedMaskModel : MiniReactiveObject, ICheckedMaskModel
{
    /// <summary>
    /// The masks
    /// </summary>
    private readonly HashSet<string> _masks = [];

    /// <summary>
    /// Gets the masks.
    /// </summary>
    /// <value>The masks.</value>
    public string[] Masks => [.. _masks.OrderBy(x => x)];

    /// <summary>
    /// Gets all.
    /// </summary>
    /// <value>All.</value>
    public string All { get; }

    /// <summary>
    /// Gets a value indicating whether this instance is all checked.
    /// </summary>
    /// <value><c>true</c> if this instance is all checked; otherwise, <c>false</c>.</value>
    public bool IsAllChecked { get; private set; } = true;

    /// <summary>
    /// The checked values
    /// </summary>
    public readonly HashSet<string> CheckedValues = [];

    /// <summary>
    /// Occurs when [check changed].
    /// </summary>
    public event EventHandler? CheckChanged;

    private bool _isUpdating;
    private bool _isDirty;

    /// <summary>
    /// Initializes a new instance of the <see cref="CheckedMaskModel"/> class.
    /// </summary>
    /// <param name="masks">The masks.</param>
    /// <param name="all">All.</param>
    public CheckedMaskModel(IEnumerable<string> masks, string all)
    {
        All = all;

        foreach (var mask in masks)
        {
            _ = _masks.Add(mask);
        }
    }

    /// <summary>
    /// Determines whether the specified mask is checked.
    /// </summary>
    /// <param name="mask">The mask.</param>
    /// <returns><c>true</c> if the specified mask is checked; otherwise, <c>false</c>.</returns>
    public bool IsChecked(string mask) => IsAllChecked || CheckedValues.Contains(mask);

    /// <summary>
    /// Begins the update.
    /// </summary>
    public void BeginUpdate()
    {
        Debug.Assert(!_isUpdating);

        _isUpdating = true;
        _isDirty = false;
    }

    /// <summary>
    /// Ends the update.
    /// </summary>
    public void EndUpdate()
    {
        Debug.Assert(_isUpdating);

        _isUpdating = false;

        if (_isDirty)
        {
            CheckChanged?.Invoke(this, EventArgs.Empty);
            _isDirty = false;
        }
    }

    private void RaiseChangedEvent()
    {
        if (!_isUpdating)
        {
            CheckChanged?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            _isDirty = true;
        }
    }

    /// <summary>
    /// Checks the specified mask.
    /// </summary>
    /// <param name="mask">The mask.</param>
    public void Check(string mask)
    {
        if (mask == All)
        {
            IsAllChecked = true;

            CheckedValues.Clear();

            RaiseChangedEvent();

            return;
        }

        if (CheckedValues.Add(mask))
        {
            RaiseChangedEvent();
        }
    }

    /// <summary>
    /// Uns the check.
    /// </summary>
    /// <param name="mask">The mask.</param>
    public void UnCheck(string mask)
    {
        if (mask == All)
        {
            IsAllChecked = false;

            RaiseChangedEvent();
            return;
        }

        if (CheckedValues.Contains(mask))
        {
            _ = CheckedValues.Remove(mask);

            RaiseChangedEvent();
        }
    }
}
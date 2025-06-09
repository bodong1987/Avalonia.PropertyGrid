using System;

namespace PropertyModels.ComponentModel;

/// <summary>
/// Interface ICheckedMaskModel
/// Implements the <see cref="PropertyModels.ComponentModel.IReactiveObject" />
/// </summary>
/// <seealso cref="PropertyModels.ComponentModel.IReactiveObject" />
public interface ICheckedMaskModel : IReactiveObject
{
    /// <summary>
    /// Gets a value indicating whether this instance is all checked.
    /// </summary>
    /// <value><c>true</c> if this instance is all checked; otherwise, <c>false</c>.</value>
    bool IsAllChecked { get; }

    /// <summary>
    /// Gets all.
    /// </summary>
    /// <value>All.</value>
    string All { get; }

    /// <summary>
    /// Gets the masks.
    /// </summary>
    /// <value>The masks.</value>
    string[] Masks { get; }

    /// <summary>
    /// Occurs when [check changed].
    /// </summary>
    event EventHandler CheckChanged;

    /// <summary>
    /// Determines whether the specified mask is checked.
    /// </summary>
    /// <param name="mask">The mask.</param>
    /// <returns><c>true</c> if the specified mask is checked; otherwise, <c>false</c>.</returns>
    bool IsChecked(string mask);

    /// <summary>
    /// Checks the specified mask.
    /// </summary>
    /// <param name="mask">The mask.</param>
    void Check(string mask);

    /// <summary>
    /// Uns the check.
    /// </summary>
    /// <param name="mask">The mask.</param>
    void UnCheck(string mask);
}
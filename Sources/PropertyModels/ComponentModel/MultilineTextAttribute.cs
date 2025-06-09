using System;

namespace PropertyModels.ComponentModel;

/// <summary>
/// Class MultilineTextAttribute.
/// Implements the <see cref="Attribute" />
/// </summary>
/// <seealso cref="Attribute" />
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class MultilineTextAttribute : Attribute
{
    /// <summary>
    /// The is multiline
    /// </summary>
    public readonly bool IsMultiline;

    /// <summary>
    /// Initializes a new instance of the <see cref="MultilineTextAttribute"/> class.
    /// </summary>
    /// <param name="isMultiline">if set to <c>true</c> [is multiline].</param>
    public MultilineTextAttribute(bool isMultiline = true)
    {
        IsMultiline = isMultiline;
    }
}
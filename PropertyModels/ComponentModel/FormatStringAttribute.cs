using System;

namespace PropertyModels.ComponentModel;

/// <summary>
/// Class FormatStringAttribute.
/// Implements the <see cref="Attribute" />
/// </summary>
/// <seealso cref="Attribute" />
[AttributeUsage(AttributeTargets.Property|AttributeTargets.Field)]
public class FormatStringAttribute : Attribute
{
    /// <summary>
    /// The format string
    /// </summary>
    public readonly string FormatString;

    /// <summary>
    /// Initializes a new instance of the <see cref="FormatStringAttribute"/> class.
    /// </summary>
    /// <param name="formatString">The format string.</param>
    public FormatStringAttribute(string formatString)
    {
        FormatString = formatString;
    }
}
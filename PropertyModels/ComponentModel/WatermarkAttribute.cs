using System;

namespace PropertyModels.ComponentModel;

/// <summary>
/// Class WatermarkAttribute.
/// Implements the <see cref="Attribute" />
/// </summary>
/// <seealso cref="Attribute" />
[AttributeUsage(AttributeTargets.Property|AttributeTargets.Field)]
public class WatermarkAttribute : Attribute
{
    /// <summary>
    /// The watermark
    /// </summary>
    public readonly string Watermark;

    /// <summary>
    /// Initializes a new instance of the <see cref="WatermarkAttribute"/> class.
    /// </summary>
    /// <param name="watermark">The watermark.</param>
    public WatermarkAttribute(string watermark)
    {
        Watermark = watermark;
    }
}
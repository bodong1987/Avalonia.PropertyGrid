using System;

namespace PropertyModels.ComponentModel;

/// <summary>
/// Class IntegerIncrementAttribute.
/// Implements the <see cref="Attribute" />
/// </summary>
/// <seealso cref="Attribute" />
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class IntegerIncrementAttribute : Attribute
{
    /// <summary>
    /// The increment
    /// </summary>
    public readonly int Increment;

    /// <summary>
    /// Initializes a new instance of the <see cref="IntegerIncrementAttribute"/> class.
    /// </summary>
    /// <param name="increment">The increment.</param>
    public IntegerIncrementAttribute(int increment)
    {
        Increment = increment;
    }
}
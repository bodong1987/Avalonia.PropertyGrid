using System;

namespace PropertyModels.ComponentModel;

/// <summary>
/// Class FloatPrecisionAttribute.
/// Use this attribute to define float value precision
/// Implements the <see cref="Attribute" />
/// </summary>
/// <seealso cref="Attribute" />
[AttributeUsage(AttributeTargets.Property|AttributeTargets.Field)]
public class FloatPrecisionAttribute : Attribute
{
    /// <summary>
    /// The increment
    /// </summary>
    public readonly decimal Increment;

    /// <summary>
    /// The format string
    /// </summary>
    public readonly string FormatString;

    /// <summary>
    /// Initializes a new instance of the <see cref="FloatPrecisionAttribute"/> class.
    /// </summary>
    /// <param name="precision">The precision.</param>
    public FloatPrecisionAttribute(int precision = 2)
    {
            decimal v = 1;
            for(int i=0; i < precision; ++i)
            {
                v *= (decimal)0.1;
            }

            Increment = v;

            FormatString = string.Format("{{0:0.{0}}}", new string('0', precision));
        }

    /// <summary>
    /// Initializes a new instance of the <see cref="FloatPrecisionAttribute"/> class.
    /// </summary>
    /// <param name="increment">The increment.</param>
    /// <param name="formatString">The format string.</param>
    public FloatPrecisionAttribute(decimal increment = (decimal)0.01, string formatString = "{0:0.00}")
    {
            Increment = increment;
            FormatString = formatString;
        }

    /// <summary>
    /// Initializes a new instance of the <see cref="FloatPrecisionAttribute"/> class.
    /// </summary>
    /// <param name="increment">The increment.</param>
    /// <param name="formatString">The format string.</param>
    public FloatPrecisionAttribute(double increment, string formatString) :
        this((decimal)increment, formatString)
    {
        }
}
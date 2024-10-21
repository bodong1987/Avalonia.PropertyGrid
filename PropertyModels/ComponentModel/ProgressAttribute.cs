using System;

namespace PropertyModels.ComponentModel
{
    /// <summary>
    /// Class ProgressAttribute.
    /// Implements the <see cref="Attribute" />
    /// </summary>
    /// <seealso cref="Attribute" />
    [AttributeUsage(AttributeTargets.Property|AttributeTargets.Field)]
    public class ProgressAttribute : Attribute
    {
        /// <summary>
        /// The minimum
        /// </summary>
        public readonly double Minimum;
        
        /// <summary>
        /// The maximum
        /// </summary>
        public readonly double Maximum;
        
        /// <summary>
        /// The format string
        /// </summary>
        public readonly string FormatString;

        /// <summary>
        /// Gets or sets a value indicating whether [show progress text].
        /// </summary>
        /// <value><c>true</c> if [show progress text]; otherwise, <c>false</c>.</value>
        public bool ShowProgressText { get; set; } = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressAttribute"/> class.
        /// </summary>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        /// <param name="formatString">The format string.</param>
        public ProgressAttribute(double min = 0, double max = 100, string formatString = "{1:0}%")
        {
            Minimum = min; 
            Maximum = max;
            FormatString = formatString;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressAttribute"/> class.
        /// </summary>
        /// <param name="formatString">The format string.</param>
        public ProgressAttribute(string formatString)
        {
            FormatString = formatString;
        }
    }
}

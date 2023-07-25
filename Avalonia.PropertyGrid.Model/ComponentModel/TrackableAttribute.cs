using System;
using System.Collections.Generic;
using System.Text;

namespace Avalonia.PropertyGrid.Model.ComponentModel
{
    /// <summary>
    /// Class TrackableAttribute.
    /// Implements the <see cref="Attribute" />
    /// </summary>
    /// <seealso cref="Attribute" />
    [AttributeUsage(AttributeTargets.Property|AttributeTargets.Field, AllowMultiple = false)]
    public class TrackableAttribute : Attribute
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
        /// Initializes a new instance of the <see cref="TrackableAttribute"/> class.
        /// </summary>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        public TrackableAttribute(double min = 0, double max = 100)
        {
            Minimum = min;
            Maximum = max;
        }
    }
}

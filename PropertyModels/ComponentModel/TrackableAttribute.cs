﻿using System;

namespace PropertyModels.ComponentModel
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
        /// Gets or sets the increment.
        /// </summary>
        /// <value>The increment.</value>
        public double Increment { get; set; } = 0.01;

        /// <summary>
        /// Gets or sets the format string.
        /// </summary>
        /// <value>The format string.</value>
        public string FormatString { get; set; } = "{0:0.00}";

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

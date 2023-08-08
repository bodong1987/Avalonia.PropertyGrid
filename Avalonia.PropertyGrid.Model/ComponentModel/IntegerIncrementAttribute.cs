using System;
using System.Collections.Generic;
using System.Text;

namespace PropertyModels.ComponentModel
{
    /// <summary>
    /// Class IntegerIncrementAttribute.
    /// Implements the <see cref="Attribute" />
    /// </summary>
    /// <seealso cref="Attribute" />
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
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
}

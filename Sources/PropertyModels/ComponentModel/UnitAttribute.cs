using System;

namespace PropertyModels.ComponentModel
{
    /// <summary>
    /// Class UnitAttribute.
    /// Implements the <see cref="Attribute" />
    /// </summary>
    /// <seealso cref="Attribute" />
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class UnitAttribute : Attribute
    {
        /// <summary>
        /// The unit
        /// </summary>
        public readonly string Unit;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitAttribute"/> class.
        /// </summary>
        /// <param name="unit">The unit.</param>
        public UnitAttribute(string unit)
        {
            Unit = unit;
        }
    }
}
using System;

namespace PropertyModels.ComponentModel
{
    /// <summary>
    /// Class WatermarkAttribute.
    /// Implements the <see cref="Attribute" />
    /// </summary>
    /// <seealso cref="Attribute" />
    [AttributeUsage(AttributeTargets.Property|AttributeTargets.Field, AllowMultiple =false)]
    public class WatermarkAttribute : Attribute
    {
        /// <summary>
        /// The watermask
        /// </summary>
        public readonly string Watermask;

        /// <summary>
        /// Initializes a new instance of the <see cref="WatermarkAttribute"/> class.
        /// </summary>
        /// <param name="watermask">The watermask.</param>
        public WatermarkAttribute(string watermask)
        {
            Watermask = watermask;
        }
    }
}

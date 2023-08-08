using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyModels.ComponentModel.DataAnnotations
{
    /// <summary>
    /// Class DependsOnPropertyAttribute.
    /// Implements the <see cref="Attribute" />
    /// </summary>
    /// <seealso cref="Attribute" />
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class DependsOnPropertyAttribute : Attribute
    {
        /// <summary>
        /// The dependency properties
        /// </summary>
        public readonly string[] DependencyProperties;

        /// <summary>
        /// Initializes a new instance of the <see cref="DependsOnPropertyAttribute"/> class.
        /// </summary>
        /// <param name="propertyNames">The property names.</param>
        public DependsOnPropertyAttribute(params string[] propertyNames)
        {
            DependencyProperties = propertyNames;
        }
    }
}

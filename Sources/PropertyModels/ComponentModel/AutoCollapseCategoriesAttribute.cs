using System;
using System.Collections.Generic;
using System.Text;

namespace PropertyModels.ComponentModel
{
    /// <summary>
    /// Attribute to specify categories that should be automatically collapsed.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class AutoCollapseCategoriesAttribute : Attribute
    {
        /// <summary>
        /// Gets the set of category names that should be automatically collapsed.
        /// </summary>
        public readonly HashSet<string> CategoryNames;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoCollapseCategoriesAttribute"/> class.
        /// </summary>
        /// <param name="categoryNames">The category names to be automatically collapsed.</param>
        public AutoCollapseCategoriesAttribute(params string[] categoryNames)
        {
            CategoryNames = new HashSet<string>(categoryNames);
        }

        /// <summary>
        /// Determines whether the specified category should be automatically collapsed.
        /// </summary>
        /// <param name="categoryName">The name of the category to check.</param>
        /// <returns><c>true</c> if the category should be collapsed; otherwise, <c>false</c>.</returns>
        public virtual bool ShouldAutoCollapse(string categoryName)
        {
            return CategoryNames.Contains(categoryName);
        }
    }
}

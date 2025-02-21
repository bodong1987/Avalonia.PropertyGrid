using System.Collections.Generic;

namespace Avalonia.PropertyGrid.ViewModels
{
    /// <summary>
    /// Class ReferencePath.
    /// </summary>
    public class ReferencePath
    {
        /// <summary>
        /// The paths
        /// </summary>
        public readonly Stack<string> Paths = new();

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        public int Count => Paths.Count;

        /// <summary>
        /// Begins the scope.
        /// </summary>
        /// <param name="scope">The scope.</param>
        public void BeginScope(string scope)
        {
            Paths.Push(scope);
        }

        /// <summary>
        /// Ends the scope.
        /// </summary>
        public void EndScope()
        {
            Paths.Pop();
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return string.Join(", ", Paths.ToArray());
        }
    }
}

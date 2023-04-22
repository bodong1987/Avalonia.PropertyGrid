using Avalonia.PropertyGrid.Model.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.PropertyGrid.Model.Services
{
    /// <summary>
    /// Interface ILocalizationService
    /// Implements the <see cref="IReactiveObject" />
    /// </summary>
    /// <seealso cref="IReactiveObject" />
    public interface ILocalizationService : IReactiveObject
    {
        /// <summary>
        /// Gets the <see cref="System.String"/> with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>System.String.</returns>
        string this[string key] { get; }

        /// <summary>
        /// Gets or sets the name of the culture.
        /// </summary>
        /// <value>The name of the culture.</value>
        string CultureName { get; set; }
    }
}

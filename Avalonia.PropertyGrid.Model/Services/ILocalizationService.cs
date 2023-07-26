using Avalonia.PropertyGrid.Model.ComponentModel;
using System;
using System.Collections.Generic;
using System.Globalization;
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

        /// <summary>
        /// Gets the available cultures.
        /// </summary>
        /// <value>The available cultures.</value>
        CultureInfo[] AvailableCultures { get; }

        /// <summary>
        /// Adds the extra service.
        /// </summary>
        /// <param name="service">The service.</param>
        void AddExtraService(ILocalizationService service);

        /// <summary>
        /// Removes the extra service.
        /// </summary>
        /// <param name="service">The service.</param>
        void RemoveExtraService(ILocalizationService service);
    }
}

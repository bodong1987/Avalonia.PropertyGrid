using PropertyModels.ComponentModel;
using System;
using System.Globalization;
// ReSharper disable UnusedMethodReturnValue.Global

namespace PropertyModels.Localization;

/// <summary>
/// Interface ICultureData
/// </summary>
public interface ICultureData
{
    /// <summary>
    /// Gets the culture.
    /// </summary>
    /// <value>The culture.</value>
    CultureInfo Culture { get; }

    /// <summary>
    /// Gets the path.
    /// </summary>
    /// <value>The path.</value>
    Uri Path { get; }

    /// <summary>
    /// Gets the <see cref="string"/> with the specified key.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>System.String.</returns>
    string this[string key] { get; }

    /// <summary>
    /// Gets a value indicating whether this instance is loaded.
    /// </summary>
    /// <value><c>true</c> if this instance is loaded; otherwise, <c>false</c>.</value>
    bool IsLoaded { get; }

    /// <summary>
    /// Reloads this instance.
    /// </summary>
    /// <returns><c>true</c> if reload success, <c>false</c> otherwise.</returns>
    bool Reload();
}

/// <summary>
/// Interface ILocalizationService
/// Implements the <see cref="IReactiveObject" />
/// </summary>
/// <seealso cref="IReactiveObject" />
public interface ILocalizationService : IReactiveObject
{
    /// <summary>
    /// Gets or sets the culture data.
    /// </summary>
    /// <value>The culture data.</value>
    ICultureData CultureData { get; }

    /// <summary>
    /// Gets the <see cref="string"/> with the specified key.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>System.String.</returns>
    string this[string key] { get; }

    /// <summary>
    /// Occurs when [on culture changed].
    /// </summary>
    event EventHandler OnCultureChanged;

    /// <summary>
    /// Gets the extra services.
    /// </summary>
    /// <returns>ILocalizationService[].</returns>
    ILocalizationService[] GetExtraServices();

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

    /// <summary>
    /// Gets the cultures.
    /// </summary>
    /// <returns>ICultureData[].</returns>
    ICultureData[] GetCultures();

    /// <summary>
    /// Selects the culture.
    /// </summary>
    /// <param name="cultureName">Name of the culture.</param>
    void SelectCulture(string cultureName);
}
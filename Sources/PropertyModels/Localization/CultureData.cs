using System;
using System.Collections.Generic;
using System.Globalization;

namespace PropertyModels.Localization;

/// <summary>
/// Class AbstractCultureData.
/// Implements the <see cref="PropertyModels.Localization.ICultureData" />
/// </summary>
/// <seealso cref="PropertyModels.Localization.ICultureData" />
public abstract class AbstractCultureData : ICultureData
{
    /// <summary>
    /// The local texts
    /// </summary>
    // ReSharper disable once FieldCanBeMadeReadOnly.Global
    protected Dictionary<string, string>? LocalTexts;

    /// <summary>
    /// Gets the culture.
    /// </summary>
    /// <value>The culture.</value>
    public CultureInfo Culture { get; }

    /// <summary>
    /// Gets the path.
    /// </summary>
    /// <value>The path.</value>
    public Uri Path { get; }

    /// <summary>
    /// Gets a value indicating whether this instance is loaded.
    /// </summary>
    /// <value><c>true</c> if this instance is loaded; otherwise, <c>false</c>.</value>
    public bool IsLoaded => LocalTexts != null;

    /// <summary>
    /// Initializes a new instance of the <see cref="AbstractCultureData"/> class.
    /// </summary>
    /// <param name="uri">The URI.</param>
    protected AbstractCultureData(Uri uri)
    {
        var localPath = uri.LocalPath;
        Culture = new CultureInfo(System.IO.Path.GetFileNameWithoutExtension(localPath));
        Path = uri;
    }

    /// <summary>
    /// Gets the <see cref="string" /> with the specified key.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>System.String.</returns>
    public string this[string key]
    {
        get
        {
            if (LocalTexts != null && LocalTexts.TryGetValue(key, out var text))
            {
                return text;
            }

            return key;
        }
    }

    /// <summary>
    /// Reloads this instance.
    /// </summary>
    /// <returns><c>true</c> if reload success, <c>false</c> otherwise.</returns>
    public abstract bool Reload();

    /// <inheritdoc />
    public virtual void Unload()
    {
        LocalTexts = null;
    }

    /// <summary>
    /// Returns a <see cref="string" /> that represents this instance.
    /// </summary>
    /// <returns>A <see cref="string" /> that represents this instance.</returns>
    public override string ToString() => Culture.NativeName;
}
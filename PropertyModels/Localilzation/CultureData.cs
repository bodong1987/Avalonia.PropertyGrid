using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using PropertyModels.Extensions;

namespace PropertyModels.Localilzation
{
    /// <summary>
    /// Class AbstractCultureData.
    /// Implements the <see cref="PropertyModels.Localilzation.ICultureData" />
    /// </summary>
    /// <seealso cref="PropertyModels.Localilzation.ICultureData" />
    public abstract class AbstractCultureData : ICultureData
    {
        /// <summary>
        /// The local texts
        /// </summary>
        protected Dictionary<string, string> LocalTexts = null;

        /// <summary>
        /// Gets the culture.
        /// </summary>
        /// <value>The culture.</value>
        public CultureInfo Culture { get; private set; }

        /// <summary>
        /// Gets the path.
        /// </summary>
        /// <value>The path.</value>
        public Uri Path { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is loaded.
        /// </summary>
        /// <value><c>true</c> if this instance is loaded; otherwise, <c>false</c>.</value>
        public bool IsLoaded => LocalTexts != null;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractCultureData"/> class.
        /// </summary>
        /// <param name="uri">The URI.</param>
        public AbstractCultureData(Uri uri)
        {
            string localPath = uri.LocalPath;
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
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public abstract bool Reload();

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return Culture.NativeName;
        }
    }
}

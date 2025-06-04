using System;
using System.Collections.Generic;
using System.Globalization;

namespace PropertyModels.Localization
{
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
        protected Dictionary<string, string>? LocalTexts = null;

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

        /// <summary>
        /// Reads the json string dictionary.
        /// </summary>
        /// <param name="json">The json.</param>
        /// <returns>Dictionary&lt;System.String, System.String&gt;.</returns>
        public static Dictionary<string, string> ReadJsonStringDictionary(string json)
        {
            var dict = new Dictionary<string, string>();

            foreach (var line in json.Split('\r', '\n'))
            {
                var configLine = line.Trim();

                if (!configLine.Contains(':') || configLine.StartsWith('\\'))
                {
                    continue;
                }

                var key = PickStringToken(configLine, 0, out var endPos);

                if (key == null)
                {
                    continue;
                }

                var value = PickStringToken(configLine, endPos + 1, out _);

                if (value == null)
                {
                    continue;
                }

                dict.Add(key, value);
            }

            return dict;
        }

        private static string? PickStringToken(string line, int startPos, out int endPos)
        {
            var begin = -1;
            var escape = -1;
            endPos = -1;

            for (var i = startPos; i < line.Length; ++i)
            {
                var ch = line[i];

                if (ch == '"')
                {
                    if (escape == i - 1 && escape != -1)
                    {
                        escape = -1;
                    }
                    else if (begin == -1)
                    {
                        begin = i;
                    }
                    else
                    {
                        endPos = i;

                        return line.Substring(begin + 1, endPos - begin - 1);
                    }
                }
                else if (ch == '\\')
                {
                    escape = i;
                }
            }

            return null;
        }
    }
}

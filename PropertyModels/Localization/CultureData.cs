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
        protected Dictionary<string, string>? LocalTexts = null;

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


        /// <summary>
        /// Reads the json string dictionary.
        /// </summary>
        /// <param name="json">The json.</param>
        /// <returns>Dictionary&lt;System.String, System.String&gt;.</returns>
        public static Dictionary<string, string> ReadJsonStringDictionary(string json)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            string[] configs = json.Split('\r', '\n');

            foreach(var line in configs)
            {
                var configLine = line.Trim();

                if(!configLine.Contains(":") || configLine.StartsWith("\\"))
                {
                    continue;
                }

                int endPos = 0;
                var key = PickStringToken(configLine, 0, out endPos);

                if(key == null)
                {
                    continue;
                }

                var value = PickStringToken(configLine, endPos + 1, out _);

                if(value == null)
                {
                    continue;
                }

                dict.Add(key, value);
            }

            return dict;
        }

        private static string? PickStringToken(string line, int startPos, out int endPos)
        {
            int Begin = -1;
            int Escape = -1;
            endPos = -1;

            for(int i=startPos; i < line.Length; ++i)
            {
                char ch = line[i];

                if(ch == '"')
                {
                    if(Escape == i-1 && Escape != -1)
                    {
                        Escape = -1;
                        continue;
                    }
                    else if(Begin == -1)
                    {
                        Begin = i;
                    }
                    else if(Begin != -1)
                    {
                        endPos = i;

                        return line.Substring(Begin + 1, endPos - Begin - 1);
                    }
                }
                else if(ch == '\\')
                {
                    Escape = i;
                }
            }

            return null;
        }
    }
}

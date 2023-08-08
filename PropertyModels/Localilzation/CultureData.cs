using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using PropertyModels.Extensions;
using System.Text.RegularExpressions;

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


        /// <summary>
        /// Reads the json string dictionary.
        /// </summary>
        /// <param name="json">The json.</param>
        /// <returns>Dictionary&lt;System.String, System.String&gt;.</returns>
        public static Dictionary<string, string> ReadJsonStringDictionary(string json)
        {
            var dict = ParseJSON(json, 0, out _);

            Dictionary<string, string> result = new Dictionary<string, string>();
            foreach(var d in dict)
            {
                result.Add(d.Key.Trim(), d.Value?.ToString()?.Trim() ?? d.Key?.Trim());
            }

            return result;
        }

        private static string DecodeString(Regex regex, string str)
        {
            return Regex.Unescape(regex.Replace(str, match => char.ConvertFromUtf32(Int32.Parse(match.Groups[1].Value, System.Globalization.NumberStyles.HexNumber))));
        }

        /// <summary>
        /// Parses the json.
        /// https://stackoverflow.com/questions/1207731/how-can-i-deserialize-json-to-a-simple-dictionarystring-string-in-asp-net
        /// </summary>
        /// <param name="json">The json.</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <returns>Dictionary&lt;System.String, System.Object&gt;.</returns>
        private static Dictionary<string, object> ParseJSON(string json, int start, out int end)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            bool escbegin = false;
            bool escend = false;
            bool inquotes = false;
            string key = null;
            int cend;
            StringBuilder sb = new StringBuilder();
            Dictionary<string, object> child = null;
            List<object> arraylist = null;
            Regex regex = new Regex(@"\\u([0-9a-z]{4})", RegexOptions.IgnoreCase);
            int autoKey = 0;
            for (int i = start; i < json.Length; i++)
            {
                char c = json[i];
                if (c == '\\') escbegin = !escbegin;
                if (!escbegin)
                {
                    if (c == '"')
                    {
                        inquotes = !inquotes;
                        if (!inquotes && arraylist != null)
                        {
                            arraylist.Add(DecodeString(regex, sb.ToString()));
                            sb.Length = 0;
                        }
                        continue;
                    }
                    if (!inquotes)
                    {
                        switch (c)
                        {
                            case '{':
                                if (i != start)
                                {
                                    child = ParseJSON(json, i, out cend);
                                    if (arraylist != null) arraylist.Add(child);
                                    else
                                    {
                                        dict.Add(key, child);
                                        key = null;
                                    }
                                    i = cend;
                                }
                                continue;
                            case '}':
                                end = i;
                                if (key != null)
                                {
                                    if (arraylist != null) dict.Add(key, arraylist);
                                    else dict.Add(key, DecodeString(regex, sb.ToString()));
                                }
                                return dict;
                            case '[':
                                arraylist = new List<object>();
                                continue;
                            case ']':
                                if (key == null)
                                {
                                    key = "array" + autoKey.ToString();
                                    autoKey++;
                                }
                                if (arraylist != null && sb.Length > 0)
                                {
                                    arraylist.Add(sb.ToString());
                                    sb.Length = 0;
                                }
                                dict.Add(key, arraylist);
                                arraylist = null;
                                key = null;
                                continue;
                            case ',':
                                if (arraylist == null && key != null)
                                {
                                    dict.Add(key, DecodeString(regex, sb.ToString()));
                                    key = null;
                                    sb.Length = 0;
                                }
                                if (arraylist != null && sb.Length > 0)
                                {
                                    arraylist.Add(sb.ToString());
                                    sb.Length = 0;
                                }
                                continue;
                            case ':':
                                key = DecodeString(regex, sb.ToString());
                                sb.Length = 0;
                                continue;
                        }
                    }
                }
                sb.Append(c);
                if (escend) escbegin = false;
                if (escbegin) escend = true;
                else escend = false;
            }
            end = json.Length - 1;
            return dict; //theoretically shouldn't ever get here
        }
    }
}

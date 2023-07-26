using Avalonia;
using Avalonia.Platform;
using Avalonia.PropertyGrid.Model.ComponentModel;
using Avalonia.PropertyGrid.Model.Extensions;
using Avalonia.PropertyGrid.Model.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Avalonia.PropertyGrid.Localization
{
    /// <summary>
    /// Class JsonAssetLocalizationService.
    /// Implements the <see cref="MiniReactiveObject" />
    /// Implements the <see cref="ILocalizationService" />
    /// </summary>
    /// <seealso cref="MiniReactiveObject" />
    /// <seealso cref="ILocalizationService" />
    public class JsonAssetLocalizationService : MiniReactiveObject, ILocalizationService
    {
        /// <summary>
        /// The local texts
        /// </summary>
        private Dictionary<string, string> LocalTexts = null;

        /// <summary>
        /// The culture name
        /// </summary>
        string _CultureName;
        /// <summary>
        /// Gets or sets the name of the culture.
        /// </summary>
        /// <value>The name of the culture.</value>
        public string CultureName
        {
            get => _CultureName;
            set => this.RaiseAndSetIfChanged(ref _CultureName, value);
        }

        readonly List<ILocalizationService> ExtraServices = new List<ILocalizationService>();

        /// <summary>
        /// Gets the <see cref="System.String"/> with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>System.String.</returns>
        public string this[string key]
        {
            get
            {
                foreach(var service in ExtraServices)
                {
                    string value = service[key];

                    if(value.IsNotNullOrEmpty() && value != key)
                    {
                        return value;
                    }
                }

                if (LocalTexts != null && LocalTexts.TryGetValue(key, out var text))
                {
                    return text;
                }

                return key;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonAssetLocalizationService"/> class.
        /// </summary>
        public JsonAssetLocalizationService()
        {
            PropertyChanged += OnPropertyChanged;       
            
            if(IsLaunguageFileExists(CultureInfo.CurrentCulture.Name))
            {
                CultureName = CultureInfo.CurrentCulture.Name;
            }
            else
            {
                CultureName = "en-US";
            }
        }

        /// <summary>
        /// Determines whether [is launguage file exists] [the specified culture].
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <returns><c>true</c> if [is launguage file exists] [the specified culture]; otherwise, <c>false</c>.</returns>
        private bool IsLaunguageFileExists(string culture)
        {
            var url = new Uri($"avares://{GetType().Assembly.GetName().Name}/Assets/Localizations/{culture}.json");

            return AssetLoader.Exists(url);
        }

        /// <summary>
        /// Handles the <see cref="E:PropertyChanged" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(CultureName) && IsLaunguageFileExists(CultureName))
            {
                var url = new Uri($"avares://{GetType().Assembly.GetName().Name}/Assets/Localizations/{CultureName}.json");

                using (var stream = AssetLoader.Open(url))
                {
                    using (StreamReader sr = new StreamReader(stream, Encoding.UTF8))
                    {
                        try
                        {
                            var tempDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(sr.ReadToEnd());

                            if (tempDict != null)
                            {
                                LocalTexts = tempDict;
                            }
                        }
                        catch
                        {

                        }                        
                    }
                }
            }
        }

        /// <summary>
        /// Adds the extra service.
        /// </summary>
        /// <param name="service">The service.</param>
        public void AddExtraService(ILocalizationService service)
        {
            ExtraServices.Add(service);
        }

        /// <summary>
        /// Removes the extra service.
        /// </summary>
        /// <param name="service">The service.</param>
        public void RemoveExtraService(ILocalizationService service)
        {
            ExtraServices.Remove(service);
        }
    }
}

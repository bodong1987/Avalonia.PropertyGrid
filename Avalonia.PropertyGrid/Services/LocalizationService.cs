using Avalonia.PropertyGrid.Localization;
using PropertyModels.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Avalonia.PropertyGrid.Services
{
    /// <summary>
    /// Class LocalizationService.
    /// </summary>
    public static class LocalizationService
    {
        /// <summary>
        /// The default
        /// </summary>
        public readonly static ILocalizationService Default = new AssemblyJsonAssetLocalizationService(typeof(LocalizationService).Assembly);

        static LocalizationService()
        {
        }
    }
}

using System;
using System.Diagnostics;
using System.Linq;
using Avalonia.PropertyGrid.Services;
using PropertyModels.ComponentModel;
using PropertyModels.Extensions;

namespace Avalonia.PropertyGrid.Utils
{
    /// <summary>
    /// Class EnumUtils.
    /// </summary>
    public static class EnumUtils
    {
        /// <summary>
        /// Gets the enum values.
        /// </summary>
        /// <param name="enumType">Type of the enum.</param>
        /// <returns>EnumValueWrapper[].</returns>
        public static EnumValueWrapper[] GetEnumValues(Type enumType)
        {
            Debug.Assert(enumType is { IsEnum: true });

            var values = enumType.GetEnumValues();

            return values.Select(x =>
            {
                var wrapper = new EnumValueWrapper((x as Enum)!);

                try
                {
                    wrapper.DisplayName = LocalizationService.Default[wrapper.DisplayName];
                }
                catch
                {
                    wrapper.DisplayName = x.ToString()!;
                }

                return wrapper;
            }).ToArray();
        }

        /// <summary>
        /// Gets the enum values.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>EnumValueWrapper[].</returns>
        public static EnumValueWrapper[] GetEnumValues<T>() where T : Enum
        {
            return GetEnumValues(typeof(T));
        }
    }
}

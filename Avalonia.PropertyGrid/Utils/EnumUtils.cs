using Avalonia.PropertyGrid.Controls;
using PropertyModels.ComponentModel;
using PropertyModels.Extensions;
using Avalonia.PropertyGrid.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

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
            Debug.Assert(enumType!=null && enumType.IsEnum);

            var values = enumType.GetEnumValues();

            return values.Select(x =>
            {
                var wrapper = new EnumValueWrapper(x as Enum);

                wrapper.DisplayName = LocalizationService.Default[wrapper.DisplayName];

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

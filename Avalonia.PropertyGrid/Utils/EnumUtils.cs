using System;
using System.Collections.Generic;
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
        public static EnumValueWrapper[] GetEnumValues(Type enumType, int[] permitValues=null)
        {
            Debug.Assert(enumType is { IsEnum: true });

            var values = enumType.GetEnumValues();

            if (permitValues != null && permitValues.Length > 0)
            {
                List<object> pv = new List<object>();
                foreach (var value in values)
                {
                    if(permitValues.Contains((int)value))
                    {
                        pv.Add(value);
                    }
                }

                values = pv.ToArray();
                
            }

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

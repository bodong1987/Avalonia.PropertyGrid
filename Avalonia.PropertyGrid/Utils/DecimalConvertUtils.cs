using PropertyModels.Extensions;
using System;
using System.Diagnostics;
using System.Globalization;

namespace Avalonia.PropertyGrid.Utils
{
    /// <summary>
    /// Class DecimalConvertUtils.
    /// </summary>
    public static class DecimalConvertUtils
    {
        /// <summary>
        /// convert numeric type or numeric string to decimal
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.Decimal.</returns>
        public static decimal ConvertTo(object? value)
        {
            Debug.Assert(value != null);

            if (value == null)
            {
                return 0;
            }

            var type = value.GetType();

            try
            {
                switch (Type.GetTypeCode(type))
                {
                    case TypeCode.Byte:
                        return Convert.ToDecimal((byte)value);
                    case TypeCode.SByte:
                        return Convert.ToDecimal((sbyte)value);
                    case TypeCode.UInt16:
                        return Convert.ToDecimal((ushort)value);
                    case TypeCode.UInt32:
                        return Convert.ToDecimal((uint)value);
                    case TypeCode.UInt64:
                        return Convert.ToDecimal((ulong)value);
                    case TypeCode.Int16:
                        return Convert.ToDecimal((short)value);
                    case TypeCode.Int32:
                        return Convert.ToDecimal((int)value);
                    case TypeCode.Int64:
                        return Convert.ToDecimal((long)value);
                    case TypeCode.Decimal:
                        return (decimal)value;
                    case TypeCode.Double:
                        return Convert.ToDecimal((double)value, CultureInfo.CurrentCulture);
                    case TypeCode.Single:
                        return Convert.ToDecimal((float)value, CultureInfo.CurrentCulture);
                    case TypeCode.String:
                        if(decimal.TryParse(value as string, NumberStyles.Any, CultureInfo.CurrentCulture, out var v))
                        {
                            return v;
                        }
                        return 0;
                    default:
                        return 0;
                }
            }
            catch (OverflowException)
            {
                Debug.WriteLine($"Overflow occurred during conversion. value = {value}");
                return 0;
            }
            catch (InvalidCastException)
            {
                Debug.WriteLine($"Invalid cast occurred during conversion. value = {value}");
                return 0;
            }
            catch (FormatException)
            {
                Debug.WriteLine($"Format exception occurred during conversion. value = {value}");
                return 0;
            }
        }
    }
}
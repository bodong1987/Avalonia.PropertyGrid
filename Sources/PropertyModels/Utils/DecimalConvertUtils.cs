using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Globalization;

namespace PropertyModels.Utils;

/// <summary>
///     decimal convert helper
/// </summary>
public static class DecimalConvertUtils
{
    /// <summary>
    ///     convert numeric type or numeric string to decimal
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>System.Decimal.</returns>
    public static decimal ConvertTo(object? value)
    {
        Debug.Assert(value != null);

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (value == null)
        {
            return 0;
        }

        var type = value.GetType();

        try
        {
            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            // ReSharper disable once ConvertSwitchStatementToSwitchExpression
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
                    return decimal.TryParse(value as string, NumberStyles.Any, CultureInfo.CurrentCulture, out var v)
                        ? v
                        : 0;
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

    /// <summary>
    ///     get config from RangeAttribute
    /// </summary>
    /// <param name="rangeAttr"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static (decimal Min, decimal Max) GetDecimalRange(RangeAttribute rangeAttr)
    {
        if (rangeAttr.OperandType == null)
        {
            throw new ArgumentException("OperandType is not initialized");
        }

        if (rangeAttr.OperandType == typeof(int) || rangeAttr.OperandType == typeof(double))
        {
            return (
                Convert.ToDecimal(rangeAttr.Minimum, CultureInfo.InvariantCulture),
                Convert.ToDecimal(rangeAttr.Maximum, CultureInfo.InvariantCulture)
            );
        }

        var converter = TypeDescriptor.GetConverter(rangeAttr.OperandType);
        var conversionCulture = rangeAttr.ParseLimitsInInvariantCulture
            ? CultureInfo.InvariantCulture
            : CultureInfo.CurrentCulture;

        var minValue = converter.ConvertFromString(
            null,
            conversionCulture,
            rangeAttr.Minimum.ToString()!
        );

        var maxValue = converter.ConvertFromString(
            null,
            conversionCulture,
            rangeAttr.Maximum.ToString()!
        );

        // 数值类型安全转换
        return (
            Convert.ToDecimal(minValue, CultureInfo.InvariantCulture),
            Convert.ToDecimal(maxValue, CultureInfo.InvariantCulture)
        );
    }
}
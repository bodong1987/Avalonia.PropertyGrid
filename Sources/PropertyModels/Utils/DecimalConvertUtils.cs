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
            return Type.GetTypeCode(type) switch
            {
                TypeCode.Byte => Convert.ToDecimal((byte)value),
                TypeCode.SByte => Convert.ToDecimal((sbyte)value),
                TypeCode.UInt16 => Convert.ToDecimal((ushort)value),
                TypeCode.UInt32 => Convert.ToDecimal((uint)value),
                TypeCode.UInt64 => Convert.ToDecimal((ulong)value),
                TypeCode.Int16 => Convert.ToDecimal((short)value),
                TypeCode.Int32 => Convert.ToDecimal((int)value),
                TypeCode.Int64 => Convert.ToDecimal((long)value),
                TypeCode.Decimal => (decimal)value,
                TypeCode.Double => Convert.ToDecimal((double)value, CultureInfo.CurrentCulture),
                TypeCode.Single => Convert.ToDecimal((float)value, CultureInfo.CurrentCulture),
                TypeCode.String => decimal.TryParse(value as string, NumberStyles.Any, CultureInfo.CurrentCulture, out var v) ? v : 0,
                _ => 0,
            };
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
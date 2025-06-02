using System;

namespace PropertyModels.ComponentModel.DataAnnotations;

/// <summary>
/// You can use this tag to mark the tolerance value of a floating point number. When the change difference is less than this value, it is considered that there is no change.
/// </summary>
[AttributeUsage(AttributeTargets.Field|AttributeTargets.Property)]
public class FloatingNumberEqualToleranceAttribute : Attribute
{
    /// <summary>
    /// value
    /// </summary>
    public float Tolerance { get; }

    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="tolerance"></param>
    public FloatingNumberEqualToleranceAttribute(float tolerance = 0.0001f)
    {
        Tolerance = tolerance;
    }
}
namespace PropertyModels.Extensions;

using System.ComponentModel;
using System.Reflection;

/// <summary>
/// Extension class for <see cref="PropertyDescriptor"/>s.
/// </summary>
public static class PropertyDescriptorExtensions
{
    /// <summary>
    /// Determines whether [is expandable type] [the specified property].
    /// </summary>
    /// <param name="property">The property.</param>
    /// <returns><c>true</c> if [is expandable type] [the specified property]; otherwise, <c>false</c>.</returns>
    public static bool IsExpandableType(this PropertyDescriptor property)
    {
        if (!property.PropertyType.IsClass)
        {
            return false;
        }

        var attr = property.GetCustomAttribute<TypeConverterAttribute>();

        if (attr?.GetConverterType()!.IsChildOf<ExpandableObjectConverter>() == true)
        {
            return true;
        }

        attr = property.PropertyType.GetCustomAttribute<TypeConverterAttribute>();

        return attr?.GetConverterType()!.IsChildOf<ExpandableObjectConverter>() == true;
    }
}
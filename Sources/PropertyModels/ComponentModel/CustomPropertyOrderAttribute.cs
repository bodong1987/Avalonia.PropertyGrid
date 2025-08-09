using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace PropertyModels.ComponentModel;

/// <summary>
/// custom property order handler
/// </summary>
public interface ICustomPropertyOrderHandler
{
    /// <summary>
    /// process categories order
    /// </summary>
    /// <param name="context">context</param>
    /// <param name="categories">categories</param>
    /// <returns></returns>
    List<KeyValuePair<string, List<PropertyDescriptor>>> HandleCategories(
        object context,
        List<KeyValuePair<string, List<PropertyDescriptor>>> categories);

    /// <summary>
    /// process properties order
    /// if no category, @category is empty
    /// if it has category, @category is not empty
    /// </summary>
    /// <param name="context">context</param>
    /// <param name="properties"></param>
    /// <param name="category"></param>
    /// <returns></returns>
    IEnumerable<PropertyDescriptor> HandleProperties(
        object context,
        IEnumerable<PropertyDescriptor> properties,
        string? category = null);
}

/// <summary>
/// use this to custom the order of property grid's elements[category, property]
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class CustomPropertyOrderAttribute : Attribute, ICustomPropertyOrderHandler
{
    /// <summary>
    /// process categories order
    /// </summary>
    /// <param name="context"></param>
    /// <param name="categories"></param>
    /// <returns></returns>
    public virtual List<KeyValuePair<string, List<PropertyDescriptor>>> HandleCategories(
        object context,
        List<KeyValuePair<string, List<PropertyDescriptor>>> categories)
    {
        return categories;
    }

    /// <summary>
    /// process properties order
    /// if no category, @category is empty
    /// if it has category, @category is not empty
    /// </summary>
    /// <param name="context"></param>
    /// <param name="properties"></param>
    /// <param name="category"></param>
    /// <returns></returns>
    public virtual IEnumerable<PropertyDescriptor> HandleProperties(object context, IEnumerable<PropertyDescriptor> properties,
        string? category = null)
    {
        return properties;
    }
}
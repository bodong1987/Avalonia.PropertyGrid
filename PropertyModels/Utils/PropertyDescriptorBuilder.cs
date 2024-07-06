using PropertyModels.ComponentModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace PropertyModels.Utils;

/// <summary>
/// Class PropertyDescriptorBuilder.
/// </summary>
public class PropertyDescriptorBuilder
{
    /// <summary>
    /// The is multiple objects
    /// </summary>
    public readonly bool IsMultipleObjects;
    /// <summary>
    /// The target
    /// </summary>
    public readonly object Target;

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyDescriptorBuilder"/> class.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <exception cref="System.ArgumentNullException">target</exception>
    public PropertyDescriptorBuilder(object target)
    {
        switch (target)
        {
            case null:
                throw new ArgumentNullException(nameof(target));
            case IEnumerable enumerable:
                IsMultipleObjects = true;
                Target = enumerable;
                break;
            default:
                IsMultipleObjects = false;
                Target = target;
                break;
        }
    }

    /// <summary>
    /// Gets the properties.
    /// </summary>
    /// <returns>PropertyDescriptorCollection.</returns>
    public PropertyDescriptorCollection GetProperties()
    {
        return IsMultipleObjects ? GetMultipleProperties(Target as IEnumerable) : GetProperties(Target);
    }

    /// <summary>
    /// Gets the properties.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <returns>PropertyDescriptorCollection.</returns>
    private static PropertyDescriptorCollection GetProperties(object target)
    {
        if(target is ICustomTypeDescriptor ctd)
        {
            return ctd.GetProperties();
        }

        return TypeDescriptor.GetProperties(target);
    }

    /// <summary>
    /// Gets the multiple properties.
    /// </summary>
    /// <param name="targets">The targets.</param>
    /// <returns>PropertyDescriptorCollection.</returns>
    // ReSharper disable once MemberCanBeMadeStatic.Local
    private PropertyDescriptorCollection GetMultipleProperties(IEnumerable targets)
    {
        var collections = (from object target in targets select GetProperties(target)).ToList();

        if(collections.Count == 0)
        {
            return new PropertyDescriptorCollection(null);
        }

        //
        var descriptors = new List<MultiObjectPropertyDescriptor>();

        var multiCollections = collections.Skip(1).ToList();

        foreach (PropertyDescriptor propertyDescriptor in collections.First())
        {
            if (!AllowMerge(propertyDescriptor))
            {
                continue;
            }

            var isMatched = true;
            var propertyDescriptors = new List<PropertyDescriptor>(collections.Count) { propertyDescriptor };

            foreach (var pd in multiCollections.Select(collection => collection.Find(propertyDescriptor.Name, false)))
            {
                if(pd == null || !pd.Equals(propertyDescriptor))
                {
                    isMatched = false;
                    break;
                }

                propertyDescriptors.Add(pd);
            }

            if(!isMatched)
            {
                continue;
            }

            descriptors.Add(new MultiObjectPropertyDescriptor(propertyDescriptors.ToArray()));
        }

        // ReSharper disable once CoVariantArrayConversion
        return new PropertyDescriptorCollection(descriptors.ToArray());
    }

    /// <summary>
    /// Allows the merge.
    /// </summary>
    /// <param name="descriptor">The descriptor.</param>
    /// <returns><c>true</c> if success, <c>false</c> otherwise.</returns>
    public static bool AllowMerge(PropertyDescriptor descriptor)
    {
        if (descriptor == null)
        {
            return false;
        }
                
        var mergeablePropertyAttribute = descriptor.Attributes[typeof(MergablePropertyAttribute)] as MergablePropertyAttribute;

        return mergeablePropertyAttribute?.AllowMerge ?? MergablePropertyAttribute.Default.AllowMerge;
    }
}
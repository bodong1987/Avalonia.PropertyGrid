using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using PropertyModels.ComponentModel;

namespace PropertyModels.Utils
{
    /// <summary>
    /// Class PropertyDescriptorBuilder.
    /// </summary>
    public class PropertyDescriptorBuilder
    {
        /// <summary>
        /// The is multiple objects
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
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
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if(target is IEnumerable enumerable)
            {
                IsMultipleObjects = true;
                Target = enumerable;
            }
            else
            {
                IsMultipleObjects = false;
                Target = target;
            }
        }

        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <returns>PropertyDescriptorCollection.</returns>
        public PropertyDescriptorCollection GetProperties()
        {
            return IsMultipleObjects ? GetMultipleProperties((Target as IEnumerable)!) : GetProperties(Target);
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
        private static PropertyDescriptorCollection GetMultipleProperties(IEnumerable targets)
        {
            var collections = new List<PropertyDescriptorCollection>();
            foreach(var target in targets)
            {
                collections.Add(GetProperties(target));
            }

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

                foreach(var collection in multiCollections)
                {
                    var pd = collection.Find(propertyDescriptor.Name, false);

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
        /// <returns><c>true</c> if allow merge, <c>false</c> otherwise.</returns>
        // ReSharper disable once MemberCanBePrivate.Global
        public static bool AllowMerge(PropertyDescriptor descriptor)
        {
            var attr = descriptor.Attributes[typeof(MergablePropertyAttribute)] as MergablePropertyAttribute;

            return attr?.AllowMerge ?? MergablePropertyAttribute.Default.AllowMerge;
        }
    }
}

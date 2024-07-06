using PropertyModels.ComponentModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

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
        public readonly bool IsMultipleObjects;
        /// <summary>
        /// The target
        /// </summary>
        public readonly Object Target;

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

            var enumerable = target as IEnumerable;
            if(enumerable != null)
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
            if(IsMultipleObjects)
            {
                return GetMultipleProperties(Target as IEnumerable);
            }
            else
            {
                return GetProperties(Target);
            }
        }

        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>PropertyDescriptorCollection.</returns>
        private PropertyDescriptorCollection GetProperties(object target)
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
        private PropertyDescriptorCollection GetMultipleProperties(IEnumerable targets)
        {
            List<PropertyDescriptorCollection> Collections = new List<PropertyDescriptorCollection>();
            foreach(var target in targets)
            {
                Collections.Add(GetProperties(target));
            }

            if(Collections.Count == 0)
            {
                return new PropertyDescriptorCollection(null);
            }

            //
            List<MultiObjectPropertyDescriptor> Descriptors = new List<MultiObjectPropertyDescriptor>();

            List<PropertyDescriptorCollection> MultiCollections = Collections.Skip(1).ToList();

            foreach (PropertyDescriptor propertyDescriptor in Collections.First())
            {
                if (!AllowMerge(propertyDescriptor))
                {
                    continue;
                }

                bool IsMatched = true;
                List<PropertyDescriptor> _propertyDescriptors = new List<PropertyDescriptor>(Collections.Count) { propertyDescriptor };

                foreach(var collection in MultiCollections)
                {
                    var pd = collection.Find(propertyDescriptor.Name, false);

                    if(pd == null || !pd.Equals(propertyDescriptor))
                    {
                        IsMatched = false;
                        break;
                    }

                    _propertyDescriptors.Add(pd);
                }

                if(!IsMatched)
                {
                    continue;
                }

                Descriptors.Add(new MultiObjectPropertyDescriptor(_propertyDescriptors.ToArray()));
            }

            return new PropertyDescriptorCollection(Descriptors.ToArray());
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
                
            MergablePropertyAttribute mergablePropertyAttribute = descriptor.Attributes[typeof(MergablePropertyAttribute)] as MergablePropertyAttribute;

            if (mergablePropertyAttribute == null)
            {
                return MergablePropertyAttribute.Default.AllowMerge;
            }
                
            return mergablePropertyAttribute.AllowMerge;
        }
    }
}

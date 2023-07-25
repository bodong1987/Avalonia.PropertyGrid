using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Avalonia.PropertyGrid.Model.Extensions
{
    /// <summary>
    /// Class TypeExtensions.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Gets any custom attributes.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="memberInfo">The member information.</param>
        /// <param name="inherit">if set to <c>true</c> [inherit].</param>
        /// <returns>T.</returns>
        public static T GetAnyCustomAttribute<T>(this MemberInfo memberInfo, bool inherit = true)
            where T : Attribute
        {
            var Attrs = memberInfo.GetCustomAttributes(typeof(T), inherit);

            if (Attrs != null && Attrs.Length > 0)
            {
                T attr = Attrs[0] as T;

                return attr;
            }

            return null;
        }

        /// <summary>
        /// Gets the custom attributes.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="memberInfo">The member information.</param>
        /// <param name="inherit">if set to <c>true</c> [inherit].</param>
        /// <returns>T[].</returns>
        public static T[] GetCustomAttributes<T>(this MemberInfo memberInfo, bool inherit = true)
            where T : Attribute
        {
            var Attrs = memberInfo.GetCustomAttributes(typeof(T), inherit);

            if (Attrs != null && Attrs.Length > 0)
            {
                return Attrs.Cast<T>().ToArray();
            }

            return new T[0];
        }

        /// <summary>
        /// check if has defined an Attribute
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="memberInfo">The member information.</param>
        /// <param name="inherit">if set to <c>true</c> [inherit].</param>
        /// <returns><c>true</c> if the specified inherit is defined; otherwise, <c>false</c>.</returns>
        public static bool IsDefined<T>(this MemberInfo memberInfo, bool inherit = true)
            where T : Attribute
        {
            return memberInfo.IsDefined(typeof(T), inherit);
        }

        /// <summary>
        /// Gets the type of the underlying.
        /// </summary>
        /// <param name="memberInfo">The member information.</param>
        /// <returns>Type.</returns>
        /// <exception cref="System.ArgumentException">Input MemberInfo must be if type EventInfo, FieldInfo, MethodInfo, or PropertyInfo</exception>
        public static Type GetUnderlyingType(this MemberInfo memberInfo)
        {
            switch (memberInfo.MemberType)
            {
                case MemberTypes.Event:
                    return ((EventInfo)memberInfo).EventHandlerType;
                case MemberTypes.Field:
                    return ((FieldInfo)memberInfo).FieldType;
                case MemberTypes.Method:
                    return ((MethodInfo)memberInfo).ReturnType;
                case MemberTypes.Property:
                    return ((PropertyInfo)memberInfo).PropertyType;
                default:
                    throw new ArgumentException
                    (
                     "Input MemberInfo must be if type EventInfo, FieldInfo, MethodInfo, or PropertyInfo"
                    );
            }
        }

        /// <summary>
        /// Gets the underlying value.
        /// </summary>
        /// <param name="memberInfo">The member information.</param>
        /// <param name="target">The target.</param>
        /// <returns>System.Object.</returns>
        /// <exception cref="System.ArgumentException">Input MemberInfo must be if type FieldInfo, or PropertyInfo</exception>
        public static object GetUnderlyingValue(this MemberInfo memberInfo, object target)
        {
            switch (memberInfo.MemberType)
            {
                case MemberTypes.Field:
                    return ((FieldInfo)memberInfo).GetValue(target);
                case MemberTypes.Property:
                    return ((PropertyInfo)memberInfo).GetValue(target, null);
                default:
                    throw new ArgumentException
                    (
                     "Input MemberInfo must be if type FieldInfo, or PropertyInfo"
                    );
            }
        }

        /// <summary>
        /// Sets the underlying value.
        /// </summary>
        /// <param name="memberInfo">The member information.</param>
        /// <param name="target">The target.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        /// <exception cref="System.ArgumentException">Input MemberInfo must be if type FieldInfo, or PropertyInfo</exception>
        public static bool SetUnderlyingValue(this MemberInfo memberInfo, object target, object value)
        {
            switch (memberInfo.MemberType)
            {
                case MemberTypes.Field:
                    ((FieldInfo)memberInfo).SetValue(target, value);
                    return true;
                case MemberTypes.Property:
                    ((PropertyInfo)memberInfo).SetValue(target, value, null);
                    return true;
                default:
                    throw new ArgumentException
                    (
                     "Input MemberInfo must be if type FieldInfo, or PropertyInfo"
                    );
            }
        }

        /// <summary>
        /// Determines whether the specified information is public.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <returns><c>true</c> if the specified information is public; otherwise, <c>false</c>.</returns>
        public static bool IsPublic(this MemberInfo info)
        {
            if (info.MemberType == MemberTypes.Field)
            {
                return (info as FieldInfo).IsPublic;
            }
            else if (info.MemberType == MemberTypes.Property)
            {
                var GetMethod = (info as PropertyInfo).GetGetMethod();
                var SetMethod = (info as PropertyInfo).GetSetMethod();
                return GetMethod != null && SetMethod != null && GetMethod.IsPublic;
            }

            return false;
        }

        /// <summary>
        /// Determines whether the specified information is static.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <returns><c>true</c> if the specified information is static; otherwise, <c>false</c>.</returns>
        public static bool IsStatic(this MemberInfo info)
        {
            if (info.MemberType == MemberTypes.Field)
            {
                return (info as FieldInfo).IsStatic;
            }
            else if (info.MemberType == MemberTypes.Property)
            {
                var GetMethod = (info as PropertyInfo).GetGetMethod();
                var SetMethod = (info as PropertyInfo).GetSetMethod();
                return GetMethod != null && SetMethod != null && GetMethod.IsStatic;
            }

            return false;
        }

        /// <summary>
        /// Determines whether the specified information is constant.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <returns><c>true</c> if the specified information is constant; otherwise, <c>false</c>.</returns>
        public static bool IsConstant(this MemberInfo info)
        {
            return info is FieldInfo && (info as FieldInfo).IsInitOnly;
        }

        /// <summary>
        /// check if this type implement form target interface
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if the specified type is implement; otherwise, <c>false</c>.</returns>
        public static bool IsImplementFrom<T>(this Type type)
            where T : class
        {
            Debug.Assert(typeof(T).IsInterface);
            return type.GetInterfaces().Contains(typeof(T));
        }

        /// <summary>
        /// check if this type implement form target interface
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="interfaceType">Type of the interface.</param>
        /// <returns><c>true</c> if the specified interface type is implement; otherwise, <c>false</c>.</returns>
        public static bool IsImplementFrom(this Type type, Type interfaceType)
        {
            Debug.Assert(interfaceType.IsInterface);

            return type.GetInterfaces().Contains(interfaceType);
        }

        /// <summary>
        /// Determines whether [is child of] [the specified type].
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if [is child of] [the specified type]; otherwise, <c>false</c>.</returns>
        public static bool IsChildOf<T>(this Type type)
            where T : class
        {
            return typeof(T).IsAssignableFrom(type);
        }

        /// <summary>
        /// Determines whether [is child of] [the specified base type].
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="baseType">Type of the base.</param>
        /// <returns><c>true</c> if [is child of] [the specified base type]; otherwise, <c>false</c>.</returns>
        public static bool IsChildOf(this Type type, Type baseType)
        {
            return baseType != null && baseType.IsAssignableFrom(type);
        }

        /// <summary>
        /// Gets the type of the converter.
        /// </summary>
        /// <param name="attribute">The input attribute.</param>
        /// <returns>System.Type.</returns>
        public static System.Type GetConverterType(this System.ComponentModel.TypeConverterAttribute attribute)
        {
            Type Result = null;
            TryGetTypeByName(attribute.ConverterTypeName, out Result, AppDomain.CurrentDomain.GetAssemblies());
            return Result;
        }

        /// <summary>
        /// Gets the type associated with the specified name.
        /// </summary>
        /// <param name="typeName">Full name of the type.</param>
        /// <param name="type">The type.</param>
        /// <param name="customAssemblies">Additional loaded assemblies (optional).</param>
        /// <returns>Returns <c>true</c> if the type was found; otherwise <c>false</c>.</returns>
        private static bool TryGetTypeByName(string typeName, out Type type, params Assembly[] customAssemblies)
        {
            if (typeName.Contains("Version=")
                && !typeName.Contains("`"))
            {
                // remove full qualified assembly type name
                typeName = typeName.Substring(0, typeName.IndexOf(','));
            }

            type = Type.GetType(typeName);

            if (type == null)
            {
                type = GetTypeFromAssemblies(typeName, customAssemblies);
            }

            // try get generic types
            if (type == null
                && typeName.Contains("`"))
            {
                var match = Regex.Match(typeName, "(?<MainType>.+`(?<ParamCount>[0-9]+))\\[(?<Types>.*)\\]");

                if (match.Success)
                {
                    int genericParameterCount = int.Parse(match.Groups["ParamCount"].Value);
                    string genericDef = match.Groups["Types"].Value;
                    List<string> typeArgs = new List<string>(genericParameterCount);
                    foreach (Match typeArgMatch in Regex.Matches(genericDef, "\\[(?<Type>.*?)\\],?"))
                    {
                        if (typeArgMatch.Success)
                        {
                            typeArgs.Add(typeArgMatch.Groups["Type"].Value.Trim());
                        }
                    }

                    Type[] genericArgumentTypes = new Type[typeArgs.Count];
                    for (int genTypeIndex = 0; genTypeIndex < typeArgs.Count; genTypeIndex++)
                    {
                        Type genericType;
                        if (TryGetTypeByName(typeArgs[genTypeIndex], out genericType, customAssemblies))
                        {
                            genericArgumentTypes[genTypeIndex] = genericType;
                        }
                        else
                        {
                            // cant find generic type
                            return false;
                        }
                    }

                    string genericTypeString = match.Groups["MainType"].Value;
                    Type genericMainType;
                    if (TryGetTypeByName(genericTypeString, out genericMainType))
                    {
                        // make generic type
                        type = genericMainType?.MakeGenericType(genericArgumentTypes);
                    }
                }
            }

            return type != null;
        }

        /// <summary>
        /// Gets the type from assemblies.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <param name="customAssemblies">The custom assemblies.</param>
        /// <returns>Type.</returns>
        private static Type GetTypeFromAssemblies(string typeName, params Assembly[] customAssemblies)
        {
            Type type = null;

            if (customAssemblies != null
               && customAssemblies.Length > 0)
            {
                foreach (var assembly in customAssemblies)
                {
                    type = assembly.GetType(typeName);

                    if (type != null)
                        return type;
                }
            }

            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in loadedAssemblies)
            {
                type = assembly.GetType(typeName);

                if (type != null)
                    return type;
            }

            return type;
        }

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <returns>Type.</returns>
        public static Type GetType(string typeName)
        {
            var type = Type.GetType(typeName);
            if (type != null)
            {
                return type;
            }
            foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = a.GetType(typeName);
                if (type != null)
                {
                    return type;
                }
            }

            return null;
        }

        /// <summary>
        /// Determines whether [is numeric type] [the specified type].
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if [is numeric type] [the specified type]; otherwise, <c>false</c>.</returns>
        public static bool IsNumericType(this Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Gets the unique flags.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="flags">The flags.</param>
        /// <returns>IEnumerable&lt;T&gt;.</returns>
        public static IEnumerable<T> GetUniqueFlags<T>(this T flags)
            where T : Enum
        {
            foreach (Enum value in Enum.GetValues(flags.GetType()))
            {
                if (flags.HasFlag(value))
                {
                    yield return (T)value;
                }
            }
        }

        /// <summary>
        /// Gets the display name.
        /// </summary>
        /// <param name="propertyInfo">The property information.</param>
        /// <returns>System.String.</returns>
        public static string GetDisplayName(this PropertyInfo propertyInfo)
        {
            var attr = propertyInfo.GetAnyCustomAttribute<DisplayNameAttribute>();

            return attr?.DisplayName ?? propertyInfo.Name;
        }

        /// <summary>
        /// Determines whether [is read only] [the specified property information].
        /// </summary>
        /// <param name="propertyInfo">The property information.</param>
        /// <returns><c>true</c> if [is read only] [the specified property information]; otherwise, <c>false</c>.</returns>
        public static bool IsReadOnly(this PropertyInfo propertyInfo)
        {
            var attr = propertyInfo.GetAnyCustomAttribute<ReadOnlyAttribute>();

            return attr != null && attr.IsReadOnly;
        }

        /// <summary>
        /// Determines whether the specified property information is browsable.
        /// </summary>
        /// <param name="propertyInfo">The property information.</param>
        /// <returns><c>true</c> if the specified property information is browsable; otherwise, <c>false</c>.</returns>
        public static bool IsBrowsable(this PropertyInfo propertyInfo)
        {
            var attr = propertyInfo.GetAnyCustomAttribute<BrowsableAttribute>();

            return attr == null || attr.Browsable;
        }

        #region For ProeprtyDescriptor
        /// <summary>
        /// Determines whether the specified property descriptor is defiend.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyDescriptor">The property descriptor.</param>
        /// <returns><c>true</c> if the specified property descriptor is defiend; otherwise, <c>false</c>.</returns>
        public static bool IsDefined<T>(this PropertyDescriptor propertyDescriptor) where T : Attribute
        {
            foreach (var attr in propertyDescriptor.Attributes)
            {
                if (attr is T)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the custom attribute.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyDescriptor">The property descriptor.</param>
        /// <returns>T.</returns>
        public static T GetCustomAttribute<T>(this PropertyDescriptor propertyDescriptor) where T : Attribute
        {
            foreach (var attr in propertyDescriptor.Attributes)
            {
                if (attr is T t)
                {
                    return t;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the custom attributes.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyDescriptor">The property descriptor.</param>
        /// <returns>T[].</returns>
        public static T[] GetCustomAttributes<T>(this PropertyDescriptor propertyDescriptor) where T : Attribute
        {
            List<T> list = new List<T>();
            foreach (var attr in propertyDescriptor.Attributes)
            {
                if (attr is T t)
                {
                    list.Add(t);
                }
            }

            return list.ToArray();
        }

        /// <summary>
        /// Sets the and raise event.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="component">The component.</param>
        /// <param name="value">The value.</param>
        public static void SetAndRaiseEvent(this PropertyDescriptor property, object component, object value)
        {
            SetAndRaiseEvent(property, component, value, out _);
        }

        /// <summary>
        /// Sets the and raise event.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="component">The component.</param>
        /// <param name="value">The value.</param>
        /// <param name="oldValue">The old value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool SetAndRaiseEvent(this PropertyDescriptor property, object component, object value, out object oldValue)
        {
            if(!IsPropertyChanged(property, component, value, out oldValue))
            {
                return false;
            }

            if (component is ComponentModel.INotifyPropertyChanging npcp)
            {
                npcp.RaisePropertyChanging(property.Name);
            }
            else if (component is IEnumerable<ComponentModel.INotifyPropertyChanging> enpcps)
            {
                foreach (ComponentModel.INotifyPropertyChanging e in enpcps)
                {
                    e.RaisePropertyChanging(property.Name);
                }
            }

            property.SetValue(component, value);

            RaiseEvent(property, component);

            return true;
        }

        /// <summary>
        /// Determines whether [is property changed] [the specified component].
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="component">The component.</param>
        /// <param name="value">The value.</param>
        /// <param name="oldValue">The old value.</param>
        /// <returns><c>true</c> if [is property changed] [the specified component]; otherwise, <c>false</c>.</returns>
        public static bool IsPropertyChanged(this PropertyDescriptor property, object component, object value, out object oldValue)
        {
            var obj = property.GetValue(component);
            oldValue = obj;

            if (obj == null && value == null)
            {
                return false;
            }
            else if (obj != null && value != null && obj.Equals(value))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Raises the event.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="component">The component.</param>
        public static void RaiseEvent(this PropertyDescriptor property, object component)
        {
            if (component is ComponentModel.INotifyPropertyChanged npc)
            {
                npc.RaisePropertyChanged(property.Name);
            }
            else if (component is IEnumerable<ComponentModel.INotifyPropertyChanged> enpcps)
            {
                foreach (ComponentModel.INotifyPropertyChanged e in enpcps)
                {
                    e.RaisePropertyChanged(property.Name);
                }
            }
        }
        #endregion
    }

}

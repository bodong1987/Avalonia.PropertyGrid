using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace PropertyModels.Utils
{
    /// <summary>
    /// Represents cached method information for ICollection methods.
    /// </summary>
    internal class CollectionMethods
    {
        public MethodInfo? Add;
        public MethodInfo? Clear;
        public MethodInfo? Contains;
        public MethodInfo? Remove;
        public PropertyInfo? Count;
    }

    /// <summary>
    /// Represents cached method information for IList methods, extending ICollection.
    /// </summary>
    internal class CachedMethods : CollectionMethods
    {
        public MethodInfo? IndexOf;
        public MethodInfo? Insert;
        public MethodInfo? RemoveAt;
        public MethodInfo? GetItem;
        public MethodInfo? SetItem;
    }
    
    /// <summary>
    /// Provides methods to invoke generic IList/ICollection methods using reflection.
    /// </summary>
    public static class GenericInterfaceInvoker
    {
        private static readonly Dictionary<Type, CachedMethods> MethodCache = new();

        /// <summary>
        /// Gets or creates cached method information for a given type.
        /// </summary>
        /// <param name="type">The type to get method information for.</param>
        /// <returns>A <see cref="CachedMethods"/> object containing method information.</returns>
        private static CachedMethods GetOrCreateListMethod(Type type)
        {
            if (!MethodCache.TryGetValue(type, out var methods))
            {
                // Find IList<T> interface
                var iListType = type.GetInterfaces()
                    .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IList<>));
                
                Debug.Assert(iListType != null);

                // Find ICollection<T> interface
                var iCollectionType = type.GetInterfaces()
                    .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICollection<>));
                
                Debug.Assert(iCollectionType != null);

                methods = new CachedMethods
                {
                    IndexOf = iListType.GetMethod("IndexOf"),
                    Insert = iListType.GetMethod("Insert"),
                    RemoveAt = iListType.GetMethod("RemoveAt"),
                    GetItem = iListType.GetMethod("get_Item"),
                    SetItem = iListType.GetMethod("set_Item"),
                    Add = iCollectionType.GetMethod("Add"),
                    Clear = iCollectionType.GetMethod("Clear"),
                    Contains = iCollectionType.GetMethod("Contains"),
                    Remove = iCollectionType.GetMethod("Remove"),
                    Count = iCollectionType.GetProperty("Count")
                };

                MethodCache.Add(type, methods);
            }

            return methods;
        }

        /// <summary>
        /// Removes an item at a specified index from the list.
        /// </summary>
        /// <param name="list">The list from which to remove the item.</param>
        /// <param name="index">The index of the item to remove.</param>
        public static void RemoveAt(object list, int index)
        {
            var type = list.GetType();
            var methods = GetOrCreateListMethod(type);
            methods.RemoveAt?.Invoke(list, [index]);
        }

        /// <summary>
        /// Adds an item to the list.
        /// </summary>
        /// <param name="list">The list to which to add the item.</param>
        /// <param name="item">The item to add.</param>
        public static void Add(object list, object item)
        {
            var type = list.GetType();
            var methods = GetOrCreateListMethod(type);
            methods.Add?.Invoke(list, [item]);
        }

        /// <summary>
        /// Clears all items from the list.
        /// </summary>
        /// <param name="list">The list to clear.</param>
        public static void Clear(object list)
        {
            var type = list.GetType();
            var methods = GetOrCreateListMethod(type);
            methods.Clear?.Invoke(list, null);
        }

        /// <summary>
        /// Determines whether the list contains a specific item.
        /// </summary>
        /// <param name="list">The list to search.</param>
        /// <param name="item">The item to locate in the list.</param>
        /// <returns><c>true</c> if the item is found; otherwise, <c>false</c>.</returns>
        public static bool Contains(object list, object item)
        {
            var type = list.GetType();
            var methods = GetOrCreateListMethod(type);
            return (bool)(methods.Contains?.Invoke(list, [item]) ?? false);
        }

        /// <summary>
        /// Gets the index of a specific item in the list.
        /// </summary>
        /// <param name="list">The list to search.</param>
        /// <param name="item">The item to locate in the list.</param>
        /// <returns>The index of the item if found; otherwise, -1.</returns>
        public static int IndexOf(object list, object item)
        {
            var type = list.GetType();
            var methods = GetOrCreateListMethod(type);
            return (int)(methods.IndexOf?.Invoke(list, [item]) ?? -1);
        }

        /// <summary>
        /// Inserts an item at a specified index in the list.
        /// </summary>
        /// <param name="list">The list in which to insert the item.</param>
        /// <param name="index">The zero-based index at which the item should be inserted.</param>
        /// <param name="item">The item to insert.</param>
        public static void Insert(object list, int index, object item)
        {
            var type = list.GetType();
            var methods = GetOrCreateListMethod(type);
            methods.Insert?.Invoke(list, [index, item]);
        }

        /// <summary>
        /// Removes the first occurrence of a specific item from the list.
        /// </summary>
        /// <param name="list">The list from which to remove the item.</param>
        /// <param name="item">The item to remove.</param>
        /// <returns><c>true</c> if the item was successfully removed; otherwise, <c>false</c>.</returns>
        public static bool Remove(object list, object item)
        {
            var type = list.GetType();
            var methods = GetOrCreateListMethod(type);
            return (bool)(methods.Remove?.Invoke(list, [item]) ?? false);
        }

        /// <summary>
        /// Gets the number of items in the list.
        /// </summary>
        /// <param name="list">The list to count items in.</param>
        /// <returns>The number of items in the list.</returns>
        public static int Count(object list)
        {
            var type = list.GetType();
            var methods = GetOrCreateListMethod(type);
            return (int)(methods.Count?.GetValue(list) ?? 0);
        }

        /// <summary>
        /// Gets the item at a specified index in the list.
        /// </summary>
        /// <param name="list">The list to retrieve the item from.</param>
        /// <param name="index">The zero-based index of the item to retrieve.</param>
        /// <returns>The item at the specified index.</returns>
        public static object GetItem(object list, int index)
        {
            var type = list.GetType();
            var methods = GetOrCreateListMethod(type);
            return methods.GetItem?.Invoke(list, [index]) ??
                   throw new InvalidOperationException("Item not found.");
        }

        /// <summary>
        /// Sets the item at a specified index in the list.
        /// </summary>
        /// <param name="list">The list in which to set the item.</param>
        /// <param name="index">The zero-based index of the item to set.</param>
        /// <param name="item">The new value for the item at the specified index.</param>
        public static void SetItem(object list, int index, object item)
        {
            var type = list.GetType();
            var methods = GetOrCreateListMethod(type);
            methods.SetItem?.Invoke(list, [index, item]);
        }
    }
}
using System;

namespace PropertyModels.ComponentModel
{
    /// <summary>
    /// Class ObjectCreator.
    /// </summary>
    public static class ObjectCreator
    {
        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args">The arguments.</param>
        /// <returns>T.</returns>
        public static T? Create<T>(params object[] args) => (T?)Create(typeof(T), args);

        /// <summary>
        /// Creates the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>System.Object.</returns>
        public static object? Create(Type type, params object[] args)
        {
            if (type == typeof(string))
            {
                return "";
            }

            return type.IsAbstract ? null : Activator.CreateInstance(type, args);
        }
    }
}

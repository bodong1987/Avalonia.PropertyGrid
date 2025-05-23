using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using PropertyModels.ComponentModel.DataAnnotations;

namespace PropertyModels.ComponentModel
{
    /// <summary>
    /// Interface IReactiveObject
    /// </summary>
    public interface IReactiveObject : INotifyPropertyChanged;

    /// <summary>
    /// Class MiniReactiveObject.
    /// can't support property dependency
    /// Implements the <see cref="PropertyModels.ComponentModel.IReactiveObject" />
    /// </summary>
    /// <seealso cref="PropertyModels.ComponentModel.IReactiveObject" />
    public class MiniReactiveObject : IReactiveObject
    {
        #region Interfaces
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        [Browsable(false)]
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        public void RaisePropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion
    }


    /// <summary>
    /// Class ReactiveObject.
    /// </summary>
    public class ReactiveObject : MiniReactiveObject
    {
        #region Properties        
        [NonSerialized]
        private readonly Stack<string> _processStack = new();
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveObject"/> class.
        /// </summary>
        public ReactiveObject()
        {
            PropertyChanged += ProcessPropertyChanged;

            AutoCollectDependencyInfo();
        }

        [NonSerialized]
        private static readonly Dictionary<Type, Dictionary<string, List<string>>?> MetaCaches = [];

        /// <summary>
        /// Automatics the collect dependency information.
        /// </summary>
        private void AutoCollectDependencyInfo()
        {
            var type = GetType();
            if (MetaCaches.TryGetValue(type, out _))
            {
                return;
            }

            Dictionary<string, List<string>> cache = [];

            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                foreach (var attr in property.GetCustomAttributes<DependsOnPropertyAttribute>())
                {
                    foreach (var name in attr.DependencyProperties)
                    {
                        if (!cache.TryGetValue(name, out var relevance))
                        {
                            relevance = [];
                            cache.Add(name, relevance);
                        }

                        relevance.Add(property.Name);
                    }
                }
            }

            if (cache.Count > 0)
            {
                MetaCaches[type] = cache;
            }
            else
            {
                MetaCaches[type] = null;
            }
        }

        /// <summary>
        /// Gets the cache.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>Dictionary&lt;System.String, List&lt;System.String&gt;&gt;.</returns>
        private static Dictionary<string, List<string>>? GetCache(Type type)
        {
            _ = MetaCaches.TryGetValue(type, out var cache);
            return cache;
        }
        #endregion

        #region Methods                        
        /// <summary>
        /// Processes the property changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void ProcessPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (_processStack.Contains(e.PropertyName!))
            {
                return;
            }

            var relativeProperties = GetCache(GetType());

            if (relativeProperties != null && relativeProperties.TryGetValue(e.PropertyName!, out var relevance))
            {
                try
                {
                    _processStack.Push(e.PropertyName!);

                    foreach (var r in relevance)
                    {
                        RaisePropertyChanged(r);
                    }
                }
                finally
                {
                    _ = _processStack.Pop();
                }
            }
        }

        #endregion
    }

    /// <summary>
    /// Class ReactiveObjectExtensions.
    /// </summary>
    public static class ReactiveObjectExtensions
    {
        /// <summary>
        /// Raises event and set value if changed.
        /// </summary>
        /// <typeparam name="TObj">The type of the t object.</typeparam>
        /// <typeparam name="TRet">The type of the t ret.</typeparam>
        /// <param name="reactiveObject">The reactive object.</param>
        /// <param name="backingField">The backing field.</param>
        /// <param name="newValue">The new value.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>TRet.</returns>
        public static TRet RaiseAndSetIfChanged<TObj, TRet>(this TObj reactiveObject,
            ref TRet backingField,
            TRet newValue,
            [CallerMemberName] string? propertyName = null
            )
            where TObj : IReactiveObject
        {
            if (EqualityComparer<TRet>.Default.Equals(backingField, newValue))
            {
                return newValue;
            }

            backingField = newValue;
            reactiveObject.RaisePropertyChanged(propertyName!);

            return newValue;
        }

        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <typeparam name="TSender">The type of the t sender.</typeparam>
        /// <param name="reactiveObject">The reactive object.</param>
        /// <param name="propertyName">Name of the property.</param>
        public static void RaisePropertyChanged<TSender>(this TSender reactiveObject, [CallerMemberName] string? propertyName = null)
            where TSender : IReactiveObject
        {
            if (propertyName != null)
            {
                reactiveObject.RaisePropertyChanged(propertyName);
            }
        }
    }
}

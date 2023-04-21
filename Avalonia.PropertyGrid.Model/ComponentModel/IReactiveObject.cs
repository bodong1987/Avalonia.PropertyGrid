using Avalonia.PropertyGrid.Model.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.PropertyGrid.Model.ComponentModel
{
    /// <summary>
    /// Interface IReactiveObject
    /// Implements the <see cref="BluePrint.Common.ComponentModel.INotifyPropertyChanged" />
    /// Implements the <see cref="BluePrint.Common.ComponentModel.INotifyPropertyChanging" />
    /// </summary>
    /// <seealso cref="BluePrint.Common.ComponentModel.INotifyPropertyChanged" />
    /// <seealso cref="BluePrint.Common.ComponentModel.INotifyPropertyChanging" />
    public interface IReactiveObject : INotifyPropertyChanged
    {
    }

    /// <summary>
    /// Class MiniReactiveObject.
    /// can't support property dependency
    /// Implements the <see cref="Avalonia.PropertyGrid.Model.ComponentModel.IReactiveObject" />
    /// </summary>
    /// <seealso cref="Avalonia.PropertyGrid.Model.ComponentModel.IReactiveObject" />
    public class MiniReactiveObject : IReactiveObject
    {
        #region Interfaces
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        [Browsable(false)]
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }


    /// <summary>
    /// Class ReactiveObject.
    /// Implements the <see cref="BluePrint.Common.ComponentModel.INotifyPropertyChanged" />
    /// </summary>
    /// <seealso cref="BluePrint.Common.ComponentModel.INotifyPropertyChanged" />
    public class ReactiveObject : MiniReactiveObject
    {
        #region Properties        
        private Stack<string> ProcessStack = new Stack<string>();
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveObject"/> class.
        /// </summary>
        public ReactiveObject()
        {
            PropertyChanged += ProcessPropertyChanged;

            AutoCollectDependsInfo();
        }

        private static Dictionary<System.Type, Dictionary<string, List<string>>> MetaCaches = new Dictionary<Type, Dictionary<string, List<string>>>();

        /// <summary>
        /// Automatics the collect depends information.
        /// </summary>
        private void AutoCollectDependsInfo()
        {
            var type = GetType();
            if (MetaCaches.TryGetValue(type, out var cache))
            {
                return;
            }

            cache = new Dictionary<string, List<string>>();

            foreach (var property in type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
            {
                foreach (var attr in property.GetCustomAttributes<DependsOnPropertyAttribute>())
                {
                    foreach (var name in attr.DependencyProperties)
                    {
                        if (!cache.TryGetValue(name, out var relevance))
                        {
                            relevance = new List<string>();
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
        private static Dictionary<string, List<string>> GetCache(Type type)
        {
            MetaCaches.TryGetValue(type, out var cache);
            return cache;
        }
        #endregion

        #region Methods                        
        /// <summary>
        /// Processes the property changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void ProcessPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (ProcessStack.Contains(e.PropertyName))
            {
                return;
            }

            var RelativeProperties = GetCache(GetType());

            if (RelativeProperties != null && RelativeProperties.TryGetValue(e.PropertyName, out var relevance))
            {
                try
                {
                    ProcessStack.Push(e.PropertyName);

                    foreach (var r in relevance)
                    {
                        RaisePropertyChanged(r);
                    }
                }
                finally
                {
                    ProcessStack.Pop();
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
        /// Raises the and set if changed.
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
            [CallerMemberName] string propertyName = null
            )
            where TObj : IReactiveObject
        {
            if (EqualityComparer<TRet>.Default.Equals(backingField, newValue))
            {
                return newValue;
            }

            backingField = newValue;
            reactiveObject.RaisePropertyChanged(propertyName);

            return newValue;
        }

        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <typeparam name="TSender">The type of the t sender.</typeparam>
        /// <param name="reactiveObject">The reactive object.</param>
        /// <param name="propertyName">Name of the property.</param>
        public static void RaisePropertyChanged<TSender>(this TSender reactiveObject, [CallerMemberName] string propertyName = null)
            where TSender : IReactiveObject
        {
            if (propertyName != null)
            {
                reactiveObject.RaisePropertyChanged(propertyName);
            }
        }
    }
}

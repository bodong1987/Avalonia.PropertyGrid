using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.PropertyGrid.Model.Extensions;
using Avalonia.PropertyGrid.Model.ComponentModel.DataAnnotations;

namespace Avalonia.PropertyGrid.Controls.Factories
{
    /// <summary>
    /// Class AbstractCellEditFactory.
    /// Implements the <see cref="Avalonia.PropertyGrid.Controls.ICellEditFactory" />
    /// </summary>
    /// <seealso cref="Avalonia.PropertyGrid.Controls.ICellEditFactory" />
    public abstract class AbstractCellEditFactory : ICellEditFactory
    {
        /// <summary>
        /// Gets the import priority.
        /// The larger the value, the earlier the object will be processed
        /// </summary>
        /// <value>The import priority.</value>
        [Browsable(false)]
        public virtual int ImportPriority => 100;

        /// <summary>
        /// Gets or sets the collection.
        /// </summary>
        /// <value>The collection.</value>
        ICellEditFactoryCollection ICellEditFactory.Collection { get; set; }

        /// <summary>
        /// Check available for target
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public virtual bool Accept(object accessToken)
        {
            return accessToken is PropertyGrid;
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>ICellEditFactory.</returns>
        public virtual ICellEditFactory Clone()
        {
            return Activator.CreateInstance(GetType()) as ICellEditFactory;
        }

        /// <summary>
        /// Handles the new property.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="propertyDescriptor">The property descriptor.</param>
        /// <returns>Control.</returns>
        public abstract Control HandleNewProperty(object target, PropertyDescriptor propertyDescriptor);

        /// <summary>
        /// Handles the property changed.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="propertyDescriptor">The property descriptor.</param>
        /// <param name="control">The control.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public abstract bool HandlePropertyChanged(object target, PropertyDescriptor propertyDescriptor, Control control);

        /// <summary>
        /// Sets the and raise.
        /// </summary>
        /// <param name="sourceControl">The source control.</param>
        /// <param name="propertyDescriptor">The property descriptor.</param>
        /// <param name="component">The component.</param>
        /// <param name="value">The value.</param>
        protected virtual void SetAndRaise(Control sourceControl, PropertyDescriptor propertyDescriptor, object component, object value) 
        {
            DataValidationErrors.ClearErrors(sourceControl);

            try
            {                
                propertyDescriptor.SetAndRaiseEvent(component, value);

                ValidateProperty(sourceControl, propertyDescriptor, component);
            }
            catch(Exception e)
            {
                DataValidationErrors.SetErrors(sourceControl, new object[] {e.Message});
            }
        }

        /// <summary>
        /// Validates the property.
        /// </summary>
        /// <param name="sourceControl">The source control.</param>
        /// <param name="propertyDescriptor">The property descriptor.</param>
        /// <param name="component">The component.</param>
        protected virtual void ValidateProperty(Control sourceControl, PropertyDescriptor propertyDescriptor, object component)
        {
            if (!ValidatorUtils.TryValidateProperty(component, propertyDescriptor, out var message))
            {
                DataValidationErrors.SetErrors(sourceControl, new object[] { message });
            }
            else
            {
                DataValidationErrors.ClearErrors(sourceControl);
            }
        }
    }
}

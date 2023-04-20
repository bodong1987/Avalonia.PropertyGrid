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
    public abstract class AbstractPropertyGridFactory : IPropertyGridControlFactory
    {
        /// <summary>
        /// Gets the import priority.
        /// The larger the value, the earlier the object will be processed
        /// </summary>
        /// <value>The import priority.</value>
        public virtual int ImportPriority => 100;

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
        /// <returns>IPropertyGridControlFactory.</returns>
        public virtual IPropertyGridControlFactory Clone()
        {
            return Activator.CreateInstance(GetType()) as IPropertyGridControlFactory;
        }

        /// <summary>
        /// Handles the new property.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="target">The target.</param>
        /// <param name="propertyDescriptor">The property descriptor.</param>
        /// <returns>Control.</returns>
        public abstract Control HandleNewProperty(PropertyGrid parent, object target, PropertyDescriptor propertyDescriptor);

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

                if(!ValidatorUtils.TryValidateProperty(component, propertyDescriptor, out var message))
                {
                    DataValidationErrors.SetErrors(sourceControl, new object[] { message });
                }
            }
            catch(Exception e)
            {
                DataValidationErrors.SetErrors(sourceControl, new object[] {e.Message});
            }
        }
    }
}

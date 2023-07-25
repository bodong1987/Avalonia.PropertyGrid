using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.PropertyGrid.Model.Extensions;
using Avalonia.PropertyGrid.Model.ComponentModel.DataAnnotations;
using Avalonia.PropertyGrid.Model.ComponentModel;
using Avalonia.PropertyGrid.ViewModels;
using Avalonia.Interactivity;

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
        /// <param name="context">The context.</param>
        /// <returns>Control.</returns>
        public abstract Control HandleNewProperty(PropertyCellContext context);

        /// <summary>
        /// Handles the property changed.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public abstract bool HandlePropertyChanged(PropertyCellContext context);

        /// <summary>
        /// Sets the and raise.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="sourceControl">The source control.</param>
        /// <param name="value">The value.</param>
        protected virtual void SetAndRaise(PropertyCellContext context, Control sourceControl, object value) 
        {
            if(context.Property.IsPropertyChanged(context.Target, value, out var oldValue))
            {
                GenericCancelableCommand command = new GenericCancelableCommand(
                    string.Format(PropertyGrid.LocalizationService["Change {0} form {1} to {2}"], context.Property.DisplayName, oldValue != null ? oldValue.ToString() : "null", value != null ? value.ToString() : "null"),
                    () =>
                    {
                        HandleSetValue(sourceControl, context, value);
                        return true;
                    },
                    () =>
                    {
                        HandleSetValue(sourceControl, context, oldValue);
                        return true;
                    }
                    )
                {
                    Tag = "CommonEvent"
                };

                ExecuteCommand(command, context, oldValue, value, value);
            }
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="propertyContext">The property context.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="value">The value.</param>
        /// <param name="context">The context.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        protected virtual bool ExecuteCommand(ICancelableCommand command, PropertyCellContext propertyContext, object oldValue, object value, object context)
        {
            RoutedCommandExecutingEventArgs evt = new RoutedCommandExecutingEventArgs(
                    PropertyGrid.CommandExecutingEvent,
                    command,
                    propertyContext.Target,
                    propertyContext.Property,
                    oldValue,
                    value,
                    context
                    );

            (propertyContext.Owner as Interactive)?.RaiseEvent(evt);

            if(evt.Canceled)
            {
                return false;
            }

            if (propertyContext.Target is INotifyCommandExecuting nce)
            {
                CommandExecutingEventArgs args = new CommandExecutingEventArgs(command, propertyContext.Target, propertyContext.Property, oldValue, value, context);

                nce.RaiseCommandExecuting(args);

                if (args.Cancel)
                {
                    return false;
                }
            }

            if(!command.Execute())
            {
                return false;
            }

            RoutedCommandExecutedEventArgs evt2 = new RoutedCommandExecutedEventArgs(
                PropertyGrid.CommandExecutedEvent,
                command,
                propertyContext.Target,
                propertyContext.Property,
                oldValue,
                value,
                context);

            (propertyContext.Owner as Interactive)?.RaiseEvent(evt2);

            return true;
        }

        /// <summary>
        /// Handles the set value.
        /// </summary>
        /// <param name="sourceControl">The source control.</param>
        /// <param name="context">The context.</param>
        /// <param name="value">The value.</param>
        protected virtual void HandleSetValue(Control sourceControl, PropertyCellContext context, object value)
        {
            DataValidationErrors.ClearErrors(sourceControl);

            try
            {
                context.Property.SetAndRaiseEvent(context.Target, value);

                ValidateProperty(sourceControl, context.Property, context.Target);
            }
            catch (Exception e)
            {
                DataValidationErrors.SetErrors(sourceControl, new object[] { e.Message });
            }
        }

        /// <summary>
        /// Handles the raise event.
        /// </summary>
        /// <param name="sourceControl">The source control.</param>
        /// <param name="context">The context.</param>
        protected virtual void HandleRaiseEvent(Control sourceControl, PropertyCellContext context)
        {
            DataValidationErrors.ClearErrors(sourceControl);

            try
            {
                context.Property.RaiseEvent(context.Target);

                ValidateProperty(sourceControl, context.Property, context.Target);
            }
            catch (Exception e)
            {
                DataValidationErrors.SetErrors(sourceControl, new object[] { e.Message });
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

        /// <summary>
        /// Handles the propagate visibility.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="propertyDescriptor">The property descriptor.</param>
        /// <param name="control">The control.</param>
        /// <param name="filterContext">The filter context.</param>
        /// <returns>System.Nullable&lt;PropertyVisibility&gt;.</returns>
        public virtual PropertyVisibility? HandlePropagateVisibility(object target, PropertyDescriptor propertyDescriptor, Control control, IPropertyGridFilterContext filterContext)
        {
            return null;
        }
    }
}

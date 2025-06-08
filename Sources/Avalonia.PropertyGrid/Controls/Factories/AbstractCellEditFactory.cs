using System;
using System.ComponentModel;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.PropertyGrid.Services;
using Avalonia.PropertyGrid.ViewModels;
using PropertyModels.ComponentModel;
using PropertyModels.ComponentModel.DataAnnotations;
using PropertyModels.Extensions;

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
        ICellEditFactoryCollection? ICellEditFactory.Collection { get; set; }

        /// <summary>
        /// Check available for target
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        /// <returns><c>true</c> if success, <c>false</c> otherwise.</returns>
        public virtual bool Accept(object accessToken) => accessToken is PropertyGrid;

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>ICellEditFactory.</returns>
        public virtual ICellEditFactory? Clone() => Activator.CreateInstance(GetType()) as ICellEditFactory;

        /// <summary>
        /// Handles the new property.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>Control.</returns>
        public abstract Control? HandleNewProperty(PropertyCellContext context);

        /// <summary>
        /// Handles the property changed.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns><c>true</c> if success, <c>false</c> otherwise.</returns>
        public abstract bool HandlePropertyChanged(PropertyCellContext context);

        /// <summary>
        /// Handles readonly flag changed
        /// </summary>
        /// <param name="control">control.</param>
        /// <param name="readOnly">readonly flag</param>
        /// <returns>Control.</returns>
        public virtual void HandleReadOnlyStateChanged(Control control, bool readOnly)
        {
            // default operation, set IsEnabled flag
            // you can override this method to change this default operation
            control.IsEnabled = !readOnly;
        }

        /// <inheritdoc />
        public virtual void SetPropertyValue(PropertyCellContext context, object? value)
        {
            SetAndRaise(context, context.CellEdit!, value, context.GetValue());
        }

        /// <summary>
        /// Gets the property value.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>System.Nullable&lt;System.Object&gt;.</returns>
        public virtual object? GetPropertyValue(PropertyCellContext context)
        {
            return context.GetValue();
        }

        /// <summary>
        /// make child factory can override property changed testing
        /// </summary>
        /// <param name="context"></param>
        /// <param name="value"></param>
        /// <param name="oldValue"></param>
        /// <returns></returns>
        protected virtual bool CheckIsPropertyChanged(PropertyCellContext context, object? value, object? oldValue)
        {
            return context.Property.IsPropertyChanged(context.Target, value, oldValue);
        }

        /// <summary>
        /// Sets and raise.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="sourceControl">The source control.</param>
        /// <param name="value">The value.</param>
        protected void SetAndRaise(PropertyCellContext context, Control sourceControl, object? value)
        {
            SetAndRaise(context, sourceControl, value, context.GetValue());
        }

        /// <summary>
        /// Sets and raise.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="sourceControl">The source control.</param>
        /// <param name="value">The value.</param>
        /// <param name="oldValue">old value</param>
        protected virtual void SetAndRaise(PropertyCellContext context, Control sourceControl, object? value, object? oldValue)
        {
            // If the cell value has changed compared to the associated value...
            if (CheckIsPropertyChanged(context, value, oldValue))
            {
                // ...we execute the 'HandleSetValue' command to update the associated value and the view.
                var command = new GenericCancelableCommand(
                    string.Format(LocalizationService.Default["Change {0} form {1} to {2}"], context.Property.DisplayName, oldValue != null ? oldValue.ToString() : "null", value != null ? value.ToString() : "null"),
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

                _ = ExecuteCommand(command, context, oldValue, value, value);
            }
            else
            {
                // ...otherwise we validate the associated property value (again).
                //
                // Info: This is necessary so that any existing validation messages are updated correctly.
                ValidateProperty(sourceControl, context.Property, context.Target);
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
        /// <returns><c>true</c> if success, <c>false</c> otherwise.</returns>
        protected virtual bool ExecuteCommand(ICancelableCommand command, PropertyCellContext propertyContext, object? oldValue, object? value, object? context)
        {
            var evt = new RoutedCommandExecutingEventArgs(
                    PropertyGrid.CommandExecutingEvent,
                    command,
                    propertyContext.Target,
                    propertyContext.Property,
                    oldValue,
                    value,
                    context);

            (propertyContext.Owner as Interactive)?.RaiseEvent(evt);

            if (evt.Canceled)
            {
                return false;
            }

            if (propertyContext.Target is INotifyCommandExecuting nce)
            {
                var args = new CommandExecutingEventArgs(command, propertyContext.Target, propertyContext.Property, oldValue, value, context);

                nce.RaiseCommandExecuting(args);

                if (args.Cancel)
                {
                    return false;
                }
            }

            if (!command.Execute())
            {
                return false;
            }

            var evt2 = new RoutedCommandExecutedEventArgs(
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
        protected virtual void HandleSetValue(Control sourceControl, PropertyCellContext context, object? value)
        {
            DataValidationErrors.ClearErrors(sourceControl);

            try
            {
                context.Property.SetAndRaiseEvent(context.Target, value);

                ValidateProperty(sourceControl, context.Property, context.Target);
            }
            catch (Exception e)
            {
                DataValidationErrors.SetErrors(sourceControl, [e.Message]);
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
                DataValidationErrors.SetErrors(sourceControl, [e.Message]);
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
                DataValidationErrors.SetErrors(sourceControl, [LocalizationService.Default[message]]);
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
        /// <param name="context">The context.</param>
        /// <param name="filterContext">The filter context.</param>
        /// <param name="filterText">The filter text.</param>
        /// <param name="filterMatchesParentCategory">Indicates whether the filter matches the parent category.</param>
        /// <returns>System.Nullable&lt;PropertyVisibility&gt;.</returns>
        public virtual PropertyVisibility? HandlePropagateVisibility(
            object? target, 
            PropertyCellContext context,
            IPropertyGridFilterContext filterContext,
            string? filterText = null,
            bool filterMatchesParentCategory = false) => null;

        /// <summary>
        /// Checks the equal.
        /// </summary>
        /// <param name="first">The first.</param>
        /// <param name="second">The second.</param>
        /// <returns><c>true</c> if success, <c>false</c> otherwise.</returns>
        protected virtual bool CheckEqual(object? first, object? second)
        {
            if (first == null && second == null)
            {
                return true;
            }

            if (first == null || second == null)
            {
                return false;
            }

            return first.Equals(second);
        }

        /// <summary>
        /// Checks the equals.
        /// </summary>
        /// <param name="first">The first.</param>
        /// <param name="second">The second.</param>
        /// <returns><c>true</c> if success, <c>false</c> otherwise.</returns>
        protected virtual bool CheckEquals(object[]? first, object[]? second)
        {
            if (first == null && second == null)
            {
                return true;
            }

            if (first == null || second == null)
            {
                return false;
            }

            if (first.Length == second.Length)
            {
                for (var i = 0; i < first.Length; i++)
                {
                    if (!first[i].Equals(second[i]))
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Arrays to string.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <returns>System.String.</returns>
        protected virtual string ArrayToString(object[]? array)
        {
            if (array == null || array.Length == 0)
            {
                return string.Empty;
            }

            return string.Join(", ", array.Select(x => x.ToString()));
        }
    }
}

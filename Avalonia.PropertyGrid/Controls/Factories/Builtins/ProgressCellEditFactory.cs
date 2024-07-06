﻿using Avalonia.Controls;
using PropertyModels.ComponentModel;
using PropertyModels.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Avalonia.PropertyGrid.Controls.Factories.Builtins
{
    /// <summary>
    /// Class ProgressCellEditFactory.
    /// Implements the <see cref="Avalonia.PropertyGrid.Controls.Factories.AbstractCellEditFactory" />
    /// </summary>
    /// <seealso cref="Avalonia.PropertyGrid.Controls.Factories.AbstractCellEditFactory" />
    public class ProgressCellEditFactory : AbstractCellEditFactory
    {
        /// <summary>
        /// Gets the import priority.
        /// The larger the value, the earlier the object will be processed
        /// </summary>
        /// <value>The import priority.</value>
        public override int ImportPriority => base.ImportPriority - 900000;

        /// <summary>
        /// Handles the new property.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>Control.</returns>
        public override Control HandleNewProperty(PropertyCellContext context)
        {
            var propertyDescriptor = context.Property;
            var target = context.Target;

            if (propertyDescriptor.PropertyType != typeof(double) || !propertyDescriptor.IsDefined<ProgressAttribute>())
            {
                return null;
            }

            ProgressBar control = new ProgressBar();

            var attr = propertyDescriptor.GetCustomAttribute<ProgressAttribute>();
            control.Minimum = attr.Minimum; 
            control.Maximum = attr.Maximum;
            control.ProgressTextFormat = attr.FormatString;
            control.ShowProgressText = attr.ShowProgressText;

            return control;
        }

        /// <summary>
        /// Handles the property changed.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns><c>true</c> if success, <c>false</c> otherwise.</returns>
        public override bool HandlePropertyChanged(PropertyCellContext context)
        {
            var propertyDescriptor = context.Property;
            var target = context.Target;
            var control = context.CellEdit;

            if (propertyDescriptor.PropertyType != typeof(double) || !propertyDescriptor.IsDefined<ProgressAttribute>())
            {
                return false;
            }

            ValidateProperty(control, propertyDescriptor, target);

            if (control is ProgressBar progressBar)
            {
                double value = (double)propertyDescriptor.GetValue(target);

                progressBar.Value = value;

                return true;
            }

            return false;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyModels.Extensions;

namespace PropertyModels.ComponentModel.DataAnnotations
{
    /// <summary>
    /// Class ValidatorUtils.
    /// </summary>
    public static class ValidatorUtils
    {
        /// <summary>
        /// Tries the validate object.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="message">The message.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool TryValidateObject(object target, out string message)
        {
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(target))
            {
                if (property.IsDefined<ValidationAttribute>())
                {
                    List<ValidationResult> Results = new List<ValidationResult>();
                    var value = property.GetValue(target);
                    if (!Validator.TryValidateValue(
                        value,
                        new ValidationContext(value)
                        {
                            DisplayName = property.DisplayName,
                            MemberName = property.Name
                        },
                        Results,
                        property.GetCustomAttributes<ValidationAttribute>()
                        ))
                    {
                        message = string.Join(Environment.NewLine, Results.Select(x => x.GetDisplayMessage()));
                        return false;
                    }
                }
            }

            message = string.Empty;
            return true;
        }

        /// <summary>
        /// Tries the validate property.
        /// </summary>
        /// <param name="component">The component.</param>
        /// <param name="property">The property.</param>
        /// <param name="message">The message.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool TryValidateProperty(object component, PropertyDescriptor property, out string message)
        {
            if (property.IsDefined<ValidationAttribute>())
            {
                List<ValidationResult> Results = new List<ValidationResult>();
                var value = property.GetValue(component);
                if (!Validator.TryValidateValue(
                    value,
                    new ValidationContext(value)
                    {
                        DisplayName = property.DisplayName,
                        MemberName = property.Name
                    },
                    Results,
                    property.GetCustomAttributes<ValidationAttribute>()
                    ))
                {
                    message = string.Join(Environment.NewLine, Results.Select(x => x.GetDisplayMessage()));
                    return false;
                }
            }

            message = string.Empty;

            return true;
        }
    }
}

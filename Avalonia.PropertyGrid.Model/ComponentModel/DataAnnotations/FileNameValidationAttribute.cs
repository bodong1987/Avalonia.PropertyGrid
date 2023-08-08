using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PropertyModels.ComponentModel.DataAnnotations
{
    /// <summary>
    /// Class FileNameValidationAttribute.
    /// Implements the <see cref="ValidationAttribute" />
    /// </summary>
    /// <seealso cref="ValidationAttribute" />
    public class FileNameValidationAttribute : ValidationAttribute
    {
        /// <summary>
        /// Validates the specified value with respect to the current validation attribute.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="validationContext">The context information about the validation operation.</param>
        /// <returns>An instance of the <see cref="T:System.ComponentModel.DataAnnotations.ValidationResult" /> class.</returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is string name)
            {
                if (IsValidFileName(name))
                {
                    return ValidationResult.Success;
                }

                return new ValidationResult($"[{validationContext.DisplayName}]{name} is not an valid file name.");
            }

            return new ValidationResult($"{validationContext.DisplayName} is not an valid file name.");
        }

        /// <summary>
        /// Determines whether [is valid file name] [the specified filename].
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns><c>true</c> if [is valid file name] [the specified filename]; otherwise, <c>false</c>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsValidFileName(string filename)
        {
            return !(filename.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) >= 0);
        }
    }
}

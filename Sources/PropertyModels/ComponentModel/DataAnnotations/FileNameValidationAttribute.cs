using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Runtime.CompilerServices;

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
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is string name)
            {
                return IsValidFileName(name) ? ValidationResult.Success : new ValidationResult($"[{validationContext.DisplayName}]{name} is not an valid file name.");
            }

            return new ValidationResult($"{validationContext.DisplayName} is not an valid file name.");
        }

        /// <summary>
        /// Determines whether [is valid file name] [the specified filename].
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns><c>true</c> if [is valid file name] [the specified filename]; otherwise, <c>false</c>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValidFileName(string filename) => !(filename.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0);
    }
}

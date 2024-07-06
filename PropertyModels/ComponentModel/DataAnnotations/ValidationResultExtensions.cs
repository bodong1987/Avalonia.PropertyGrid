using PropertyModels.Extensions;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace PropertyModels.ComponentModel.DataAnnotations
{
    /// <summary>
    /// Class ValidationResultExtensions.
    /// </summary>
    public static class ValidationResultExtensions
    {
        /// <summary>
        /// Determines whether the specified validation result is success.
        /// </summary>
        /// <param name="validationResult">The validation result.</param>
        /// <returns><c>true</c> if the specified validation result is success; otherwise, <c>false</c>.</returns>
        public static bool IsSuccess(this ValidationResult validationResult)
        {
            return !IsFailure(validationResult);
        }

        /// <summary>
        /// Determines whether the specified validation result is failure.
        /// </summary>
        /// <param name="validationResult">The validation result.</param>
        /// <returns><c>true</c> if the specified validation result is failure; otherwise, <c>false</c>.</returns>
        public static bool IsFailure(this ValidationResult validationResult)
        {
            return validationResult != null && validationResult != ValidationResult.Success && validationResult.ErrorMessage.IsNotNullOrEmpty();
        }

        /// <summary>
        /// Gets the display message.
        /// </summary>
        /// <param name="validationResult">The validation result.</param>
        /// <returns>string.</returns>
        public static string GetDisplayMessage(this ValidationResult validationResult)
        {
            if (validationResult == null)
            {
                return "Success";
            }

            if (validationResult.MemberNames.Count() > 0)
            {
                return $"{validationResult.ErrorMessage}:{string.Join(",", validationResult.MemberNames)}";
            }

            return validationResult.ErrorMessage;
        }
    }
}

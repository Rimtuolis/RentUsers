
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace AuthorizationAPI.Core.Requests.Validators
{
    public class IdentifierValidatorAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult("Nenurodytas identifikatorius");
            }
            var regex = new Regex("^[a-fA-F0-9]{24}$");
            return !regex.IsMatch(value.ToString()!) ? new ValidationResult("Neteisingas identifikatorius") : ValidationResult.Success;
        }

    }
}

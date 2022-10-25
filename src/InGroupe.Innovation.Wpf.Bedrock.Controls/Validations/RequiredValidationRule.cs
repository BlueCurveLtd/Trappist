using System.Globalization;
using System.Windows.Controls;

namespace InGroupe.Innovation.Wpf.Bedrock.Controls.Validations
{
    public sealed class RequiredValidationRule : ValidationRule
    {
        public ushort? MinLength { get; set; }

        public RequiredValidationRule()
        {

        }

        public RequiredValidationRule(ValidationStep validationStep, bool validatesOnTargetUpdated)
            : base(validationStep, validatesOnTargetUpdated)
        {

        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value is string stringValue && !string.IsNullOrWhiteSpace(stringValue))
            {
                if (this.MinLength is not null && stringValue.Length >= this.MinLength!)
                {
                    return new ValidationResult(true, null);
                }

                if (this.MinLength is null)
                {
                    return new ValidationResult(true, null);
                }
            }

            return new ValidationResult(false, "Required Field Validation");
        }
    }
}

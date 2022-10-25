using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows.Controls;

namespace InGroupe.Innovation.Wpf.Bedrock.Controls.Validations
{
    public sealed class DateOnlyValidationRule : ValidationRule
    {
        [DisallowNull]
        public string Format { get; set; } = "dd/MM/yyyy";

        public DateOnlyValidationRule()
        {

        }

        public DateOnlyValidationRule(ValidationStep validationStep, bool validatesOnTargetUpdated)
            : base(validationStep, validatesOnTargetUpdated)
        {

        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (DateTime.TryParseExact(value as string, this.Format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var _))
            {
                return new ValidationResult(true, null);
            }

            return new ValidationResult(false, "Required Field Validation");
        }
    }
}

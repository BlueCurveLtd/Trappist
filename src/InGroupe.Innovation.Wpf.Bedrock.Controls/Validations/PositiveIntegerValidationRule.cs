using System.Globalization;
using System.Windows.Controls;

namespace InGroupe.Innovation.Wpf.Bedrock.Controls.Validations
{
    public sealed class PositiveIntegerValidationRule : ValidationRule
    {
        public PositiveIntegerValidationRule()
        {

        }

        public PositiveIntegerValidationRule(ValidationStep validationStep, bool validatesOnTargetUpdated)
            : base(validationStep, validatesOnTargetUpdated)
        {

        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var convertedValue = TryConvert(value);

            if (convertedValue is > 0UL)
            {
                return new ValidationResult(true, null);
            }

            return new ValidationResult(false, "Required Field Validation");
        }

        private static ulong? TryConvert(object value) => value switch
        {
            short s => (ulong)s,
            int i => (ulong)i,
            long l => (ulong)l,
            ushort us => us,
            uint ui => ui,
            ulong ul => ul,
            _ => null
        };
    }
}

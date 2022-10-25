using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows.Controls;

namespace InGroupe.Innovation.Wpf.Bedrock.Controls.Validations
{
    public class DateTimeValidationRule : ValidationRule
    {
        private const string ISO8601Pattern = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffffffK";

        public bool UseCurrentCulture { get; set; }

        [AllowNull]
        [MaybeNull]
        public CultureInfo? Culture { get; set; }

        public virtual bool IsStandard { get; set; } = true;

        [AllowNull]
        [MaybeNull]
        public string? Format { get; set; }

        public DateTimeValidationRule()
        {

        }

        public DateTimeValidationRule(ValidationStep validationStep, bool validatesOnTargetUpdated)
            : base(validationStep, validatesOnTargetUpdated)
        {

        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value is string stringValue && !string.IsNullOrWhiteSpace(stringValue))
            {
                
                if (this.IsStandard && DateTime.TryParseExact(stringValue, ISO8601Pattern, CultureInfo.InvariantCulture, DateTimeStyles.None, out var _))
                {
                    return new ValidationResult(true, null);
                }

                if (!this.IsStandard && !string.IsNullOrWhiteSpace(this.Format) && DateTime.TryParseExact(stringValue, this.Format, this.Culture ?? CultureInfo.InvariantCulture, DateTimeStyles.None, out var _))
                {
                    return new ValidationResult(true, null);
                }

                if (!this.IsStandard && string.IsNullOrWhiteSpace(this.Format) && this.Culture != null && DateTime.TryParseExact(stringValue, this.Culture.DateTimeFormat.LongDatePattern, this.Culture, DateTimeStyles.None, out var _))
                {
                    return new ValidationResult(true, null);
                }
            }

            return new ValidationResult(false, "Required Field Validation");
        }
    }
}

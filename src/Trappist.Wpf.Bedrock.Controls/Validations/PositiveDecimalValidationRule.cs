using System.Globalization;
using System.Windows.Controls;

namespace Trappist.Wpf.Bedrock.Controls.Validations;

public sealed class PositiveDecimalValidationRule : ValidationRule
{
    public PositiveDecimalValidationRule()
    {

    }

    public PositiveDecimalValidationRule(ValidationStep validationStep, bool validatesOnTargetUpdated)
        : base(validationStep, validatesOnTargetUpdated)
    {

    }

    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        var convertedValue = TryConvert(value);

        if (convertedValue is > 0M)
        {
            return new ValidationResult(true, null);
        }

        return new ValidationResult(false, "Required Field Validation");
    }

    private static decimal? TryConvert(object value) => value switch
    {
        float single => new decimal(single),
        double d => new decimal(d),
        decimal m => m,
        _ => null
    };
}

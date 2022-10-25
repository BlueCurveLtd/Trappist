using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace InGroupe.Innovation.Wpf.Bedrock.DataAnnotations;

public sealed class RequiredIfAttribute : ValidationAttribute
{
    /// <summary>
    /// The property to observe.
    /// </summary>
    public string? PropertyName { get; set; }

    /// <summary>
    /// The desired value of the property to observe.
    /// </summary>
    public object? DesiredValue { get; set; }

    /// <summary>
    /// The converters to use to convert the property to observe value to the desired value.
    /// </summary>
    private Type? Converter { get; set; }

    private readonly RequiredAttribute innerAttribute;

    /// <summary>
    /// Initializes a new instance of the <see cref="RequiredIfAttribute"/> class.
    /// </summary>
    public RequiredIfAttribute()
        => this.innerAttribute = new RequiredAttribute();

    /// <summary>
    /// Initializes a new instance of the <see cref="RequiredIfAttribute"/> class.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="desiredValue">The desired value.</param>
    public RequiredIfAttribute([DisallowNull] string propertyName, object? desiredValue)
    {
        this.PropertyName = propertyName;
        this.DesiredValue = desiredValue;
        this.innerAttribute = new RequiredAttribute();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequiredIfAttribute"/> class.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="desiredValue">The desired value.</param>
    /// <param name="converter">The converter.</param>
    public RequiredIfAttribute([DisallowNull] string propertyName, object? desiredValue, Type converter)
    {
        this.PropertyName = propertyName;
        this.DesiredValue = desiredValue;
        this.Converter = converter;
        this.innerAttribute = new RequiredAttribute();
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var dependentValue = validationContext.ObjectInstance?.GetType()?.GetProperty(this.PropertyName ?? string.Empty)?.GetValue(validationContext.ObjectInstance, null);


        //  && value is not null
        if (dependentValue is not null && this.DesiredValue is not null)
        {
            if (!dependentValue.GetType().Equals(this.DesiredValue.GetType()) && this.Converter is null)
            {
                throw new ValidationException(
                    "Cannot performs requirement validation because no converter has been provided." +
                    $" [SourceProperty]: '{this.PropertyName}' - [SourceType]:'{ validationContext.ObjectInstance!.GetType()}' - [SourceProperty]: '{validationContext.MemberName}' - [TargetType]: '{validationContext.ObjectType}' ");
            }

            if (!dependentValue.GetType().Equals(this.DesiredValue.GetType()))
            {
                var converter = (RequiredConverterBase)Activator.CreateInstance(this.Converter!)!;
                if (!this.innerAttribute.IsValid(converter.Convert(value)))
                {
                    return new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName), new string[]
                    {
                        validationContext.MemberName!
                    });
                }
            }

            if (dependentValue.GetType().Equals(this.DesiredValue.GetType()))
            {
                if (!this.innerAttribute.IsValid(value))
                {
                    return new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName), new string[]
                    {
                    validationContext.MemberName!
                    });
                }
            }
        }

        return ValidationResult.Success;
    }
}

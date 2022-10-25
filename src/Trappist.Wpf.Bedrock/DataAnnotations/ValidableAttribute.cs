using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Trappist.Wpf.Bedrock.ObjectModel;

namespace Trappist.Wpf.Bedrock.DataAnnotations;

/// <summary>
/// Validates if the marked object, is valid using bedrock validation infrastructure.
/// </summary>
/// <seealso cref="ValidationAttribute" />
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public sealed class ValidableAttribute : ValidationAttribute
{
    public override bool IsValid([AllowNull] object? value) => value switch
    {
        ValidableObjectModel validableObjectModel => !validableObjectModel.HasErrors,
        IEnumerable enumerable => ValidateCollection(enumerable.Cast<ValidableObjectModel>()),
        _ => default
    };

    private static bool ValidateCollection(IEnumerable<ValidableObjectModel> enumerable)
        => enumerable.All(x => !x.HasErrors);
}

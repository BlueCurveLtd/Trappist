using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using InGroupe.Innovation.Wpf.Bedrock.ObjectModel;

namespace InGroupe.Innovation.Wpf.Bedrock.DataAnnotations;

/// <summary>
/// Validates if the marked object, is valid using bedrock validation infrastructure.
/// </summary>
/// <seealso cref="System.ComponentModel.DataAnnotations.ValidationAttribute" />
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public sealed class PartiallyValidableAttribute : ValidationAttribute
{
    public override bool IsValid([AllowNull] object? value) => value switch
    {
        ValidableObjectModel validableObjectModel => !validableObjectModel.HasErrors,
        IEnumerable enumerable => ValidateCollection(enumerable),
        _ => default
    };

    private static bool ValidateCollection(IEnumerable enumerable)
    {
        var validables = new List<ValidableObjectModel>();

        foreach(var item in enumerable)
        {
            if (item is ValidableObjectModel model)
            {
                validables.Add(model);
            }
        }

        if (validables.Count > 0)
        {
            return validables.All(x => !x.HasErrors);
        }

        return true;
    }
}

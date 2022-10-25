using System;
using System.Diagnostics.CodeAnalysis;

namespace Trappist.Wpf.Bedrock.Navigation;

[AttributeUsage(AttributeTargets.Property)]
public class NavigableAttribute : Attribute
{
    public NavigableAttribute()
    {

    }

    public NavigableAttribute([DisallowNull] string name)
    {
        this.Name = name;
    }

    public string? Name { get; }
}

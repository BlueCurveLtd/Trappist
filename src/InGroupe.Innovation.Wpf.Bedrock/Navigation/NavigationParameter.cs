using System.Diagnostics.CodeAnalysis;

namespace InGroupe.Innovation.Wpf.Bedrock.Navigation;

public sealed class NavigationParameter
{
    [DisallowNull]
    public string? Key { get; init; }
    public object? Value { get; init; }
}

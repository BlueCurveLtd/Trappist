using System.Diagnostics.CodeAnalysis;

namespace Trappist.Wpf.Bedrock.Navigation;

public sealed class NavigationParameter
{
    [DisallowNull]
    public string? Key { get; init; }
    public object? Value { get; init; }
}

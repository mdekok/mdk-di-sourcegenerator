using Microsoft.CodeAnalysis;

namespace Mdk.DISourceGenerator.Parts;

/// <summary>
/// DI part of the class the DIAttribute is assigned to.
/// </summary>
internal class DIClassPart(INamedTypeSymbol classType) : IDIPart
{
    /// <inheritdoc />
    public bool IsGeneric => classType.IsGenericType;

    /// <inheritdoc />
    public string ToSource()
    {
        if (_source is not null)
            return _source;

        string? unboundGenericPart = this.IsGeneric
            ? $"<{new string(',', classType.TypeArguments.Length - 1)}>"
            : null;

        return _source = $"global::{classType.ContainingNamespace}.{classType.Name}{unboundGenericPart}";
    }
    private string? _source;
}
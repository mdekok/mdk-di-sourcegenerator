using Microsoft.CodeAnalysis;

namespace Mdk.DISourceGenerator.Lib.Parts;

/// <summary>DI part of the class the DIAttribute is assigned to.</summary>
public class DIClassPart(INamedTypeSymbol classType) : DIPart
{
    /// <inheritdoc />
    public override INamedTypeSymbol? NamedTypeSymbol => classType;

    /// <inheritdoc />
    public override string ToSource()
    {
        if (_source is not null)
            return _source;

        string? unboundGenericPart = this.IsGenericType
            ? $"<{new string(',', classType.TypeArguments.Length - 1)}>"
            : null;

        return _source = $"global::{classType.ContainingNamespace}.{classType.Name}{unboundGenericPart}";
    }
    private string? _source;
}
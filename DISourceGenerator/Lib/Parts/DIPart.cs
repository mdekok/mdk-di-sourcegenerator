using Microsoft.CodeAnalysis;

namespace Mdk.DISourceGenerator.Lib.Parts;

/// <summary>Base abstract class to be subclassed by the attributed class, service type and implementation type.
/// </summary>
public abstract class DIPart : IDIPart
{
    /// <inheritdoc />
    public abstract INamedTypeSymbol? NamedTypeSymbol { get; }

    /// <inheritdoc />
    public TypeKind TypeKind => this.NamedTypeSymbol?.TypeKind ?? TypeKind.Unknown;

    /// <inheritdoc />
    public bool IsGeneric => this.NamedTypeSymbol?.IsGenericType ?? false;

    /// <inheritdoc />
    public bool IsUnboundGeneric => this.NamedTypeSymbol?.IsUnboundGenericType ?? false;

    /// <inheritdoc />
    public abstract string ToSource();
}
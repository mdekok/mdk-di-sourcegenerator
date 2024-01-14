using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace Mdk.DISourceGenerator.Lib.Parts;

/// <summary>Base abstract class to be subclassed by the attributed class, service type and implementation type.
/// </summary>
public abstract class DIPart : IDIPart
{
    /// <inheritdoc />
    public abstract INamedTypeSymbol? NamedTypeSymbol { get; }

    /// <inheritdoc />
    public string? Name => this.NamedTypeSymbol?.Name;

    /// <inheritdoc />
    public TypeKind TypeKind => this.NamedTypeSymbol?.TypeKind ?? TypeKind.Unknown;

    /// <inheritdoc />
    public bool IsGenericType => this.NamedTypeSymbol?.IsGenericType ?? false;

    /// <inheritdoc />
    public bool IsUnboundGenericType => this.NamedTypeSymbol?.IsUnboundGenericType ?? false;

    /// <inheritdoc />
    public ImmutableArray<INamedTypeSymbol> AllInterfaces => this.NamedTypeSymbol?.AllInterfaces ?? new();

    /// <inheritdoc />
    public bool Equals(IDIPart other) => this.NamedTypeSymbol?.Equals(other.NamedTypeSymbol, SymbolEqualityComparer.Default) ?? false;
  
    /// <inheritdoc />
    public abstract string ToSource();
}
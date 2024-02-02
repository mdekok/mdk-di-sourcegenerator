using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace Mdk.DISourceGenerator.Lib.Parts;

/// <summary>The DIPart interface implemented by the class, service and implementation type.</summary>
public interface IDIPart
{
    /// <summary>Gets the symbol of the DI part.</summary>
    INamedTypeSymbol? NamedTypeSymbol { get; }

    /// <summary>Gets the name of the DI part.</summary>
    string? Name { get; }

    /// <summary>Gets the kind of the type this DI part is (Class, Interface, ...).</summary>
    TypeKind TypeKind { get; }

    /// <summary>Gets a value indicating whether the DI part is a generic type.</summary>
    bool IsGenericType { get; }

    /// <summary>Gets a value indicating whether the DI part is a unbound generic type.</summary>
    bool IsUnboundGenericType { get; }

    /// <summary>Gets the list of all (recursive) base classes if IDPart is class type.</summary>
    ImmutableArray<INamedTypeSymbol> BaseClasses { get; }

    /// <summary>Gets the list of all interfaces.</summary>
    ImmutableArray<INamedTypeSymbol> AllInterfaces { get; }

    /// <summary>Determines if this DIPart is equal to another, according to the default <see cref="SymbolEqualityComparer"/>.</summary>
    /// <param name="other">The other DIPart.</param>
    /// <returns>True of equal.</returns>
    public bool Equals(IDIPart other);

    /// <summary>Converts DIPart to source.</summary>
    /// <returns>partial source string.</returns>
    string ToSource();
}
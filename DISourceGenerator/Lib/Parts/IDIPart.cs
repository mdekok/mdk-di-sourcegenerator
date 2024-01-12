using Microsoft.CodeAnalysis;

namespace Mdk.DISourceGenerator.Lib.Parts;

/// <summary>The DIPart interface implemented by the class, service and implementation type.</summary>
public interface IDIPart
{
    /// <summary>Gets the symbol of the DI part.</summary>
    INamedTypeSymbol? NamedTypeSymbol { get; }

    /// <summary>Gets the kind of the type this part is (Class, Interface, ...).</summary>
    TypeKind TypeKind { get; }

    /// <summary>Gets a value indicating whether the DI part is a generic type.</summary>
    bool IsGeneric { get; }

    /// <summary>Gets a value indicating whether the DI part is a unbound generic type.</summary>
    bool IsUnboundGeneric { get; }

    /// <summary>Converts DIPart to source.</summary>
    /// <returns>partial source string.</returns>
    string ToSource();
}
using Mdk.DISourceGenerator.Lib;
using Microsoft.CodeAnalysis;
using System.Linq;

namespace Mdk.DISourceGenerator.Validation.Validators;

/// <summary>DI001 validator</summary>
internal static class DI002ValidatorServiceInterfaceNotImplemented
{
    /// <summary>Validates on service interface not implemented.</summary>
    public static DiagnosticDescriptor? Validate(DIRegistration registration, SourceContext sourceContext)
    {
        // [Add{Lifetime}<Interface>]
        // class Implementation { } // Implementation must implement Interface.
        if (registration.ServiceType.TypeKind == TypeKind.Interface
            && !registration.ServiceType.IsGeneric
            && !sourceContext
                .ClassSymbol
                .AllInterfaces
                .Any(namedTypeSymbol => namedTypeSymbol.Equals(
                    registration.ServiceType.NamedTypeSymbol, SymbolEqualityComparer.Default)))
            return ServiceInterfaceNotImplemented;

        return null;
    }

    public static readonly DiagnosticDescriptor ServiceInterfaceNotImplemented
        = new("DI002",
            "Interface missing on class",
            "Implement registered interface on attributed class",
            "Generator",
            DiagnosticSeverity.Error,
            true);
}
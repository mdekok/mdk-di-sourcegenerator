using Mdk.DISourceGenerator.Analyzers.Lib;
using Mdk.DISourceGenerator.Lib;
using Mdk.DISourceGenerator.Lib.Parts;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Linq;

namespace Mdk.DISourceGenerator.Analyzers;

/// <summary>DI0002 analyzer: Interface missing on class</summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class DI0002ServiceInterfaceNotImplementedAnalyzer : DIAnalyzerBase
{
    /// <inheritdoc/>
    protected override DiagnosticDescriptor BuildRule() => new(
        "DI0002",
        "Interface missing on class",
        "Interface missing on class: Add interface '{0}' to Class '{1}' as expected from DI attribute",
        Constants.DiagnosticCategory,
        DiagnosticSeverity.Error,
        true);

    /// <inheritdoc/>
    public override ValidateAttributeDelegate ValidateAttribute => Validate;

    /// <inheritdoc/>
    public static ValidationResult Validate(DIRegistration registration)
    {
        // [Add{Lifetime}<Interface>]
        // class Implementation { } // Implementation must implement Interface.

        if (registration.ServiceType is IDIPart serviceType
            && serviceType.TypeKind == TypeKind.Interface
            && !serviceType.IsGenericType // just test simple interfaces
            && !registration
                .ClassType
                .AllInterfaces
                .Any(interfaceType => interfaceType.Equals(serviceType.NamedTypeSymbol, SymbolEqualityComparer.Default)))
            return new(true, false);

        return ValidationResult.NoDiagnostic;
    }

    /// <inheritdoc/>
    public override Diagnostic BuildDiagnostic(DIRegistration registration)
        => Diagnostic.Create(Rule, registration.ClassType.NamedTypeSymbol?.Locations[0],
            registration.ServiceType?.Name, registration.ClassType.Name);
}

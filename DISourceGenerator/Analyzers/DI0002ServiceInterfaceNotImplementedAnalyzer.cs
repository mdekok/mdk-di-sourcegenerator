using Mdk.DISourceGenerator.Analyzers.Lib;
using Mdk.DISourceGenerator.Lib;
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
        "Class '{0}' does not implement registered interface '{1}'",
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

        if (registration.ServiceType is null)
            return ValidationResult.NoDiagnostic;

        if (registration.ServiceType.TypeKind == TypeKind.Interface
            && !registration.ServiceType.IsGenericType // just test simple interfaces
            && !registration
                .ClassType
                .AllInterfaces
                .Any(interfaceType => interfaceType.Equals(registration.ServiceType.NamedTypeSymbol, SymbolEqualityComparer.Default)))
            return new(true, false);

        return ValidationResult.NoDiagnostic;
    }

    /// <inheritdoc/>
    public override Diagnostic BuildDiagnostic(DIRegistration registration)
        => Diagnostic.Create(Rule, registration.ClassType.NamedTypeSymbol?.Locations[0],
            registration.ClassType.Name, registration.ServiceType?.Name);
}

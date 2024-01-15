using Mdk.DISourceGenerator.Analyzers.Lib;
using Mdk.DISourceGenerator.Lib;
using Mdk.DISourceGenerator.Lib.Parts;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Mdk.DISourceGenerator.Analyzers;

/// <summary>DI0001 analyzer: Implementation type missing</summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class DI0001ImplementationTypeMissingAnalyzer : DIAnalyzerBase
{
    /// <inheritdoc/>
    protected override DiagnosticDescriptor BuildRule() => new(
        "DI0001",
        "Implementation type missing in DI attribute",
        "Implementation type missing in DI attribute: Add the implementation type like [{0}<Interface, Implementation>]",
        Constants.DiagnosticCategory,
        DiagnosticSeverity.Error,
        true);

    /// <inheritdoc/>
    public override ValidateAttributeDelegate ValidateAttribute => Validate;

    /// <inheritdoc/>
    public static ValidationResult Validate(DIRegistration registration)
    {
        // [Add{Lifetime}<ServiceType<int>>]
        // is not allowed if service type is bound generic interface and class type is generic.
        // should be [Add{Lifetime}<ServiceType<int>, ImplementationType>].

        if (registration.ImplementationType is null
            && registration.ServiceType is IDIPart serviceType
            && serviceType.TypeKind == TypeKind.Interface
            && serviceType.IsGenericType
            && !serviceType.IsUnboundGenericType
            && registration.ClassType.IsGenericType)
            return new(true, false);

        return ValidationResult.NoDiagnostic;
    }

    /// <inheritdoc/>
    public override Diagnostic BuildDiagnostic(DIRegistration registration)
        => Diagnostic.Create(Rule, registration.ClassType.NamedTypeSymbol?.Locations[0],
            registration.Method);
}
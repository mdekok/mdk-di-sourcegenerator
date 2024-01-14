using Mdk.DISourceGenerator.Analyzers.Lib;
using Mdk.DISourceGenerator.Lib;
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
        "Implementation type missing",
        "Add the implementation type to the attribute, like [AddScoped<Interface, Implementation>]",
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

        if (registration.ServiceType is null)
            return ValidationResult.NoDiagnostic;

        if (registration.ServiceType.TypeKind == TypeKind.Interface
            && registration.ServiceType.IsGenericType
            && !registration.ServiceType.IsUnboundGenericType
            && registration.ClassType.IsGenericType
            && registration.ImplementationType is null)
            return new(true, false);

        return ValidationResult.NoDiagnostic;
    }

    /// <inheritdoc/>
    public override Diagnostic BuildDiagnostic(DIRegistration registration)
        => Diagnostic.Create(Rule, registration.ClassType.NamedTypeSymbol?.Locations[0]);
}
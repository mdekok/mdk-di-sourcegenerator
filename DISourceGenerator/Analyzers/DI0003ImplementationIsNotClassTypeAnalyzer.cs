using Mdk.DISourceGenerator.Analyzers.Lib;
using Mdk.DISourceGenerator.Lib;
using Mdk.DISourceGenerator.Lib.Parts;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Mdk.DISourceGenerator.Analyzers;

/// <summary>DI0003 analyzer: Implementation type is not the same as class type</summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class DI0003ImplementationIsNotClassTypeAnalyzer : DIAnalyzerBase
{
    /// <inheritdoc/>
    protected override DiagnosticDescriptor BuildRule() => new(
        "DI0003",
        "Implementation type is not the same as class type",
        "Implementation type '{0}' is not the same as class type '{1}': Move attribute to class '{0}'",
        Constants.DiagnosticCategory,
        DiagnosticSeverity.Error,
        true);

    /// <inheritdoc/>
    public override ValidateAttributeDelegate ValidateAttribute => Validate;

    /// <inheritdoc/>
    public static ValidationResult Validate(DIRegistration registration)
    {
        // [Add{Lifetime}<ServiceType, ImplementationType>]
        // class ClassType : ServiceType { }
        // Is not allowed if ImplementationType is not the same as ClassType.

        if (registration.ImplementationType is IDIPart implementationType
            && !implementationType.IsGenericType // just test simple classes
            && !implementationType.Equals(registration.ClassType))
        {
            return new(true, false);
        }

        return ValidationResult.NoDiagnostic;
    }

    /// <inheritdoc/>
    public override Diagnostic BuildDiagnostic(DIRegistration registration)
        => Diagnostic.Create(Rule,
            registration.ClassType.NamedTypeSymbol?.Locations[0],
            registration.ImplementationType?.Name, registration.ClassType.Name);
}

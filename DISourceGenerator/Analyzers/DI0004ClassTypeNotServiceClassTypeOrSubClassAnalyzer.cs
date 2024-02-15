using Mdk.DISourceGenerator.Analyzers.Lib;
using Mdk.DISourceGenerator.Lib;
using Mdk.DISourceGenerator.Lib.Parts;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Linq;

namespace Mdk.DISourceGenerator.Analyzers;

/// <summary>DI0004 analyzer: Class type is not the same as or subclass of service class type</summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class DI0004ClassTypeNotServiceClassTypeOrSubClassAnalyzer : DIAnalyzerBase
{
    /// <inheritdoc/>
    protected override DiagnosticDescriptor BuildRule() => new(
        "DI0004",
        "Class type is not the same as or subclass of service class type",
        "Type '{0}' is not the same as or subclass of service type '{1}'",
        Constants.DiagnosticCategory,
        DiagnosticSeverity.Error,
        true);

    /// <inheritdoc/>
    public override ValidateAttributeDelegate ValidateAttribute => Validate;

    /// <inheritdoc/>
    public static ValidationResult Validate(DIRegistration registration)
    {
        // [Add{Lifetime}<ServiceType>] where ServiceType is a class
        // class ClassType { }
        // Is not allowed if ClassType is not the same as or subclass of ServiceType.

        if (registration.ServiceType is IDIPart serviceType
            && serviceType.TypeKind == TypeKind.Class
            && !serviceType.IsGenericType // just test simple classes
            && !serviceType.Equals(registration.ClassType)
            && !registration
                .ClassType
                .BaseClasses
                .Any(baseClass => baseClass.Equals(serviceType.NamedTypeSymbol, SymbolEqualityComparer.Default)))
        {
            return new(true, false);
        }

        return ValidationResult.NoDiagnostic;
    }

    /// <inheritdoc/>
    public override Diagnostic BuildDiagnostic(DIRegistration registration)
        => Diagnostic.Create(Rule,
            registration.ClassType.NamedTypeSymbol?.Locations[0],
            registration.ClassType.Name, registration.ServiceType?.Name);
}

using Mdk.DISourceGenerator.Analyzers.Lib;
using Mdk.DISourceGenerator.Lib;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Linq;

namespace Mdk.DISourceGenerator.Analyzers;

/// <summary>DI0005 analyzer: IHostedService missing on class</summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class DI0005IHostedServiceIsNotImplementedAnalyzer : DIAnalyzerBase
{
    /// <inheritdoc/>
    protected override DiagnosticDescriptor BuildRule() => new(
        "DI0005",
        "IHostedService missing on class",
        "IHostedService interface missing on class: Add interface IHostedService to Class '{0}'",
        Constants.DiagnosticCategory,
        DiagnosticSeverity.Error,
        true);

    /// <inheritdoc/>
    public override ValidateAttributeDelegate ValidateAttribute => Validate;

    /// <inheritdoc/>
    public static ValidationResult Validate(DIRegistration registration)
    {
        // [AddHostedService]
        // class Implementation { }
        // is not allowed, Implementation must implement IHostedService.

        if (registration.Method == "AddHostedService"
            && !registration
                .ClassType
                .AllInterfaces
                .Any(interfaceType => interfaceType.Name == "IHostedService"))
            return new(true, false);

        return ValidationResult.NoDiagnostic;
    }

    /// <inheritdoc/>
    public override Diagnostic BuildDiagnostic(DIRegistration registration)
        => Diagnostic.Create(Rule,
            registration.ClassType.NamedTypeSymbol?.Locations[0],
            registration.ClassType.Name);
}

using Mdk.DISourceGenerator.Analyzers.Lib;
using Mdk.DISourceGenerator.Lib;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Mdk.DISourceGenerator.Analyzers;

/// <summary>DI0003 analyzer: Keyed registration has no key</summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class DI0006KeyedRegistrationHasNoKeyAnalyzer : DIAnalyzerBase
{
    /// <inheritdoc/>
    protected override DiagnosticDescriptor BuildRule() => new(
        "DI0006",
        "Keyed registration has no key",
        "Keyed registration '{0}' has no key as parameter",
        Constants.DiagnosticCategory,
        DiagnosticSeverity.Error,
        true);

    /// <inheritdoc/>
    public override ValidateAttributeDelegate ValidateAttribute => Validate;

    /// <inheritdoc/>
    public static ValidationResult Validate(DIRegistration registration)
    {
        // [AddKeyed{Lifetime}...("key")]
        // If keyed then key parameter mandatory.

        if ((registration.Method == "AddKeyedSingleton" || registration.Method == "AddKeyedScoped" || registration.Method == "AddKeyedTransient")
            && string.IsNullOrEmpty(registration.Key))
            return new(true, false);

        return ValidationResult.NoDiagnostic;
    }

    /// <inheritdoc/>
    public override Diagnostic BuildDiagnostic(DIRegistration registration)
        => Diagnostic.Create(Rule,
            registration.ClassType.NamedTypeSymbol?.Locations[0],
            registration.Method);
}

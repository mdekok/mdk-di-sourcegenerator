using DISourceGeneratorAnalyzer.Lib;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DISourceGeneratorAnalyzer;

/// <summary>DI001 analyzer: Implementation type missing</summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class DISG0001ImplementationTypeMissingAnalyzer : DISGAnalyzerBase
{
    /// <inheritdoc/>
    protected override DiagnosticDescriptor BuildRule() => new(
        "DI001",
        "Implementation type missing",
        "Add the implementation type to the attribute, like [AddScoped<Interface, Implementation>]",
        Constants.DiagnosticCategory,
        DiagnosticSeverity.Error,
        true);

    /// <inheritdoc/>
    protected override Diagnostic? AnalyzeAttribute(
        INamedTypeSymbol classType,
        INamedTypeSymbol? serviceType,
        INamedTypeSymbol? implementationType)
    {
        // [Add{Lifetime}<ServiceType<int>>] is not allowed if service type is interface.
        // should be [Add{Lifetime}<ServiceType<int>, ImplementationType>].

        if (serviceType is null)
            return null;

        if (serviceType.TypeKind == TypeKind.Interface
            && serviceType.IsGenericType
            && !serviceType.IsUnboundGenericType
            && classType.IsGenericType
            && implementationType is null)
        {
            return Diagnostic.Create(Rule,
                classType.Locations[0]);
        }

        return null;
    }
}

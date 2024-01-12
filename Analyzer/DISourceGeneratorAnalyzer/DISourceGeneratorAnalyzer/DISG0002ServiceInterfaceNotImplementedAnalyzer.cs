using DISourceGeneratorAnalyzer.Lib;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Linq;

namespace DISourceGeneratorAnalyzer;

/// <summary>DI002 analyzer: Interface missing on class</summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class DISG0002ServiceInterfaceNotImplementedAnalyzer : DISGAnalyzerBase
{
    /// <inheritdoc/>
    protected override DiagnosticDescriptor BuildRule() => new(
        "DI002",
        "Interface missing on class",
        "Class '{0}' does not implement registered interface '{1}'",
        Constants.DiagnosticCategory,
        DiagnosticSeverity.Error,
        true);

    /// <inheritdoc/>
    protected override Diagnostic? AnalyzeAttribute(
        INamedTypeSymbol classType,
        INamedTypeSymbol? serviceType,
        INamedTypeSymbol? implementationType)
    {
        // [Add{Lifetime}<Interface>]
        // class Implementation { } // Implementation must implement Interface.

        if (serviceType is null)
            return null;

        if (serviceType.TypeKind == TypeKind.Interface
            && !serviceType.IsGenericType
            && !classType
                .AllInterfaces
                .Any(interfaceType => interfaceType.Equals(serviceType, SymbolEqualityComparer.Default)))
            return Diagnostic.Create(Rule,
                classType.Locations[0],
                classType.Name, serviceType.Name);

        return null;
    }
}

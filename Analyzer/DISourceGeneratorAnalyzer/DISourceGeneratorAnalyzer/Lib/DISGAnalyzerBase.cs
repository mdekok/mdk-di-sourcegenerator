using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace DISourceGeneratorAnalyzer.Lib;

/// <summary>Base class for all DISourceGenerator analyzers</summary>
// Add fallowing attribute to the DISGAnalyzerBase subclasses
// if inherited analyzer is not picked up by Visual Studio.
// [DiagnosticAnalyzer(LanguageNames.CSharp)]
public abstract class DISGAnalyzerBase : DiagnosticAnalyzer
{
    /// <summary>DiagnosticDescriptor for the analyzer</summary>
    protected DiagnosticDescriptor Rule => _rule ??= this.BuildRule();
    private DiagnosticDescriptor? _rule;

    /// <summary>Builds the DiagnosticDescriptor for the analyzer</summary>
    /// <returns>DiagnosticDescriptor</returns>
    protected abstract DiagnosticDescriptor BuildRule();

    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(this.Rule);

    /// <summary>Initializes the analyzer.</summary>
    /// <param name="context">The analysis context.</param>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);
    }

    /// <summary>Implementation of the actual analysis of the symbol.</summary>
    /// <param name="context">The analysis context.</param>
    private void AnalyzeSymbol(SymbolAnalysisContext context)
    {
        INamedTypeSymbol classType = (INamedTypeSymbol)context.Symbol;

        if (classType.TypeKind != TypeKind.Class)
            return;

        foreach (AttributeData attribute in classType
            .GetAttributes()
            .Where(attributeData => attributeData
                .AttributeClass?
                .ContainingNamespace?
                .ToString() == Constants.DIAttributesNamespace))
        {
            if (this.AnalyzeAttribute(classType, GetServiceType(attribute), GetImplementationType(attribute))
                is Diagnostic diagnostic)
            {
                context.ReportDiagnostic(diagnostic);
            }
        }
    }

    protected abstract Diagnostic? AnalyzeAttribute(
        INamedTypeSymbol classType,
        INamedTypeSymbol? serviceType,
        INamedTypeSymbol? implementationType);

    private static INamedTypeSymbol? GetServiceType(AttributeData attribute)
    {
        if (attribute.AttributeClass is not INamedTypeSymbol attributeClass)
            return null;

        // Get type as generic type: [AddScoped<ServiceType, ImplementationType>]
        if (attributeClass.TypeParameters.FirstOrDefault(
            symbol => symbol.Name == "ServiceType") is ITypeParameterSymbol typeParameterSymbol)
            return attributeClass.TypeArguments[attributeClass.TypeParameters.IndexOf(typeParameterSymbol)] as INamedTypeSymbol;

        // Get type as constructor parameter: [AddScoped(typeof(ServiceType), typeof(ImplementationType))]
        if (attribute.ConstructorArguments.Length > 0)
            return attribute.ConstructorArguments[0].Value as INamedTypeSymbol;

        return null;
    }

    private static INamedTypeSymbol? GetImplementationType(AttributeData attribute)
    {
        if (attribute.AttributeClass is not INamedTypeSymbol attributeClass)
            return null;

        // Get type as generic type: [AddScoped<ServiceType, ImplementationType>]
        if (attributeClass.TypeParameters.FirstOrDefault(
            symbol => symbol.Name == "ImplementationType") is ITypeParameterSymbol typeParameterSymbol)
            return attributeClass.TypeArguments[attributeClass.TypeParameters.IndexOf(typeParameterSymbol)] as INamedTypeSymbol;

        // Get type as constructor parameter: [AddScoped(typeof(ServiceType), typeof(ImplementationType))]
        if (attribute.ConstructorArguments.Length > 1)
            return attribute.ConstructorArguments[1].Value as INamedTypeSymbol;

        return null;
    }
}

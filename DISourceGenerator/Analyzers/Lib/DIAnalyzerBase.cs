using Mdk.DISourceGenerator.Lib;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace Mdk.DISourceGenerator.Analyzers.Lib;

/// <summary>Base class for all DISourceGenerator analyzers</summary>
// Add fallowing attribute to the DISGAnalyzerBase subclasses
// if inherited analyzer is not picked up by Visual Studio.
// [DiagnosticAnalyzer(LanguageNames.CSharp)]
public abstract class DIAnalyzerBase : DiagnosticAnalyzer
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
        INamedTypeSymbol classSymbol = (INamedTypeSymbol)context.Symbol;

        if (classSymbol.TypeKind != TypeKind.Class)
            return;

        foreach (AttributeData attribute in classSymbol
            .GetAttributes()
            .Where(attributeData => attributeData
                .AttributeClass?
                .ContainingNamespace?
                .ToString() == Constants.DIAttributesNamespace))
        {
            if (DIRegistrationBuilder.Build(attribute, classSymbol) is DIRegistration registration
                && this.ValidateAttribute(registration).HasDiagnostic)
            {
                context.ReportDiagnostic(this.BuildDiagnostic(registration));
            }
        }
    }

    public abstract ValidateAttributeDelegate ValidateAttribute { get; }

    public abstract Diagnostic BuildDiagnostic(DIRegistration registration);
}

public delegate ValidationResult ValidateAttributeDelegate(DIRegistration registration);
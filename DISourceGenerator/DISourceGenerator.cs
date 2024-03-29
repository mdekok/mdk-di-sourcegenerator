﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Mdk.DISourceGenerator.Lib;

namespace Mdk.DISourceGenerator;

/// <summary>Source generator to add simple dependency injection registrations.</summary>
[Generator]
public class DISourceGenerator : IIncrementalGenerator
{
    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValuesProvider<ClassDeclarationSyntax> classesWithAttribute
            = context
                .SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: static (node, ct) => IsSyntaxTargetForGeneration(node),
                    transform: static (ctx, ct) => GetSemanticTargetForGeneration(ctx))
                .Where(static classDeclarationSyntax => classDeclarationSyntax is not null)!;

        IncrementalValueProvider<(Compilation, ImmutableArray<ClassDeclarationSyntax>)> compilation
            = context
                .CompilationProvider
                .Combine(classesWithAttribute.Collect());

        context.RegisterSourceOutput(compilation, this.Execute);
    }

    /// <summary>Test if class node has any attributes as a first filter.</summary>
    /// <param name="syntaxNode">The syntax node.</param>
    /// <returns>True if node represents a class with attributes, false otherwise.</returns>
    public static bool IsSyntaxTargetForGeneration(SyntaxNode syntaxNode)
        => syntaxNode is ClassDeclarationSyntax classDeclarationSyntax
            && classDeclarationSyntax.AttributeLists.Count > 0;

    /// <summary>Gets the target for source generation by additional filtering on the various DIAttributes.</summary>
    /// <param name="context">The syntax context.</param>
    /// <returns>A ClassDeclarationSyntax.</returns>
    public static ClassDeclarationSyntax? GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
    {
        ClassDeclarationSyntax classDeclarationSyntax = (ClassDeclarationSyntax)context.Node;

        foreach (AttributeListSyntax attributeListSyntax in classDeclarationSyntax.AttributeLists)
            foreach (AttributeSyntax attributeSyntax in attributeListSyntax.Attributes)
            {
                if (context.SemanticModel.GetSymbolInfo(attributeSyntax).Symbol is ISymbol attributeSymbol
                    && IsDIAttributeClass(attributeSymbol))
                    return classDeclarationSyntax;
            }

        return null;
    }

    /// <summary>Test if a symbol is a DIAttribute.</summary>
    /// <param name="symbol">The symbol.</param>
    /// <returns>True if symbol is a DIAttribute, false otherwise.</returns>
    private static bool IsDIAttributeClass(ISymbol? symbol)
        // All DIAttributes (AddSingleton, AddScoped, AddTransient and generic versions) are in the namespace: Mdk.DIAttributes.
        => symbol?.ContainingNamespace.ToString() == "Mdk.DIAttributes";

    /// <summary>Adds the generated source code to the context.</summary>
    /// <param name="context">The context.</param>
    /// <param name="tuple">The compilation and classes with DIAttributes.</param>
    private void Execute(SourceProductionContext context, (Compilation Left, ImmutableArray<ClassDeclarationSyntax> Right) tuple)
    {
#if DEBUG
        // Debugger.Launch();
#endif

        Compilation compilation = tuple.Left;
        ImmutableArray<ClassDeclarationSyntax> classDeclarationSyntaxList = tuple.Right;

        string assemblyName = compilation.AssemblyName ?? "UnknownAssemblyName";

        List<DIRegistration> registrations = [];

        foreach (ClassDeclarationSyntax classDeclarationSyntax in classDeclarationSyntaxList)
        {
            SemanticModel model = compilation.GetSemanticModel(classDeclarationSyntax.SyntaxTree);
            if (model.GetDeclaredSymbol(classDeclarationSyntax) is not INamedTypeSymbol classSymbol)
                continue;

            foreach (AttributeData attribute in classSymbol.GetAttributes())
            {
                if (IsDIAttributeClass(attribute.AttributeClass)
                    && DIRegistrationBuilder.Build(attribute, classSymbol) is DIRegistration registration
                    && DIRegistrationValidator.Validate(registration))
                {
                    registrations.Add(registration);
                }
            }
        }

        // Collect all referenced assemblies with dependency registrations.
        IEnumerable<IAssemblySymbol> referencedDIAssemblies = compilation
            .SourceModule
            .ReferencedAssemblySymbols
            .Where(referenceAssembly => referenceAssembly
                .TypeNames.Any(typeName => typeName == DISourceWriter.RegistrationStaticClassName));

        // If no (referenced) DIAttributes found, then don't generate source code.
        if (!classDeclarationSyntaxList.Any() && !referencedDIAssemblies.Any())
            return;

        context.AddSource($"DISourceGenerator.{compilation.AssemblyName}.g.cs",
            DISourceWriter.Write(assemblyName, registrations, referencedDIAssemblies));
    }
}
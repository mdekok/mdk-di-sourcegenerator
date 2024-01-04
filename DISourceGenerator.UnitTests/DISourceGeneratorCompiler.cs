using Mdk.DIAttributes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Runtime.CompilerServices;

namespace Mdk.DISourceGenerator.UnitTests;

public static class DISourceGeneratorCompiler
{
    /// <summary>
    /// Compiles the source code and returns the generated dependency injection source code.
    /// </summary>
    /// <param name="sourceCode">The source code.</param>
    /// <param name="assemblyName">Name of the assembly the source code is part of.</param>
    /// <returns>The generated source code.</returns>
    public static string? GetGeneratedOutput(
        string sourceCode,
        string assemblyName)
    {
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);

        IEnumerable<MetadataReference> references = AppDomain.CurrentDomain
            .GetAssemblies()
            .Where(assembly => !assembly.IsDynamic)
            .Select(assembly => MetadataReference.CreateFromFile(assembly.Location))
            .Cast<MetadataReference>();

        CSharpCompilation compilation = CSharpCompilation.Create(assemblyName,
            [syntaxTree],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        DISourceGenerator generator = new();

        CSharpGeneratorDriver.Create(generator)
            .RunGeneratorsAndUpdateCompilation(compilation,
                out var outputCompilation,
                out var diagnostics
            );

        return outputCompilation.SyntaxTrees.Skip(1).LastOrDefault()?.ToString();
    }

    [ModuleInitializer]
    internal static void Initialize()
    {
        // Dummy construction to make sure the DIAttributes assembly is available as MetaDataReference above.
        DIAttribute dummy = new AddSingleton();
    }
}
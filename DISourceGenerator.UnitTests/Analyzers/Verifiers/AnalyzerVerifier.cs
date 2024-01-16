using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using Microsoft.CodeAnalysis.Testing;

namespace Mdk.DISourceGenerator.UnitTests.Analyzers.Verifiers;

/// <summary>The analyzer verifier.</summary>
/// <see href="https://www.thinktecture.com/en/net/roslyn-source-generators-analyzers-code-fixes-testing/"/>
public static class AnalyzerVerifier<TAnalyzer>
   where TAnalyzer : DiagnosticAnalyzer, new()
{
    public static DiagnosticResult Diagnostic(string diagnosticId)
        => CSharpAnalyzerVerifier<TAnalyzer, XUnitVerifier>.Diagnostic(diagnosticId);

    public static Task VerifyAnalyzerAsync(
       string source,
       params DiagnosticResult[] expected)
    {
        AnalyzerTest test = new(source, expected);
        return test.RunAsync(CancellationToken.None);
    }

    private class AnalyzerTest : CSharpAnalyzerTest<TAnalyzer, DefaultVerifier>
    // Used DefaultVerifier instead of XUnitVerifier because of the following error:
    // System.MissingMethodException : Method not found: 'Void Xunit.Sdk.EqualException..ctor(System.Object, System.Object)
    // https://github.com/dotnet/roslyn-analyzers/pull/7052
    {
        public AnalyzerTest(
           string source,
           params DiagnosticResult[] expected)
        {
            TestCode = source;
            ExpectedDiagnostics.AddRange(expected);

            TestState.AdditionalReferences.Add(typeof(DIAttributes.DIAttribute).Assembly);
        }
    }
}

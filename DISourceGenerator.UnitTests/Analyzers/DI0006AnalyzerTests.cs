using Microsoft.CodeAnalysis.Testing;

using VerifyCS = Mdk.DISourceGenerator.UnitTests.Analyzers.Verifiers.AnalyzerVerifier<
    Mdk.DISourceGenerator.Analyzers.DI0006KeyedRegistrationHasNoKeyAnalyzer>;

namespace Mdk.DISourceGenerator.UnitTests.Analyzers;

/// <summary>The DI0006 analyzer unit tests.</summary>
public class DI0006AnalyzerTest
{
    [Fact]
    public async Task DI0006_Positive()
    {
        // Arrange
        var test = @"
    using Mdk.DIAttributes;

    [AddKeyedScoped()]
    class {|#0:DI0006|} { }";

        var expected = new[]
        {
            VerifyCS.Diagnostic("DI0006")
                .WithLocation(0)
                .WithArguments("AddKeyedScoped"),

            DiagnosticResult.CompilerError("CS1729")
                .WithSpan(4, 6, 4, 22)
                .WithArguments("Mdk.DIAttributes.AddKeyedScoped", "0")
        };

        // Act & Assert
        await VerifyCS.VerifyAnalyzerAsync(test, expected);
    }

    [Fact]
    public async Task DI0006_Negative0()
    {
        // Arrange, Act & Assert
        await VerifyCS.VerifyAnalyzerAsync("");
    }

    [Fact]
    public async Task DI0006_Negative1()
    {
        // Arrange
        var test = @"
    using Mdk.DIAttributes;

    [AddKeyedScoped(""key"")]
    class DI0006 { }";

        // Act & Assert
        await VerifyCS.VerifyAnalyzerAsync(test);
    }
}

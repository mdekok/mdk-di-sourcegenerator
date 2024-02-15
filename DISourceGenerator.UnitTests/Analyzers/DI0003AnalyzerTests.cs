using Microsoft.CodeAnalysis.Testing;

using VerifyCS = Mdk.DISourceGenerator.UnitTests.Analyzers.Verifiers.AnalyzerVerifier<
    Mdk.DISourceGenerator.Analyzers.DI0003ImplementationIsNotClassTypeAnalyzer>;

namespace Mdk.DISourceGenerator.UnitTests.Analyzers;

/// <summary>The DI0003 analyzer unit tests.</summary>
public class DI0003AnalyzerTest
{
    [Fact]
    public async Task DI0003_Positive()
    {
        // Arrange
        var test = @"
    using Mdk.DIAttributes;

    [AddScoped<IInterface, Implementation>]
    class {|#0:DI0003|} : IInterface { }

    public interface IInterface { }

    class Implementation : IInterface { }";

        DiagnosticResult expected = VerifyCS
            .Diagnostic("DI0003")
            .WithLocation(0)
            .WithArguments("Implementation", "DI0003");

        // Act & Assert
        await VerifyCS.VerifyAnalyzerAsync(test, expected);
    }

    [Fact]
    public async Task DI0003_Negative0()
    {
        // Arrange, Act & Assert
        await VerifyCS.VerifyAnalyzerAsync("");
    }

    [Fact]
    public async Task DI0003_Negative1()
    {
        // Arrange
        var test = @"
    using Mdk.DIAttributes;

    [AddScoped<IInterface, DI0003>]
    class DI0003 : IInterface { }

    public interface IInterface { }";

        // Act & Assert
        await VerifyCS.VerifyAnalyzerAsync(test);
    }
}

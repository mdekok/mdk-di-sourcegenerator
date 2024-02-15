using Microsoft.CodeAnalysis.Testing;

using VerifyCS = Mdk.DISourceGenerator.UnitTests.Analyzers.Verifiers.AnalyzerVerifier<
    Mdk.DISourceGenerator.Analyzers.DI0002ServiceInterfaceNotImplementedAnalyzer>;

namespace Mdk.DISourceGenerator.UnitTests.Analyzers;

/// <summary>The DI0002 analyzer unit tests.</summary>
public class DI0002AnalyzerTest
{
    [Fact]
    public async Task DI0002_Positive()
    {
        // Arrange
        var test = @"
    using Mdk.DIAttributes;

    [AddScoped<IInterface>]
    class {|#0:Implementation|} { }

    interface IInterface {}";

        DiagnosticResult expected = VerifyCS
            .Diagnostic("DI0002")
            .WithLocation(0)
            .WithArguments("IInterface", "Implementation");

        // Act & Assert
        await VerifyCS.VerifyAnalyzerAsync(test, expected);
    }

    [Fact]
    public async Task DI0002_Negative0()
    {
        // Arrange, Act & Assert
        await VerifyCS.VerifyAnalyzerAsync("");
    }

    [Fact]
    public async Task DI0002_Negative1()
    {
        // Arrange
        var test = @"
    using Mdk.DIAttributes;

    [AddScoped<IInterface>]
    class Implementation: IInterface { }

    interface IInterface {}";

        // Act & Assert
        await VerifyCS.VerifyAnalyzerAsync(test);
    }
}

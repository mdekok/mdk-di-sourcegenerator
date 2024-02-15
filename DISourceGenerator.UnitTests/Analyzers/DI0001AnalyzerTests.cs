using Microsoft.CodeAnalysis.Testing;

using VerifyCS = Mdk.DISourceGenerator.UnitTests.Analyzers.Verifiers.AnalyzerVerifier<
    Mdk.DISourceGenerator.Analyzers.DI0001ImplementationTypeMissingAnalyzer>;

namespace Mdk.DISourceGenerator.UnitTests.Analyzers;

/// <summary>The DI0001 analyzer unit tests.</summary>
public class DI0001AnalyzerTest
{
    [Fact]
    public async Task DI0001_Positive()
    {
        // Arrange
        var test = @"
    using Mdk.DIAttributes;

    [AddScoped<IGenericType<int>>]
    class {|#0:GenericType|}<T> : IGenericType<T> { }

    public interface IGenericType<T> { }";

        DiagnosticResult expected = VerifyCS
            .Diagnostic("DI0001")
            .WithLocation(0)
            .WithArguments("AddScoped");

        // Act & Assert
        await VerifyCS.VerifyAnalyzerAsync(test, expected);
    }

    [Fact]
    public async Task DI0001_Negative0()
    {
        // Arrange, Act & Assert
        await VerifyCS.VerifyAnalyzerAsync("");
    }

    [Fact]
    public async Task DI0001_Negative1()
    {
        // Arrange
        var test = @"
    using Mdk.DIAttributes;

    [AddScoped<IGenericType<int>, GenericType<int>>]
    class GenericType<T> : IGenericType<T> { }

    public interface IGenericType<T> { }";

        // Act & Assert
        await VerifyCS.VerifyAnalyzerAsync(test);
    }

    [Fact]
    public async Task DI0001_Negative2()
    {
        // Arrange
        var test = @"
    using Mdk.DIAttributes;

    [AddScoped<IGenericType<int>>]
    class GenericType : IGenericType<int> { }

    public interface IGenericType<T> { }";

        // Act & Assert
        await VerifyCS.VerifyAnalyzerAsync(test);
    }
}

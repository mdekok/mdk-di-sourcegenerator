using Mdk.DISourceGenerator.Lib;
using System.Reflection;
using Xunit.Abstractions;

namespace Mdk.DISourceGenerator.UnitTests;

#pragma warning disable MethodDocumentationHeader // The method must have a documentation header.
#pragma warning disable ClassDocumentationHeader // The class must have a documentation header.

public class DISourceGeneratorTests(ITestOutputHelper output)
{
    private static readonly string assemblyName = Assembly.GetExecutingAssembly().GetName().Name!;
    private readonly ITestOutputHelper output = output;

    [Theory]
    // Attribute without ServiceType or ImplementationType.
    [InlineData("A01", "[AddSingleton]", "AddSingleton<global::Library1.Greeter>()")]
    [InlineData("A02", "[AddScoped]", "AddScoped<global::Library1.Greeter>()")]
    [InlineData("A03", "[AddTransient]", "AddTransient<global::Library1.Greeter>()")]

    // Attribute with generic ServiceType class.
    [InlineData("A11", "[AddSingleton<Greeter>]", "AddSingleton<global::Library1.Greeter>()")]
    [InlineData("A12", "[AddScoped<Greeter>]", "AddScoped<global::Library1.Greeter>()")]
    [InlineData("A13", "[AddTransient<Greeter>]", "AddTransient<global::Library1.Greeter>()")]

    // Attribute with generic ServiceType interface.
    [InlineData("A21", "[AddSingleton<IGreeter>]", "AddSingleton<global::Library1.IGreeter, global::Library1.Greeter>()")]
    [InlineData("A22", "[AddScoped<IGreeter>]", "AddScoped<global::Library1.IGreeter, global::Library1.Greeter>()")]
    [InlineData("A23", "[AddTransient<IGreeter>]", "AddTransient<global::Library1.IGreeter, global::Library1.Greeter>()")]

    // Attribute with generic ServiceType and ImplementationType.
    [InlineData("A31", "[AddSingleton<IGreeter, Greeter>]", "AddSingleton<global::Library1.IGreeter, global::Library1.Greeter>()")]
    [InlineData("A32", "[AddScoped<IGreeter, Greeter>]", "AddScoped<global::Library1.IGreeter, global::Library1.Greeter>()")]
    [InlineData("A33", "[AddTransient<IGreeter, Greeter>]", "AddTransient<global::Library1.IGreeter, global::Library1.Greeter>()")]

    // Attribute with ServiceType class as parameter.
    [InlineData("A41", "[AddSingleton(typeof(Greeter))]", "AddSingleton<global::Library1.Greeter>()")]
    [InlineData("A42", "[AddScoped(typeof(Greeter))]", "AddScoped<global::Library1.Greeter>()")]
    [InlineData("A43", "[AddTransient(typeof(Greeter))]", "AddTransient<global::Library1.Greeter>()")]

    // Attribute with ServiceType interface as parameter.
    [InlineData("A51", "[AddSingleton(typeof(IGreeter))]", "AddSingleton<global::Library1.IGreeter, global::Library1.Greeter>()")]
    [InlineData("A52", "[AddScoped(typeof(IGreeter))]", "AddScoped<global::Library1.IGreeter, global::Library1.Greeter>()")]
    [InlineData("A53", "[AddTransient(typeof(IGreeter))]", "AddTransient<global::Library1.IGreeter, global::Library1.Greeter>()")]

    // Attribute with ServiceType and ImplementationType as parameters.
    [InlineData("A61", "[AddSingleton(typeof(IGreeter), typeof(Greeter))]", "AddSingleton<global::Library1.IGreeter, global::Library1.Greeter>()")]
    [InlineData("A62", "[AddScoped(typeof(IGreeter), typeof(Greeter))]", "AddScoped<global::Library1.IGreeter, global::Library1.Greeter>()")]
    [InlineData("A63", "[AddTransient(typeof(IGreeter), typeof(Greeter))]", "AddTransient<global::Library1.IGreeter, global::Library1.Greeter>()")]

    // Exotic cases
    [InlineData("A101", "[AddSingleton<Greeter, Greeter>]", "AddSingleton<global::Library1.Greeter>()")]
    [InlineData("A102", "[AddSingleton(typeof(Greeter), typeof(Greeter)]", "AddSingleton<global::Library1.Greeter>()")]
    [InlineData("A103", "[AddSingleton<Library1.Greeter>]", "AddSingleton<global::Library1.Greeter>()")]
    [InlineData("A104", "[AddSingleton(typeof(Library1.Greeter)]", "AddSingleton<global::Library1.Greeter>()")]
    public void DIAttribute_GeneratesRegistration(string code, string attribute, string generatedSource)
    {
        this.output.WriteLine(code);
        this.output.WriteLine(attribute);
        this.output.WriteLine(generatedSource);

        // Arrange
        string inputSource = $$"""
            using Mdk.DIAttributes;
            
            namespace Library1;

            {{attribute}}
            public class Greeter : IGreeter
            {
                public string Greet() => "Hello from Library1!";
            }
            
            public interface IGreeter
            {
                string Greet();
            }
            """;
        string expectedOutputSource = DISourceWriter.MergeRegistrationSourceCode(assemblyName, [generatedSource]);

        // Act
        string? outputSource = DISourceGeneratorCompiler.GetGeneratedOutput(inputSource, assemblyName);

        // Assert
        Assert.Equal(expectedOutputSource, outputSource);
    }

    [Theory]
    // Attribute without ServiceType or ImplementationType and generic class.
    [InlineData("B01", "[AddSingleton]", "AddSingleton(typeof(global::Library1.GenericType<>))")]
    [InlineData("B02", "[AddScoped]", "AddScoped(typeof(global::Library1.GenericType<>))")]
    [InlineData("B03", "[AddTransient]", "AddTransient(typeof(global::Library1.GenericType<>))")]

    // Attribute with unbound generic ServiceType as parameter.
    [InlineData("B11", "[AddSingleton(typeof(Library1.IGenericType<>))]", "AddSingleton(typeof(global::Library1.IGenericType<>), typeof(global::Library1.GenericType<>))")]
    [InlineData("B12", "[AddScoped(typeof(Library1.IGenericType<>))]", "AddScoped(typeof(global::Library1.IGenericType<>), typeof(global::Library1.GenericType<>))")]
    [InlineData("B13", "[AddTransient(typeof(Library1.IGenericType<>))]", "AddTransient(typeof(global::Library1.IGenericType<>), typeof(global::Library1.GenericType<>))")]

    // Attribute with unbound generic ServiceType and ImplementationType as parameter.
    [InlineData("B21", "[AddSingleton(typeof(Library1.IGenericType<>), typeof(Library1.GenericType<>))]", "AddSingleton(typeof(global::Library1.IGenericType<>), typeof(global::Library1.GenericType<>))")]
    [InlineData("B22", "[AddScoped(typeof(Library1.IGenericType<>), typeof(Library1.GenericType<>))]", "AddScoped(typeof(global::Library1.IGenericType<>), typeof(global::Library1.GenericType<>))")]
    [InlineData("B23", "[AddTransient(typeof(Library1.IGenericType<>), typeof(Library1.GenericType<>))]", "AddTransient(typeof(global::Library1.IGenericType<>), typeof(global::Library1.GenericType<>))")]

    // Attribute with unbound generic ServiceType and ImplementationType as parameter.
    [InlineData("B31", "[AddSingleton(typeof(Library1.IGenericType<>), typeof(Library1.GenericType<>))]", "AddSingleton(typeof(global::Library1.IGenericType<>), typeof(global::Library1.GenericType<>))")]
    [InlineData("B32", "[AddScoped(typeof(Library1.IGenericType<>), typeof(Library1.GenericType<>))]", "AddScoped(typeof(global::Library1.IGenericType<>), typeof(global::Library1.GenericType<>))")]
    [InlineData("B33", "[AddTransient(typeof(Library1.IGenericType<>), typeof(Library1.GenericType<>))]", "AddTransient(typeof(global::Library1.IGenericType<>), typeof(global::Library1.GenericType<>))")]

    // Attribute with bound generic ImplementationType.
    [InlineData("B41", "[AddSingleton<Library1.GenericType<int>>]", "AddSingleton<global::Library1.GenericType<int>>()")]
    [InlineData("B42", "[AddScoped<Library1.GenericType<int>>]", "AddScoped<global::Library1.GenericType<int>>()")]
    [InlineData("B43", "[AddTransient<Library1.GenericType<int>>]", "AddTransient<global::Library1.GenericType<int>>()")]

    // Attribute with bound generic ServiceType not supported.
    [InlineData("B51", "[AddSingleton<Library1.IGenericType<int>>]")]
    [InlineData("B52", "[AddScoped<Library1.IGenericType<int>>]")]
    [InlineData("B53", "[AddTransient<Library1.IGenericType<int>>]")]

    // Attribute with bound generic ServiceType and ImplementationType.
    [InlineData("B61", "[AddSingleton<Library1.IGenericType<int>, Library1.GenericType<int>>]", "AddSingleton<global::Library1.IGenericType<int>, global::Library1.GenericType<int>>()")]
    [InlineData("B62", "[AddScoped<Library1.IGenericType<int>, Library1.GenericType<int>>]", "AddScoped<global::Library1.IGenericType<int>, global::Library1.GenericType<int>>()")]
    [InlineData("B63", "[AddTransient<Library1.IGenericType<int>, Library1.GenericType<int>>]", "AddTransient<global::Library1.IGenericType<int>, global::Library1.GenericType<int>>()")]

    // Attribute with bound generic ServiceType as parameter.
    [InlineData("B71", "[AddSingleton(typeof(Library1.GenericType<int>))]", "AddSingleton<global::Library1.GenericType<int>>()")]
    [InlineData("B72", "[AddScoped(typeof(Library1.GenericType<int>))]", "AddScoped<global::Library1.GenericType<int>>()")]
    [InlineData("B73", "[AddTransient(typeof(Library1.GenericType<int>))]", "AddTransient<global::Library1.GenericType<int>>()")]

    // Attribute with bound generic ServiceType and ImplementationType as parameter.
    [InlineData("B81", "[AddSingleton(typeof(Library1.IGenericType<int>), typeof(Library1.GenericType<int>))]", "AddSingleton<global::Library1.IGenericType<int>, global::Library1.GenericType<int>>()")]
    [InlineData("B82", "[AddScoped(typeof(Library1.IGenericType<int>), typeof(Library1.GenericType<int>))]", "AddScoped<global::Library1.IGenericType<int>, global::Library1.GenericType<int>>()")]
    [InlineData("B83", "[AddTransient(typeof(Library1.IGenericType<int>), typeof(Library1.GenericType<int>))]", "AddTransient<global::Library1.IGenericType<int>, global::Library1.GenericType<int>>()")]

    public void DIAttributeWithGenericTypes_GeneratesRegistration(string code, string attribute, string? generatedSource = null)
    {
        this.output.WriteLine(code);
        this.output.WriteLine(attribute);
        this.output.WriteLine(generatedSource ?? "null");

        // Arrange
        string inputSource = $$$"""
            using Mdk.DIAttributes;

            namespace Library1;

            {{{attribute}}}
            internal class GenericType<T> : IGenericType<T>
            {
                public T Value { get; set; }
            }

            public interface IGenericType<T>
            {
                T Value { get; set; }
            }
            """;
        string expectedOutputSource = DISourceWriter.MergeRegistrationSourceCode(assemblyName, generatedSource is not null ? [generatedSource] : null);

        // Act
        string? outputSource = DISourceGeneratorCompiler.GetGeneratedOutput(inputSource, assemblyName);

        // Assert
        Assert.Equal(expectedOutputSource, outputSource);
    }

    [Theory]
    // Attribute without ServiceType or ImplementationType and generic class.
    [InlineData("C01", "[AddSingleton]", "AddSingleton(typeof(global::Library1.GenericType<,>))")]
    [InlineData("C02", "[AddScoped]", "AddScoped(typeof(global::Library1.GenericType<,>))")]
    [InlineData("C03", "[AddTransient]", "AddTransient(typeof(global::Library1.GenericType<,>))")]

    // Attribute with unbound generic ServiceType as parameter.
    [InlineData("C11", "[AddSingleton(typeof(Library1.GenericType<,>))]", "AddSingleton(typeof(global::Library1.GenericType<,>))")]
    [InlineData("C12", "[AddScoped(typeof(Library1.GenericType<,>))]", "AddScoped(typeof(global::Library1.GenericType<,>))")]
    [InlineData("C13", "[AddTransient(typeof(Library1.GenericType<,>))]", "AddTransient(typeof(global::Library1.GenericType<,>))")]

    // Attribute with unbound generic ServiceType and ImplementationType as parameter.
    [InlineData("C21", "[AddSingleton(typeof(Library1.IGenericType<,>), typeof(Library1.GenericType<,>))]", "AddSingleton(typeof(global::Library1.IGenericType<,>), typeof(global::Library1.GenericType<,>))")]
    [InlineData("C22", "[AddScoped(typeof(Library1.IGenericType<,>), typeof(Library1.GenericType<,>))]", "AddScoped(typeof(global::Library1.IGenericType<,>), typeof(global::Library1.GenericType<,>))")]
    [InlineData("C23", "[AddTransient(typeof(Library1.IGenericType<,>), typeof(Library1.GenericType<,>))]", "AddTransient(typeof(global::Library1.IGenericType<,>), typeof(global::Library1.GenericType<,>))")]

    // Attribute with bound generic ServiceType.
    [InlineData("C31", "[AddSingleton<Library1.GenericType<int, string>>]", "AddSingleton<global::Library1.GenericType<int, string>>()")]
    [InlineData("C32", "[AddScoped<Library1.GenericType<int, string>>]", "AddScoped<global::Library1.GenericType<int, string>>()")]
    [InlineData("C33", "[AddTransient<Library1.GenericType<int, string>>]", "AddTransient<global::Library1.GenericType<int, string>>()")]

    // Attribute with bound generic ServiceType and ImplementationType.
    [InlineData("C41", "[AddSingleton<Library1.IGenericType<int, string>, Library1.GenericType<int, string>>]", "AddSingleton<global::Library1.IGenericType<int, string>, global::Library1.GenericType<int, string>>()")]
    [InlineData("C42", "[AddScoped<Library1.IGenericType<int, string>, Library1.GenericType<int, string>>]", "AddScoped<global::Library1.IGenericType<int, string>, global::Library1.GenericType<int, string>>()")]
    [InlineData("C43", "[AddTransient<Library1.IGenericType<int, string>, Library1.GenericType<int, string>>]", "AddTransient<global::Library1.IGenericType<int, string>, global::Library1.GenericType<int, string>>()")]

    // Attribute with bound generic ServiceType as parameter.
    [InlineData("C51", "[AddSingleton(typeof(Library1.GenericType<int, string>))]", "AddSingleton<global::Library1.GenericType<int, string>>()")]
    [InlineData("C52", "[AddScoped(typeof(Library1.GenericType<int, string>))]", "AddScoped<global::Library1.GenericType<int, string>>()")]
    [InlineData("C53", "[AddTransient(typeof(Library1.GenericType<int, string>))]", "AddTransient<global::Library1.GenericType<int, string>>()")]

    // Attribute with bound generic ServiceType and ImplementationType as parameter.
    [InlineData("C61", "[AddSingleton(typeof(Library1.IGenericType<int, string>), typeof(Library1.GenericType<int, string>))]", "AddSingleton<global::Library1.IGenericType<int, string>, global::Library1.GenericType<int, string>>()")]
    [InlineData("C62", "[AddScoped(typeof(Library1.IGenericType<int, string>), typeof(Library1.GenericType<int, string>))]", "AddScoped<global::Library1.IGenericType<int, string>, global::Library1.GenericType<int, string>>()")]
    [InlineData("C63", "[AddTransient(typeof(Library1.IGenericType<int, string>), typeof(Library1.GenericType<int, string>))]", "AddTransient<global::Library1.IGenericType<int, string>, global::Library1.GenericType<int, string>>()")]
    public void DIAttributeWithMultipleGenericTypes_GeneratesRegistration(string code, string attribute, string generatedSource)
    {
        this.output.WriteLine(code);
        this.output.WriteLine(attribute);
        this.output.WriteLine(generatedSource);

        // Arrange
        string inputSource = $$$"""
            using Mdk.DIAttributes;

            namespace Library1;

            {{{attribute}}}
            internal class GenericType<T, U> : IGenericType<T, U> { }

            public interface IGenericType<T, U> { }
            """;
        string expectedOutputSource = DISourceWriter.MergeRegistrationSourceCode(assemblyName, [generatedSource]);

        // Act
        string? outputSource = DISourceGeneratorCompiler.GetGeneratedOutput(inputSource, assemblyName);

        // Assert
        Assert.Equal(expectedOutputSource, outputSource);
    }


    [Theory]
    [InlineData("Singleton")]
    [InlineData("AddSingletonX")]
    public void InvalidAttributeName_GeneratesNoRegistration(string attribute)
    {
        // Arrange
        string input = $$"""
            using Mdk.DIAttributes;

            namespace Library1;

            [{{attribute}}]
            public class Greeter : IGreeter
            {
                public string Greet() => "Hello from Library1!";
            }
            
            public interface IGreeter
            {
                string Greet();
            }
            """;

        // Act
        string? output = DISourceGeneratorCompiler.GetGeneratedOutput(input, assemblyName);

        // Assert
        Assert.Null(output);
    }

    [Theory]
    [InlineData("Assembly.Name", "AssemblyName")]
    [InlineData("Assembly.Name.And.More", "AssemblyNameAndMore")]
    public void AssemblyNameWithDots_RemovesDots(string assemblyName, string sanitizedAssemblyName)
    {
        // Arrange
        string input = $$$"""
            using Mdk.DIAttributes;

            namespace Library1;

            [AddScoped]
            public class Greeter { }
            """;

        // Act
        string? output = DISourceGeneratorCompiler.GetGeneratedOutput(input, assemblyName);

        // Assert
        Assert.Contains($"registeredServices{sanitizedAssemblyName}", output);
    }

    [Fact]
    public void MultipleAttributes_GeneratesMultipleRegistrations()
    {
        // Arrange
        string input = $$$"""
            using Mdk.DIAttributes;

            namespace Library1;

            public interface IInterface1 { }
            public interface IInterface2 { }

            [AddScoped<IInterface1>]
            [AddScoped<IInterface2>]
            public class MultipleInterfacedClass : IInterface1, IInterface2 { }
            """;
        string expectedResult = DISourceWriter.MergeRegistrationSourceCode(assemblyName, [
            "AddScoped<global::Library1.IInterface1, global::Library1.MultipleInterfacedClass>()",
            "AddScoped<global::Library1.IInterface2, global::Library1.MultipleInterfacedClass>()"]);

        // Act
        string? output = DISourceGeneratorCompiler.GetGeneratedOutput(input, assemblyName);

        // Assert
        Assert.Equal(expectedResult, output);
    }

    [Fact]
    public void DI0001_GeneratesNoRegistration()
    {
        // Arrange
        string input = $$$"""
            using Mdk.DIAttributes;

            namespace ns;

            public interface IInterface<T> { }

            [AddScoped<IInterface<int>>]
            public class GenericType<T> : IInterface<T> { }
            """;
        string expectedOutputSource = DISourceWriter.MergeRegistrationSourceCode(assemblyName);

        // Act
        string? outputSource = DISourceGeneratorCompiler.GetGeneratedOutput(input, assemblyName);

        // Assert
        Assert.Equal(expectedOutputSource, outputSource);
    }

    [Fact]
    public void DI0002_GeneratesNoRegistration()
    {
        // Arrange
        string input = $$$"""
            using Mdk.DIAttributes;

            namespace ns;

            public interface IInterface { }

            [AddScoped<IInterface>]
            public class NonInterfacedClass { }
            """;
        string expectedOutputSource = DISourceWriter.MergeRegistrationSourceCode(assemblyName);

        // Act
        string? outputSource = DISourceGeneratorCompiler.GetGeneratedOutput(input, assemblyName);

        // Assert
        Assert.Equal(expectedOutputSource, outputSource);
    }

    [Fact]
    public void DI0003_GeneratesNoRegistration()
    {
        // Arrange
        string input = $$$"""
            using Mdk.DIAttributes;

            namespace ns;

            [AddScoped<IInterface, Implementation>]
            internal class DI0003 { }

            public interface IInterface { }

            internal class Implementation : IInterface { }
            """;
        string expectedResult = DISourceWriter.MergeRegistrationSourceCode(assemblyName);

        // Act
        string? output = DISourceGeneratorCompiler.GetGeneratedOutput(input, assemblyName);

        // Assert
        Assert.Equal(expectedResult, output);
    }

    [Fact]
    public void DI0004_GeneratesNoRegistration()
    {
        // Arrange
        string input = $$$"""
            using Mdk.DIAttributes;

            namespace ns;

            [AddScoped<OtherClass>]
            internal class DI0004 { }

            internal class OtherClass { }
            """;
        string expectedResult = DISourceWriter.MergeRegistrationSourceCode(assemblyName);

        // Act
        string? output = DISourceGeneratorCompiler.GetGeneratedOutput(input, assemblyName);

        // Assert
        Assert.Equal(expectedResult, output);
    }

    [Fact]
    public void ClassWithInheritedInterface_GeneratesRegistration()
    {
        // Arrange
        string input = $$$"""
            using Mdk.DIAttributes;

            namespace Library1;

            public interface IInterface { }

            [AddScoped<IInterface>]
            public class IndirectInterfacedClass : BaseClass { }

            public class BaseClass: IInterface { }
            """;
        string expectedOutputSource = DISourceWriter.MergeRegistrationSourceCode(assemblyName,
            ["AddScoped<global::Library1.IInterface, global::Library1.IndirectInterfacedClass>()"]);

        // Act
        string? outputSource = DISourceGeneratorCompiler.GetGeneratedOutput(input, assemblyName);

        // Assert
        Assert.Equal(expectedOutputSource, outputSource);
    }

    [Fact]
    public void TestInterfaces_GeneratesRegistration()
    {
        // Arrange
        string input = $$$"""
            using Mdk.DIAttributes;

            namespace Library1;

            public interface IInterface<T1, T2> { }
            public class class1 {}

            [AddSingleton<IInterface<class1, string>>]
            public class MyClass: IInterface<class1, string> { }
            """;
        string expectedOutputSource = DISourceWriter.MergeRegistrationSourceCode(assemblyName,
            ["AddSingleton<global::Library1.IInterface<Library1.class1, string>, global::Library1.MyClass>()"]);

        // Act
        string? outputSource = DISourceGeneratorCompiler.GetGeneratedOutput(input, assemblyName);

        // Assert
        Assert.Equal(expectedOutputSource, outputSource);
    }

    [Fact]
    public void MultiLevelGenericsValueType_GeneratesRegistration()
    {
        // Arrange
        string input = $$$"""
            using Mdk.DIAttributes;

            namespace Library1;

            public interface Intf1<T> { }
            public interface Intf2<T> { }
            public class Impl1<T> { }

            [AddScoped<Intf1<Impl1<int>>, Impl2<int>>]
            public class Impl2<T> : Intf1<Impl1<T>> { }
            """;
        string expectedOutputSource = DISourceWriter.MergeRegistrationSourceCode(assemblyName,
            ["AddScoped<global::Library1.Intf1<Library1.Impl1<int>>, global::Library1.Impl2<int>>()"]);

        // Act
        string? outputSource = DISourceGeneratorCompiler.GetGeneratedOutput(input, assemblyName);

        // Assert
        Assert.Equal(expectedOutputSource, outputSource);
    }

    [Fact]
    public void MultiLevelGenericsReferenceType_GeneratesRegistration()
    {
        // Arrange
        string input = $$$"""
            using Mdk.DIAttributes;

            namespace Library1;

            public interface Intf1<T> { }
            public interface Intf2<T> { }
            public class Impl1<T> { }
            public class Impl2 { }

            [AddScoped<Intf1<Impl1<Impl2>>, Impl2<Impl2>>]
            public class Impl2<T> : Intf1<Impl1<T>> { }
            """;
        string expectedOutputSource = DISourceWriter.MergeRegistrationSourceCode(assemblyName,
            ["AddScoped<global::Library1.Intf1<Library1.Impl1<Library1.Impl2>>, global::Library1.Impl2<Library1.Impl2>>()"]);

        // Act
        string? outputSource = DISourceGeneratorCompiler.GetGeneratedOutput(input, assemblyName);

        // Assert
        Assert.Equal(expectedOutputSource, outputSource);
    }

    [Fact]
    public void RequestResponse_GeneratesRegistration()
    {
        // Arrange
        string input = $$$"""
            using Mdk.DIAttributes;

            namespace Library1;

            public interface IRequest<out TResponse> {}
            public interface IRequestHandler<TRequest, TResponse> {}

            public record RequestRecord(RequestClass RequestClass) : IRequest<Result<ResponseClass, Error>>;
            public class RequestClass {}
            public class ResponseClass {}
            public class Result<TValue, TError> {}
            public record Error(string Message, int Code = 0);

            [AddSingleton<IRequestHandler<RequestClass, ResponseClass>>]
            public class MyClass: IRequestHandler<RequestClass, ResponseClass> { }
            """;
        string expectedOutputSource = DISourceWriter.MergeRegistrationSourceCode(assemblyName,
            ["AddSingleton<global::Library1.IRequestHandler<Library1.RequestClass, Library1.ResponseClass>, global::Library1.MyClass>()"]);

        // Act
        string? outputSource = DISourceGeneratorCompiler.GetGeneratedOutput(input, assemblyName);

        // Assert
        Assert.Equal(expectedOutputSource, outputSource);
    }
}
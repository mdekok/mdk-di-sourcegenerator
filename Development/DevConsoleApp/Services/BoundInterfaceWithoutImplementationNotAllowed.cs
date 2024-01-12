using Mdk.DIAttributes;

namespace DevConsoleApp.Services;

// For testing diagnostic: DI001, Implementation type missing
[AddScoped<IGenericType<int>, GenericType<int>>]
// [AddScoped<IGenericType<int>>]
internal class GenericType<T> : IGenericType<T>
{
    public T? Value { get; set; }
}

public interface IGenericType<T>
{
    T? Value { get; set; }
}

// For testing diagnostic: DI001, Implementation type missing
[AddScoped<IGenericType<int>>]
internal class NonGenericType : IGenericType<int>
{
    public int Value { get; set; }
}


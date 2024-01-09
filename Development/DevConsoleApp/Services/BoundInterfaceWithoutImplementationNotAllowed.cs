using Mdk.DIAttributes;

namespace DevConsoleApp.Services;

// For testen diagnostic: DI001, Implementation type missing
// [AddScoped<IGenericType<int>>]
[AddScoped<IGenericType<int>, GenericType<int>>]
internal class GenericType<T> : IGenericType<T>
{
    public T? Value { get; set; }
}

public interface IGenericType<T>
{
    T? Value { get; set; }
}

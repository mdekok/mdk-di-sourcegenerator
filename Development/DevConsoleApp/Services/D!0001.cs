using Mdk.DIAttributes;

namespace DevConsoleApp.Services;

// For testing diagnostic: DI001, Implementation type missing
// [AddScoped<IGenericType<int>>]
internal class GenericType<T> : IGenericType<T> { }

public interface IGenericType<T> { }

// For testing diagnostic: DI001, Implementation type missing
[AddScoped<IGenericType<int>>]
internal class NonGenericType : IGenericType<int> { }


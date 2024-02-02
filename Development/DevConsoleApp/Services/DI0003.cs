using Mdk.DIAttributes;

namespace DevConsoleApp.Services;

// For testing diagnostic: DI003, Implementation type is not the same as class type
[AddScoped<IInterface, DI0003>]
// [AddScoped<IInterface, Implementation>] // Uncomment to test diagnostic
internal class DI0003 : IInterface { }

public interface IInterface { }

internal class Implementation : IInterface { }
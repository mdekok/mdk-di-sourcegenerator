using Mdk.DIAttributes;

namespace DevConsoleApp.Services;

[AddScoped<IInterface, DI0003>]
// [AddScoped<IInterface, Implementation>]
internal class DI0003 : IInterface
{
}

public interface IInterface { }

internal class Implementation : IInterface { }
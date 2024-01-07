using Mdk.DIAttributes;

namespace DevConsoleApp.Services;

public interface IInterface1 { }

[AddScoped<IInterface1>]
public class RegisterInterfaceMultipleTimes1 : IInterface1 { }

[AddScoped<IInterface1>]
public class RegisterInterfaceMultipleTimes2 : IInterface1 { }
using Mdk.DIAttributes;

namespace DevConsoleApp.Services;

[AddScoped<IDummyInterface>]
public class RegisterInterfaceMultipleTimes1 : IDummyInterface { }

[AddScoped<IDummyInterface>]
public class RegisterInterfaceMultipleTimes2 : IDummyInterface { }
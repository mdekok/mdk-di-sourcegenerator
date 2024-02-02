using Mdk.DIAttributes;

namespace DevConsoleApp.Services;

// For testing diagnostic: DI004, Class type is not the same as or subclass of service class type
[AddScoped<DI0004Super2Class>]
// public class DI0004 { } // Uncomment to test diagnostic
public class DI0004 : DI0004SuperClass { }

public class DI0004SuperClass : DI0004Super2Class { }

public class DI0004Super2Class { }
using Mdk.DIAttributes;

namespace BusinessLogic;

[AddScoped]
public class MyService { }

[AddScoped<IInterface1>]
public class MyInterfacedService : IInterface1 { }

public interface IInterface1 { }


[AddScoped]
public class MyGenericService<T> { }

[AddScoped(typeof(IInterface2<>))]
public class MyInterfacedGenericService<T> : IInterface2<T> { }

public interface IInterface2<T> { }

[AddScoped<IInterface2<int>>]
public class MyInterfacedIntGenericService : IInterface2<int> { }


[AddScoped<IInterface3>]
[AddScoped<IInterface4>]
public class MyDoubleInterfacedService : IInterface3, IInterface4 { }

public interface IInterface3 { }
public interface IInterface4 { }


// Following two classes generate compiler errors
// because of multiple registrations for the same service type.
// This is a diagnostic.
// Uncomment to test.
//[AddScoped<IInterface5>]
//public class RegisterInterfaceMultipleTimes1 : IInterface5 { }

//[AddScoped<IInterface5>]
//public class RegisterInterfaceMultipleTimes2 : IInterface5 { }

//public interface IInterface5 { }
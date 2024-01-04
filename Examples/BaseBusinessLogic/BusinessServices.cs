using Mdk.DIAttributes;

namespace BusinessBaseLogic;

[AddScoped]
public class MyBaseService { }

[AddScoped<IBaseInterface>]
public class MyInterfacedBaseService : IBaseInterface { }

public interface IBaseInterface { }
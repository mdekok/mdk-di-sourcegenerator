using Mdk.DIAttributes;

namespace VsixTestClassLib;

[AddScoped<IInterface3, Implementation>]
public class DI0003 : IInterface3 { }

public interface IInterface3 { }

public class Implementation : IInterface3 { }

using Mdk.DIAttributes;

namespace VsixTestClassLib;

[AddScoped<IInterface2>]
public class DI0002 // : IInterface
{ }

public interface IInterface2 { }

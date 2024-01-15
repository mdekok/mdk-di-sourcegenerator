using Mdk.DIAttributes;

namespace VsixTestClassLib;

[AddScoped<IInterface1<int>>]
public class DISG0001<T> : IInterface1<int> { }

public interface IInterface1<T> { }

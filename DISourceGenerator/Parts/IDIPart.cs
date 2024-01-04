namespace Mdk.DISourceGenerator.Parts;

/// <summary>The DIPart interface implemented by the class, service and implementation type.</summary>
public interface IDIPart
{
    /// <summary>Gets a value indicating whether the DI part is a generic type.</summary>
    bool IsGeneric { get; }

    /// <summary>Converts DIPart to source.</summary>
    /// <returns>partial source string.</returns>
    string ToSource();
}
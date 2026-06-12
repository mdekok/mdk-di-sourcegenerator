namespace Mdk.DISourceGenerator.Analyzers.Lib;

public readonly struct ValidationResult(bool hasDiagnostic, bool canGenerateSource = true)
{
    public bool HasDiagnostic { get; } = hasDiagnostic;
    public bool CanGenerateSource { get; } = canGenerateSource;

    public static ValidationResult NoDiagnostic { get; } = new(false);
}

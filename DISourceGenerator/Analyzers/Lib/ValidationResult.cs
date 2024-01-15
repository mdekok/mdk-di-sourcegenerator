namespace Mdk.DISourceGenerator.Analyzers.Lib;

public readonly struct ValidationResult
{
    public ValidationResult(bool hasDiagnostic, bool canGenerateSource = true)
    {
        this.HasDiagnostic = hasDiagnostic;
        this.CanGenerateSource = canGenerateSource;
    }

    public bool HasDiagnostic { get; }
    public bool CanGenerateSource { get; }

    public static ValidationResult NoDiagnostic { get; } = new(false);
}

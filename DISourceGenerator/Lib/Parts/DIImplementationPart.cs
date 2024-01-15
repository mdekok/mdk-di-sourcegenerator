using Microsoft.CodeAnalysis;

namespace Mdk.DISourceGenerator.Lib.Parts;

/// <summary>DIAttributePart of the implementation type.</summary>
public class DIImplementationPart(AttributeData attribute)
    : DIAttributePart(attribute)
{
    /// <inheritdoc />
    protected override string AttributeParameterTypeName => "ImplementationType";

    /// <inheritdoc />
    protected override int AttributeParameterIndex => 1;
}

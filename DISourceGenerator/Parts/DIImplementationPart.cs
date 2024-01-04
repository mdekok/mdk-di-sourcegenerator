using Microsoft.CodeAnalysis;

namespace Mdk.DISourceGenerator.Parts;

/// <summary>DIAttributePart of the implementation type.</summary>
internal class DIImplementationPart(AttributeData attribute)
    : DIAttributePart(attribute)
{
    /// <inheritdoc />
    protected override string AttributeParameterTypeName => "ImplementationType";

    /// <inheritdoc />
    protected override int AttributeParameterIndex => 1;
}

using Mdk.DISourceGenerator.Lib.Parts;
using Microsoft.CodeAnalysis;

namespace Mdk.DISourceGenerator.Lib;

/// <summary>Builds a <seealso cref="DIRegistration"/> record containing all DI registration data.</summary>
public static class DIRegistrationBuilder
{
    /// <summary>Builds a <seealso cref="DIRegistration"/> record containing all DI registration data.</summary>
    /// <param name="attribute">The DIAttribute.</param>
    /// <param name="classType">The class type the DIAttribute is assigned to.</param>
    /// <returns>A nullable DIRegistration record.</returns>
    public static DIRegistration? Build(AttributeData attribute, DIClassPart classType)
    {
        if (attribute.AttributeClass is not INamedTypeSymbol attributeClass)
            return null;

        string method = attributeClass.Name; // AddSingleton, AddScoped or AddTransient.

        var serviceType = new DIServicePart(attribute);
        var implementationType = new DIImplementationPart(attribute);

        if (implementationType.IsDefined)
            // [Add{Lifetime}(typeof(ServiceType), typeof(ImplementationType))]
            // or [Add{Lifetime}<ServiceType, ImplementationType>]
            return new(method, serviceType, implementationType, doNotGenerateAsGeneric: serviceType.IsUnboundGeneric);

        if (serviceType.IsDefined)
        {
            if (serviceType.IsUnboundGeneric)
                // [Add{Lifetime}(typeof(ServiceType<>))]
                return new(method, serviceType, classType, doNotGenerateAsGeneric: true);

            if (serviceType.IsGeneric)
                // [Add{Lifetime}<ServiceType<int>>] or [Add{Lifetime}(typeof(ServiceType<int>))]
                return classType.IsGeneric
                    ? new(method, serviceType)
                    : new(method, serviceType, classType);

            return new(method, serviceType, classType);
        }

        // [AddSingleton], [AddScoped] or [AddTransient]
        return new(method, classType, doNotGenerateAsGeneric: classType.IsGeneric);
    }
}

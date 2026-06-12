using Mdk.DISourceGenerator.Lib.Parts;
using Microsoft.CodeAnalysis;
using System.Linq;

namespace Mdk.DISourceGenerator.Lib;

/// <summary>Builds a <seealso cref="DIRegistration"/> record containing all DI registration data.</summary>
internal static class DIRegistrationBuilder
{
    /// <summary>Builds a <seealso cref="DIRegistration"/> record containing all DI registration data.</summary>
    /// <param name="attribute">The DIAttribute.</param>
    /// <param name="classSymbol">The class syntax the DIAttribute is assigned to.</param>
    /// <returns>A nullable DIRegistration record.</returns>
    public static DIRegistration? Build(AttributeData attribute, INamedTypeSymbol classSymbol)
    {
        if (attribute.AttributeClass is not INamedTypeSymbol attributeClass)
            return null;

        string method = attributeClass.Name; // AddSingleton, AddScoped or AddTransient.

        DIClassPart classType = new(classSymbol);
        DIServicePart serviceType = new(attribute);
        DIImplementationPart implementationType = new(attribute);
        // Optional key for keyed registration is the first string argument.
        string? key = attribute.ConstructorArguments.FirstOrDefault(argument => argument.Value is string).Value as string;

        if (implementationType.IsDefined)
            // [Add{Lifetime}(typeof(ServiceType), typeof(ImplementationType))]
            // or [Add{Lifetime}<ServiceType, ImplementationType>]
            return new(method, classType, serviceType, implementationType, key, doNotGenerateAsGeneric: serviceType.IsUnboundGenericType);

        if (serviceType.IsDefined)
        {
            if (serviceType.IsUnboundGenericType)
                // [Add{Lifetime}(typeof(ServiceType<>))]
                return new(method, classType, serviceType, classType, key, doNotGenerateAsGeneric: true);

            if (serviceType.IsGenericType)
                // [Add{Lifetime}<ServiceType<int>>] or [Add{Lifetime}(typeof(ServiceType<int>))]
                return classType.IsGenericType
                    ? new(method, classType, serviceType, key: key)
                    : new(method, classType, serviceType, classType, key);

            return new(method, classType, serviceType, classType, key);
        }

        // [AddSingleton], [AddScoped] or [AddTransient]
        return new(method, classType, classType, key: key, doNotGenerateAsGeneric: classType.IsGenericType);
    }
}

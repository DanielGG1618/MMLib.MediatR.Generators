using Microsoft.CodeAnalysis;

namespace AutoApiGen.Extensions;

public static class AttributeDataExtensions
{
    public static TypedConstant ArgumentOfParameterWithName(this AttributeData attribute, string name) =>
        attribute.NamedArguments
            .FirstOrDefault(argument => argument.Key == name)
            .Value;

    public static bool HasParameterWithName(this AttributeData attribute, string name) =>
        attribute.NamedArguments
            .Any(argument => argument.Key == name);
    
    private static TypedConstant FirstConstructorArgument(this AttributeData attributeData) =>
        attributeData.ConstructorArguments.First();

    private static ITypeSymbol FirstTypeArgument(this AttributeData attributeData) =>
        attributeData.AttributeClass!.TypeArguments[0];
    
    public static string GetControllerName(this AttributeData attributeData) =>
        !attributeData.AttributeClass!.IsGenericType ? "Controller"
        : attributeData.FirstTypeArgument().IsController() ? attributeData.FirstTypeArgument().Name
        : $"{attributeData.FirstTypeArgument().Name}sController";

    public static string GetRoute(this AttributeData attributeData) =>
        attributeData.GetRouteBase() + attributeData.FirstConstructorArgument().Value;

    private static string GetRouteBase(this AttributeData attributeData) =>
        attributeData.AttributeClass!.IsGenericType
            ? attributeData.FirstTypeArgument().IsController()
                ? attributeData.FirstTypeArgument()
                    .GetAttributes()
                    .FirstOrDefault(attribute => attribute.AttributeClass?.BaseType?.Name is "RouteAttribute")?
                    .FirstConstructorArgument().Value as string ?? ""
                : $"{attributeData.FirstTypeArgument().Name.ToLower()}s/"
            : "";
}
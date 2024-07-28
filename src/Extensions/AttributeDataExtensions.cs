using Microsoft.CodeAnalysis;

namespace AutoApiGen.Extensions;

public static class AttributeDataExtensions
{
    public static string GetControllerName(this AttributeData attributeData) =>
        !attributeData.AttributeClass!.IsGenericType
            ? "Controller"
            : attributeData.FirstTypeArgument().IsController()
                ? attributeData.FirstTypeArgument().Name
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

    private static TypedConstant FirstConstructorArgument(this AttributeData attributeData) =>
        attributeData.ConstructorArguments.FirstOrDefault();

    private static ITypeSymbol FirstTypeArgument(this AttributeData attributeData) =>
        attributeData.AttributeClass!.TypeArguments[0];
}
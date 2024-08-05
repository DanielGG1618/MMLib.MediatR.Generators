using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoApiGen.Extensions;

public static class AttributeSyntaxExtensions
{
    public static bool ContainsAttributeWithNameFrom(
        this IEnumerable<AttributeSyntax> attributes,
        ISet<string> names
    ) => attributes.Any(attribute => names.Contains(attribute.Name.NameOrDefault()));

    public static string FirstConstructorArgument(this AttributeSyntax attribute) =>
        attribute.ArgumentList?.Arguments
            .First().Expression is LiteralExpressionSyntax literalExpression
            ? literalExpression.Token.ValueText
            : "";
}

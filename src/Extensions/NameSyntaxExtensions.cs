using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoApiGen.Extensions;

internal static class NameSyntaxExtensions
{
    public static string GetIdentifierNameOrDefault(this NameSyntax nameSyntax, string @default = "") =>
        (nameSyntax as IdentifierNameSyntax)?.Identifier.Text ?? @default;
}

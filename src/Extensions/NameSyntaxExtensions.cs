using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoApiGen.Extensions;

internal static class NameSyntaxExtensions
{
    public static string NameOrDefault(this NameSyntax nameSyntax, string @default = "") =>
        (nameSyntax as IdentifierNameSyntax)?.Identifier.Text ?? @default;
}

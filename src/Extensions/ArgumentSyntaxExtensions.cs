using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoApiGen.Extensions;

public static class ArgumentSyntaxExtensions
{
    public static string? GetParameterName(this AttributeArgumentSyntax argument, SemanticModel semanticModel) =>
        semanticModel.GetSymbolInfo(argument.Expression)
            .Symbol is IParameterSymbol parameterSymbol
            ? parameterSymbol.Name
            : null;
}

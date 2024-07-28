using Microsoft.CodeAnalysis;

namespace AutoApiGen.Extensions;

public static class TypeSymbolExtensions
{
    public static bool IsController(this ITypeSymbol? typeSymbol) =>
        typeSymbol?.GetAttributes().Any(a => a.AttributeClass!.Name == "ApiControllerAttribute") is true;
}
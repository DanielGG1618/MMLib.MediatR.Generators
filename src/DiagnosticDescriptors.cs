using Microsoft.CodeAnalysis;

namespace AutoApiGen;

public static class DiagnosticDescriptors
{
    public static readonly DiagnosticDescriptor LiteralExpressionRequired = new(
        id: "AAG0001",
        title: "Literal expression required",
        messageFormat: "This context requires a literal expression",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );
}
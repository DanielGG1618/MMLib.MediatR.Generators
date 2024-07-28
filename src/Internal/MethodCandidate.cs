using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoApiGen.Internal;

internal record MethodCandidate(
    AttributeSyntax HttpMethodAttribute,
    SemanticModel SemanticModel,
    TypeDeclarationSyntax TypeDeclaration,
    string? RequestType
);
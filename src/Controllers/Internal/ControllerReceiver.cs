using System.Collections.Generic;
using AutoApiGen.Controllers.Internal.Models;
using AutoApiGen.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoApiGen.Controllers.Internal;

internal sealed class ControllerReceiver : ISyntaxReceiver
{
    private readonly List<TypeDeclarationSyntax> _candidates = [];

    public IReadOnlyList<TypeDeclarationSyntax> Candidates => _candidates.AsReadOnly();

    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        if (syntaxNode is TypeDeclarationSyntax typeDeclaration
            && typeDeclaration.HaveAnyOfAttributes(HttpMethods.Attributes))
            _candidates.Add(typeDeclaration);
    }
}
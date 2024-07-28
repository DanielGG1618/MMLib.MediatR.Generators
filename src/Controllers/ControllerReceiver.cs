using System.Collections.Generic;
using AutoApiGen.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoApiGen.Controllers
{
    internal sealed class ControllerReceiver : ISyntaxReceiver
    {
        private readonly List<TypeDeclarationSyntax> _candidates = new();

        public IEnumerable<TypeDeclarationSyntax> Candidates => _candidates;

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is TypeDeclarationSyntax typeDeclaration
                && typeDeclaration.HaveAnyOfAttributes(HttpMethods.Attributes))
            {
                _candidates.Add(typeDeclaration);
            }
        }
    }
}

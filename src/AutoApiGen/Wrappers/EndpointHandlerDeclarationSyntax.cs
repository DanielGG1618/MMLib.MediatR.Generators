using AutoApiGen.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoApiGen.Wrappers;

internal class EndpointHandlerDeclarationSyntax
{
    private readonly ClassDeclarationSyntax _class;

    public static EndpointHandlerDeclarationSyntax Wrap(ClassDeclarationSyntax @class)
    {
        //TODO add validation logic if possible
        return new EndpointHandlerDeclarationSyntax(@class);
    }

    public string GetEndpointMethodName() =>
        _class.Parent switch
        {
            TypeDeclarationSyntax parentType => parentType.Name(),
            NamespaceDeclarationSyntax @namespace => $"{@namespace.Name.NameOrDefault()}_{_class.Name()}",
            _ => $"{_class.Name()}Method"
        };

    public string GetControllerName() =>
        _class.AttributeLists.SelectMany(list => list.Attributes)
            .Single(attr =>
                RenameThisClass.EndpointAttributeNames.Contains(attr.Name.NameOrDefault())
            ).ArgumentList?.Arguments
            .First().Expression is LiteralExpressionSyntax literal
            ? literal.Token.ValueText.WithCapitalFirstLetter() + "Controller"
            : "ThisShitReturnedNull"; //TODO

    private EndpointHandlerDeclarationSyntax(ClassDeclarationSyntax @class) => 
        _class = @class;
}

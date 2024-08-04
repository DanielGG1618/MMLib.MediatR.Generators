using AutoApiGen.Extensions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static AutoApiGen.StaticData;

namespace AutoApiGen.Wrappers;

internal class EndpointContractDeclarationSyntax
{
    private readonly ClassDeclarationSyntax _class;
    private readonly AttributeSyntax _attribute;

    public static EndpointContractDeclarationSyntax Wrap(ClassDeclarationSyntax @class)
    {
        //TODO add validation logic if possible
        //such a class should 
        // - [x] have exactly one attribute with name from EndpointAttributeNames
        // - [ ] implement mediator IRequest
        // - [ ] be a command or query

        var attribute = @class.Attributes().Single(attr =>
            EndpointAttributeNames.Contains(attr.Name.NameOrDefault())
        );

        return new EndpointContractDeclarationSyntax(@class, attribute);
    }
    
    //TODO add static method for validation

    public string GetControllerName() =>
        _class.AttributeLists.SelectMany(list => list.Attributes)
            .Single(attr =>
                EndpointAttributeNames.Contains(attr.Name.NameOrDefault())
            ).ArgumentList?.Arguments
            .First().Expression is LiteralExpressionSyntax literal
            ? literal.Token.ValueText.WithCapitalFirstLetter() + "Controller"
            : "ThisShitReturnedNull"; //TODO

    public string GetMethodName()
    {
        if (_class.Parent is TypeDeclarationSyntax type)
            return type.Name();

        var className = _class.Name();
        
        return Suffixes.SingleOrDefault(suffix => className.EndsWith(suffix)) is { } matchingSuffix
            ? className.Remove(className.Length - matchingSuffix.Length)
            : className;
    }

    private EndpointContractDeclarationSyntax(ClassDeclarationSyntax @class, AttributeSyntax attribute)
    {
        _class = @class;
        _attribute = attribute;
    }
}

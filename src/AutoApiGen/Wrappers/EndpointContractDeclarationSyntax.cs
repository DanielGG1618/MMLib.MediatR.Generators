using AutoApiGen.Extensions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static AutoApiGen.StaticData;

namespace AutoApiGen.Wrappers;

internal class EndpointContractDeclarationSyntax
{
    private readonly ClassDeclarationSyntax _class;

    public static EndpointContractDeclarationSyntax Wrap(ClassDeclarationSyntax @class)
    {
        //TODO add validation logic if possible
        //such a class should 
        // - implement mediator IRequest
        // - be a command or query
        return new EndpointContractDeclarationSyntax(@class);
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

    private EndpointContractDeclarationSyntax(ClassDeclarationSyntax @class) => 
        _class = @class;

    public string GetMethodName()
    {
        if (_class.Parent is TypeDeclarationSyntax type)
            return type.Name();

        var className = _class.Name();
        
        return Suffixes.SingleOrDefault(suffix => className.EndsWith(suffix)) is { } matchingSuffix
            ? className.Remove(className.Length - matchingSuffix.Length)
            : className;
    }
}

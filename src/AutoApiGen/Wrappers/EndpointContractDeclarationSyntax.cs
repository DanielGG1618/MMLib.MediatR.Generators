using System.Diagnostics;
using AutoApiGen.Extensions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static AutoApiGen.StaticData;

namespace AutoApiGen.Wrappers;

internal class EndpointContractDeclarationSyntax
{
    private readonly TypeDeclarationSyntax _type;
    
    public AttributeSyntax Attribute { get; }

    public static EndpointContractDeclarationSyntax Wrap(TypeDeclarationSyntax type)
    {
        //TODO add validation logic if possible
        //such a class should 
        // - [x] have exactly one attribute with name from EndpointAttributeNames
        // - [x] implement mediator IRequest

        var attribute = type.Attributes().Single(attr =>
            EndpointAttributeNames.Contains(attr.Name.NameOrDefault())
        );
        
        var a = type.BaseList?.Types.Select(baseType => baseType.Type);

        if (type.BaseList?.Types.Any(baseType =>
                baseType.Type is SimpleNameSyntax
                {
                    Identifier.Text: "IRequest" or "ICommand" or "IQuery"
                }
            ) is false)
            throw new InvalidOperationException("Endpoint contract should implement IRequest");

        return new EndpointContractDeclarationSyntax(type, attribute);
    }
    
    //TODO add static method for validation

    public string GetHttpMethod()
    {
        var name = Attribute.Name.NameOrDefault();

        return name.Remove(name.Length - "Endpoint".Length);
    }

    public string GetMethodName()
    {
        if (_type.Parent is TypeDeclarationSyntax type)
            return type.Name();

        var className = _type.Name();
        
        return Suffixes.SingleOrDefault(suffix => className.EndsWith(suffix)) is { } matchingSuffix
            ? className.Remove(className.Length - matchingSuffix.Length)
            : className;
    }
    
    public string GetControllerName() =>
        _type.AttributeLists.SelectMany(list => list.Attributes)
            .Single(attr =>
                EndpointAttributeNames.Contains(attr.Name.NameOrDefault())
            ).ArgumentList?.Arguments
            .First().Expression is LiteralExpressionSyntax literal
            ? literal.Token.ValueText.WithCapitalFirstLetter() + "Controller"
            : "ThisShitReturnedNull"; //TODO
    
    private EndpointContractDeclarationSyntax(TypeDeclarationSyntax type, AttributeSyntax attribute)
    {
        _type = type;
        Attribute = attribute;
    }

}

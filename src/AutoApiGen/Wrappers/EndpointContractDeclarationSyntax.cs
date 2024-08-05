using System.Diagnostics;
using AutoApiGen.Extensions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static AutoApiGen.StaticData;

namespace AutoApiGen.Wrappers;

internal class EndpointContractDeclarationSyntax
{
    private readonly TypeDeclarationSyntax _type;
    private readonly AttributeSyntax _attribute;    
    private readonly RouteString _route;
    
    public string BaseRoute => 
        _route.BaseRoute;
    
    public string RelationalRoute => 
        _route.RelationalRoute;

    public static EndpointContractDeclarationSyntax Wrap(TypeDeclarationSyntax type) =>
        IsValid(type)
            ? new EndpointContractDeclarationSyntax(
                type,
                attribute: type.Attributes().Single(attr =>
                    EndpointAttributeNames.Contains(attr.Name.NameOrDefault())
                )
            )
            : throw new InvalidOperationException("Endpoint contract should implement IRequest");

    public static bool IsValid(TypeDeclarationSyntax type) =>
        type.BaseList?.Types.Any(baseType =>
            baseType.Type is SimpleNameSyntax
            {
                Identifier.Text: "IRequest" or "ICommand" or "IQuery"
            }
        ) is true;

    public string GetHttpMethod()
    {
        var name = _attribute.Name.NameOrDefault();

        return name.Remove(name.Length - "Endpoint".Length);
    }

    public string GetMethodName() =>
        _type.Parent is TypeDeclarationSyntax parent 
            ? parent.Name()
            : Suffixes.SingleOrDefault(suffix => _type.Name().EndsWith(suffix)) is {} matchingSuffix
                ? _type.Name().Remove(_type.Name().Length - matchingSuffix.Length)
                : _type.Name();

    public string GetControllerName() =>
        _route.BaseRoute.WithCapitalFirstLetter() + "Controller";
    
    private EndpointContractDeclarationSyntax(TypeDeclarationSyntax type, AttributeSyntax attribute)
    {
        _type = type;
        _attribute = attribute;
        _route = RouteString.Wrap(_attribute.FirstConstructorArgument());
    }
}

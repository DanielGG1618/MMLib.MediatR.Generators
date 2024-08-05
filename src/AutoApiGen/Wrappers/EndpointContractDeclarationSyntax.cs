﻿using AutoApiGen.Extensions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static AutoApiGen.StaticData;

namespace AutoApiGen.Wrappers;

internal class EndpointContractDeclarationSyntax
{
    private readonly TypeDeclarationSyntax _type;
    private readonly EndpointAttributeSyntax _attribute;   
    
    public string BaseRoute => 
        _attribute.BaseRoute;
    
    public string RelationalRoute => 
        _attribute.RelationalRoute;

    public static EndpointContractDeclarationSyntax Wrap(TypeDeclarationSyntax type) =>
        IsValid(type)
            ? new EndpointContractDeclarationSyntax(
                type,
                attribute: EndpointAttributeSyntax.Wrap(
                    type.Attributes().Single(attr =>
                        EndpointAttributeNames.Contains(attr.Name.NameOrDefault())
                    )
                )
            )
            : throw new InvalidOperationException("Provided type is not valid Endpoint Contract");

    public static bool IsValid(TypeDeclarationSyntax type) =>
        type.BaseList?.Types.Any(baseType =>
            baseType.Type is SimpleNameSyntax
            {
                Identifier.Text: "IRequest" or "ICommand" or "IQuery"
            }
        ) is true;

    public string GetHttpMethod() =>
        _attribute.GetHttpMethod();

    public string GetMethodName() =>
        _type.Parent is TypeDeclarationSyntax parent 
            ? parent.Name()
            : Suffixes.SingleOrDefault(suffix => _type.Name().EndsWith(suffix)) is {} matchingSuffix
                ? _type.Name().Remove(_type.Name().Length - matchingSuffix.Length)
                : _type.Name();

    public string GetControllerName() =>
        BaseRoute.WithCapitalFirstLetter() + "Controller";
    
    private EndpointContractDeclarationSyntax(TypeDeclarationSyntax type, EndpointAttributeSyntax attribute)
    {
        _type = type;
        _attribute = attribute;
    }
}

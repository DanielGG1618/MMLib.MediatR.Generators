﻿using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using AutoApiGen.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoApiGen.Internal.Models;

internal partial record MethodModel
{
    private static readonly string AdditionalParametersAttributeName =
        nameof(AdditionalParametersAttribute).Replace(nameof(Attribute), string.Empty);

    private const string RequestMethodDefinitionName = "RequestMethodDefinition";

    public static MethodModel Build(MethodCandidate candidate, Templates templates, string controllerName)
    {
        var httpType = GetMethodType(candidate);
        var name = GetMethodName(candidate);
        var template = GetTemplate(candidate);
        var typeSymbol = candidate.SemanticModel.GetDeclaredSymbol(candidate.TypeDeclaration);

        var method = new MethodModel
        {
            HttpMethod = httpType,
            Name = name,
            Template = template,
            RequestType = candidate.RequestType,
            ResponseType = GetResponseType(candidate, httpType),
            Attributes = GetAttributes(templates, controllerName, candidate.TypeDeclaration),
            Comment = GetComment(typeSymbol)
        };

        method.InitParameters(candidate, httpType, typeSymbol);

        return method;
    }

    private static string? GetAttributes(
        Templates templates,
        string controllerName,
        TypeDeclarationSyntax typeDeclaration
    )
    {
        var requestMethod = typeDeclaration.GetMethodSymbol(RequestMethodDefinitionName);
        if (requestMethod is null)
            return templates.GetControllerTemplate(TemplateType.MethodAttributes, controllerName);

        var attributes = requestMethod.AttributeLists.SelectMany(a => a.Attributes);
        var names = string.Join(
            Environment.NewLine,
            attributes.Select(b => $"[{b.Name}({b.ArgumentList?.Arguments})]")
        );
        return names;
    }

    private static string? GetComment(INamedTypeSymbol? typeSymbol)
    {
        var comment = typeSymbol?.GetDocumentationCommentXml();
        return string.IsNullOrWhiteSpace(comment)
            ? null
            : XDocument.Parse(comment)
                .Descendants("summary")
                .FirstOrDefault()?.Value.Trim();
    }

    private static string? GetTemplate(MethodCandidate candidate)
        => candidate.HttpMethodAttribute.GetFirstArgumentWithoutName();

    private static string GetMethodName(MethodCandidate candidate)
    {
        var name = candidate.HttpMethodAttribute
            .GetStringArgument(nameof(HttpGetAttribute.Name));

        if (!string.IsNullOrWhiteSpace(name))
            return name!;

        name = candidate.TypeDeclaration.GetTypeName();
        if (candidate.TypeDeclaration.Parent is ClassDeclarationSyntax classDeclaration)
            name = $"{classDeclaration.GetTypeName()}{name}";

        return name;
    }

    private static string GetMethodType(MethodCandidate candidate)
        => candidate.HttpMethodAttribute.Name.ToString().Replace(Types.Http, string.Empty);

    private void InitParameters(MethodCandidate candidate, string httpMethod, INamedTypeSymbol? typeSymbol)
    {
        List<ParameterModel> parameters = [];
        var methodFromSource
            = candidate.HttpMethodAttribute.GetArgument<MemberAccessExpressionSyntax>(nameof(HttpMethodAttribute.From));
        if (!Enum.TryParse(methodFromSource?.Name.Identifier.ValueText, out From methodFrom))
        {
            methodFrom = httpMethod switch
            {
                HttpMethods.Get    => From.Route,
                HttpMethods.Delete => From.Route,
                HttpMethods.Post   => From.Body,
                HttpMethods.Put    => From.Body,
                _                  => From.Ignore
            };
        }

        if (methodFrom != From.Ignore)
            parameters.Add(
                new ParameterModel(
                    GetParameterName(httpMethod),
                    candidate.RequestType,
                    AttributeFrom(methodFrom)
                )
            );

        InitAdditionalParams(candidate, parameters, typeSymbol);

        Parameters.AddRange(parameters);
    }

    private void InitAdditionalParams(
        MethodCandidate candidate,
        List<ParameterModel> parameters,
        INamedTypeSymbol? typeSymbol
    )
    {
        var additionalParamsAttribute = candidate
            .TypeDeclaration
            .GetAttribute(AdditionalParametersAttributeName);

        if (additionalParamsAttribute is null)
            return;

        var properties = typeSymbol?.GetProperties() ?? [];
        RequestProperties.AddRange(properties.Keys);

        if (additionalParamsAttribute.ArgumentList?.Arguments is not {} arguments)
            return;

        foreach (var argument in arguments)
        {
            if ((argument.Expression as LiteralExpressionSyntax)?.Token.ValueText is not {} name)
                continue;

            if (properties.TryGetValue(name, out ITypeSymbol property))
                parameters.Add(new ParameterModel(name, property.Name, CanPostInitiateCommand: true));
        }
    }

    private static string GetParameterName(string httpMethod) =>
        httpMethod == HttpMethods.Get ? "query" : "command";

    private static string AttributeFrom(From methodFrom) =>
        $"[From{methodFrom}]";

    private static string? GetResponseType(MethodCandidate candidate, string httpMethod)
    {
        var type = candidate
            .HttpMethodAttribute
            .GetTypeArgument(nameof(HttpMethodAttribute.ResponseType), candidate.SemanticModel)?
            .ToDisplayString();

        return
            !string.IsNullOrWhiteSpace(type) ? type
            : httpMethod is HttpMethods.Get ? GetResponseTypeForGetMethod(candidate)
            : Types.ActionResult;
    }

    private static string? GetResponseTypeForGetMethod(MethodCandidate candidate)
    {
        var typeName = string.Empty;

        if (
            candidate
                .TypeDeclaration
                .BaseList?
                .Types
                .FirstOrDefault(
                    type => type.Type is GenericNameSyntax
                    {
                        Identifier.ValueText: Types.Request,
                        TypeArgumentList.Arguments.Count: 1
                    }
                )?.Type is GenericNameSyntax request
        )
        {
            var responseType = request.TypeArgumentList.Arguments.First();

            typeName = responseType.Kind() is SyntaxKind.PredefinedType
                ? (responseType as PredefinedTypeSyntax)?.Keyword.ValueText
                : candidate.SemanticModel
                    .GetTypeInfo(request.TypeArgumentList.Arguments.First())
                    .Type?.ToDisplayString();
        }

        typeName = typeName?.Replace(Types.EnumerableGenericNamespaces, string.Empty);

        return typeName is Types.Unit
            ? Types.ActionResult
            : typeName;
    }
}

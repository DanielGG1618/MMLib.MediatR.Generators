﻿using System.Collections.Generic;
using AutoApiGen.Internal.Models;
using AutoApiGen.Internal.Static;

namespace AutoApiGen.Internal;

internal class ScribanFunctions : ScriptObject
{
    public static string MethodBody(string controllerName, MethodModel method, Templates templates)
        => SourceCodeGenerator
            .RenderBody(method, templates.GetMethodBodyTemplate(controllerName, method.HttpMethod, method.Name));

    public static string? GetParameter(IEnumerable<ParameterModel> parameters, string requestType)
    {
        parameters = parameters as ParameterModel[] ?? parameters.ToArray();
        return parameters.Any()
            ? GetRequestParameter(parameters, requestType)
            : $"new {requestType}()";
    }

    private static string? GetRequestParameter(IEnumerable<ParameterModel> parameters, string requestType)
        => parameters.FirstOrDefault(parameter =>
            parameter.Type?.Equals(requestType, StringComparison.CurrentCultureIgnoreCase) is true
        )?.Name;

    public static string PostInitiate(
        string request,
        IEnumerable<ParameterModel> parameters,
        List<string> requestProperties
    )
    {
        parameters = parameters as ParameterModel[] ?? parameters.ToArray();
        var additionalParameters = parameters/*.Where(p => p.CanPostInitiateCommand)*/.ToList();

        if (!additionalParameters.Any())
            return string.Empty;

        var stringBuilder = new StringBuilder();
        foreach (var parameter in additionalParameters)
        {
            var property = requestProperties.First(property =>
                property.Equals(parameter.Name, StringComparison.CurrentCultureIgnoreCase)
            );
            stringBuilder.AppendLine($"{GetRequestParameter(parameters, request)}.{property} = {parameter.Name};");
        }

        return stringBuilder.ToString();
    }
}
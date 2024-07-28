using System;
using System.Collections.Generic;
using AutoApiGen.Helpers;

namespace AutoApiGen.Controllers;

internal class Templates
{
    private readonly Dictionary<string, string> _templates = [];

    public void AddTemplate(TemplateType type, string controllerName, string template) => 
        _templates[GetName(type, controllerName)] = template;

    public void AddMethodBodyTemplate(string controllerName, string httpType, string methodName, string template) =>
        _templates[GetMethodBodyTemplateName(controllerName, httpType, methodName)] = template;

    public string? GetControllerTemplate(TemplateType type, string controllerName) =>
        _templates.TryGetValue(GetName(type, controllerName), out var template) ? template
        : _templates.TryGetValue(GetName(type, string.Empty), out template) ? template
        : type switch
        {
            TemplateType.Controller => EmbeddedResource.GetContent("Controllers.Templates.Controller.txt"),
            TemplateType.ControllerAttributes => EmbeddedResource.GetContent(
                "Controllers.Templates.ControllerAttributes.txt"
            ),
            TemplateType.ControllerUsings => EmbeddedResource.GetContent("Controllers.Templates.Usings.txt"),
            TemplateType.ControllerBody => EmbeddedResource.GetContent("Controllers.Templates.Method.txt"),
            TemplateType.MethodAttributes => null,
            TemplateType.MethodBody => null,
            _ => throw new ArgumentOutOfRangeException(nameof(type), $"Unexpected template type: {type}.")
        };

    public string GetMethodBodyTemplate(string controllerName, string httpType, string methodName) =>
        _templates.TryGetValue(
            GetMethodBodyTemplateName(controllerName, httpType, methodName),
            out var template
        ) ? template
        : _templates.TryGetValue(
            GetMethodBodyTemplateName(string.Empty, httpType, string.Empty),
            out template
        ) ? template
        : EmbeddedResource.GetContent($"Controllers.Templates.Http{httpType}MethodBody.txt");

    private static string GetName(TemplateType type, string controllerName) =>
        $"{type}-{controllerName}";

    private static string GetMethodBodyTemplateName(string controllerName, string httpType, string methodName) =>
        $"{controllerName}-{httpType}-{methodName}";
}
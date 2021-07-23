﻿using MMLib.MediatR.Generators.Helpers;
using System;
using System.Collections.Generic;

namespace MMLib.MediatR.Generators.Controllers
{
    internal class Templates
    {
        private Dictionary<string, string> _templates = new();

        public void AddTemplate(TemplateType type, string controllerName, string template)
        {
            _templates[GetName(type, controllerName)] = template;
        }

        public string GetTemplate(TemplateType type, string controllerName)
            => _templates.ContainsKey(GetName(type, controllerName))
                ? _templates[GetName(type, controllerName)]
                : _templates.ContainsKey(GetName(type, string.Empty))
                    ? _templates[GetName(type, string.Empty)]
                    : type switch
                    {
                        TemplateType.Controller => EmbeddedResource.GetContent("Controllers.Templates.Controller.txt"),
                        TemplateType.ControllerAttributes=> EmbeddedResource.GetContent("Controllers.Templates.Attributes.txt"),
                        TemplateType.ControllerUsings => EmbeddedResource.GetContent("Controllers.Templates.Usings.txt"),
                        TemplateType.ControllerBody => EmbeddedResource.GetContent("Controllers.Templates.Body.txt"),
                        _ => throw new ArgumentOutOfRangeException(nameof(type), $"Unexpected template type: {type}.")
                    };

        private static string GetName(TemplateType type, string controllerName)
            => $"{type}-{controllerName}";
    }
}
using System.Collections.Generic;
using System.Linq;
using AutoApiGen.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoApiGen.Controllers.Internal.Models;

internal partial record ControllerModel
{
    internal static ControllersModelBuilder Builder(GeneratorExecutionContext context)
        => new(context);

    private static ControllerModel Build(string name,
        IEnumerable<MethodCandidate> methods,
        Compilation compilation,
        Templates templates
    ) => new()
    {
        Name = name,
        Namespace = $"{compilation.AssemblyName}.Controllers",
        Methods = methods.Select(m => MethodModel.Build(m, templates, name))
    };
    
    internal class ControllersModelBuilder
    {
        private readonly GeneratorExecutionContext _context;
        private readonly Compilation _compilation;
        private readonly Dictionary<string, List<MethodCandidate>> _controllers = new(StringComparer.OrdinalIgnoreCase);

        public ControllersModelBuilder(GeneratorExecutionContext context)
        {
            _context = context;
            _compilation = context.Compilation;
        }

        public void AddCandidate(TypeDeclarationSyntax candidate)
        {
            var attribute = candidate.GetAttribute(HttpMethods.Attributes);
            var semanticModel = _compilation.GetSemanticModel(candidate.SyntaxTree);
            var controllerName = attribute
                .GetStringArgument(nameof(HttpMethodAttribute.Controller));

            if (string.IsNullOrWhiteSpace(controllerName))
            {
                _context.ReportMissingArgument(attribute, nameof(HttpMethodAttribute.Controller));
                return;
            }

            controllerName = controllerName.CheckControllerName();

            if (!_controllers.ContainsKey(controllerName))
                _controllers.Add(controllerName, []);

            var requestType = semanticModel.GetDeclaredSymbol(candidate);
            _controllers[controllerName].Add(
                new MethodCandidate(
                    attribute,
                    semanticModel,
                    candidate,
                    requestType.ToDisplayString()
                )
            );
        }

        public IEnumerable<ControllerModel> Build(Templates templates)
            => _controllers.Select(controller =>
                ControllerModel.Build(
                    controller.Key,
                    controller.Value,
                    _compilation,
                    templates
                )
            );
    }
}
using AutoApiGen.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoApiGen;

public class EndpointHandlerDeclarationSyntax
{
    private readonly ClassDeclarationSyntax _class;

    public static EndpointHandlerDeclarationSyntax Wrap(ClassDeclarationSyntax @class)
    {
        //TODO add validation logic if possible
        return new EndpointHandlerDeclarationSyntax(@class);
    }

    public string GetEndpointMethodName() =>
        _class.Parent switch
        {
            TypeDeclarationSyntax parentType => parentType.Name(),
            NamespaceDeclarationSyntax @namespace => $"{@namespace.Name.NameOrDefault()}_{_class.Name()}",
            _ => $"{_class.Name()}Method"
        };

    public string GetControllerName(Compilation compilation) =>
        (compilation
             .GetAttributesOf(_class)
             .First(attribute => attribute.AttributeClass?.BaseType?.Name is "EndpointAttribute")
             .ArgumentOfParameterWithName("Route")
             .Value?.ToString().WithCapitalFirstLetter()
         ?? "ThisShitReturnedNull") //TODO
        + "Controller"; 

    private EndpointHandlerDeclarationSyntax(ClassDeclarationSyntax @class) => 
        _class = @class;
}

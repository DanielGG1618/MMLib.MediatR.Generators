namespace AutoApiGen.Internal;

internal class EmbededResourceTemplatesProvider : ITemplatesProvider
{
    public Template Get(TemplateType type) =>
        Template.Parse(type switch
            {
                TemplateType.Controller => EmbeddedResource.GetContent("Templates.Controller.txt"),
                TemplateType.ControllerAttributes => EmbeddedResource.GetContent("Templates.ControllerAttributes.txt"),
                TemplateType.ControllerUsings => EmbeddedResource.GetContent("Templates.Usings.txt"),
                TemplateType.ControllerBody => EmbeddedResource.GetContent("Templates.Method.txt"),
                TemplateType.MethodAttributes => null,
                TemplateType.MethodBody => null,
                _ => throw new ArgumentOutOfRangeException(nameof(type), $"Unexpected template type: {type}.")
            }
        );

    public Template GetMethodBodyTemplate(string httpType) =>
        Template.Parse(
            EmbeddedResource.GetContent($"Templates.Http{httpType}MethodBody.txt")
        );
}
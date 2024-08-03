using Scriban;

namespace AutoApiGen.Internal;

internal class EmbeddedResourceTemplatesProvider : ITemplatesProvider
{
    public Template Get() =>
        Template.Parse(EmbeddedResource.GetContent("Templates.Controller.txt"));
}
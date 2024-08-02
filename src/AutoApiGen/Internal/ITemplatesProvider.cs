using Scriban;

namespace AutoApiGen.Internal;

internal interface ITemplatesProvider
{
    Template Get(TemplateType type);
    Template GetMethodBodyTemplate(string httpType);
}

using Scriban;

namespace AutoApiGen.Internal;

internal interface ITemplatesProvider
{
    Template Get();
}

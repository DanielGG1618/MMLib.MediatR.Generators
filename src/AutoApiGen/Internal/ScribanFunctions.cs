using System.Text;
using AutoApiGen.Internal.Models;
using AutoApiGen.Internal.Static;
using Scriban.Runtime;

namespace AutoApiGen.Internal;

internal class ScribanFunctions : ScriptObject
{
    public static string? GetParameter(IEnumerable<ParameterModel> parameters, string requestTypeName)
    {
        parameters = parameters as ParameterModel[] ?? parameters.ToArray();
        
        return parameters.Any()
            ? GetRequestParameter(parameters, requestTypeName)
            : $"new {requestTypeName}()";
    }

    private static string? GetRequestParameter(IEnumerable<ParameterModel> parameters, string requestType)
        => parameters.FirstOrDefault(parameter =>
            parameter.TypeName.Equals(requestType, StringComparison.CurrentCultureIgnoreCase)
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
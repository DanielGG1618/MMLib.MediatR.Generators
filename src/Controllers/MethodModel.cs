using System.Collections.Generic;

namespace AutoApiGen.Controllers
{
    internal partial record MethodModel
    {
        public string Name { get; init; }

        public string Template { get; init; }

        public string HttpMethod { get; init; }

        public string ResponseType { get; init; }

        public string Comment { get; init; }

        public string Attributes { get; set; }

        public List<ParameterModel> Parameters { get; init; } = new();

        public string RequestType { get; init; }

        public List<string> RequestProperties { get; init; } = new();
    }
}
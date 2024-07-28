using System.Collections.Generic;

namespace AutoApiGen.Controllers
{
    internal partial record ControllerModel
    {
        public string Namespace { get; init; }

        public string Name { get; init; }

        public IEnumerable<MethodModel> Methods { get; init; }
    }
}

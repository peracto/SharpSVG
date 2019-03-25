using Peracto.Svg.Accessor;

namespace Peracto.Svg.Parser
{
    class ElementAttribute : IElementAttribute
    {
        public ElementAttribute(string name, object convertedValue)
        {
            Name = name;
            Value = convertedValue;
        }
        public string Name { get;  }
        public object Value { get; }
    }
}
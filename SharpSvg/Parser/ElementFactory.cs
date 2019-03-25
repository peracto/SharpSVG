using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Peracto.Svg.Accessor;

namespace Peracto.Svg.Parser
{
    public class ElementFactory
    {
        private readonly IDictionary<string, IElementFactory> _dict;

        public ElementFactory()
        {
            var elements = (
                from property in typeof(ElementDefinitions).GetFields(BindingFlags.Static | BindingFlags.Public)
                where typeof(IElementFactory).IsAssignableFrom(property.FieldType)
                select (IElementFactory) property.GetValue(null)
            );

            _dict = elements.ToDictionary(
                (k) => k.ElementType.ToLower(),
                (v) => v
            );
        }

        public IElement Create(string name)
        {
            if (!_dict.TryGetValue(name.ToLower(), out var factory))
                factory = RegisterGeneric(name);
            return factory.Create();
        }

        private ElementDefinition RegisterGeneric(string name)
        {
            var factory = new ElementDefinition(name, ElementContentType.Element);
      _dict.Add(name.ToLower(), factory);
            return factory;
        }
    }
}
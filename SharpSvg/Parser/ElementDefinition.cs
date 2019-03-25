using System;
using Peracto.Svg.Accessor;

namespace Peracto.Svg.Parser
{
    public class ElementDefinition : Attribute, IElementDefinition, IElementFactory
    {
        private readonly ElementContentType _elementContentType;

        public ElementDefinition(string elementType, ElementContentType contentType = ElementContentType.None)
        {
            ElementType = elementType;
            _elementContentType = contentType;
        }

        public string ElementType { get; }

        ElementContentType IElementDefinition.ElementContentType => _elementContentType;

        IElement IElementFactory.Create()
        {
            return new Element(this);
        }
    }
}
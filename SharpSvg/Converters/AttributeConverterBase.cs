using Peracto.Svg.Accessor;
using Peracto.Svg.Parser;

namespace Peracto.Svg.Converters
{
  public abstract class AttributeConverterBase<TBase> : IElementAttributeFactory
  {
    protected AttributeConverterBase(string name)
    {
      AttributeName = name;
    }

    public string AttributeName { get; }

    IElementAttribute IElementAttributeFactory.Create(string attributeValue)
    {
      return TryCreate(attributeValue, out var value)
        ? new ElementAttribute(AttributeName, value)
        : null;
    }
    protected abstract bool TryCreate(string attributeValue, out TBase value);
  }
}
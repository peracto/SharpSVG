namespace Peracto.Svg.Converters
{
  public class NumberAttributeConverter : AttributeConverterBase<float>
  {
    public NumberAttributeConverter(string name) : base(name)
    {
    }

    protected override bool TryCreate(string attributeValue, IElement elementFactory, out float rc)
    {
      return float.TryParse(attributeValue, out rc);
    }
  }
}
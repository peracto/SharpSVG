using Peracto.Svg.Types;

namespace Peracto.Svg.Converters
{
  public class ViewBoxAttributeConverter : AttributeConverterBase<ViewBox>
  {
    public ViewBoxAttributeConverter(string name) : base(name)
    {
    }

    protected override bool TryCreate(string attributeValue, out ViewBox rc)
    {
      rc = ViewBox.Parse(attributeValue);
      return rc != null;
    }
  }
}
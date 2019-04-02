using Peracto.Svg.Brush;

namespace Peracto.Svg.Converters
{
  public class HrefAttributeConverter : AttributeConverterBase<HrefServer>
  {
    public HrefAttributeConverter(string name) : base(name)
    {
    }

    protected override bool TryCreate(string uriString, out HrefServer value)
    {
      try
      {
        value = new HrefServer(uriString);
        return true;
      }
      catch
      {
        value = null;
        return false;
      }
    }
  }
}
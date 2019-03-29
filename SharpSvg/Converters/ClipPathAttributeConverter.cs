using System;
using Peracto.Svg.Clipping;

namespace Peracto.Svg.Converters
{
  public class ClipPathAttributeConverter : AttributeConverterBase<IClipPathFactory>
  {
    public ClipPathAttributeConverter(string name) : base(name)
    {
    }

    protected override bool TryCreate(string value, out IClipPathFactory rc)
    {
      rc = Create(value.Trim());
      return rc != null;
    }

    public static bool TryCreateX(string value, out IClipPathFactory rc)
    {
      rc = Create(value.Trim());
      return rc != null;
    }


    private static IClipPathFactory Create(string original)
    {
      var value = original.ToLower();
      if (value.StartsWith("url(#"))
        return GetUrlColor(original);
      return null;
    }

    private static IClipPathFactory GetUrlColor(string value)
    {
      var leftParen = value.IndexOf(')', 5);
      return new ClipPathFactory(
        new Uri($"internal://{value.Substring(4, leftParen - 4)}", UriKind.Absolute)
      );
    }
  }
}
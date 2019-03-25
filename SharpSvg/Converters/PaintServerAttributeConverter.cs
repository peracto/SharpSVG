using System;
using Peracto.Svg.Brush;
using Peracto.Svg.Types;
using System.Text.RegularExpressions;

namespace Peracto.Svg.Converters
{


  public class PaintServerAttributeConverter : AttributeConverterBase<IBrushFactory>
  {
    private readonly bool _isServer;
    public PaintServerAttributeConverter(string name, bool isServer = true) : base(name)
    {
      _isServer = isServer;
    }

    protected override bool TryCreate(string value, IElement elementFactory, out IBrushFactory rc)
    {

      rc = Create(value.Trim());
      return rc != null;
    }

    private IBrushFactory Create(string original)
    {
      var value = original.ToLower();

      if (string.IsNullOrWhiteSpace(value))
        return null;
      if (value == "inherit")
        return null;
      if (_isServer && value.StartsWith("url(#"))
        return GetUrlColor(original);
      if (value == "currentcolor")
        return CurrentColorBrushFactory.Instance;
      if (value == "none")
        return NoneColorBrushFactory.Instance;
      if (TryParse(value, out var colour))
        return SolidColourBrushFactory.GetColor(colour);
      return null;
    }

    private IBrushFactory GetUrlColor(string value)
    {
      var leftParen = value.IndexOf(')', 5);
      return new UriBrushFactory(
        new Uri($"internal://{value.Substring(4, leftParen - 4)}", UriKind.Absolute),
        TryCreate(value.Substring(leftParen + 1).Trim(), null, out var fallback) ? fallback : null
      );
    }

    private static bool TryParse(string value, out PxColor colour)
    {
      if (!Regex.IsMatch(value, "^#[0-9A-Fa-f]+$"))
        return PxColorRegistry.TryGetColor(value, out colour);

      switch (value.Length)
      {
        case 4:
          colour = PxColor.FromRgb(Convert.ToInt32(string.Format("{0}{0}{1}{1}{2}{2}", value[1], value[2], value[3]), 16));
          return true;
        case 7:
          colour = PxColor.FromRgb(Convert.ToInt32(value.Substring(1), 16));
          return true;
        default:
          colour = PxColors.Black;
          return false;
      }
    }


  }
}
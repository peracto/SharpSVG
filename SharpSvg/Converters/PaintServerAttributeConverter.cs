using Peracto.Svg.Brush;
using Peracto.Svg.Types;
using Peracto.Svg.Utility;
using System;
using System.Linq;
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

    protected override bool TryCreate(string value, out IBrushFactory rc)
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
        return CurrentColorBrushFactory.Instance;
      if (_isServer && value.StartsWith("url(#"))
        return GetUrlColor(original);
      if (value.StartsWith("rgb("))
        return GetRgbColor(value);
      if (value == "currentcolor")
        return CurrentColorBrushFactory.Instance;
      if (value == "none")
        return NoneColorBrushFactory.Instance;
      if (TryParse(value, out var colour))
        return SolidColourBrushFactory.GetColor(colour);
      return null;
    }

    private IBrushFactory GetRgbColor(string value)
    {
      //rgb(204,0,102)
      var rc = ArgumentParser
        .Parse(value.Substring(value.IndexOf('(') + 1).Replace(')', ' '))
        .ToList();

      if (rc.Count != 3) return SolidColourBrushFactory.Black;

      PercentageAttributeConverter.TryParse(rc[0], out var r);
      PercentageAttributeConverter.TryParse(rc[1], out var g);
      PercentageAttributeConverter.TryParse(rc[2], out var b);

      return SolidColourBrushFactory.GetColor(
        new PxColor(
          r.Unit == PercentUnit.Percent ? r.Value / 100 : r.Value / 255f,
          g.Unit == PercentUnit.Percent ? g.Value / 100 : g.Value / 255f,
          b.Unit == PercentUnit.Percent ? b.Value / 100 : b.Value / 255f
        )
      );
    }

    private IBrushFactory GetUrlColor(string value)
    {
      var leftParen = value.IndexOf(')', 5);
      return new UriBrushFactory(
        new Uri($"internal://{value.Substring(4, leftParen - 4)}", UriKind.Absolute),
        TryCreate(value.Substring(leftParen + 1).Trim(), out var fallback) ? fallback : null
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
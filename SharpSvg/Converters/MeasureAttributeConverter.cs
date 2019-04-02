using Peracto.Svg.Types;
using System.Text.RegularExpressions;

namespace Peracto.Svg.Converters
{
  public class MeasureAttributeConverter : AttributeConverterBase<Measure>
  {
    private static readonly Regex MeasureRegex = new Regex(@"^\s*([+-]?(?=\.\d|\d)(?:\d+)?(?:\.?\d*)(?:[eE][+-]?\d+)?)([a-z%]+)?\s*$");

    public static MeasureAttributeConverter CreateH(string name)
    {
      return new MeasureAttributeConverter(name, MeasureUsage.Horizontal);
    }

    public static MeasureAttributeConverter CreateV(string name)
    {
      return new MeasureAttributeConverter(name, MeasureUsage.Vertical);
    }

    private readonly MeasureUsage _measureUsage;

    public MeasureAttributeConverter(string name, MeasureUsage renderingType) : base(name)
    {
      _measureUsage = renderingType;
    }

    protected override bool TryCreate(string attributeValue, out Measure rc)
    {
      return TryParse(attributeValue, _measureUsage, out rc);
    }

    public static bool TryParse(string value, MeasureUsage measureUsage, out Measure measure)
    {
      var unit = value.Trim().ToLower();

      if (string.IsNullOrWhiteSpace(unit))
      {
        measure = new Measure(0f, MeasureUnit.User, measureUsage);
        return true;
      }

      if (unit == "none")
      {
        measure = Measure.None;
        return true;
      }

      var match = MeasureRegex.Match(value);

    //  Console.WriteLine($"Matching {value} to Measure {match.Success}");

      if (!match.Success)
      {
        measure = Measure.None;
        return false;
      }

      measure = new Measure(
        float.Parse(match.Groups[1].Value),
        TranslateToUnitType(match.Groups[2].Value),
        measureUsage
      );

      return true;
    }

    private static MeasureUnit TranslateToUnitType(string type)
    {
      switch (type)
      {
        case "": return MeasureUnit.User;
        case "mm": return MeasureUnit.Millimeter;
        case "cm": return MeasureUnit.Centimeter;
        case "in": return MeasureUnit.Inch;
        case "px": return MeasureUnit.Pixel;
        case "pt": return MeasureUnit.Point;
        case "pc": return MeasureUnit.Pica;
        case "%": return MeasureUnit.Percentage;
        case "em": return MeasureUnit.Em;
        case "ex": return MeasureUnit.Ex;
        default: return MeasureUnit.Unknown;
      }
    }
  }
}
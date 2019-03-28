using System.Text.RegularExpressions;
using Peracto.Svg.Types;

namespace Peracto.Svg.Converters
{
  public class PercentageAttributeConverter : AttributeConverterBase<Percent>
  {
    private static readonly Regex PercentRegex = new Regex(@"^\s*([+-]?(?=\.\d|\d)(?:\d+)?(?:\.?\d*)(?:[eE][+-]?\d+)?)([%])?\s*$");

    public PercentageAttributeConverter(string name) : base(name)
    {
    }

    protected override bool TryCreate(string attributeValue, out Percent rc)
    {
      return TryParse(attributeValue, out rc);
    }

    public static bool TryParse(string value, out Percent percent)
    {
      var match = PercentRegex.Match(value);

      if (!match.Success)
      {
        percent = new Percent(1, PercentUnit.None);
        return false;
      }

      percent = new Percent(
        float.Parse(match.Groups[1].Value),
        match.Groups[2].Value == "%" ? PercentUnit.Percent : PercentUnit.None
      );
      return true;
    }

  }
}
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Peracto.Svg.Converters
{
  public class NumberListAttributeConverter : AttributeConverterBase<float[]>
  {
    public NumberListAttributeConverter(string name) : base(name)
    {
    }

    protected override bool TryCreate(string value,  out float[] rc)
    {
      if (value == "none")
      {
        rc = new float[] { };
        return true;
      }
      rc = Regex.Replace(value.Trim(), @"[\s,]+", ",")
        .Split(',')
        .Select(s => float.Parse(s.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture))
        .ToArray();
      return true;
    }
  }
}
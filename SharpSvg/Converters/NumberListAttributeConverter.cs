using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Peracto.Svg.Types;

namespace Peracto.Svg.Converters
{
  public class MeasureListAttributeConverter : AttributeConverterBase<Measure[]>
  {
    public MeasureListAttributeConverter(string name) : base(name)
    {
    }

    protected override bool TryCreate(string value, out Measure[] rc)
    {
      if (value == "none")
      {
        rc = new Measure[] { };
        return true;
      }
      rc = Regex.Replace(value.Trim(), @"[\s,]+", ",")
        .Split(',')
        .Select(s => MeasureAttributeConverter.TryParse(s.Trim(),MeasureUsage.Horizontal,out var m)?m:Measure.ZeroH)
        .ToArray();
      return true;
    }
  }


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
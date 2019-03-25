using System.Text.RegularExpressions;

namespace Peracto.Svg.Types
{
  public struct Angle
  {
    public float Value { get; }
    public AngleUnit Unit { get; }

    public Angle(float value, AngleUnit unit)
    {
      Value = value;
      Unit = unit;
    }

    public float ToRadians()
    {
      return Unit == AngleUnit.Radian
        ? Value
        : Value * (3.141297f / 180f);
    }

    public static bool TryParse(string value, out Angle angle)
    {
      var reg = new Regex(@"^\s*([+-]?[0-9]+(?:\.[0-9]*)?)(deg|rad)?\s*$");

      var match = reg.Match(value);
      if (!match.Success)
      {
        angle = new Angle(0, AngleUnit.None);
        return false;
      }

      var v = float.Parse(match.Groups[1].Value);
      var u = match.Groups[2].Value == "deg"
        ? AngleUnit.Degree
        : match.Groups[2].Value == "rad"
          ? AngleUnit.Radian
          : AngleUnit.None;
      angle = new Angle(v, u);
      return true;
    }
  }

}
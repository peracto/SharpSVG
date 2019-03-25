using System;

namespace Peracto.Svg.Types
{
  public struct Percent
  {
    public float Value { get; }
    public PercentUnit Unit { get; }
    public static readonly Percent Hundred = new Percent(100, PercentUnit.Percent);
    public static readonly Percent Zero = new Percent(0, PercentUnit.Percent);

    public Percent(float value, PercentUnit unit)
    {
      Value = value;
      Unit = unit;
    }

    public float AsCheckedNumber()
    {
      return Unit == PercentUnit.Percent
        ? Math.Min(Math.Max(Value / 100, 0), 1)
        : Math.Min(Math.Max(Value, 0), 1);
    }
  }
}
using System;

namespace Peracto.Svg
{
  public static class MathEx
  {
    public static unsafe bool NearEqual(float a, float b)
    {
      if (IsZero(a - b)) return true;
      var num1 = *(int*)&a;
      var num2 = *(int*)&b;
      return num1 < 0 == num2 < 0 && Math.Abs(num1 - num2) <= 4;
    }

    public static bool IsZero(float a)
    {
      return Math.Abs(a) < 9.99999997475243E-07;
    }

    public static bool IsOne(float a)
    {
      return Math.Abs(a - 1f) < 9.99999997475243E-07;
    }
  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Peracto.Svg.Barcode
{
  public static class Barcode
  {
    public static IEnumerable<int>[] EanLeft = "3211,2221,2122,1411,1132,1231,1114,1312,1213,3112"
        .Split(',')
        .Select(Encode)
        .ToArray();

    public static IEnumerable<int> Sentinel = Encode("111");
    public static IEnumerable<int> Guard = Encode("11111");

    public static long[] Powers = Enumerable.Range(0, 10).Select((v) => (long)Math.Pow(10, v)).ToArray();

    private static IEnumerable<int> Encode(string s)
    {
      return s.Select((c) => c - '0').ToList();
    }

    public static string CreateSvg(int value)
    {
      var x = (
          from c in Ean8(value)
          from b in c
          select b
      );

      var q = x.Aggregate(new StringBuilder(), (a, b) =>
      {
        a.Append((char)(b + '0'));
        a.Append(' ');
        return a;
      });
      return q.ToString();
    }


    public static int CheckDigit(long value)
    {
      var acc = 0L;
      var b = 3L;
      while (value > 0)
      {
        acc += (value % 10) * b;
        value /= 10;
        b ^= 2; //XOR - flip 1 and 3
      }

      var r = (int)(acc % 10);
      return r == 0 ? 0 : (10 - r);
    }

    public static IEnumerable<IEnumerable<int>> Ean8(long value)
    {
      yield return Sentinel;
      for (var i = 6; i >= 0; i--)
      {
        if (i == 3) yield return Guard;
        yield return EanLeft[(int)((value / Powers[i]) % 10)];
      }
      yield return EanLeft[CheckDigit(value)];
      yield return Sentinel;
    }
  }
}


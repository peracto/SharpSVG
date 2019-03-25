using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Peracto.Svg.Barcode
{
  public static class Barcode2Of5
  {
    public static IEnumerable<int>[] EanLeft = "3111111131,1131111131,3131111111,1111311131,3111311111,1131311111,1111113131,1111313111,3111113111,1131113111"
      .Split(',')
      .Select(Encode)
      .ToArray();

    public static IEnumerable<int> SentinelStart = Encode("212111");
    public static IEnumerable<int> SentinelEnd = Encode("21112");

    public static long[] Powers = Enumerable.Range(0, 18).Select((v) => (long)Math.Pow(10, v)).ToArray();

    private static IEnumerable<int> Encode(string s)
    {
      return s.Select((c) => c - '0').ToList();
    }

    public static string CreateSvg(int value)
    {
      var x = (
        from c in Encode(value)
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

    public static IEnumerable<IEnumerable<int>> Encode(long value)
    {
      yield return SentinelStart;
      var y = false;
      for (var i = 16; i >= 0; i--)
      {
        var val = (int)((value / Powers[i]) % 10);
        if (!y) y = val > 0;
        if (y) yield return EanLeft[val];
      }

      yield return SentinelEnd;
    }
  }
}
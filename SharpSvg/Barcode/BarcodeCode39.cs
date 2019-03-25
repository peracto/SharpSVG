using System;
using System.Collections.Generic;
using System.Linq;

namespace Peracto.Svg.Barcode
{
  public static class BarcodeCode39
  {
    /*
            private static imp = [
                //          1010101010

            "101001101101","111221211",
            "110100101011","211211112",
            "101100101011","112211112",
            "110110010101","212211111",
            "101001101011","111221112"
            "110100110101","211221111"
            "101100110101","112221111"
            "101001011011","111211212"
            "110100101101","211211211"
            "101100101101","112211211"
            ];

                "101000111011101" - "
    */
    //                                          101010101
    public static IEnumerable<int>[] EanLeft = "1112212111,2112111121,1122111121,2122111111,1112211121,2112211111,1122211111,1112112121,2112112111,1122112111"
      .Split(',')
      .Select(Encode)
      .ToArray();
    //                                                     1010101010
    public static IEnumerable<int> SentinelStart = Encode("1211212111");
    public static IEnumerable<int> SentinelEnd = Encode("121121211");

    public static long[] Powers = Enumerable.Range(0, 18).Select((v) => (long)Math.Pow(10, v)).ToArray();

    private static IEnumerable<int> Encode(string s)
    {
      return s.Select((c) => c - '0').ToList();
    }

    public static IEnumerable<int> Encode(long value)
    {
      return (
        from c in InternalEncode(value)
        from b in c
        select b
      );
    }

    private static IEnumerable<IEnumerable<int>> InternalEncode(long value)
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
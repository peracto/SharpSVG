using System.Collections.Generic;
using Peracto.Svg.Types;

namespace Peracto.Svg.Converters
{
  public static class LocalHelpers
  {
    public static PxPoint ToPoint(this IList<float> list, int index)
    {
      return new PxPoint(list[index], list[index + 1]);
    }
    public static PxPoint ToPoint(this IList<float> list, int index, PxPoint cursor)
    {
      return new PxPoint(list[index], list[index + 1]).Add(cursor);
    }
  }
}
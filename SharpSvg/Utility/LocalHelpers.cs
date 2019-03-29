using System.Collections.Generic;
using Peracto.Svg.Types;

namespace Peracto.Svg.Utility
{
  public static class LocalHelpers
  {
    public static PxPoint ToPoint(this IList<float> list, int index)
    {
      return new PxPoint(list[index], list[index + 1]);
    }
    public static PxPoint ToPoint(this IList<float> list, int index, PxPoint cursor)
    {
      return new PxPoint(list[index]+cursor.X, list[index + 1]+cursor.Y);
    }
  }
}
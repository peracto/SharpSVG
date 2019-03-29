using System;
using System.Diagnostics;

namespace Peracto.Svg.Types
{
  [DebuggerDisplay("Point({X},{Y})")]
  public struct PxPoint
  {
    public static readonly PxPoint Zero = new PxPoint(0, 0);
    public PxPoint(float x, float y)
    {
      X = x;
      Y = y;
    }

    public float X { get; }
    public float Y { get; }

    public PxPoint Add(PxPoint pt)
    {
      return new PxPoint(X+pt.X,Y+pt.Y);
    }

    public PxPoint Reflect(PxPoint mirror)
    {
      var dx = Math.Abs(mirror.X - X);
      var dy = Math.Abs(mirror.Y - Y);

      return new PxPoint(
        mirror.X + (mirror.X >= X? dx:-dx),
        mirror.Y + (mirror.Y >= Y? dy:-dy)
      );

    }
  }
}
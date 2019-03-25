using System;
using Peracto.Svg.Types;

namespace Peracto.Svg.Paths
{
  public sealed class ArcPathCommand : IPathCommand
  {
    public PxPoint Point0 { get; }
    public PxPoint Point1 { get; }

    public float AxisRotation { get; }
    public bool LargeArcFlag { get; }
    public bool SweepFlag { get; }
    public float RadiusX { get; }
    public float RadiusY { get; }

    public ArcPathCommand(
      PxPoint origin,
      float radiusX,
      float radiusY,
      float axisRotation,
      bool largeArcFlag,
      bool sweepFlag,
      PxPoint point1)
    {
      Point0 = origin;
      RadiusX = Math.Abs(radiusX);
      RadiusY = Math.Abs(radiusY);
      AxisRotation = axisRotation;
      LargeArcFlag = largeArcFlag;
      SweepFlag = sweepFlag;
      Point1 = point1;
    }

    public PxPoint NextPoint => Point1;
    public PxPoint ShortPoint => Point1;

    void IPathCommand.Visit<T>(T state, IPathVisitor<T> visitor)
    {
      visitor.CreateArcSegment(state, Point0, Point1, LargeArcFlag, AxisRotation,RadiusX,RadiusY,SweepFlag);
    }
  }
}

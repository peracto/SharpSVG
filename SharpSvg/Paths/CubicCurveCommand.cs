using Peracto.Svg.Types;

namespace Peracto.Svg.Paths
{
  public sealed class CubicCurveCommand : IPathCommand
  {
    public PxPoint Point0 { get; }
    public PxPoint Point1 { get; }
    public PxPoint Point2 { get; }
    public PxPoint Point3 { get; }

    public CubicCurveCommand(PxPoint origin, PxPoint point1, PxPoint point2, PxPoint point3)
    {
      Point0 = origin;
      Point1 = point1;
      Point2 = point2;
      Point3 = point3;
    }

    public PxPoint NextPoint => Point3;
    public PxPoint ShortPoint => Point1.Reflect(Point0);

    void IPathCommand.Visit<T>(T state, IPathVisitor<T> visitor)
    {
      visitor.CreateBezierSegment(state, Point0, Point1, Point2, Point3);
    }
  }
}

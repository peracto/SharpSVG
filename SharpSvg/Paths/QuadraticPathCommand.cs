using Peracto.Svg.Types;

namespace Peracto.Svg.Paths
{
  public sealed class QuadraticPathCommand : IPathCommand
  {
    public PxPoint Point0 { get; }
    public PxPoint Point1 { get; }
    public PxPoint Point2 { get; }

    public QuadraticPathCommand(PxPoint origin, PxPoint point1, PxPoint point2)
    {
      Point0 = origin;
      Point1 = point1;
      Point2 = point2;
    }

    public PxPoint NextPoint => Point2;
    public PxPoint ShortPoint => Point1.Reflect(Point0);

    void IPathCommand.Visit<T>(T state, IPathVisitor<T> visitor)
    {
      visitor.CreateQuadSegment(state, Point0,Point1, Point2);
    }
  }
}
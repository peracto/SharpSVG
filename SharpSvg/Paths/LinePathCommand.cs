using Peracto.Svg.Types;

namespace Peracto.Svg.Paths
{
  public sealed class LinePathCommand : IPathCommand
  {
    public readonly PxPoint Point0;
    public readonly PxPoint Point1;

    public LinePathCommand(PxPoint origin, PxPoint point1)
    {
      Point0 = origin;
      Point1 = point1;
    }

    public PxPoint NextPoint => Point1;
    public PxPoint ShortPoint => Point1;

    void IPathCommand.Visit<T>(T state, IPathVisitor<T> visitor)
    {
      visitor.CreateLineSegment(state, Point0, Point1);
    }
  }
}
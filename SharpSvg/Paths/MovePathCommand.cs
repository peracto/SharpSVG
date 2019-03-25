using Peracto.Svg.Types;

namespace Peracto.Svg.Paths
{
  public class MovePathCommand : IPathCommand
  {
    public readonly PxPoint Point0;
    public readonly PxPoint Point1;

    public MovePathCommand(PxPoint origin, PxPoint point1)
    {
      Point0 = origin;
      Point1 = point1;
    }

    public PxPoint NextPoint => Point1;
    public PxPoint ShortPoint => Point1;

    void IPathCommand.Visit<T>(T state, IPathVisitor<T> visitor)
    {
      visitor.BeginPath(state, Point1, true);
    }
  }
}
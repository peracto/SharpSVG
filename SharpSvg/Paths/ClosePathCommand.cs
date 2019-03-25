using Peracto.Svg.Types;

namespace Peracto.Svg.Paths
{
  public sealed class ClosePathCommand : IPathCommand
  {
    public PxPoint Point0 { get; }
    public PxPoint Point1 { get; }
    public bool ClosePath { get; }

    public PxPoint NextPoint => Point1;
    public PxPoint ShortPoint => Point1;

    void IPathCommand.Visit<T>(T state, IPathVisitor<T> visitor)
    {
      visitor.ClosePath(state,Point0, ClosePath);
    }

    public ClosePathCommand(PxPoint origin, PxPoint point1, bool closePath)
    {
      Point0 = origin;
      Point1 = point1;
      ClosePath = closePath;
    }
  }
}
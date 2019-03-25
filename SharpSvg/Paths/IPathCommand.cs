using Peracto.Svg.Types;

namespace Peracto.Svg.Paths
{
 public interface IPathCommand
  {
    PxPoint NextPoint { get; }

    PxPoint ShortPoint { get; }

    void Visit<T>(T state, IPathVisitor<T> visitor) where T : class;
  }
}

using System.Collections.Generic;
using System.Linq;
using Peracto.Svg.Types;

namespace Peracto.Svg.Paths
{
  public class PathSegment
  {
    public PxRectangle Bounds { get; }
    public IPathCommand[] Commands { get; }
    public bool IsClosed { get; }

    public PathSegment(IEnumerable<IPathCommand> commands, PxRectangle bounds, bool isClosed)
    {
      Bounds = bounds;
      Commands = commands.ToArray();
      IsClosed = isClosed;
    }
  }
}

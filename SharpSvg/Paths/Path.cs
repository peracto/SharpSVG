using Peracto.Svg.Types;

namespace Peracto.Svg.Paths
{
  public class Path
  {
    public PathSegment[] Segments { get; }
    public PxRectangle Bounds { get; }

    public Path(PathSegment[] segments, PxRectangle bounds)
    {
      Segments = segments;
      Bounds = bounds;
    }
  }
}

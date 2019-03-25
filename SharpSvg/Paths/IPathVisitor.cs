using Peracto.Svg.Types;

namespace Peracto.Svg.Paths
{
  public interface IPathVisitor<in T> where T : class
  {
    void CreateArcSegment(T state, PxPoint point0, PxPoint point1, bool isLargeArc, float axisRotation, float radiusX, float radiusY, bool sweepClockwise);
    void CreateBezierSegment(T state, PxPoint point0, PxPoint point1, PxPoint point2, PxPoint point3);
    void CreateLineSegment(T state, PxPoint point0, PxPoint point1);
    void BeginPath(T state, PxPoint point0, bool bFilled);
    void CreateQuadSegment(T state, PxPoint point0, PxPoint point1, PxPoint point2);
    void ClosePath(T state, PxPoint point0, bool isClosed);
  }
}

using System;
using Peracto.Svg.Paths;
using Peracto.Svg.Render.Dx.Utility;
using Peracto.Svg.Types;
using DX = SharpDX;
using D2D1 = SharpDX.Direct2D1;

// ReSharper disable PossibleNullReferenceException

namespace Peracto.Svg.Render.Dx.Path
{

  public static class PathBuilder
  {
    private static readonly IPathVisitor<D2D1.GeometrySink> Visitor = new PathVisitor();

    public static D2D1.Geometry Create(D2D1.RenderTarget target, Paths.Path path,
      D2D1.FillMode fillMode = D2D1.FillMode.Winding)
    {
      var segments = path.Segments;
      var length = segments.Length;
      var geometries = length > 1 ? new D2D1.Geometry[segments.Length] : null;
      var i = 0;
      foreach (var segment in path.Segments)
      {
        var geometry = new D2D1.PathGeometry(target.Factory);
        using (var sink = geometry.Open())
        {
          sink.SetSegmentFlags(D2D1.PathSegment.None);
          foreach (var x in segment.Commands)
            x.Visit(sink, Visitor);
          sink.Close();
        }

        if (length == 1) return geometry;
        geometries[i++] = geometry;
      }

      try
      {
        return new D2D1.GeometryGroup(target.Factory, fillMode, geometries);
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    private class PathVisitor : IPathVisitor<D2D1.GeometrySink>
    {
      public void CreateArcSegment(D2D1.GeometrySink state, PxPoint point0, PxPoint point1, bool isLargeArc, float axisRotation, float radiusX, float radiusY, bool sweepClockwise)
      {
        state.AddArc(new D2D1.ArcSegment()
        {
          ArcSize = isLargeArc ? D2D1.ArcSize.Large : D2D1.ArcSize.Small,
          Point = point1.ToDx(),
          RotationAngle = axisRotation,
          Size = new DX.Size2F(radiusX, radiusY),
          SweepDirection = sweepClockwise
            ? D2D1.SweepDirection.Clockwise
            : D2D1.SweepDirection.CounterClockwise
        });
      }

      public void CreateBezierSegment(D2D1.GeometrySink state, PxPoint point0, PxPoint point1, PxPoint point2, PxPoint point3)
      {
        state.AddBezier(new D2D1.BezierSegment()
        {
          Point1 = point1.ToDx(),
          Point2 = point2.ToDx(),
          Point3 = point3.ToDx()
        });
      }

      public void CreateLineSegment(D2D1.GeometrySink state, PxPoint point0, PxPoint point1)
      {
        state.AddLine(point1.ToDx());
      }

      public void BeginPath(D2D1.GeometrySink state, PxPoint point0, bool bFilled)
      {
        state.BeginFigure(
          point0.ToDx(),
          D2D1.FigureBegin.Filled
        );
      }

      public void CreateQuadSegment(D2D1.GeometrySink state, PxPoint point0, PxPoint point1, PxPoint point2)
      {
        state.AddQuadraticBezier(new D2D1.QuadraticBezierSegment()
        {
          Point1 = point1.ToDx(),
          Point2 = point2.ToDx()
        });
      }

      public void ClosePath(D2D1.GeometrySink state, PxPoint point0, bool isClosed)
      {
        state.EndFigure(isClosed ? D2D1.FigureEnd.Closed : D2D1.FigureEnd.Open);
      }
    }
  }
}

using Peracto.Svg.Clipping;
using Peracto.Svg.Render.Dx.Utility;
using Peracto.Svg.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using D2D1 = SharpDX.Direct2D1;

// ReSharper disable PossibleNullReferenceException

namespace Peracto.Svg.Render.Dx.Path
{
  public class ClipPathBuilder
  {
    public static D2D1.Geometry Create(D2D1.RenderTarget target, IFrameContext context, IClip clipPath)
    {
      if (clipPath == null) return null;

      IList<D2D1.Geometry> list = (
        from element in clipPath.Children
        let geom = CreateGeometry(target, element, context)
        where geom != null
        select geom
      ).ToArray();

      switch (list.Count)
      {
        case 0:
          return null;
        case 1:
          return list[0];
        default:
          return new D2D1.GeometryGroup(target.Factory, D2D1.FillMode.Winding, list.ToArray());
      }
    }

    private static D2D1.Geometry CreateGeometry(D2D1.RenderTarget target, IElement element, IFrameContext context)
    {
      switch(element.ElementType)
      {
        case "rect":
          return new D2D1.RectangleGeometry(target.Factory, element.GetBounds(context).ToDx());
        case "circle":
          var r = element.GetRadius(context);
          return new D2D1.EllipseGeometry(target.Factory, new D2D1.Ellipse()
          {
            Point = element.GetCxCy(context).ToDx(),
            RadiusX = r,
            RadiusY = r
          });
        default:
          return null;
      }
    }

    public static PxRectangle GetBounds(IElement element, IFrameContext context)
    {
      switch (element.ElementType)
      {
        case "rect":
          return element.GetBounds(context);
        case "circle":
          var pt = element.GetCxCy(context);
          var r = element.GetRadius(context);
          return new PxRectangle(pt.X - r, pt.Y - r, r + r, r + r);
        case "ellipse":
          var r1 = element.GetRxRy(context);
          var pt1 = element.GetCxCy(context);
          return new PxRectangle(pt1.X - r1.X, pt1.Y - r1.Y, r1.X * 2, r1.Y * 2);
        case "line":
          var p1 = element.GetX1Y1(context);
          var p2 = element.GetX2Y2(context);
          var x1 = Math.Min(p1.X, p2.X);
          var y1 = Math.Min(p1.Y, p2.Y);
          var x2 = Math.Max(p1.X, p2.X);
          var y2 = Math.Max(p1.Y, p2.Y);
          return new PxRectangle(
            x1, 
            y1, 
            x2 - x1, 
            y2 - y1
            );
        case "path":
          var p = element.GetPath();
          return p.Bounds;
        case "polygon":
        case "polyline":
        case "text":
        default:
          return new PxRectangle(0, 0, context.Size.Width, context.Size.Height);
      }
    }


  }
}

using Peracto.Svg.Brush;
using Peracto.Svg.Clipping;
using Peracto.Svg.Render.Dx.Font;
using Peracto.Svg.Render.Dx.Utility;
using Peracto.Svg.Types;
using SharpDX;
using SharpDX.DirectWrite;
using SharpDX.Mathematics.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using D2D1 = SharpDX.Direct2D1;

// ReSharper disable PossibleNullReferenceException

namespace Peracto.Svg.Render.Dx.Path
{

  public static class FontHelpers
  {
    public static short[] GetGlyphIndices(this FontFace @this, string input)
    {
      var codePoints = new List<int>();
      for (var i = 0; i < input.Length; i += char.IsSurrogatePair(input, i) ? 2 : 1)
        codePoints.Add(char.ConvertToUtf32(input, i));
      var cpi = codePoints.ToArray();
      return @this.GetGlyphIndices(cpi);
    }
  }
  public class ClipPathBuilder
  {
    public static D2D1.Geometry Create(D2D1.RenderTarget target, FontManager fontManager, IFrameContext context, IClip clipPath, IElement targetElement)
    {
      if (clipPath == null) return null;

      IList<D2D1.Geometry> list = (
        from element in clipPath.Children
        let geom = CreateGeometry(target, fontManager, element, context, clipPath.ClipPathUnits, targetElement)
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


    private static D2D1.Geometry CreateGeometry(D2D1.RenderTarget target, FontManager fontManager, IElement element, IFrameContext context, ClipPathUnits clipPathUnits, IElement targetElement)
    {
      if (clipPathUnits == ClipPathUnits.UserSpaceOnUse)
      {
        switch (element.ElementType)
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
          case "path":
            return PathBuilder.Create(target, element.GetPath(), D2D1.FillMode.Alternate);
          case "text":
            var geom = new D2D1.PathGeometry(target.Factory);
            using (var sink = geom.Open())
            {
              var font = element.GetFont(context);
              var fontFace = fontManager.GetFontFace(font);
              var glyphIndices = fontFace.GetGlyphIndices("Clip Test");
              var xx = new[] {35f, 50f, 55f, 65f, 45f, 48f, 52f, 32f, 61f};
              fontFace.GetGlyphRunOutline(
                font.Size,
                glyphIndices,
                xx,
                null,
                glyphIndices.Length,
                false,
                false,
                sink);

              sink.Close();

              return new D2D1.TransformedGeometry(
                target.Factory,
                geom,
                Matrix3x2.Translation(0,font.Size)
                );

              //return geom;
            }

          default:
            return null;
        }
      }
      else
      {
        var targetBounds = GetBounds(targetElement, context);

        switch (element.ElementType)
        {
          case "rect":
            var rectBounds = element.GetBounds(context);
            var w = targetBounds.Width * rectBounds.Width;
            var h = targetBounds.Height * rectBounds.Height;
            var x = targetBounds.X + (targetBounds.Width * rectBounds.X);
            var y = targetBounds.Y + (targetBounds.Height * rectBounds.Y);
            return new D2D1.RectangleGeometry(
              target.Factory, 
              new RawRectangleF(x,y,x+w,y+h)
              );
          case "circle":
            var r = element.GetRadius(context);
            var cxCy = element.GetCxCy(context);
            return new D2D1.EllipseGeometry(target.Factory, new D2D1.Ellipse()
            {
              Point = new RawVector2(targetBounds.X+(cxCy.X*targetBounds.Width),targetBounds.Y+(cxCy.Y*targetBounds.Height)),
              RadiusX = r*targetBounds.Width,
              RadiusY = r*targetBounds.Width
            });
          case "path":
            return PathBuilder.Create(target, element.GetPath(), D2D1.FillMode.Alternate);
          default:
            return null;
        }
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

using Peracto.Svg.Brush;
using Peracto.Svg.Transform;
using Peracto.Svg.Types;
using System;
using D2D1 = SharpDX.Direct2D1;
using DX = SharpDX;
using DXM = SharpDX.Mathematics.Interop;

namespace Peracto.Svg.Render.Dx.Utility
{
  public static class RenderHelpers
  {
    
    public static D2D1.CapStyle ToDx(this LineCap lineCap)
    {
      switch (lineCap)
      {
        case LineCap.Butt:
          return D2D1.CapStyle.Flat;
        case LineCap.Round:
          return D2D1.CapStyle.Flat;
  //        return D2D1.CapStyle.Round;
        case LineCap.Square:
          return D2D1.CapStyle.Flat;
//          return D2D1.CapStyle.Square;
        default:
          throw new ArgumentOutOfRangeException(nameof(lineCap), lineCap, null);
      }
    }

    public static D2D1.LineJoin ToDx(this LineJoin lineJoin)
    {
      switch (lineJoin)
      {
        case LineJoin.Arcs:
          return D2D1.LineJoin.Round;
        case LineJoin.Bevel:
          return D2D1.LineJoin.Bevel;
        case LineJoin.Miter:
          return D2D1.LineJoin.Miter;
        case LineJoin.MiterClip:
          return D2D1.LineJoin.MiterOrBevel;
        case LineJoin.Round:
          return D2D1.LineJoin.Round;
        default:
          throw new ArgumentOutOfRangeException(nameof(lineJoin), lineJoin, null);
      }
    }

    public static DXM.RawColor4 ToDx(this PxColor color, float opacity)
    {
      return new DXM.RawColor4(color.R, color.G, color.B, color.A * opacity);
    }

    public static DX.Color4 ToDx4(this PxColor color)
    {
      return new DX.Color4(color.R, color.G, color.B, color.A);
    }


    public static DXM.RawRectangleF ToDx(this PxRectangle rect)
    {
      return new DXM.RawRectangleF(
        rect.X,
        rect.Y,
        rect.X + rect.Width,
        rect.Y + rect.Height
      );
    }

    public static PxSize FromDx(this DX.Size2F sz)
    {
      return new PxSize(sz.Width, sz.Height);
    }

    public static DX.Vector2 ToDx(this PxPoint sz)
    {
      return new DX.Vector2(sz.X, sz.Y);
    }
  
    public static DX.Matrix3x2 ToMatrix3x2(this PxMatrix m)
    {
      return new DX.Matrix3x2(m.M11, m.M12, m.M21, m.M22, m.M31, m.M32);
    }


    public static DXM.RawMatrix3x2 ToDx(this ITransform transform)
    {
      if (transform != null)
      {
        var m = transform.Matrix;
        return new DXM.RawMatrix3x2(m.M11, m.M12, m.M21, m.M22, m.M31, m.M32);
      }
      else
      {
        return new DXM.RawMatrix3x2(1, 0, 0, 1, 0, 0); //Identity
      }
    }
  }
}
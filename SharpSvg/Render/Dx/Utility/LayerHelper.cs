using Peracto.Svg.Render.Dx.Render;
using Peracto.Svg.Types;
using System;
using D2D1 = SharpDX.Direct2D1;
using DXM = SharpDX.Mathematics.Interop;

namespace Peracto.Svg.Render.Dx.Utility
{
  public static class LayerHelper 
  {
    private static DXM.RawRectangleF _emptyClipBounds = new DXM.RawRectangleF(0f, 0f, 999999, 999999);
    public static IDisposable Create(D2D1.RenderTarget target, PxSize size, float opacity)
    {
      var bounds = new DXM.RawRectangleF(0, 0, size.Width, size.Height);
      return opacity < 1f
        ? new ComplexLayer(target, ref bounds, null, opacity)
        : new SimpleLayer(target, ref bounds) as IDisposable;
    }

    public static IDisposable Create(RendererDirect2D render, IElement element, IFrameContext context)
    {
      var opacity = element.GetOpacity();
      var path = element.GetClipPath();
      if (opacity >= 1f && path == null) return null;

      return Create(
        render.Target,
        render.GetClipGeometry(element, context, path),
        opacity,
        false,
        ref _emptyClipBounds
      );
    }

    public static IDisposable Create(RendererDirect2D render, IElement element, IFrameContext context, PxSize clipSize)
    {
      var clip = new DXM.RawRectangleF(0f, 0f, clipSize.Width, clipSize.Height);
      return Create(
        render.Target,
        render.GetClipGeometry(element, context, element.GetClipPath()),
        element.GetOpacity(),
        true,
        ref clip
      );
    }

    private static IDisposable Create(D2D1.RenderTarget target, D2D1.Geometry geometryPath, float opacity,bool clipElement, ref DXM.RawRectangleF clipBounds)
    {
      var complexClip = geometryPath != null || opacity < 1.0f;
      var simpleClip = !complexClip && clipElement;

      return complexClip
        ? new ComplexLayer(target, ref clipBounds, geometryPath, opacity)
        : simpleClip
          ? new SimpleLayer(target, ref clipBounds)
          : (IDisposable) null;
    }


    private class SimpleLayer : IDisposable
    {
      private readonly D2D1.RenderTarget _target;

      internal SimpleLayer(D2D1.RenderTarget target, ref DXM.RawRectangleF clipBounds)
      {
        _target = target;
        target.PushAxisAlignedClip(clipBounds, D2D1.AntialiasMode.PerPrimitive);
      }

      public void Dispose()
      {
        _target.PopAxisAlignedClip();
      }
    }

    private class ComplexLayer : IDisposable
    {
      private readonly D2D1.RenderTarget _target;
      private readonly D2D1.Geometry _geometry;

      internal ComplexLayer(D2D1.RenderTarget target, ref DXM.RawRectangleF clipBounds, D2D1.Geometry geometryPath,
        float opacity)
      {
        _target = target;
        _geometry = geometryPath;
        target.PushLayer(clipBounds, geometryPath, opacity);
      }

      public void Dispose()
      {
        _geometry?.Dispose();
        _target.PopLayer();
      }
    }
  }
}
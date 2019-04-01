using Peracto.Svg.Render.Dx.Path;
using Peracto.Svg.Types;
using System;
using System.Diagnostics;
using Peracto.Svg.Render.Dx.Font;
using D2D1 = SharpDX.Direct2D1;
using DX = SharpDX;
using DXM = SharpDX.Mathematics.Interop;

namespace Peracto.Svg.Render.Dx.Utility
{
  public class LayerHelper : IDisposable
  {
    private readonly D2D1.RenderTarget _target;
    private readonly D2D1.LayerParameters _layer;

    public LayerHelper(D2D1.RenderTarget rt, D2D1.LayerParameters layer)
    {
      _target = rt;
      _layer = layer;
      rt.PushLayer(ref _layer, null);
    }

    public void Dispose()
    {
      _target.PopLayer();
      _layer.GeometricMask?.Dispose();
    }


    public static IDisposable Create(D2D1.RenderTarget target, PxSize size, float opacity)
    {
      return new LayerHelper(
        target,
        CreateLayerParams(new DXM.RawRectangleF(0, 0, size.Width, size.Height), opacity)
      );
    }

    private static DXM.RawRectangleF EmptyClipBounds = new DXM.RawRectangleF(0f, 0f, 999999, 999999);

    public static IDisposable Create(
      D2D1.RenderTarget target,
      FontManager fontManager,
      IElement element,
      IFrameContext context
    )
    {
      return Create(target, fontManager, element, context, false, ref EmptyClipBounds);
    }
    public static IDisposable Create(
      D2D1.RenderTarget target,
      FontManager fontManager,
      IElement element,
      IFrameContext context,
      PxSize clipSize
    )
    {
      var clip = new DXM.RawRectangleF(0f, 0f, clipSize.Width, clipSize.Height);
      return Create(target, fontManager, element, context, true, ref clip);
    }

    private static IDisposable Create(
      D2D1.RenderTarget target,
      FontManager fontManager,
      IElement element,
      IFrameContext context,
      bool clipElement,
      ref DXM.RawRectangleF clipBounds 
    )
    {
      var opacity = element.GetOpacity();
      var clip = element.GetClipPath();
      return !clipElement && clip == null && opacity >= 1
        ? null
        : new LayerHelper(
          target,
          CreateLayerParams(
            clip == null
              ? null
              : ClipPathBuilder.Create(target, fontManager, context, clip, element),
            opacity,
            ref clipBounds
          )
        );
    }

    private static readonly DXM.RawMatrix3x2 Identity = DX.Matrix3x2.Identity;

    private static D2D1.LayerParameters CreateLayerParams(D2D1.Geometry geom, float opacity, ref DXM.RawRectangleF clipBounds)
    {
      return new D2D1.LayerParameters()
      {
        GeometricMask = geom,
        MaskTransform = Identity,
        ContentBounds = clipBounds,
        LayerOptions = D2D1.LayerOptions.None,
        Opacity = opacity,
        OpacityBrush = null
      };
    }

    private static D2D1.LayerParameters CreateLayerParams(DXM.RawRectangleF bounds, float opacity)
    {
      return new D2D1.LayerParameters()
      {
        ContentBounds = bounds,
        LayerOptions = D2D1.LayerOptions.None,
        Opacity = opacity,
        OpacityBrush = null
      };
    }


  }
}
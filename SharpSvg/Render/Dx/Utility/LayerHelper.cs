using Peracto.Svg.Render.Dx.Path;
using Peracto.Svg.Types;
using System;
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

    public static IDisposable CreateClip(D2D1.RenderTarget target, PxRectangle rect, float opacity, bool fill = false)
    {
      var lh = new LayerHelper(
        target,
        new D2D1.LayerParameters()
        {
          ContentBounds = rect.ToDx(),
          LayerOptions = D2D1.LayerOptions.None,
          Opacity = opacity,
          OpacityBrush = null
        }
      );

      if(fill) target.Clear(new DXM.RawColor4(1,(rect.X%100)/100f,0,1));

      return lh;
    }

    public static IDisposable Create(D2D1.RenderTarget target, PxSize size, float opacity)
    {
      return new LayerHelper(
        target,
        new D2D1.LayerParameters()
        {
          ContentBounds = new DXM.RawRectangleF(0, 0, size.Width, size.Height),
          LayerOptions = D2D1.LayerOptions.None,
          Opacity = opacity,
          OpacityBrush = null
        });
    }

    public static IDisposable Create(D2D1.RenderTarget target, IElement element, IFrameContext context, bool setPosition)
    {
      var t = TransformHelper.Create(target, element, context, setPosition);
      var clip = element.GetClipPath();
      if (clip == null) return t;
      var geom = ClipPathBuilder.Create(target, context, clip);
      if (geom == null) return t;

      return Disposable.CreateAggregateDispose(
        t,
        new LayerHelper(
          target,
          new D2D1.LayerParameters()
          {
            GeometricMask = geom,
            MaskTransform = DX.Matrix3x2.Identity,
            ContentBounds = new DXM.RawRectangleF(0f, 0f, 999999,999999), 
            LayerOptions = D2D1.LayerOptions.None,
            Opacity = 1,
            OpacityBrush = null
          }
          )
      );
    }
  }
}
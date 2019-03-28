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

    public static IDisposable Create(D2D1.RenderTarget target, FontManager fontManager, IElement element, IFrameContext context, bool setPosition)
    {
      var t = TransformHelper.Create(target, element, context, setPosition);
      var opacity = element.GetOpacity();
      var clip = element.GetClipPath();

      Console.WriteLine($"Creating layer {element.ElementType} Opacity:{opacity} Clip:{clip}");

      if (clip == null && opacity>=1) return t;

      if (clip == null)
      {
        return Disposable.CreateAggregateDispose(
          t,
          new LayerHelper(
            target,
            new D2D1.LayerParameters()
            {
              GeometricMask = null,
              MaskTransform = DX.Matrix3x2.Identity,
              ContentBounds = new DXM.RawRectangleF(0f, 0f, 999999, 999999),
              LayerOptions = D2D1.LayerOptions.None,
              Opacity = opacity,
              OpacityBrush = null
            }
          )
        );
      }

      var geom = ClipPathBuilder.Create(target, fontManager, context, clip, element);
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
            Opacity = opacity,
            OpacityBrush = null
          }
          )
      );
    }
  }
}
using Peracto.Svg.Render.Dx.IO;
using Peracto.Svg.Render.Dx.Render;
using Peracto.Svg.Render.Dx.Utility;
using Peracto.Svg.Types;
using System;
using System.Threading.Tasks;
using D2D1 = SharpDX.Direct2D1;
using DX = SharpDX;
using DXM = SharpDX.Mathematics.Interop;

namespace Peracto.Svg.Render.Dx.Elements
{
  public static class ImageRender 
  {
    public static async Task Render(IElement element, IFrameContext context, RendererDirect2D render)
    {
      var renderStream = await render.LoadSource(element.GetHref(), element.OwnerDocument.BaseUri);

      if (renderStream == null) return;

      switch (renderStream.RenderStreamType)
      {
        case RenderStreamType.Document:
          await RenderDocument(element, context, render, renderStream.Document);
          return;
        case RenderStreamType.Bitmap:
          RenderBitmap(element, context, render, renderStream.Bitmap);
          return;
        case RenderStreamType.Internal:
        case RenderStreamType.Invalid:
        default:
          return;
      }
    }

    private static void RenderBitmap(IElement element, IFrameContext context, RendererDirect2D render, D2D1.Bitmap bitmap)
    {
      using (LayerHelper.Create(render.Target, element, context, true))
      {
        using (new TransformHelper(
            render.Target,
            element
              .GetPreserveAspectRatio()
              .CalcMatrix(element.GetSize(context,context.Size), bitmap.Size.FromDx().AsRectangle())
          ))
        {
          render.Target.DrawBitmap(
            bitmap,
            new DXM.RawRectangleF(0,0,bitmap.Size.Width,bitmap.Size.Height), 
            element.GetOpacity(),
            D2D1.BitmapInterpolationMode.NearestNeighbor
          );
        }
      }
    }

    private static async Task RenderDocument(IElement element, IFrameContext context, RendererDirect2D render, IDocument doc)
    {
      var fragment = doc?.RootElement;
      if (fragment == null) return;

      using (LayerHelper.Create(render.Target, element, context, true))
      {
        var imageSize = element.GetSize(context, context.Size);
        var imageState = context.Create(imageSize);

        var fragmentVirtualSize = fragment.TryGetViewBox(out var viewBox)
          ? viewBox.Size
          : fragment.GetSize(imageState, imageState.Size);

        var aspectRect = element
          .GetPreserveAspectRatio()
          .ApplyAspectRatio(imageSize.AsRectangle(), fragmentVirtualSize);

        using (CreateClipFrame(element, imageSize, render))
        {
          var matrix =
            DX.Matrix3x2.Translation(aspectRect.X, aspectRect.Y) *
            DX.Matrix3x2.Scaling(imageSize.Width / fragmentVirtualSize.Width,
              imageSize.Height / fragmentVirtualSize.Height);

          render.Target.Transform = matrix * render.Target.Transform;

          var fragmentState = imageState.Create(fragmentVirtualSize);
          foreach (var child in fragment.Children)
            await render.GetRenderer(child.ElementType)(child, fragmentState, render);
        }
      }
    }

    private static IDisposable CreateClipFrame(IElement element, PxSize frameSize, RendererDirect2D render)
    {
      return new LayerHelper(
        render.Target,
        new D2D1.LayerParameters()
        {
          ContentBounds = new DXM.RawRectangleF(0, 0, frameSize.Width, frameSize.Height),
          LayerOptions = D2D1.LayerOptions.None,
          Opacity = element.GetOpacity(),
          OpacityBrush = null
        });
    }
  }
}


/*
   public D2D1.Bitmap GetBitmap(D2D1.Bitmap frame, RendererDirect2D render)
   {
     //frame.
     //using (var converter = new SharpDX.WIC.FormatConverter(render.Base.WicFactory))
     using (var scaler = new SharpDX.WIC.BitmapScaler(render.Base.WicFactory))
     {
       var size = frame.Size;
       // var aspectSize = aspect.ApplyAspectRatio(bounds, new Geometry.Size(size.Width, size.Height)).Size;

       /*   var scale = Math.Min(Math.Min(
            aspectSize.Width / size.Width,
            aspectSize.Height / size.Height
            ) * 10.5, 1.0f);
       scaler.Initialize(frame, (int)(size.Width * scale), (int)(size.Height * scale), WIC.BitmapInterpolationMode.HighQualityCubic);
            *x/

       scaler.Initialize(frame, (int)(size.Width + 0.5), (int)(size.Height + 0.5), SharpDX.WIC.BitmapInterpolationMode.HighQualityCubic);
       //converter.Initialize(scaler, WIC.PixelFormat.Format32bppPRGBA);
       // converter.Initialize(frame, WIC.PixelFormat.Format32bppPRGBA);

       return D2D1.Bitmap.FromWicBitmap(render.Target, scaler);
     }
   }
*/

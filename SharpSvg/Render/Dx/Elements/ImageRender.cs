using Peracto.Svg.Render.Dx.IO;
using Peracto.Svg.Render.Dx.Render;
using Peracto.Svg.Render.Dx.Utility;
using System.Threading.Tasks;
using D2D1 = SharpDX.Direct2D1;
using DXM = SharpDX.Mathematics.Interop;

namespace Peracto.Svg.Render.Dx.Elements
{
  public static class ImageRender
  {
    public static async Task Render(IElement element, IFrameContext context, RendererDirect2D render)
    {
      var renderStream = await render.LoadSource(element.GetHref(), element.OwnerDocument.BaseUri);

      if (renderStream == null) return;

      // ReSharper disable once SwitchStatementMissingSomeCases
      switch (renderStream.RenderStreamType)
      {
        case RenderStreamType.Document:
          await renderStream.Document.Render(element, context, render);
          return;
        case RenderStreamType.Bitmap:
          renderStream.Bitmap.Render(element, context, render);
          return;
        default:
          return;
      }
    }

    private static void Render(this D2D1.Bitmap bitmap, IElement element, IFrameContext context, RendererDirect2D render)
    {
      var size = element.GetSize(context, context.Size);
      var viewPort = bitmap.Size.FromDx().AsRectangle();

      using (TransformHelper.CreatePosition(render, element, context))
      using (LayerHelper.Create(render, element, context, size))
      using (TransformHelper.Create(render, element.GetPreserveAspectRatio().CalcMatrix(size, viewPort)))
      {
        render.Target.DrawBitmap(
          bitmap,
          new DXM.RawRectangleF(0, 0, bitmap.Size.Width, bitmap.Size.Height),
          element.GetOpacity(),
          D2D1.BitmapInterpolationMode.NearestNeighbor
        );
      }
    }

    public static async Task Render(this IDocument doc,IElement element, IFrameContext context, RendererDirect2D render)
    {
      var root = doc.RootElement;
      if (root == null) return;

      var size = element.GetSize(context, context.Size);
      var viewPort = root.GetViewBox()?.AsRectangle() ?? size.AsRectangle();
      var newContext = context.Create(viewPort.Size);

      using (TransformHelper.CreatePosition(render, element, context))
      using (TransformHelper.Create(render, element.GetPreserveAspectRatio().CalcMatrix(size, viewPort)))
      using (LayerHelper.Create(render, element, context, viewPort.Size))
        foreach (var child in root.Children)
          await render.GetRenderer(child.ElementType)(child, newContext, render);
    }
  }
}


using Peracto.Svg.Image;
using Peracto.Svg.Render.Dx.Render;
using Peracto.Svg.Render.Dx.Utility;
using Peracto.Svg.Types;
using System;

namespace Peracto.Svg.Render.Dx.Elements
{
  public static class SvgRender
  {
    public static async System.Threading.Tasks.Task Render(IElement element, IFrameContext context,
      RendererDirect2D render)
    {
      var ratio = element.GetPreserveAspectRatio();
      var size = element.GetSize(context, context.Size);

      var viewPort = element.TryGetViewBox(out var viewBox)
        ? viewBox.AsRectangle()
        : element.GetSize(context, context.Size).AsRectangle();

      var newContext = context.Create(viewPort.Size);

      using (TransformHelper.Create(render.Target, element, context, true))
      using (LayerHelper.Create(render.Target, render.FontManager, element, context, size))
      using (new TransformHelper(render.Target, ratio.CalcMatrix(size, viewPort.Size.AsRectangle())))
      {
        foreach (var child in element.Children)
          await render.GetRenderer(child.ElementType)(child, newContext, render);
      }
    }
  }
}
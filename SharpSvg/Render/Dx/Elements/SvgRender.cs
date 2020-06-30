using Peracto.Svg.Render.Dx.Render;
using Peracto.Svg.Render.Dx.Utility;
using System.Threading.Tasks;

namespace Peracto.Svg.Render.Dx.Elements
{
  public static class SvgRender
  {
    public static async Task Render(IElement element, IFrameContext context,RendererDirect2D render)
    {
        if (TransformHelper.IsHidden(element)) return;

            var size = element.GetSize(context, context.Size);
      var viewPort = element.GetViewBox()?.AsRectangle() ?? size.AsRectangle();

      var newContext = context.Create(viewPort.Size);

      using (element.IsRootElement ? null : TransformHelper.CreatePosition(render, element, context))
      using (LayerHelper.Create(render, element, context, size))
      using (TransformHelper.Create(render, element.GetPreserveAspectRatio().CalcMatrix(size, viewPort)))
      {
        foreach (var child in element.Children)
          await render.GetRenderer(child.ElementType)(child, newContext, render);
      }
    }
  }
}
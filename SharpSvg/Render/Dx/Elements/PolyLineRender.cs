using System.Threading.Tasks;
using Peracto.Svg.Render.Dx.Render;
using Peracto.Svg.Render.Dx.Utility;

namespace Peracto.Svg.Render.Dx.Elements
{
  public static class PolyLineRender 
  {
    public static Task Render(IElement element, IFrameContext context, RendererDirect2D render)
    {
        if (TransformHelper.IsHidden(element)) return Task.CompletedTask;

            using (TransformHelper.Create(render, element, context))
      using (LayerHelper.Create(render, element, context))
      {
        render.DrawPolyline(
          element,
          context,
          element.GetPoints(),
          element.GetFill(context),
          element.GetStroke(context)
        );
        return Task.CompletedTask;
      }
    }
  }
}
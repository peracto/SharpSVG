using System.Threading.Tasks;
using Peracto.Svg.Render.Dx.Render;
using Peracto.Svg.Render.Dx.Utility;

namespace Peracto.Svg.Render.Dx.Elements
{
  public static class EllipseRender 
  {
    public static Task Render(IElement element, IFrameContext context, RendererDirect2D render)
    {
      using (TransformHelper.Create(render, element, context))
      using (LayerHelper.Create(render, element, context))
      {
        render.DrawEllipse(
          element,
          context,
          element.GetCxCy(context),
          element.GetRxRy(context),
          element.GetFill(context),
          element.GetStroke(context)
        );
        return Task.CompletedTask;

      }
    }
  }
}
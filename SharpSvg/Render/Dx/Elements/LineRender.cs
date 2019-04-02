using System.Threading.Tasks;
using Peracto.Svg.Render.Dx.Render;
using Peracto.Svg.Render.Dx.Utility;

namespace Peracto.Svg.Render.Dx.Elements
{
  public static class LineRender 
  {
    public static Task Render(IElement element, IFrameContext context, RendererDirect2D render)
    {
      using (TransformHelper.Create(render, element, context))
      using (LayerHelper.Create(render, element, context))
      {
        render.DrawLine(
          element,
          context,
          element.GetX1Y1(context),
          element.GetX2Y2(context),
          element.GetStroke(context)
        );
      }
      return Task.CompletedTask;

    }
  }
}
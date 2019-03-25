using Peracto.Svg.Render.Dx.Render;
using Peracto.Svg.Render.Dx.Utility;

namespace Peracto.Svg.Render.Dx.Elements
{
  public static class LineRender 
  {
    public static async System.Threading.Tasks.Task Render(IElement element, IFrameContext context, RendererDirect2D render)
    {
      using (LayerHelper.Create(render.Target, element, context, false))
      {
        render.DrawLine(
          element,
          context,
          element.GetX1Y1(context),
          element.GetX2Y2(context),
          element.GetStroke(context)
        );
      }
    }
  }
}
using Peracto.Svg.Render.Dx.Render;
using Peracto.Svg.Render.Dx.Utility;

namespace Peracto.Svg.Render.Dx.Elements
{
  public static class EllipseRender 
  {
    public static async System.Threading.Tasks.Task Render(IElement element, IFrameContext context, RendererDirect2D render)
    {
      using (LayerHelper.Create(render.Target, render.FontManager, element, context, false))
      {
        render.DrawEllipse(
          element,
          context,
          element.GetCxCy(context),
          element.GetRxRy(context),
          element.GetFill(context),
          element.GetStroke(context)
        );
      }
    }
  }
}
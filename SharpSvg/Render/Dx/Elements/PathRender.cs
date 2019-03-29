using System.Threading.Tasks;
using Peracto.Svg.Render.Dx.Render;
using Peracto.Svg.Render.Dx.Utility;

namespace Peracto.Svg.Render.Dx.Elements
{
  public static class PathRender 
  {
    public static Task Render(IElement element, IFrameContext context, RendererDirect2D render)
    {
      using (LayerHelper.Create(render.Target, render.FontManager, element, context, false))
      {
        render.DrawPath(
          element,
          context,
          element.GetPath(),
          element.GetFill(context),
          element.GetStroke(context)
        );
        return Task.CompletedTask;

      }
    }
  }
}
using Peracto.Svg.Render.Dx.Render;
using Peracto.Svg.Render.Dx.Utility;

namespace Peracto.Svg.Render.Dx.Elements
{
  public static class GRender 
  {
    public static async System.Threading.Tasks.Task Render(IElement element, IFrameContext context, RendererDirect2D render)
    {
      using (TransformHelper.Create(render, element, context))
      using (LayerHelper.Create(render, element, context))
        foreach (var child in element.Children)
          await render.GetRenderer(child.ElementType)(child, context, render);
    }
  }
}
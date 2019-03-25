using Peracto.Svg.Render.Dx.Render;
using Peracto.Svg.Render.Dx.Utility;
using System.Threading.Tasks;

namespace Peracto.Svg.Render.Dx.Elements
{
  public static class UseRender
  {
    public static async Task Render(IElement element, IFrameContext context, RendererDirect2D render)
    {
      var href = element.GetHref();
      if (href == null || href.Scheme != "internal") return;

      var fragment = element.GetElementById(href.Fragment.Substring(1));

      if (fragment == null) return;

      try
      {
        fragment.SetParentOverride(element);

        using (LayerHelper.Create(render.Target, element, context, true))
          await render.GetRenderer(fragment.ElementType)(
            fragment,
            context.Create(element.GetSize(context, context.Size)),
            render
          );
      }
      finally
      {
        fragment.SetParentOverride(null);
      }
    }
  }
}

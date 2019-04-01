using System;
using Peracto.Svg.Render.Dx.Render;
using Peracto.Svg.Render.Dx.Utility;
using System.Threading.Tasks;
using Peracto.Svg.Render.Dx.IO;

namespace Peracto.Svg.Render.Dx.Elements
{
  public static class UseRender
  {
    public static async Task<IElement> LoadExtenal(Uri href, RendererDirect2D render, Uri baseUri)
    {
      var lastSegment = Uri.UnescapeDataString(href.Segments[href.Segments.Length - 1]);

      var idx = lastSegment.LastIndexOf('#');
      if (idx <= 0) return null;

      var du = new Uri(href,lastSegment.Substring(0,idx));

      var renderStream = await render.LoadSource(du, baseUri);

      if(renderStream.RenderStreamType != RenderStreamType.Document)
        return null;

      return renderStream.Document.GetElementById(lastSegment.Substring(idx+1));
    }

    public static async Task Render(IElement element, IFrameContext context, RendererDirect2D render)
    {
      var href = element.GetHref();
      if (href == null) return;
      var fragment = (href.Scheme == "internal")
        ? element.OwnerDocument?.GetElementById(href.Fragment.Substring(1))
        : await LoadExtenal(href,render, element.OwnerDocument.BaseUri);

      if (fragment == null) return;

      try
      {
        fragment.SetParentOverride(element);

        using (TransformHelper.Create(render.Target, element, context, true))
        using (LayerHelper.Create(render.Target, render.FontManager, element, context))
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

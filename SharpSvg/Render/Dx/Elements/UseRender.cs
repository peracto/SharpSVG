using System;
using Peracto.Svg.Render.Dx.Render;
using Peracto.Svg.Render.Dx.Utility;
using System.Threading.Tasks;
using Peracto.Svg.Render.Dx.IO;

namespace Peracto.Svg.Render.Dx.Elements
{
  public static class UseRender
  {
    public static async Task Render(IElement element, IFrameContext context, RendererDirect2D render)
    {
      var href = element.GetHref();
      if (href == null) return;
      var fragment = (href.Scheme == "internal")
        ? element.OwnerDocument?.GetElementById(href.Fragment.Substring(1))
        : await LoadExternal(href, render, element.OwnerDocument.BaseUri);

      if (fragment == null) return;

      try
      {
        fragment.SetParentOverride(element);
        using (TransformHelper.CreatePosition(render, element, context))
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

    public static async Task<IElement> LoadExternal(Uri href, RendererDirect2D render, Uri baseUri)
    {
      var lastSegment = Uri.UnescapeDataString(href.Segments[href.Segments.Length - 1]);

      var idx = lastSegment.LastIndexOf('#');
      if (idx <= 0) return null;

      var du = new Uri(href, lastSegment.Substring(0, idx));

      var renderStream = await render.LoadSource(du, baseUri);

      return renderStream.RenderStreamType != RenderStreamType.Document 
        ? null 
        : renderStream.Document.GetElementById(lastSegment.Substring(idx + 1));
    }
  }
}

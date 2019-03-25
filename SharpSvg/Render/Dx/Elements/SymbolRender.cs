using System;
using Peracto.Svg.Render.Dx.Render;
using Peracto.Svg.Render.Dx.Utility;

namespace Peracto.Svg.Render.Dx.Elements
{
  public static class SymbolRender 
  {
    public static async System.Threading.Tasks.Task Render(IElement element, IFrameContext context, RendererDirect2D render)
    {
      if (element.Parent.ElementType != "use") return;

      using (CreateFrame(element, context, render, out var newstate))
        foreach (var child in element.Children)
          await render.GetRenderer(child.ElementType)(child, newstate, render);
    }

    private static IDisposable CreateFrame(IElement element, IFrameContext context, RendererDirect2D render, out IFrameContext newContext)
    {
      var viewSize = element.TryGetViewBox(out var viewBox)
        ? viewBox.AsRectangle()
        : element.GetSize(context, context.Size).AsRectangle(0,0);

      newContext = context.Create(viewSize.Size);

      return viewBox == null
        ? null
        : new TransformHelper(
          render.Target,
          element
            .GetPreserveAspectRatio()
            .CalcMatrix(context.Size, viewSize)
        );
    }
  }
}

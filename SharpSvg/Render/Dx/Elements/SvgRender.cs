using System;
using Peracto.Svg.Render.Dx.Render;
using Peracto.Svg.Render.Dx.Utility;
using Peracto.Svg.Types;

namespace Peracto.Svg.Render.Dx.Elements
{
  public static class SvgRender 
  {
    public static async System.Threading.Tasks.Task Render(IElement element, IFrameContext context, RendererDirect2D render)
    {
      using (CreateFrame(element, context, render, out var frameState))
        foreach (var child in element.Children)
          await render.GetRenderer(child.ElementType)(child, frameState, render);
    }

    private static IDisposable CreateFrame(IElement element, IFrameContext context, RendererDirect2D render,
      out IFrameContext newContext)
    {
      var viewSize = element.TryGetViewBox(out var viewBox)
        ? viewBox.AsRectangle()
        : element.GetSize(context, context.Size).AsRectangle();

      newContext = context.Create(viewSize.Size);

      var offset = element.IsRootElement ? new PxPoint() : element.GetPosition(context);

      var matrix = viewBox == null
        ? PxMatrix.Translate(offset.X, offset.Y)
        : PxMatrix.Translate(offset.X, offset.Y) *
          element.GetPreserveAspectRatio().CalcMatrix(context.Size, viewSize);
        ;

        Console.WriteLine($"SVG Box:{context.Size.Width}:{context.Size.Height} ... Clip:{offset.X},{offset.Y},{viewSize.Width},{viewSize.Height} matrix:sx{matrix.M11},sy{matrix.M22},x:{matrix.M31},y:{matrix.M32}");

      return Disposable.CreateAggregateDispose(
        LayerHelper.CreateClip(
          render.Target,
          new PxRectangle(offset.X,offset.Y,viewSize.Width,viewSize.Height),
          element.GetOpacity(),
          false//!element.IsRootElement
        ),
        new TransformHelper(render.Target,matrix)
      );
    }
  }
}

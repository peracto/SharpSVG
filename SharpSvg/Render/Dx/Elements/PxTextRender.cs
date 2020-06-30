using Peracto.Svg.Render.Dx.Render;
using Peracto.Svg.Render.Dx.Utility;
using Peracto.Svg.Types;
using System.Linq;
using System.Threading.Tasks;
using D2D1 = SharpDX.Direct2D1;
using DX = SharpDX;
using DXM = SharpDX.Mathematics.Interop;

namespace Peracto.Svg.Render.Dx.Elements
{
  public static class PxTextRender 
  {
    private static float CalcScale(PxSize frameSize, PxSize textSize)
    {
      var scale = frameSize.Width / textSize.Width;
      var textHeight = textSize.Height;
      while (textHeight * scale > frameSize.Height)
        scale -= 0.1f;
      return scale;
    }

    public static Task Render(IElement element, IFrameContext context, RendererDirect2D render)
    {
        if (TransformHelper.IsHidden(element)) return Task.CompletedTask;

            using (TransformHelper.Create(render, element, context))
      using (LayerHelper.Create(render, element, context))
      {
        var font = element.GetFont(context);
        var fill = element.GetFill(context);
        var fillBrush = render.CreateBrush(element, context, fill.Brush, fill.Opacity);

        if(fillBrush==null) return Task.CompletedTask;

        foreach (var text in element.Children.OfType<ITextContent>())
        {
          using (var textLayout = render.CreateTextLayout(font, text.Content, 999999))
          {
            var frameSize = context.Size;
            var textSize = new PxSize(textLayout.Metrics.WidthIncludingTrailingWhitespace, textLayout.Metrics.Height);
            var scale = CalcScale(frameSize,textSize);

            var matrix = PxMatrix.Translation(
              scale,
              scale,
              (frameSize.Width - (scale * textSize.Width)) / 2f,
              (frameSize.Height - (scale * textSize.Height)) / 2f
            );

            using (TransformHelper.Create(render, matrix))
            {
              render.Target.DrawTextLayout(
                new DXM.RawVector2(0, 0),
                textLayout,
                fillBrush,
                D2D1.DrawTextOptions.EnableColorFont
              );
            }
          }
        }
        return Task.CompletedTask;
      }
    }
  }
}
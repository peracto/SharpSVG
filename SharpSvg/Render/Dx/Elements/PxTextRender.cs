using Peracto.Svg.Render.Dx.Render;
using Peracto.Svg.Render.Dx.Utility;
using Peracto.Svg.Types;
using System.Linq;
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

    public static async System.Threading.Tasks.Task Render(IElement element, IFrameContext context, RendererDirect2D render)
    {
      using (LayerHelper.Create(render.Target, element, context, false))
      {
        var font = element.GetFont(context);
        var fill = element.GetFill(context);
        var fillBrush = render.CreateBrush(element, context, fill.Brush, fill.Opacity);

        if(fillBrush==null) return;

        foreach (var text in element.Children.OfType<ITextContent>())
        {
          using (var textLayout = render.CreateTextLayout(font, text.Content, 999999))
          {
            var frameSize = context.Size;
            var textSize = new PxSize(textLayout.Metrics.WidthIncludingTrailingWhitespace, textLayout.Metrics.Height);
            var scale = CalcScale(frameSize,textSize);

            var matrix = DX.Matrix3x2.Transformation(
              scale,
              scale,
              0,
              (frameSize.Width - (scale * textSize.Width)) / 2f,
              (frameSize.Height - (scale * textSize.Height)) / 2f
            );

            using (new TransformHelper(render.Target, matrix))
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
      }
    }
  }
}
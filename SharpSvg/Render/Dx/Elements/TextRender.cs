﻿using Peracto.Svg.Brush;
using Peracto.Svg.Render.Dx.Render;
using Peracto.Svg.Render.Dx.Utility;
using Peracto.Svg.Text;
using D2D1 = SharpDX.Direct2D1;
using DW = SharpDX.DirectWrite;
using DXM = SharpDX.Mathematics.Interop;

namespace Peracto.Svg.Render.Dx.Elements
{
  public static class TextRender 
  {
    public static async System.Threading.Tasks.Task Render(IElement element, IFrameContext context, RendererDirect2D render)
    {
      using (LayerHelper.Create(render.Target, element, context, false))
      {
        var x = 0f;
        var y = 0f;

        IBrush brush = null;
        var fillOpacity = -1f;
        SharpDX.Direct2D1.Brush fillBrush = null;
        foreach (var t in element.GetText(context))
        {
          if (t.TextEntryType == TextEntryType.Cursor)
          {
            if (t.X.HasValue) x = t.X.Value;
            if (t.Y.HasValue) y = t.Y.Value;
            x += t.Dx;
            y += t.Dy;
            continue;
          }

          using (var textLayout = render.CreateTextLayout(t.Font, t.Content, 999999))
          {
            
            if (brush != t.Fill.Brush || !MathEx.NearEqual(fillOpacity,t.Fill.Opacity))
            {
              fillBrush = render.CreateBrush(element, context, t.Fill.Brush, 1f); //t.Fill.Opacity);
              brush = t.Fill.Brush;
              fillOpacity = t.Fill.Opacity;
            }

            if (fillBrush != null)
              render.Target.DrawTextLayout(
                new DXM.RawVector2(x, y - CalcBaselineOffset(textLayout, t.Font.DominantBaseline)),
                textLayout,
                fillBrush,
                D2D1.DrawTextOptions.EnableColorFont
              );

            x += textLayout.Metrics.WidthIncludingTrailingWhitespace;
          }
        }
      }
    }

    private static float CalcBaselineOffset(DW.TextLayout tl, Text.DominantBaseline db)
    {
      var lm = tl.GetLineMetrics()[0];
      switch (db)
      {
        case Text.DominantBaseline.Middle:
          return (lm.Height + (lm.Height - lm.Baseline)) / 2;
        case Text.DominantBaseline.Auto:
        case Text.DominantBaseline.Baseline:
          return lm.Baseline;
        default:
          return lm.Height - lm.Baseline;
      }
    }
  }
}
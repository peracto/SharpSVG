using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Peracto.Svg.Brush;
using Peracto.Svg.Render.Dx.Render;
using Peracto.Svg.Render.Dx.Utility;
using Peracto.Svg.Text;
using SharpDX;
using D2D1 = SharpDX.Direct2D1;
using DW = SharpDX.DirectWrite;
using DXM = SharpDX.Mathematics.Interop;

namespace Peracto.Svg.Render.Dx.Elements
{
    public static class TextRender
    {
        private class TextSpan
        {
            public float? X { get; }
            public float? Y { get; }
            public IList<TextItem> Items;
            public TextAnchor TextAnchor;

            public TextSpan(float? x, float? y, TextAnchor textAnchor, IList<TextItem> items)
            {
                X = x;
                Y = y;
                Items = items;
                TextAnchor = textAnchor;
            }
        }

        private class TextItem
        {
            public float Dx { get; }
            public float Dy { get; }
            public Text.Font Font { get; }
            public string Text { get; }
            public Fill Fill { get; }
            public Stroke Stroke { get; }
            public readonly DominantBaseline DominantBaseline;

            public TextItem(float dx, float dy, Text.Font font, Fill fill, Stroke stroke, DominantBaseline dominantBaseline,
                string text)
            {
                Dx = dx;
                Dy = dy;
                Font = font;
                Text = text;
                Fill = fill;
                Stroke = stroke;
                DominantBaseline = dominantBaseline;
            }
        }

        private static IEnumerable<TextSpan> GetSpans(IElement element, IFrameContext context)
        {
            var stack = new Stack<(IEnumerator<IElement> e, Text.Font font, Fill fill, Stroke stroke)>();
            var enumerator = element.Children.GetEnumerator();

            var font = element.GetFont(context);
            var fill = element.GetFill(context);
            var stroke = element.GetStroke(context);
            var x = element.GetX(context);
            var y = element.GetY(context);
            var dx = element.GetDx(context);
            var dy = element.GetDy(context);
            var textAnchor = element.GetTextAnchor();
            var dominantBaseline = element.GetDominantBaseline();
            var items = new List<TextItem>();

            while (true)
            {
                while (enumerator.MoveNext())
                {
                    var e = enumerator.Current;
                    if (e == null) continue;

                    if (e is ITextContent text)
                    {
                        items.Add(new TextItem(dx, dy, font, fill, stroke, dominantBaseline, text.Content));
                        dx = 0;
                        dy = 0;
                    }
                    else if (e.ElementType == "tspan")
                    {
                        var mx = e.GetX(context);
                        var my = e.GetY(context);

                        dx = e.GetDx(context);
                        dy = e.GetDy(context);
                        font = e.GetFont(context, font);
                        fill = e.GetFill(context);
                        stroke = e.GetStroke(context);
                        dominantBaseline = element.GetDominantBaseline();

                        if (mx.HasValue || my.HasValue)
                        {
                            yield return new TextSpan(x, y, textAnchor, items);
                            textAnchor = e.GetTextAnchor();
                            items = new List<TextItem>();
                            x = mx;
                            y = my;
                        }

                        var c = e.Children;
                        if (c.Count == 0) continue;
                        stack.Push((enumerator, font, fill, stroke));
                        enumerator = c.GetEnumerator();
                    }
                }

                if (stack.Count == 0) break;
                (enumerator, font, fill, stroke) = stack.Pop();
            }

            if (items.Count > 0)
                yield return new TextSpan(x, y, textAnchor, items);
        }

        public static Task Render(IElement element, IFrameContext context, RendererDirect2D render)
        {
            using (TransformHelper.Create(render, element, context))
            using (LayerHelper.Create(render, element, context))
            {
                var x = 0f;
                var y = 0f;

                IBrush brush = null;
                IBrush sbrush = null;
                var fillOpacity = -1f;
                D2D1.Brush fillBrush = null;
                D2D1.Brush strokeBrush = null;
                var strokeOpacity = -1f;

                foreach (var span in GetSpans(element, context))
                {
                    if (span.X.HasValue) x = span.X.Value;
                    if (span.Y.HasValue) y = span.Y.Value;

                    var c = span.Items.Count;
                    var layouts = new List<DW.TextLayout>(c);
                    var w = 0f;
                    for (var i = 0; i < c; i++)
                    {
                        var ti = span.Items[i];

                        var l = render.CreateTextLayout(ti.Font, ti.Text, 9999999);
                        layouts.Add(l);
                        w += ti.Dx + l.Metrics.WidthIncludingTrailingWhitespace;
                    }

                    x -= span.TextAnchor == TextAnchor.Middle
                        ? w / 2
                        : span.TextAnchor == TextAnchor.End
                            ? w
                            : 0;

                    for (var i = 0; i < c; i++)
                    {
                        var ti = span.Items[i];
                        var l = layouts[i];

                        if (brush != ti.Fill.Brush || !MathEx.NearEqual(fillOpacity, ti.Fill.Opacity))
                        {
                            fillBrush = render.CreateBrush(element, context, ti.Fill.Brush, ti.Fill.Opacity);
                            brush = ti.Fill.Brush;
                            fillOpacity = ti.Fill.Opacity;
                        }

                        if (sbrush != ti.Stroke.Brush || !MathEx.NearEqual(strokeOpacity, ti.Stroke.Opacity))
                        {
                            strokeBrush = render.CreateBrush(element, context, ti.Stroke.Brush, ti.Stroke.Opacity);
                            sbrush = ti.Stroke.Brush;
                            fillOpacity = ti.Fill.Opacity;
                        }

                        x += ti.Dx;
                        y += ti.Dy;


                        if (fillBrush != null)
                        {
                            if (ti.Stroke.Brush != null)
                            {
                                l.Draw(new CustomColorRenderer(render.Target, fillBrush, strokeBrush, ti.Stroke.Width), x,
                                    y - CalcBaselineOffset(l, ti.DominantBaseline));
                            }
                            else
                            {
                                render.Target.DrawTextLayout(
                                    new DXM.RawVector2(x, y - CalcBaselineOffset(l, ti.DominantBaseline)),
                                    l,
                                    fillBrush,
                                    D2D1.DrawTextOptions.EnableColorFont
                                );
                            }
                        }

                        x += l.Metrics.WidthIncludingTrailingWhitespace;

                        l.Dispose();
                    }
                }

                return Task.CompletedTask;

            }
        }

        private static float CalcBaselineOffset(DW.TextLayout tl, DominantBaseline db)
        {
            var lm = tl.GetLineMetrics()[0];
            switch (db)
            {
                case DominantBaseline.Middle:
                    return (lm.Height + (lm.Height - lm.Baseline)) / 2;
                case DominantBaseline.Auto:
                case DominantBaseline.Baseline:
                    return lm.Baseline;
                default:
                    return lm.Height - lm.Baseline;
            }
        }
    }

    public class CustomColorRenderer : SharpDX.DirectWrite.TextRendererBase
    {
        public CustomColorRenderer(D2D1.RenderTarget renderTarget, D2D1.Brush fillBrush, D2D1.Brush strokeBrush, float strokeWidth)
        {
            this._renderTarget = renderTarget;
            _fillBrush = fillBrush;
            _strokeBrush = strokeBrush;
            _strokeWidth = strokeWidth;
        }
        private readonly D2D1.RenderTarget _renderTarget;
        private readonly D2D1.Brush _fillBrush;
        private readonly D2D1.Brush _strokeBrush;
        private readonly float _strokeWidth;

        public override Result DrawGlyphRun(object clientDrawingContext, float baselineOriginX, float baselineOriginY,
            D2D1.MeasuringMode measuringMode, DW.GlyphRun glyphRun, DW.GlyphRunDescription glyphRunDescription,
            ComObject clientDrawingEffect)
        {
            using (var pathGeometry = new D2D1.PathGeometry(_renderTarget.Factory))
            {
                using (var geometrySink = pathGeometry.Open())
                using (var fontFace = glyphRun.FontFace)
                {

                    if (glyphRun.Indices.Length > 0)
                        fontFace.GetGlyphRunOutline(
                            glyphRun.FontSize,
                            glyphRun.Indices,
                            glyphRun.Advances,
                            glyphRun.Offsets,
                            glyphRun.Indices.Length,
                            glyphRun.IsSideways,
                            glyphRun.BidiLevel % 2 != 0,
                            geometrySink
                        );
                    geometrySink.Close();
                }

                var matrix = new Matrix3x2()
                {
                    M11 = 1,
                    M12 = 0,
                    M21 = 0,
                    M22 = 1,
                    M31 = baselineOriginX,
                    M32 = baselineOriginY
                };

                var sw = _renderTarget.StrokeWidth;
                using (var transformedGeometry =
                    new D2D1.TransformedGeometry(_renderTarget.Factory, pathGeometry, matrix))
                {
                    _renderTarget.StrokeWidth = _strokeWidth;
                    _renderTarget.DrawGeometry(transformedGeometry, _strokeBrush);
                    _renderTarget.FillGeometry(transformedGeometry, _fillBrush);
                }
                _renderTarget.StrokeWidth = sw;

            }

            return SharpDX.Result.Ok;
        }
    }
}
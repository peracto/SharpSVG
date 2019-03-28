using Peracto.Svg.Barcode;
using Peracto.Svg.Brush;
using Peracto.Svg.Render.Dx.Brush;
using Peracto.Svg.Render.Dx.Font;
using Peracto.Svg.Render.Dx.IO;
using Peracto.Svg.Render.Dx.Path;
using Peracto.Svg.Render.Dx.Utility;
using Peracto.Svg.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using D2D1 = SharpDX.Direct2D1;
using DW = SharpDX.DirectWrite;
using DXM = SharpDX.Mathematics.Interop;
using WIC = SharpDX.WIC;

namespace Peracto.Svg.Render.Dx.Render
{
  public class RendererDirect2D : IDisposable
  {
    private readonly D2D1.RenderTarget _dc;
    private readonly ElementRenderRegistry<RendererDirect2D> _registry;
    private readonly FontManager _fontManager;
    private readonly DisposeBucket _disposeManager;
    private readonly IBrushVisitor<D2D1.Brush> _brushManager;
    private readonly RenderControllerBase _renderBase;

    public D2D1.RenderTarget Target => _dc;
    public FontManager FontManager => _fontManager;
    public RendererDirect2D(
      RenderControllerBase renderBase,
      D2D1.RenderTarget dc
    )
    {
      _dc = dc;
      _renderBase = renderBase;
      _registry = renderBase.RenderRegistry;
      _fontManager = renderBase.FontManager;
      _brushManager = new BrushManager(dc);
      _disposeManager = new DisposeBucket();
    }

    private readonly IDictionary<Uri, RenderStream> _renderStreamCache = new Dictionary<Uri, RenderStream>();

    public async System.Threading.Tasks.Task<RenderStream> LoadSource(Uri href, Uri baseUri)
    {
      if (href == null) return null;

      if (_renderStreamCache.TryGetValue(href, out var cache))
        return cache;

      cache = await Base.LoadToStream(href, baseUri, LoadImage);
      _renderStreamCache.Add(href, cache);
      return cache;

    }

    public DW.TextLayout CreateTextLayout(Text.Font font, string text, float width)
    {
      return _fontManager.GetTextLayout(font, text, width);
    }

    public RenderDelegate<RendererDirect2D> GetRenderer(string name)
    {
      return _registry.Get(name);
    }

    public RenderControllerBase Base => _renderBase;

    public D2D1.Bitmap LoadImage(Stream stream)
    {
      using (var converter = new WIC.FormatConverter(_renderBase.WicFactory))
      using (var scaler = new WIC.BitmapScaler(_renderBase.WicFactory))
      using (var bmd = new WIC.BitmapDecoder(_renderBase.WicFactory, stream, WIC.DecodeOptions.CacheOnLoad))
      using (var frame = bmd.GetFrame(0))
      {
        var size = frame.Size;
        scaler.Initialize(frame, (int)(size.Width + 0.5), (int)(size.Height + 0.5 ), WIC.BitmapInterpolationMode.HighQualityCubic);
        converter.Initialize(scaler, WIC.PixelFormat.Format32bppPRGBA);
        return D2D1.Bitmap.FromWicBitmap(Target, converter);
      }
    }

    public D2D1.Brush CreateBrush(IElement element, IFrameContext context, IBrush fill, float opacity)
    {
      return opacity > 0 ? fill?.Visit(_brushManager, element, context, opacity) : null;
    }

    public void DrawCircle(
      IElement element,
      IFrameContext context,
      PxPoint pt,
      float radius,
      Fill fill,
      Stroke strokeStyle
    )
    {
      if (MathEx.IsZero(radius)) return;


      var ellipse = new D2D1.Ellipse(
        new DXM.RawVector2(pt.X, pt.Y),
        radius,
        radius
      );

      var fillBrush = CreateBrush(element, context, fill.Brush, fill.Opacity);
      var strokeBrush = strokeStyle.Width > 0 ? CreateBrush(element, context, strokeStyle.Brush, strokeStyle.Opacity) : null;

      if (fillBrush != null) Target.FillEllipse(ellipse, fillBrush);
      if (strokeBrush != null) Target.DrawEllipse(ellipse, strokeBrush, strokeStyle.Width /*,strokeStyle*/);
    }

    public void DrawEllipse(
      IElement element,
      IFrameContext context,
      PxPoint pt,
      PxPoint radius,
      Fill fill,
      Stroke strokeStyle
    )
    {

      if (MathEx.IsZero(radius.X)  || MathEx.IsZero(radius.Y)) return;

      var ellipse = new D2D1.Ellipse(
        new DXM.RawVector2(pt.X, pt.Y),
        radius.X,
        radius.Y
      );

      var fillBrush = CreateBrush(element, context, fill.Brush, fill.Opacity);
      var strokeBrush = strokeStyle.Width > 0 ? CreateBrush(element, context, strokeStyle.Brush, strokeStyle.Opacity) : null;

      if (fillBrush != null) Target.FillEllipse(ellipse, fillBrush);
      if (strokeBrush != null) Target.DrawEllipse(ellipse, strokeBrush, strokeStyle.Width, GetStrokeStyle(strokeStyle));
    }

    public void DrawLine(
      IElement element,
      IFrameContext context,
      PxPoint point1,
      PxPoint point2,
      Stroke strokeStyle
    )
    {
      if (strokeStyle.Width <= 0) return;
      var strokeBrush = CreateBrush(element, context, strokeStyle.Brush, strokeStyle.Opacity);
      if (strokeBrush == null) return;

      var dx = Math.Abs(point1.X - point2.X);
      var dy = Math.Abs(point1.Y - point2.Y);
      if (MathEx.IsZero((float)Math.Sqrt((dx * dx) + (dy * dy)))) return;

      Target.DrawLine(
        point1.ToDx(),
        point2.ToDx(),
        strokeBrush,
        strokeStyle.Width,
        GetStrokeStyle(strokeStyle)
      );
    }

    public void DrawPath(
      IElement element,
      IFrameContext context,
      Paths.Path path,
      Fill fill,
      Stroke strokeStyle
    )
    {
      if (path == null) return;

      var fillBrush = CreateBrush(element, context, fill.Brush, fill.Opacity);
      var strokeBrush = strokeStyle.Width > 0 ? CreateBrush(element, context, strokeStyle.Brush, strokeStyle.Opacity) : null;
      var geometry = PathBuilder.Create(_dc, path);
      if (fillBrush != null)
        Target.FillGeometry(geometry, fillBrush);

      if (strokeBrush != null)
        Target.DrawGeometry(
          geometry,
          strokeBrush,
          strokeStyle.Width,
          GetStrokeStyle(strokeStyle)
        );
    }

    public void DrawPolygon(
      IElement element,
      IFrameContext context,
      PxPoint[] xpoints,
      Fill fill,
      Stroke strokeStyle
    )
    {
      if (xpoints == null || xpoints.Length == 0) return;

      var points = xpoints
        .Select(s => new DXM.RawVector2(s.X, s.Y))
        .ToList();

      using (var geometry = new D2D1.PathGeometry(_dc.Factory))
      using (var sink = geometry.Open())
      {
        sink.BeginFigure(points[0], D2D1.FigureBegin.Filled);
        sink.AddLines(points.Skip(1).ToArray());
        sink.EndFigure(D2D1.FigureEnd.Closed);
        sink.Close();

        var fillBrush = CreateBrush(element, context, fill.Brush, fill.Opacity);
        var strokeBrush = strokeStyle.Width > 0 ? CreateBrush(element, context, strokeStyle.Brush, strokeStyle.Opacity) : null;

        if (fillBrush != null) Target.FillGeometry(geometry, fillBrush);
        if (strokeBrush != null)
          Target.DrawGeometry(geometry, strokeBrush, strokeStyle.Width, GetStrokeStyle(strokeStyle));
      }
    }

    public void DrawPolyline(
      IElement element,
      IFrameContext context,
      PxPoint[] xpoints,
      Fill fill,
      Stroke strokeStyle
    )
    {
      if (xpoints == null || xpoints.Length == 0) return;

      var points = xpoints
        .Select(s => new DXM.RawVector2(s.X, s.Y))
        .ToList();

      using (var geometry = new D2D1.PathGeometry(_dc.Factory))
      using (var sink = geometry.Open())
      {
        sink.BeginFigure(points[0], D2D1.FigureBegin.Filled);
        sink.AddLines(points.Skip(1).ToArray());
        sink.EndFigure(D2D1.FigureEnd.Open);
        sink.Close();

        var fillBrush = CreateBrush(element, context, fill.Brush, fill.Opacity);
        var strokeBrush = strokeStyle.Width > 0 ? CreateBrush(element, context, strokeStyle.Brush, strokeStyle.Opacity) : null;

        if (fillBrush != null) Target.FillGeometry(geometry, fillBrush);
        if (strokeBrush != null)
          Target.DrawGeometry(geometry, strokeBrush, strokeStyle.Width, GetStrokeStyle(strokeStyle));
      }
    }

    private D2D1.StrokeStyle GetStrokeStyle(Stroke strokeStyle)
    {
      if (strokeStyle == null) return null;
      var style = new D2D1.StrokeStyleProperties()
      {
        StartCap = strokeStyle.LineCap.ToDx(),
        EndCap = strokeStyle.LineCap.ToDx(),
        DashCap = D2D1.CapStyle.Flat,//Round,
        LineJoin = strokeStyle.LineJoin.ToDx(),
        MiterLimit = strokeStyle.MiterLimit,
        DashStyle = strokeStyle.DashArray != null && strokeStyle.DashArray.Length > 0 ? D2D1.DashStyle.Custom : D2D1.DashStyle.Solid,
        DashOffset = strokeStyle.DashOffset
      };

      return style.DashStyle == D2D1.DashStyle.Solid
       ? new D2D1.StrokeStyle(_dc.Factory, style)
       : new D2D1.StrokeStyle(_dc.Factory, style, strokeStyle.DashArray);
    }

    public void DrawRectangle(
      IElement element,
      IFrameContext context,
      PxRectangle bounds,
      PxPoint radius,
      Fill fill,
      Stroke strokeStyle
    )
    {

      if (MathEx.IsZero(bounds.Width) || MathEx.IsZero(bounds.Height)) return;

      var fillBrush = CreateBrush(element, context, fill.Brush, fill.Opacity);
      var strokeBrush = strokeStyle.Width > 0 ? CreateBrush(element, context, strokeStyle.Brush, strokeStyle.Opacity) : null;
      var rect = bounds.ToDx();

      if (radius.X > 0 || radius.Y > 0)
      {
        var roundedRect = new D2D1.RoundedRectangle()
        {
          Rect = rect,
          RadiusX = MathEx.IsZero(radius.X)?radius.Y: radius.X,
          RadiusY = MathEx.IsZero(radius.Y)?radius.X: radius.Y
        };
        if (fillBrush != null) Target.FillRoundedRectangle(roundedRect, fillBrush);
        if (strokeBrush != null)
          Target.DrawRoundedRectangle(roundedRect, strokeBrush, strokeStyle.Width, GetStrokeStyle(strokeStyle));
      }
      else
      {
        if (fillBrush != null) Target.FillRectangle(rect, fillBrush);
        if (strokeBrush != null)
          Target.DrawRectangle(rect, strokeBrush, strokeStyle.Width, GetStrokeStyle(strokeStyle));
      }
    }

    public void DrawBarcode(
      IElement element,
      IFrameContext context,
      PxRectangle bounds,
      Fill fill,
      Stroke strokeStyle,
      string value
    )
    {
      var fillBrush = CreateBrush(element, context, fill.Brush, fill.Opacity);
      var strokeBrush = strokeStyle.Width > 0 ? CreateBrush(element, context, strokeStyle.Brush, strokeStyle.Opacity) : null;
      var rect = bounds.ToDx();

      if (fillBrush != null) Target.FillRectangle(rect, fillBrush);

      if (value == null || strokeBrush == null) return;

      if (!long.TryParse(value, out var v)) return;

      var ss = GetStrokeStyle(strokeStyle);

      var x = bounds.X;
      var a = BarcodeCode39.Encode(v).ToList();
      var w = a.Sum();
      var s = bounds.Width / w;
      var i = 0;
      foreach (var q in a)
      {
        Target.DrawLine(
          new DXM.RawVector2(x, rect.Top),
          new DXM.RawVector2(x, rect.Bottom),
          i % 2 == 0 ? strokeBrush : fillBrush,
          (q * s),
          ss
        );
        i++;
        x += (q * s);
      }
    }
    public void Dispose()
    {
      if (!_isDisposed) Dispose(true);
    }

    private bool _isDisposed;

    public void Dispose(bool dispose)
    {
      if (_isDisposed) return;
      _isDisposed = true;
      foreach (var d in _renderStreamCache.Values)
        if (d is IDisposable x) x.Dispose();
      _renderStreamCache.Clear();
      _disposeManager?.Dispose();
    }
  }
}

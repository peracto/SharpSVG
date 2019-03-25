﻿using Peracto.Svg.Brush;
using Peracto.Svg.Render.Dx.Utility;
using Peracto.Svg.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using D2D1 = SharpDX.Direct2D1;
using DX = SharpDX.Mathematics.Interop;
using LinearGradientBrush = Peracto.Svg.Brush.LinearGradientBrush;
using RadialGradientBrush = Peracto.Svg.Brush.RadialGradientBrush;
using SolidColorBrush = Peracto.Svg.Brush.SolidColorBrush;

namespace Peracto.Svg.Render.Dx.Brush
{
  public class BrushManager : IBrushVisitor<D2D1.Brush>, IDisposable
  {
    private readonly IDictionary<string, D2D1.Brush> _brushes = new Dictionary<string, D2D1.Brush>();
    private readonly D2D1.RenderTarget _dc;

    public BrushManager(D2D1.RenderTarget dc)
    {
      _dc = dc;
    }

    public void Dispose()
    {
      foreach (var b in _brushes) b.Value.Dispose();
      _brushes.Clear();
    }

    public D2D1.Brush CreateSolidBrush(IElement element, IFrameContext context, float opacity, SolidColorBrush brush)
    {
      var tag = brush.Color.AddOpacity(opacity).GetTag();
      if (_brushes.TryGetValue(tag, out var dxBrush)) return dxBrush;

      dxBrush = new D2D1.SolidColorBrush(
        _dc,
        brush.Color.ToDx4(),
        opacity >= 1
          ? new D2D1.BrushProperties?()
          : new D2D1.BrushProperties() {Opacity = opacity}
      );

      _brushes.Add(tag, dxBrush);
      return dxBrush;
    }

    public D2D1.Brush CreateLinearGradientBrush(IElement element, IFrameContext context, float opacity, LinearGradientBrush brush)
    {
      var tag = brush.GradientUnits == GradientUnits.UserSpaceOnUse ? $"{brush.Tag}::{context.LayerId}" : null;
      if (tag != null && _brushes.TryGetValue(tag, out var dxBrush)) return dxBrush;

      var renderSize = Path.ClipPathBuilder.GetBounds(element, context);

      //TODO: Should rendersize relate to the frame if it's in object space?
      var x1 = brush.X1.Resolve(element, context, renderSize);
      var y1 = brush.Y1.Resolve(element, context, renderSize);
      var x2 = brush.X2.Resolve(element, context, renderSize);
      var y2 = brush.Y2.Resolve(element, context, renderSize);

      var dx = x2 - x1;
      var dy = y2 - y1;
      var length = (float) Math.Sqrt((dx * dx) + (dy * dy));

      var gsc = (from stop in brush.Stops
        select new D2D1.GradientStop()
        {
          Color = ResolveSolidColour(stop.Colour.Create(element), stop.Opacity),
          Position = ResolvePercent(stop.Offset, length)
        }).ToArray();

      dxBrush = new D2D1.LinearGradientBrush(
        _dc,
        new D2D1.LinearGradientBrushProperties()
        {
          StartPoint = new DX.RawVector2(x1+renderSize.X, y1+renderSize.Y),
          EndPoint = new DX.RawVector2(x2+renderSize.X, y2+renderSize.Y)
        },
        new D2D1.GradientStopCollection(_dc, gsc, D2D1.Gamma.StandardRgb, D2D1.ExtendMode.Clamp)
      );

      if (opacity < 1) dxBrush.Opacity = opacity;
      if (brush.GradientTransform != null) dxBrush.Transform = brush.GradientTransform.ToDx();

      if(tag!=null) _brushes.Add(tag, dxBrush);
      return dxBrush;
    }

    public D2D1.Brush CreateRadialGradientBrush(IElement element, IFrameContext context, float opacity, RadialGradientBrush brush)
    {
      var tag = brush.GradientUnits == GradientUnits.UserSpaceOnUse ? $"{brush.Tag}::{context.LayerId}" : null;
      if (tag != null && _brushes.TryGetValue(tag, out var dxBrush)) return dxBrush;

      var renderSize = Path.ClipPathBuilder.GetBounds(element, context);
     // var size = (brush.GradientUnits == GradientUnits.ObjectBoundingBox) ? renderSize.Size : context.Size;

      var cx = brush.Cx.Resolve(element, context, renderSize);
      var cy = brush.Cy.Resolve(element, context, renderSize);
      var fx = brush.Fx.Resolve(element, context, renderSize);
      var fy = brush.Fy.Resolve(element, context, renderSize);
      var rx = brush.R.Resolve(element, context, renderSize);

      var gsc = (
        from stop in brush.Stops
        select new D2D1.GradientStop()
        {
          Color = ResolveSolidColour(stop.Colour.Create(element), stop.Opacity),
          Position = ResolvePercent(stop.Offset, rx)
        });

      dxBrush = new D2D1.RadialGradientBrush(
        _dc,
        new D2D1.RadialGradientBrushProperties()
        {
          Center = new DX.RawVector2(cx + renderSize.X, cy + renderSize.Y),
          GradientOriginOffset = new DX.RawVector2(fx - cx, fy - cy),
          RadiusX = rx,
          RadiusY = rx
        },
        new D2D1.GradientStopCollection(
          _dc, 
          gsc.ToArray(), 
          D2D1.Gamma.StandardRgb, 
          D2D1.ExtendMode.Clamp
          )
      );

      if (opacity < 1) dxBrush.Opacity = opacity;
      if (brush.GradientTransform != null) dxBrush.Transform = brush.GradientTransform.ToDx();

      if (tag != null) _brushes.Add(tag, dxBrush);
      return dxBrush;
    }


    private DX.RawColor4 ResolveSolidColour(IBrush brush, float opacity)
    {
      return ((brush is SolidColorBrush sc) ? sc.Color : PxColors.Black).ToDx(opacity);
    }

    private float ResolvePercent(Percent value, float length)
    {
      return value.Unit == PercentUnit.Percent ? (((value.Value * length)/100)/length) : value.Value;
    }
  }
}
using System;
using System.Linq;
using Peracto.Svg.Types;
using AA = Peracto.Svg.Accessor.AttributeAccessors;

namespace Peracto.Svg.Brush
{
  public class UriBrushFactory : IBrushFactory
  {
    private readonly Uri _uri;
    private readonly IBrushFactory _fallback;

    public UriBrushFactory(Uri uri, IBrushFactory fallback)
    {
      _uri = uri;
      _fallback = fallback;
    }

    IBrush IBrushFactory.Create(IElement context)
    {
      var tag = _uri.Fragment.Substring(1);

      if (context.OwnerDocument.GetResource("BRUSH::" + tag) is IBrush brush)
        return brush;

      brush = CreateNew(context, tag);
      context.OwnerDocument.SetResource("BRUSH::" + tag, brush);
      return brush;
    }

    private IBrush CreateNew(IElement context, string tag)
    {
      var element = context.OwnerDocument.GetElementById(tag);

      return element == null
        ? _fallback?.Create(context)
        : element.ElementType == "linearGradient"
        ? CreateLinearGradient(tag, context, element)
        : element.ElementType == "radialGradient"
        ? CreateRadialGradient(tag, context, element)
        : null;
    }

    private IElement ResolveHref(IElement element)
    {
      var href = AA.Href.GetValue(element)?.Resolve(element);
      if (href == null) return element;
      if (href.Scheme != "internal")
      {
        Console.WriteLine("Cant reference external document");
        return element;
      }

      return element.OwnerDocument.GetElementById(href.Fragment.Substring(1));
    }

    public IBrush CreateLinearGradient(string tag, IElement contextElement, IElement element)
    {
      var stops = ResolveHref(element);

      return new LinearGradientBrush(
        tag,
        AA.GradientUnits.GetValue(element),
        AA.GradientTransform.GetValue(element),
        AA.SpreadMethod.GetValue(element),
        AA.X1.GetValue(element),
        AA.Y1.GetValue(element),
        AA.X2.TryGetValue(element, out var x1) ? x1 : Measure.HundredPercentH,
        AA.Y2.GetValue(element),
        stops
          .ChildrenOfType("stop")
          .Select(stop => new Stop(
            AA.StopColor.GetValue(stop),
            AA.Offset.GetValue(stop),
            AA.StopOpacity.GetValue(stop).AsCheckedNumber()
          ))
          .ToArray());
    }

    public IBrush CreateRadialGradient(string tag, IElement contextElement, IElement element)
    {
      var stops = ResolveHref(element);

      return new RadialGradientBrush(
        tag,
        AA.GradientUnits.GetValue(element),
        AA.GradientTransform.GetValue(element),
        AA.SpreadMethod.GetValue(element),
        AA.Cx.TryGetValue(element, out var cx) ? cx : Measure.FiftyPercentH,
        AA.Cy.TryGetValue(element, out var cy) ? cy : Measure.FiftyPercentH,
        AA.R.TryGetValue(element, out var r) ? r : Measure.FiftyPercentH,
        AA.Fx.TryGetValue(element, out var fx) ? fx : cx,
        AA.Fy.TryGetValue(element, out var fy) ? fy : cy,
        stops
          .ChildrenOfType("stop")
          .Select(stop => new Stop(
            AA.StopColor.GetValue(stop),
            AA.Offset.GetValue(stop),
            AA.StopOpacity.GetValue(stop).AsCheckedNumber()
          ))
          .ToArray());
    }
  }
}
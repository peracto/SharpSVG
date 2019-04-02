using Peracto.Svg.Accessor;
using Peracto.Svg.Brush;
using Peracto.Svg.Clipping;
using Peracto.Svg.Image;
using Peracto.Svg.Paths;
using Peracto.Svg.Text;
using Peracto.Svg.Transform;
using Peracto.Svg.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using AA = Peracto.Svg.Accessor.AttributeAccessors;

namespace Peracto.Svg
{

  public static class ElementHelpers
  {
    public static IEnumerable<IElement> Descendants(this IElement element, string elementType = null)
    {
      var stack = new Stack<IEnumerator<IElement>>();
      var current = element.Children.GetEnumerator();

      if (elementType == null || element.ElementType == elementType) yield return element;

      while (true)
      {
        while (current.MoveNext())
        {
          var e = current.Current;
          if (e == null) continue;
          if (elementType == null || e.ElementType == elementType)
            yield return e;
          var c = e.Children;
          if (c.Count == 0) continue;
          stack.Push(current);
          current = c.GetEnumerator();
        }

        if (stack.Count == 0) break;
        current = stack.Pop();
      }
    }

    public static IElement GetElementById(this IElement element, string id)
    {
      return element?.OwnerDocument?.Descendants().FirstOrDefault(e => e.Id == id);
    }


    public static IEnumerable<IElement> ChildrenOfType(this IElement element, string elementName)
    {
      return element.Children.Where(child => child.ElementType == elementName);
    }

    public static Font GetFont(this IElement element, IFrameContext context)
    {
      return new Font(
        AA.FontFamily.GetValue(element),
        GetFontSize(element, context),
        AA.FontWeight.GetValue(element),
        AA.TextAnchor.GetValue(element),
        AA.TextDecoration.GetValue(element),
        AA.DominantBaseline.GetValue(element),
        AA.FontStretch.GetValue(element),
        AA.FontStyle.GetValue(element)
      );
    }

    private static T TryGetDefault<T>(this IAccessor<T> accessor, IElement element, T defaultValue)
    {
      return accessor.TryGetValue(element, out var value) ? value : defaultValue;
    }

    public static IEnumerable<TextEntry> GetText(this IElement element, IFrameContext context)
    {
      return Text.Text.Create(element, context);
    }

    public static Font GetFont(this IElement element, IFrameContext context, Font font)
    {
      if (font == null) return GetFont(element, context);

      var fontFamily = AA.FontFamily.TryGetDefault(element, font.Family);
      var fontSize = AA.FontSize.TryGetValue(element, out var fontSizeX) ? ComputeFontSize(element, context, fontSizeX): font.Size;
      var fontWeight = AA.FontWeight.TryGetDefault(element, font.Weight);
      var textAnchor = AA.TextAnchor.TryGetDefault(element, font.TextAnchor);
      var textDecoration = AA.TextDecoration.TryGetDefault(element, font.TextDecoration);
      var dominantBaseline = AA.DominantBaseline.TryGetDefault(element, font.DominantBaseline);
      var fontStretch = AA.FontStretch.TryGetDefault(element, font.Stretch);
      var style = AA.FontStyle.TryGetDefault(element, font.Style);

      if (font.Family == fontFamily &&
          font.Size == fontSize &&
          font.Weight == fontWeight &&
          font.TextAnchor == textAnchor &&
          font.TextDecoration == textDecoration &&
          font.DominantBaseline == dominantBaseline &&
          font.Stretch == fontStretch &&
          font.Style == style)
        return font;
      else return new Font(
        fontFamily,
        fontSize,
        fontWeight,
        textAnchor,
        textDecoration,
        dominantBaseline,
        fontStretch,
        style
        );
    }

    private static float ComputeFontSize(IElement element, IFrameContext context,Measure fontSize)
    {
      // ReSharper disable once SwitchStatementMissingSomeCases
      switch (fontSize.Unit)
      {
        case MeasureUnit.Em:
          return fontSize.Value * GetFontSize(element.Parent, context);
        case MeasureUnit.Ex:
          return (fontSize.Value * GetFontSize(element.Parent, context)) / 2;
        case MeasureUnit.Percentage:
          return (fontSize.Value * GetFontSize(element.Parent, context)) / 100;
        default:
          return context.ToDeviceValue(fontSize);
      }
    }


    public static ClipPathUnits GetClipPathUnits(this IElement element)
    {
      return AA.ClipPathUnits.GetValue(element);
    }

    public static float GetFontSize(this IElement element, IFrameContext context)
    {
      return ComputeFontSize(element, context, AA.FontSize.GetValue(element));
    }

    public static Uri GetHref(this IElement element)
    {
      return AA.Href.GetValue(element)?.Resolve(element);
    }

    public static IClip GetClipPath(this IElement element)
    {
      return AA.ClipPath.GetValue(element)?.Create(element);
    }

    public static Path GetPath(this IElement element)
    {
      return AA.D.GetValue(element);
    }

    public static PxPoint[] GetPoints(this IElement element)
    {
      return AA.Points.GetValue(element);
    }
   
    public static PxPoint GetCxCy(this IElement element, IFrameContext context)
    {
      return new PxPoint(
        AA.Cx.GetMeasure(element, context),
        AA.Cy.GetMeasure(element, context)
      );
    }

    public static PxPoint GetRxRy(this IElement element, IFrameContext context)
    {
      return new PxPoint(
        AA.Rx.GetMeasure(element, context),
        AA.Ry.GetMeasure(element, context)
      );
    }

    public static PxPoint GetX1Y1(this IElement element, IFrameContext context)
    {
      return new PxPoint(
        AA.X1.GetMeasure(element, context),
        AA.Y1.GetMeasure(element, context)
      );
    }

    public static PxPoint GetX2Y2(this IElement element, IFrameContext context)
    {
      return new PxPoint(
        AA.X2.GetMeasure(element, context),
        AA.Y2.GetMeasure(element, context)
      );
    }

    public static float? GetX(this IElement element, IFrameContext context)
    {
      return GetNullableMeasure(AA.X, element, context);
    }

    public static float? GetY(this IElement element, IFrameContext context)
    {
      return GetNullableMeasure(AA.Y, element, context);
    }

    public static float GetDx(this IElement element, IFrameContext context)
    {
      return AA.Dx.GetMeasure(element, context);
    }

    public static float GetDy(this IElement element, IFrameContext context)
    {
      return AA.Dy.GetMeasure(element, context);
    }

    public static PxRectangle GetBounds(this IElement element, IFrameContext context)
    {
      return new PxRectangle(
        AA.X.GetMeasure(element, context),
        AA.Y.GetMeasure(element, context),
        AA.Width.GetMeasure(element, context),
        AA.Height.GetMeasure(element, context)
      );
    }

    public static ViewBox GetViewBox(this IElement element)
    {
      return AA.ViewBox.GetValue(element);
    }

    public static PreserveAspectRatio GetPreserveAspectRatio(this IElement element)
    {
      return AA.PreserveAspectRatio.GetValue(element);
    }

    public static ITransform GetTransform(this IElement element)
    {
      return AA.Transform.GetValue(element);
    }

    public static PxSize GetSize(this IElement element, IFrameContext context, PxSize defaultSize)
    {
      return new PxSize(
        AA.Width.GetMeasure(element, context,defaultSize.Width),
        AA.Height.GetMeasure(element, context,defaultSize.Height)
      );
    }

    public static PxPoint GetPosition(this IElement element, IFrameContext context)
    {
      return new PxPoint(
        AA.X.GetMeasure(element, context),
        AA.Y.GetMeasure(element, context)
      );
    }

    public static PxMatrix GetPositionMatrix(this IElement element, IFrameContext context)
    {
      return PxMatrix.Translate(
        AA.X.GetMeasure(element, context),
        AA.Y.GetMeasure(element, context)
      );
    }

    public static float GetRadius(this IElement element, IFrameContext context)
    {
      return AA.R.GetMeasure(element, context);
    }
  
    public static float GetOpacity(this IElement element)
    {
      return AA.Opacity.TryGetValue(element, out var measure)
        ? measure.AsCheckedNumber()
        : 1.0f;
    }

    public static Fill GetFill(this IElement element, IFrameContext context)
    {
      return new Fill(
        AA.Fill.GetValue(element)?.Create(element),
        AA.FillOpacity.GetValue(element).AsCheckedNumber()
        );
    }

    public static Stroke GetStroke(this IElement element, IFrameContext context)
    {
      try
      {
        return new Stroke(
          AA.Stroke.GetValue(element)?.Create(element),
          AA.StrokeWidth.GetValue(element).Resolve(element, context),
          AA.StrokeOpacity.GetValue(element).AsCheckedNumber(),
          AA.StrokeLineCap.GetValue(element),
          AA.StrokeLineJoin.GetValue(element),
          AA.StrokeMiterLimit.GetValue(element),
          AA.StrokeDashOffset.GetValue(element).Resolve(element, context),
          AA.StrokeDashArray.GetValue(element)?.Select(context.ToDeviceValue).ToArray()
        );
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    public static float Resolve(this Percent measure, float something)
    {
      return measure.Unit == PercentUnit.Percent 
        ? (something * measure.Value) / 100 
        : measure.Value;
    }

    public static float Resolve(this Measure measure, IElement element, IFrameContext context)
    {
      // ReSharper disable once SwitchStatementMissingSomeCases
      switch (measure.Unit)
      {
        case MeasureUnit.Em:
          return measure.Value * element.GetFontSize(context);
        case MeasureUnit.Ex:
          return (measure.Value * element.GetFontSize(context)) / 2;
        default:
          return context.ToDeviceValue(measure);
      }
    }

    private static float ResolvePercentage(float value, ref PxRectangle size, MeasureUsage usage)
    {
      // ReSharper disable once SwitchStatementMissingSomeCases
      switch (usage)
      {
        case MeasureUsage.Horizontal:
          return (size.Width * value) / 100;
        case MeasureUsage.Vertical:
          return (size.Height * value) / 100;
        default:
          return 0.0f;
      }
    }

    public static float Resolve(this Measure measure, IElement element, IFrameContext context, PxRectangle size,
      bool TreatUserAsPercentage = false)
    {
      // ReSharper disable once SwitchStatementMissingSomeCases
      switch (measure.Unit)
      {
        case MeasureUnit.Em:
          return measure.Value * element.GetFontSize(context);
        case MeasureUnit.Ex:
          return (measure.Value * element.GetFontSize(context)) / 2;
        case MeasureUnit.User:
          return TreatUserAsPercentage
            ? ResolvePercentage(measure.Value * 100, ref size, measure.Usage)
            : context.ToDeviceValue(measure);
        case MeasureUnit.Percentage:
          return ResolvePercentage(measure.Value, ref size, measure.Usage);
        default:
          return context.ToDeviceValue(measure);
      }
    }

    private static float GetMeasure(this IAccessor<Measure> accessor, IElement element, IFrameContext context,
      float defaultValue = 0)
    {
      return accessor.TryGetValue(element, out var measure)
        ? measure.Resolve(element, context)
        : defaultValue;
    }

    private static float? GetNullableMeasure(this IAccessor<Measure> accessor, IElement element, IFrameContext context)
    {
      return accessor.TryGetValue(element, out var measure)
        ? measure.Resolve(element, context)
        : new float?();
    }
  }
}
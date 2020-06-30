using Peracto.Svg.Brush;
using Peracto.Svg.Converters;
using Peracto.Svg.Text;
using Peracto.Svg.Types;

// ReSharper disable UnusedMember.Global
// ReSharper disable StringLiteralTypo

namespace Peracto.Svg.Parser
{
  public static class AttributeDefinitions
  {
    public static readonly TokenAttributeConverter Id = new TokenAttributeConverter("id");
    public static readonly TokenAttributeConverter PxDebug = new TokenAttributeConverter("pxdebug");
    public static readonly MeasureAttributeConverter X = MeasureAttributeConverter.CreateH("x");
    public static readonly MeasureAttributeConverter TextLength = MeasureAttributeConverter.CreateV("textLength");
    public static readonly MeasureAttributeConverter Dx = MeasureAttributeConverter.CreateH("dx");
    public static readonly MeasureAttributeConverter Dy = MeasureAttributeConverter.CreateV("dy");
    public static readonly MeasureAttributeConverter Fx = MeasureAttributeConverter.CreateH("fx");
    public static readonly MeasureAttributeConverter Fy = MeasureAttributeConverter.CreateV("fy");
    public static readonly MeasureAttributeConverter Fr = MeasureAttributeConverter.CreateV("fr");
    public static readonly MeasureAttributeConverter Rx = MeasureAttributeConverter.CreateH("rx");
    public static readonly MeasureAttributeConverter Ry = MeasureAttributeConverter.CreateV("ry");
    public static readonly MeasureAttributeConverter Cx = MeasureAttributeConverter.CreateH("cx");
    public static readonly MeasureAttributeConverter Cy = MeasureAttributeConverter.CreateV("cy");
    public static readonly MeasureAttributeConverter R = MeasureAttributeConverter.CreateH("r");
    public static readonly MeasureAttributeConverter X1 = MeasureAttributeConverter.CreateH("x1");
    public static readonly MeasureAttributeConverter X2 = MeasureAttributeConverter.CreateH("x2");
    public static readonly MeasureAttributeConverter Y1 = MeasureAttributeConverter.CreateV("y1");
    public static readonly MeasureAttributeConverter Y2 = MeasureAttributeConverter.CreateV("y2");
    public static readonly MeasureAttributeConverter Y = MeasureAttributeConverter.CreateV("y");
    public static readonly MeasureAttributeConverter Width = MeasureAttributeConverter.CreateH("width");
    public static readonly MeasureAttributeConverter Height = MeasureAttributeConverter.CreateV("height");
    public static readonly MeasureAttributeConverter StrokeWidth = MeasureAttributeConverter.CreateH("stroke-width");
    public static readonly PaintServerAttributeConverter Stroke = new PaintServerAttributeConverter("stroke");
    public static readonly PaintServerAttributeConverter Color = new PaintServerAttributeConverter("color");
    public static readonly PaintServerAttributeConverter Fill = new PaintServerAttributeConverter("fill");
    public static readonly PaintServerAttributeConverter StopColor = new PaintServerAttributeConverter("stop-color", false);
    public static readonly ClipPathAttributeConverter ClipPath = new ClipPathAttributeConverter("clip-path");
    public static readonly HrefAttributeConverter Href = new HrefAttributeConverter("href");
    public static readonly PercentageAttributeConverter Offset = new PercentageAttributeConverter("offset");
    public static readonly PercentageAttributeConverter FillOpacity = new PercentageAttributeConverter("fill-opacity");
    public static readonly PercentageAttributeConverter Opacity = new PercentageAttributeConverter("opacity");
    public static readonly PercentageAttributeConverter StrokeOpacity = new PercentageAttributeConverter("stroke-opacity");
    public static readonly PercentageAttributeConverter StopOpacity = new PercentageAttributeConverter("stop-opacity");
    public static readonly MeasureListAttributeConverter StrokeDashArray = new MeasureListAttributeConverter("stroke-dasharray");
    public static readonly MeasureAttributeConverter StrokeDashOffset = MeasureAttributeConverter.CreateH("stroke-dashoffset");
    public static readonly GenericEnumConverter<LineCap> StrokeLineCap = new GenericEnumConverter<LineCap>("stroke-linecap");
    public static readonly GenericEnumConverter<LineJoin> StrokeLineJoin = new GenericEnumConverter<LineJoin>("stroke-linejoin");
    public static readonly NumberAttributeConverter StrokeMiterLimit = new NumberAttributeConverter("stroke-miterlimit");
    public static readonly GenericEnumConverter<SpreadMethod> SpreadMethod = new GenericEnumConverter<SpreadMethod>("spreadMethod");
    public static readonly PreserveAspectRatioConverter PreserveAspectRatio = new PreserveAspectRatioConverter("preserveaspectratio");
  //  public static readonly StyleAttributeConverter StyleAttribute = new StyleAttributeConverter("style");
    public static readonly TokenListAttributeConverter Class = new TokenListAttributeConverter("class", @"[\s,]+");
    public static readonly TransformAttributeConverter TransformAttribute = new TransformAttributeConverter("transform");
    public static readonly TransformOriginAttributeConverter TransformOriginAttribute = new TransformOriginAttributeConverter("transform-origin");
    public static readonly TransformAttributeConverter GradientTransformAttribute = new TransformAttributeConverter("gradientTransform");
    public static readonly ViewBoxAttributeConverter ViewBoxAttribute = new ViewBoxAttributeConverter("viewbox");
    public static readonly PathAttributeConverter D = new PathAttributeConverter("d");
    public static readonly PointsAttributeConverter PointsAttribute = new PointsAttributeConverter("points");
    public static readonly TokenAttributeConverter TextTransform = new TokenAttributeConverter("text-transform");
    public static readonly TokenAttributeConverter WordSpacing = new TokenAttributeConverter("word-spacing");
    public static readonly TokenAttributeConverter LetterSpacing = new TokenAttributeConverter("letter-spacing");
    public static readonly TokenListAttributeConverter FontFamily = new TokenListAttributeConverter("font-family", @"\s*[,]\s*");
    public static readonly MeasureAttributeConverter FontSize = new MeasureAttributeConverter("font-size", MeasureUsage.Vertical);
    public static readonly GenericEnumConverter<GradientUnits> GradientUnits = new GenericEnumConverter<GradientUnits>("gradientUnits");
    public static readonly GenericEnumConverter<ClipPathUnits> ClipPathUnits = new GenericEnumConverter<ClipPathUnits>("clipPathUnits");
    public static readonly GenericEnumConverter<FontWeight> FontWeight = new GenericEnumConverter<FontWeight>("font-weight");
    public static readonly GenericEnumConverter<FontStyle> FontStyle = new GenericEnumConverter<FontStyle>("font-style");
    public static readonly GenericEnumConverter<FontStretch> FontStretch = new GenericEnumConverter<FontStretch>("font-stretch");
    public static readonly GenericEnumConverter<DominantBaseline> DominantBaseline = new GenericEnumConverter<DominantBaseline>("dominant-baseline");
    public static readonly GenericEnumConverter<TextAnchor> TextAnchor = new GenericEnumConverter<TextAnchor>("text-anchor");
    public static readonly GenericEnumConverter<TextDecoration> TextDecoration = new GenericEnumConverter<TextDecoration>("text-decoration");
  }
}


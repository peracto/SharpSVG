using Peracto.Svg.Brush;
using Peracto.Svg.Clipping;
using Peracto.Svg.Image;
using Peracto.Svg.Paths;
using Peracto.Svg.Text;
using Peracto.Svg.Transform;
using Peracto.Svg.Types;

// ReSharper disable StringLiteralTypo

namespace Peracto.Svg.Accessor
{
  public class AttributeAccessors
  {
    private static readonly string[] EmptyFont = new[] { "Arial", "Calibri" };
    public static readonly IAccessor<string> Id = new AttributeAccessor<string>("id");
    public static readonly IAccessor<Measure> X = new AttributeAccessor<Measure>("x", Measure.ZeroH);
    public static readonly IAccessor<Measure> Dx = new AttributeAccessor<Measure>("dx", Measure.ZeroH);
    public static readonly IAccessor<Measure> Dy = new AttributeAccessor<Measure>("dy", Measure.ZeroV);
    public static readonly IAccessor<Measure> Fr = new AttributeAccessor<Measure>("fr", Measure.ZeroH);
    public static readonly IAccessor<Measure> Fx = new AttributeAccessor<Measure>("fx", Measure.FiftyPercentH);
    public static readonly IAccessor<Measure> Fy = new AttributeAccessor<Measure>("fy", Measure.FiftyPercentV);
    public static readonly IAccessor<Measure> Rx = new AttributeAccessor<Measure>("rx", Measure.ZeroH);
    public static readonly IAccessor<Measure> Ry = new AttributeAccessor<Measure>("ry", Measure.ZeroV);
    public static readonly IAccessor<Measure> Cx = new AttributeAccessor<Measure>("cx", Measure.ZeroH);
    public static readonly IAccessor<Measure> Cy = new AttributeAccessor<Measure>("cy", Measure.ZeroV);
    public static readonly IAccessor<Measure> R = new AttributeAccessor<Measure>("r", Measure.ZeroH);
    public static readonly IAccessor<Measure> X1 = new AttributeAccessor<Measure>("x1", Measure.ZeroH);
    public static readonly IAccessor<Measure> X2 = new AttributeAccessor<Measure>("x2", Measure.ZeroH);
    public static readonly IAccessor<Measure> Y1 = new AttributeAccessor<Measure>("y1", Measure.ZeroV);
    public static readonly IAccessor<Measure> Y2 = new AttributeAccessor<Measure>("y2", Measure.ZeroV);
    public static readonly IAccessor<Measure> Y = new AttributeAccessor<Measure>("y", Measure.ZeroV);
    public static readonly IAccessor<Measure> Width = new AttributeAccessor<Measure>("width", Measure.ZeroH);
    public static readonly IAccessor<Measure> TextLength = new AttributeAccessor<Measure>("textLength", Measure.ZeroH);

    public static readonly IAccessor<Measure> Height = new AttributeAccessor<Measure>("height", Measure.ZeroV);

    public static readonly IAccessor<Measure> StrokeWidth = new AttributeAccessor<Measure>("stroke-width", Measure.One, true);

    public static readonly IAccessor<Percent> Offset = new AttributeAccessor<Percent>("offset", Percent.Zero);

    public static readonly IAccessor<Percent> FillOpacity = new AttributeAccessor<Percent>("fill-opacity", Percent.Hundred);

    public static readonly IAccessor<Percent> Opacity = new AttributeAccessor<Percent>("opacity", Percent.Hundred);
    public static readonly IAccessor<Percent> StopOpacity = new AttributeAccessor<Percent>("stop-opacity", Percent.Hundred);

    public static readonly IAccessor<Percent> StrokeOpacity = new AttributeAccessor<Percent>("stroke-opacity", Percent.Hundred, true);

    public static readonly IAccessor<Measure> FontSize = new AttributeAccessor<Measure>("font-size", Measure.TwelvePoint, true);

    public static readonly IAccessor<IBrushFactory> Stroke = new AttributeAccessor<IBrushFactory>("stroke", NoneColorBrushFactory.Instance, true);
    public static readonly IAccessor<IClipPathFactory> ClipPath = new AttributeAccessor<IClipPathFactory>("clip-path", null, true);

    public static readonly IAccessor<IBrushFactory> Fill = new AttributeAccessor<IBrushFactory>("fill", SolidColourBrushFactory.Black, true);
    public static readonly IAccessor<IBrushFactory> Color = new AttributeAccessor<IBrushFactory>("color", SolidColourBrushFactory.Black);
    public static readonly IAccessor<HrefServer> Href = new AttributeAccessor<HrefServer>("href");
    public static readonly IAccessor<Measure[]> StrokeDashArray = new AttributeAccessor<Measure[]>("stroke-dasharray");

    public static readonly IAccessor<PreserveAspectRatio> PreserveAspectRatio = new AttributeAccessor<PreserveAspectRatio>("preserveaspectratio", Image.PreserveAspectRatio.XMidYMidMeet);

    public static readonly IAccessor<object> Style = new AttributeAccessor<object>("style");
    public static readonly IAccessor<IBrushFactory> StopColor = new AttributeAccessor<IBrushFactory>("stop-color");
    public static readonly IAccessor<string[]> Class = new AttributeAccessor<string[]>("class");
    public static readonly IAccessor<string[]> FontFamily = new AttributeAccessor<string[]>("font-family", EmptyFont, true);

    public static readonly IAccessor<ITransform> Transform = new AttributeAccessor<ITransform>("transform");

    public static readonly IAccessor<ITransform> GradientTransform = new AttributeAccessor<ITransform>("gradientTransform");

    public static readonly IAccessor<Percent> StrokeDashOffset = new AttributeAccessor<Percent>("stroke-dashoffset", Percent.Zero);

    public static readonly IAccessor<SpreadMethod> SpreadMethod = new AttributeAccessor<SpreadMethod>("spreadMethod", Brush.SpreadMethod.Pad);
    public static readonly IAccessor<GradientUnits> GradientUnits = new AttributeAccessor<GradientUnits>("gradientUnits", Brush.GradientUnits.ObjectBoundingBox);
    public static readonly IAccessor<LineCap> StrokeLineCap = new AttributeAccessor<LineCap>("stroke-linecap", LineCap.Butt, true);
    public static readonly IAccessor<LineJoin> StrokeLineJoin = new AttributeAccessor<LineJoin>("stroke-linejoin", LineJoin.Miter, true);
    public static readonly IAccessor<float> StrokeMiterLimit = new AttributeAccessor<float>("stroke-miterlimit", 4, true);

    public static readonly IAccessor<ViewBox> ViewBox = new AttributeAccessor<ViewBox>("viewbox");
    public static readonly IAccessor<Path> D = new AttributeAccessor<Path>("d");
    public static readonly IAccessor<PxPoint[]> Points = new AttributeAccessor<PxPoint[]>("points");
    public static readonly IAccessor<string> TextTransform = new AttributeAccessor<string>("text-transform");

    public static readonly IAccessor<ClipPathUnits> ClipPathUnits = new AttributeAccessor<ClipPathUnits>("clipPathUnits",Brush.ClipPathUnits.UserSpaceOnUse);

    public static readonly IAccessor<string> WordSpacing = new AttributeAccessor<string>("word-spacing");
    public static readonly IAccessor<string> LetterSpacing = new AttributeAccessor<string>("letter-spacing");
    public static readonly IAccessor<FontWeight> FontWeight = new EnumAttributeAccessor<FontWeight>("font-weight", Text.FontWeight.Normal,Text.FontWeight.Inherit,true);
    public static readonly IAccessor<FontStretch> FontStretch = new EnumAttributeAccessor<FontStretch>("font-stretch", Text.FontStretch.Normal,Text.FontStretch.Inherit);
    public static readonly IAccessor<DominantBaseline> DominantBaseline = new EnumAttributeAccessor<DominantBaseline>("dominant-baseline", Text.DominantBaseline.Auto, Text.DominantBaseline.Inherit, true);
    public static readonly IAccessor<TextAnchor> TextAnchor = new EnumAttributeAccessor<TextAnchor>("text-anchor", Text.TextAnchor.Inherit,Text.TextAnchor.Inherit);
    public static readonly IAccessor<TextDecoration> TextDecoration = new EnumAttributeAccessor<TextDecoration>("text-decoration", Text.TextDecoration.Inherit, Text.TextDecoration.Inherit);
    public static readonly IAccessor<FontStyle> FontStyle = new EnumAttributeAccessor<FontStyle>("font-style", Text.FontStyle.Normal, Text.FontStyle.Inherit);
  }
}

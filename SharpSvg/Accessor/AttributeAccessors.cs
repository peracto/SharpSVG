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
    private static IAccessor<T> Create<T>(string name, T defaultValue = default(T), bool inherit = false)
    {
      return new AttributeAccessor<T>(name,defaultValue,inherit);
    }
    private static IAccessor<T> CreateEnum<T>(string name, T defaultValue, T inheritValue, bool inherit = false) where T: struct
    {
      return new EnumAttributeAccessor<T>(name, defaultValue, inheritValue, inherit);
    }
    private static IAccessor<T> CreateEnum<T>(string name, T defaultValue, bool inherit = false)
    {
      return new EnumAttributeAccessor<T>(name, defaultValue, inherit);
    }

    private static readonly string[] EmptyFont = new[] {"Arial", "Calibri"};
    public static readonly IAccessor<string[]> Class = Create<string[]>("class");
    public static readonly IAccessor<ClipPathUnits> ClipPathUnits = CreateEnum("clipPathUnits", Brush.ClipPathUnits.UserSpaceOnUse);
    public static readonly IAccessor<Measure> Cx = Create("cx", Measure.ZeroH);
    public static readonly IAccessor<Measure> Cy = Create("cy", Measure.ZeroV);
    public static readonly IAccessor<Path> D = Create<Path>("d");
    public static readonly IAccessor<Measure> Dx = Create("dx", Measure.ZeroH);
    public static readonly IAccessor<Measure> Dy = Create("dy", Measure.ZeroV);
    public static readonly IAccessor<Measure> Fr = Create("fr", Measure.ZeroH);
    public static readonly IAccessor<Measure> Fx = Create("fx", Measure.FiftyPercentH);
    public static readonly IAccessor<Measure> Fy = Create("fy", Measure.FiftyPercentV);
    public static readonly IAccessor<ITransform> GradientTransform = Create<ITransform>("gradientTransform");
    public static readonly IAccessor<GradientUnits> GradientUnits = CreateEnum("gradientUnits", Brush.GradientUnits.ObjectBoundingBox);
    public static readonly IAccessor<Measure> Height = Create("height", Measure.ZeroV);
    public static readonly IAccessor<HrefServer> Href = Create<HrefServer>("href");
    public static readonly IAccessor<string> Id = Create<string>("id");
    public static readonly IAccessor<Percent> Offset = Create("offset", Percent.Zero);
    public static readonly IAccessor<PxPoint[]> Points = Create<PxPoint[]>("points");
    public static readonly IAccessor<PreserveAspectRatio> PreserveAspectRatio = Create("preserveaspectratio", Image.PreserveAspectRatio.XMidYMidMeet);
    public static readonly IAccessor<Measure> R = Create("r", Measure.ZeroH);
    public static readonly IAccessor<Measure> Rx = Create("rx", Measure.ZeroH);
    public static readonly IAccessor<Measure> Ry = Create("ry", Measure.ZeroV);
    public static readonly IAccessor<SpreadMethod> SpreadMethod = Create("spreadMethod", Brush.SpreadMethod.Pad);
    public static readonly IAccessor<object> Style = Create<object>("style");
    public static readonly IAccessor<Measure> TextLength = Create("textLength", Measure.ZeroH);
    public static readonly IAccessor<ITransform> Transform = Create<ITransform>("transform");
    public static readonly IAccessor<ITransformOrigin> TransformOrigin = Create<ITransformOrigin>("transform-origin");
    public static readonly IAccessor<ViewBox> ViewBox = Create<ViewBox>("viewbox");
    public static readonly IAccessor<Measure> Width = Create("width", Measure.ZeroH);
    public static readonly IAccessor<Measure> X = Create("x", Measure.ZeroH);
    public static readonly IAccessor<Measure> X1 = Create("x1", Measure.ZeroH);
    public static readonly IAccessor<Measure> X2 = Create("x2", Measure.ZeroH);
    public static readonly IAccessor<Measure> Y = Create("y", Measure.ZeroV);
    public static readonly IAccessor<Measure> Y1 = Create("y1", Measure.ZeroV);
    public static readonly IAccessor<Measure> Y2 = Create("y2", Measure.ZeroV);

    //alignment-baseline 
    //baseline-shift
    //clip
    public static readonly IAccessor<IClipPathFactory> ClipPath =Create<IClipPathFactory>("clip-path"); //, null, false);
    // "clip-rule":
    // "color":
    public static readonly IAccessor<IBrushFactory> Color =Create("color", SolidColourBrushFactory.Black, true);
    // "color-interpolation":
    // "color-interpolation-filters":
    // "color-profile":
    // "color-rendering":
    // "cursor":
    // "direction":
    // "display":
    public static readonly IAccessor<DominantBaseline> DominantBaseline =CreateEnum("dominant-baseline", Text.DominantBaseline.Auto,Text.DominantBaseline.Inherit, true);
    public static readonly IAccessor<IBrushFactory> Fill =Create("fill", SolidColourBrushFactory.Black, true);
    public static readonly IAccessor<Percent> FillOpacity =Create("fill-opacity", Percent.Hundred, true);
    // "enable-background":
    // "fill-rule":
    // "filter":
    // "flood-color":
    // "flood-opacity":
    // "font":
    public static readonly IAccessor<string[]> FontFamily =Create("font-family", EmptyFont, true);
    public static readonly IAccessor<Measure> FontSize =Create("font-size", Measure.TwelvePoint, true);
    // "font-size-adjust":
    public static readonly IAccessor<FontStretch> FontStretch =CreateEnum("font-stretch", Text.FontStretch.Normal, Text.FontStretch.Inherit);
    public static readonly IAccessor<FontStyle> FontStyle =CreateEnum("font-style", Text.FontStyle.Normal, Text.FontStyle.Inherit);
    // "font-variant":
    public static readonly IAccessor<FontWeight> FontWeight =CreateEnum("font-weight", Text.FontWeight.Normal, Text.FontWeight.Inherit, true);
    // "glyph-orientation-horizontal":
    // "glyph-orientation-vertical":
    // "image-rendering":
    // "kerning":
    // "letter-spacing":
    public static readonly IAccessor<string> LetterSpacing = Create<string>("letter-spacing");
    // "lighting-color":
    // "marker":
    // "marker-end":
    // "marker-mid":
    // "marker-start":
    // "mask":
    public static readonly IAccessor<Percent> Opacity = Create("opacity", Percent.Hundred);
    // "overflow":
    // "pointer-events":
    // "shape-rendering":
    public static readonly IAccessor<IBrushFactory> StopColor = Create<IBrushFactory>("stop-color");
    public static readonly IAccessor<Percent> StopOpacity =Create("stop-opacity", Percent.Hundred);
    public static readonly IAccessor<IBrushFactory> Stroke =Create("stroke", NoneColorBrushFactory.Instance, true);
    public static readonly IAccessor<Measure[]> StrokeDashArray = Create<Measure[]>("stroke-dasharray");
    public static readonly IAccessor<Measure> StrokeDashOffset =Create("stroke-dashoffset", Measure.ZeroH);
    public static readonly IAccessor<LineCap> StrokeLineCap =CreateEnum("stroke-linecap", LineCap.Butt, true);
    public static readonly IAccessor<LineJoin> StrokeLineJoin =CreateEnum("stroke-linejoin", LineJoin.Miter, true);
    public static readonly IAccessor<float> StrokeMiterLimit =Create("stroke-miterlimit", 4.0f, true);
    public static readonly IAccessor<Percent> StrokeOpacity =Create("stroke-opacity", Percent.Hundred, true);
    public static readonly IAccessor<Measure> StrokeWidth =Create("stroke-width", Measure.One, true);
    public static readonly IAccessor<TextAnchor> TextAnchor =CreateEnum("text-anchor", Text.TextAnchor.Inherit, Text.TextAnchor.Inherit);
    public static readonly IAccessor<TextDecoration> TextDecoration =CreateEnum("text-decoration", Text.TextDecoration.Inherit,Text.TextDecoration.Inherit);
    //  case "text-rendering":
    public static readonly IAccessor<string> TextTransform = Create<string>("text-transform");
      //case "unicode-bidi":
     // case "visibility":
    public static readonly IAccessor<string> WordSpacing = Create<string>("word-spacing");
    //  case "writing-mode":
  }
}

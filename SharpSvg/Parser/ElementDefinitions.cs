using Peracto.Svg.Accessor;

namespace Peracto.Svg.Parser
{
  public static class ElementDefinitions
  {
    //Container
    public static readonly ElementDefinition A = new ElementDefinition("a");
    public static readonly ElementDefinition Animate = new ElementDefinition("animate");
    public static readonly ElementDefinition AnimateMotion = new ElementDefinition("animateMotion");
    public static readonly ElementDefinition AnimateTransform = new ElementDefinition("animateTransform");

    public static readonly ElementDefinition Circle = new ElementDefinition("circle");

    //TODO: How to resolve a clip path?
    public static readonly ElementDefinition ClipPath = new ElementDefinition("clipPath");

    //Depracated
    public static readonly ElementDefinition ColorProfile = new ElementDefinition("color-profile");

    public static readonly ElementDefinition Defs = new ElementDefinition("defs");

    // Not rendered on visual media
    public static readonly ElementDefinition Desc = new ElementDefinition("desc");

    // Discard
    public static readonly ElementDefinition Discard = new ElementDefinition("discard");
    public static readonly ElementDefinition Ellipse = new ElementDefinition("ellipse");
    public static readonly ElementDefinition FeBlend = new ElementDefinition("feBlend");
    public static readonly ElementDefinition FeColorMatrix = new ElementDefinition("feColorMatrix");
    public static readonly ElementDefinition FeComponentTransfer = new ElementDefinition("FeComponentTransfer");

    public static readonly ElementDefinition
      FeComposite = new ElementDefinition("FeComposite");

    public static readonly ElementDefinition FeConvolveMatrix =
      new ElementDefinition("FeConvolveMatrix");

    public static readonly ElementDefinition FeDiffuseLighting =
      new ElementDefinition("FeDiffuseLighting");

    public static readonly ElementDefinition FeDisplacementMap =
      new ElementDefinition("FeDisplacementMap");

    public static readonly ElementDefinition FeDistantLight =
      new ElementDefinition("FeDistantLight");

    public static readonly ElementDefinition FeDropShadow =
      new ElementDefinition("FeDropShadow");

    public static readonly ElementDefinition FeFlood = new ElementDefinition("FeFlood");
    public static readonly ElementDefinition FeFuncA = new ElementDefinition("FeFuncA");
    public static readonly ElementDefinition FeFuncB = new ElementDefinition("FeFuncB");
    public static readonly ElementDefinition FeFuncG = new ElementDefinition("FeFuncG");
    public static readonly ElementDefinition FeFuncR = new ElementDefinition("FeFuncR");


    public static readonly ElementDefinition Filter = new ElementDefinition("filter");

    public static readonly ElementDefinition ForeignObject =
      new ElementDefinition("foreignObject");

    public static readonly ElementDefinition G = new ElementDefinition("g");

    public static readonly ElementDefinition Hatch = new ElementDefinition("hatch");
    public static readonly ElementDefinition HatchPath = new ElementDefinition("hatchpath");

    public static readonly ElementDefinition Image = new ElementDefinition("image");

    public static readonly ElementDefinition Line = new ElementDefinition("line");

    public static readonly ElementDefinition LinearGradient =
      new ElementDefinition("linearGradient");

    public static readonly ElementDefinition Marker = new ElementDefinition("marker");
    public static readonly ElementDefinition Mask = new ElementDefinition("mask");
    public static readonly ElementDefinition Mesh = new ElementDefinition("mesh");

    public static readonly ElementDefinition MeshGradient =
      new ElementDefinition("meshGradient");

    public static readonly ElementDefinition MeshPatch = new ElementDefinition("meshPatch");
    public static readonly ElementDefinition MeshRow = new ElementDefinition("meshRow");
    public static readonly ElementDefinition Metadata = new ElementDefinition("metadata");
    public static readonly ElementDefinition MPath = new ElementDefinition("mpath");

    public static readonly ElementDefinition Path = new ElementDefinition("path");
    public static readonly ElementDefinition Pattern = new ElementDefinition("pattern");
    public static readonly ElementDefinition Polygon = new ElementDefinition("polygon");
    public static readonly ElementDefinition Polyline = new ElementDefinition("polyline");

    public static readonly ElementDefinition RadialGradient =
      new ElementDefinition("radialGradient");

    public static readonly ElementDefinition Rect = new ElementDefinition("rect");

    public static readonly ElementDefinition Script = new ElementDefinition("script", ElementContentType.Embedded);
    public static readonly ElementDefinition Set = new ElementDefinition("set");
    public static readonly ElementDefinition SolidColor = new ElementDefinition("solidcolor");
    public static readonly ElementDefinition Stop = new ElementDefinition("stop");
    public static readonly ElementDefinition Style = new ElementDefinition("style", ElementContentType.Embedded);
    public static readonly ElementDefinition Svg = new ElementDefinition("svg");
    public static readonly ElementDefinition Switch = new ElementDefinition("switch");
    public static readonly ElementDefinition Symbol = new ElementDefinition("symbol");

    public static readonly ElementDefinition Text = new ElementDefinition("text", ElementContentType.Element);
    public static readonly ElementDefinition TextPath = new ElementDefinition("textPath", ElementContentType.Element);
    public static readonly ElementDefinition Title = new ElementDefinition("title", ElementContentType.Element);
    public static readonly ElementDefinition TextSpan = new ElementDefinition("tspan", ElementContentType.Element);

    public static readonly ElementDefinition Unknown = new ElementDefinition("unknown");
    public static readonly ElementDefinition Use = new ElementDefinition("use");
  }
}
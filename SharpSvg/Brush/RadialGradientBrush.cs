using Peracto.Svg.Transform;
using Peracto.Svg.Types;

namespace Peracto.Svg.Brush
{
  public class RadialGradientBrush : IBrush
  {
    public Stop[] Stops { get; }
    public GradientUnits GradientUnits { get; }
    public ITransform GradientTransform { get; }
    public SpreadMethod SpreadMethod { get; }
    public Measure Cx { get; }
    public Measure Cy { get; }
    public Measure Fx { get; }
    public Measure Fy { get; }
    public Measure R { get; }

    public RadialGradientBrush(
      string tag,
      GradientUnits gradientUnits,
      ITransform gradientTransform,
      SpreadMethod spreadMethod,
      Measure cx,
      Measure cy,
      Measure r,
      Measure fx,
      Measure fy,
      Stop[] stops)
    {
      Tag = "LGB:" + tag;
      GradientUnits = gradientUnits;
      GradientTransform = gradientTransform;
      SpreadMethod = spreadMethod;
      Stops = stops;
      Cx = cx;
      Cy = cy;
      Fx = fx;
      Fy = fy;
      R = r;
    }

    public string Tag { get; }
    public BrushType BrushType => BrushType.RadialGradient;
    public T Visit<T>(IBrushVisitor<T> cb, IElement element, IFrameContext context, float opacity)
    {
      return cb.CreateRadialGradientBrush(element, context, opacity, this);
    }
  }
}

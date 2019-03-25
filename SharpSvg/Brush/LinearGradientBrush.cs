using Peracto.Svg.Transform;
using Peracto.Svg.Types;

namespace Peracto.Svg.Brush
{
  public class LinearGradientBrush : IBrush
  {
    public Stop[] Stops { get; }
    public GradientUnits GradientUnits { get; }
    public ITransform GradientTransform { get; }
    public SpreadMethod SpreadMethod { get; }
    public Measure X1{ get; }
    public Measure Y1 { get; }
    public Measure X2 { get; }
    public Measure Y2 { get; }
    
    public LinearGradientBrush(
      string tag,
      GradientUnits gradientUnits,
      ITransform gradientTransform,
      SpreadMethod spreadMethod,
      Measure x1,
      Measure y1,
      Measure x2,
      Measure y2,
      Stop[] stops)
    {
      Tag = "LGB:" + tag;
      GradientUnits = gradientUnits;
      GradientTransform = gradientTransform;
      SpreadMethod = spreadMethod;
      X1 = x1;
      Y1 = y1;
      X2 = x2;
      Y2 = y2;
      Stops = stops;
    }

    public string Tag { get; }
    public BrushType BrushType => BrushType.LinearGradient;
    public T Visit<T>(IBrushVisitor<T> cb, IElement element, IFrameContext context, float opacity)
    {
      return cb.CreateLinearGradientBrush(element,context,opacity, this);
    }
  }
}

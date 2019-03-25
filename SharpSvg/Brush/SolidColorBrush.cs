using Peracto.Svg.Types;

namespace Peracto.Svg.Brush
{
  public class SolidColorBrush : IBrush
  {
    public SolidColorBrush(PxColor color)
    {
      Tag = "SCB:"+color;
      Color = color;
    }

    public string Tag { get; }
    public BrushType BrushType => BrushType.SolidColor;

    public PxColor Color { get; }

    public T Visit<T>(IBrushVisitor<T> cb, IElement element, IFrameContext context, float opacity)
    {
      return cb.CreateSolidBrush(element, context, opacity, this);
    }
  }
}

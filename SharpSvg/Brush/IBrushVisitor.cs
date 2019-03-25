namespace Peracto.Svg.Brush
{
  public interface IBrushVisitor<out TBrush>
  {
    TBrush CreateLinearGradientBrush(IElement element, IFrameContext context, float opacity, LinearGradientBrush brush);
    TBrush CreateSolidBrush(IElement element, IFrameContext context, float opacity, SolidColorBrush brush);
    TBrush CreateRadialGradientBrush(IElement element, IFrameContext context, float opacity, RadialGradientBrush brush);
  }
}
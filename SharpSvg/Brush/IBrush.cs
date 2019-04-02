namespace Peracto.Svg.Brush
{
  public interface IBrush
  {
    BrushType BrushType { get; }
    string Tag { get; }
    T Visit<T>(IBrushVisitor<T> cb, IElement element, IFrameContext context, float opacity);
  }
}
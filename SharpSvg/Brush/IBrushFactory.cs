namespace Peracto.Svg.Brush
{
  public interface IBrushFactory
  {
    IBrush Create(IElement element);
  }
}
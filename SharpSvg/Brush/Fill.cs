namespace Peracto.Svg.Brush
{
  public class Fill
  {
    public readonly IBrush Brush;
    public readonly float Opacity;

    public Fill(
      IBrush brush,
      float opacity
    )
    {
      Brush = brush;
      Opacity = opacity;
    }
  }
}
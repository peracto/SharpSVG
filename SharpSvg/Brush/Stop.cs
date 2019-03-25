using Peracto.Svg.Types;

namespace Peracto.Svg.Brush
{
  public class Stop
  {
    public IBrushFactory Colour { get; }
    public Percent Offset { get; }
    public float Opacity { get; }

    public Stop(IBrushFactory colour, Percent offset, float opacity)
    {
      Colour = colour;
      Offset = offset;
      Opacity = opacity;
    }
  }
}
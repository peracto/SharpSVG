using Peracto.Svg.Types;

namespace Peracto.Svg.Brush
{
  public class Stroke
  {
    public float[] DashArray { get; }
    public LineCap LineCap { get; }
    public LineJoin LineJoin { get; }
    public float MiterLimit { get; }
    public IBrush Brush { get; }
    public float Width { get; }
    public float Opacity { get; }
    public float DashOffset { get; }

    public Stroke(
      IBrush brush,
      float width,
      float opacity,
      LineCap lineCap,
      LineJoin lineJoin,
      float miterLimit,
      float dashOffset,
      float[] dashArray
    )
    {
      Brush = brush;
      Width = width;
      Opacity = opacity;
      LineCap = lineCap;
      LineJoin = lineJoin;
      MiterLimit = miterLimit;
      DashArray = dashArray;
      DashOffset = dashOffset;
    }
  }
}

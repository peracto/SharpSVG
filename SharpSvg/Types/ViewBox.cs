using System;
using System.Globalization;

namespace Peracto.Svg.Types
{
  public class ViewBox
  {
    public float MinX { get; }
    public float MinY { get; }
    public float Width { get; }
    public float Height { get; }

    public ViewBox(float minX, float minY, float width, float height)
    {
      MinX = minX;
      MinY = minY;
      Width = width;
      Height = height;
    }

    public PxSize Size => new PxSize(Width, Height);

    public PxRectangle AsRectangle()
    {
      return new PxRectangle(MinX, MinY, Width, Height);
    }

    public static ViewBox Parse(string value)
    {
      var coords = value.Split(new[] {',', ' '}, StringSplitOptions.RemoveEmptyEntries);

      if (coords.Length != 4)
        throw new FormatException("The 'viewBox' attribute must be in the format 'minX, minY, width, height'.");

      return new ViewBox(float.Parse(coords[0], NumberStyles.Float, CultureInfo.InvariantCulture),
        float.Parse(coords[1], NumberStyles.Float, CultureInfo.InvariantCulture),
        float.Parse(coords[2], NumberStyles.Float, CultureInfo.InvariantCulture),
        float.Parse(coords[3], NumberStyles.Float, CultureInfo.InvariantCulture));
    }
  }
}

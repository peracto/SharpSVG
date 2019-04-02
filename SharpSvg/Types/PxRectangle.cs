namespace Peracto.Svg.Types
{
  public struct PxRectangle
  {
    public PxRectangle(float x, float y, float width, float height)
    {
      X = x;
      Y = y;

      Width = width;
      Height = height;
    }

    public PxSize Size => new PxSize(Width, Height);

    public float X { get; }
    public float Y { get; }
    public float Width { get; }
    public float Height { get; }
  }
}
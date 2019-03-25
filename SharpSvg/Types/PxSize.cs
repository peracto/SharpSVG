namespace Peracto.Svg.Types
{
  public struct PxSize
  {
    public static readonly PxSize Zero = new PxSize();
    public PxSize(float width, float height)
    {
      Width = width;
      Height = height;
    }

    public readonly float Width;
    public readonly float Height;

    public PxRectangle AsRectangle()
    {
      return new PxRectangle(0, 0, Width, Height);
    }
    public PxRectangle AsRectangle(float x, float y)
    {
      return new PxRectangle(x, y, Width, Height);
    }

  }
}
﻿namespace Peracto.Svg.Types
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

    public static PxRectangle FromXywh(float x, float y, float width, float height)
    {
      return new PxRectangle(x, y, width, height);
    }

    public float X { get; }
    public float Y { get; }
    public float Width { get; }
    public float Height { get; }
  }
}
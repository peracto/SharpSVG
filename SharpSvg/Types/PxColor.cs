using System;

namespace Peracto.Svg.Types
{
  public struct PxColor : IEquatable<PxColor>
  {
    public readonly float R;
    public readonly float G;
    public readonly float B;
    public readonly float A;

    public PxColor(float red, float green, float blue, float alpha = 1f)
    {
      R = red;
      G = green;
      B = blue;
      A = alpha;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:Colour" /> struct.
    /// </summary>
    /// <param name="argb">M11 packed unsigned integer containing all four color components in ARGB order.</param>
    public PxColor(uint argb)
    {
      A = (argb >> 0x18 & 255) / 255.0f;
      R = (argb >> 0x10 & 255) / 255.0f;
      G = (argb >> 0x08 & 255) / 255.0f;
      B = (argb & 255) / 255.0f;
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="T:Colour" /> struct.
    /// </summary>
    /// <param name="argb">M11 packed unsigned integer containing all four color components in ARGB order.</param>
    public PxColor(int rgb, int alpha)
    {
      A = (alpha & 255) / 255.0f;
      R = (rgb >> 0x10 & 255) / 255.0f;
      G = (rgb >> 0x08 & 255) / 255.0f;
      B = (rgb & 255) / 255.0f;
    }

    /// <summary>Converts the color from an ARGB integer.</summary>
    /// <param name="argb">M11 packed integer containing all four color components in ARGB order</param>
    /// <returns>M11 color.</returns>
    public static PxColor FromRgb(int rgb)
    {
      return new PxColor(rgb,255);
    }

    public static PxColor FromRgb(int r, int g, int b)
    {
      return new PxColor(
        r / 255.0f,
        g / 255.0f,
        b / 255.0f,
        1
      );
    }


    public PxColor AddOpacity(float opacity)
    {
      return new PxColor(R, G, B, A * opacity);
    }

    public string GetTag()
    {
      return $"R:{R}:G:{G}:B:{B}:A:{A}";
    }

    /// <summary>Returns a string that represents the current object.</summary>
    public override string ToString()
    {
      return $"R:{R}:G:{G}:B:{B}:A:{A}";
    }

    /// <summary>Returns a hash code for this instance.</summary>
    /// <returns>
    /// M11 hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
    /// </returns>
    public override int GetHashCode()
    {
      return ((R.GetHashCode() * 397 ^ G.GetHashCode()) * 397 ^ B.GetHashCode()) * 397 ^ A.GetHashCode();
    }

    /// <summary>Determines if the specified values are equal.</summary>
    public static bool operator ==(PxColor left, PxColor right)
    {
      return left.Equals(right);
    }

    /// <summary>Determines if the specified values are not equal.</summary>
    public static bool operator !=(PxColor left, PxColor right)
    {
      return !left.Equals(right);
    }

    /// <summary>
    /// Determines whether the specified <see cref="T:Colour" /> is equal to this instance.
    /// </summary>
    public bool Equals(PxColor other)
    {
      return
        MathEx.NearEqual(A, other.A) &&
        MathEx.NearEqual(R , other.R) &&
        MathEx.NearEqual(G , other.G) &&
        MathEx.NearEqual(B , other.B);
    }

    /// <summary>
    /// Determines whether the specified <see cref="T:System.Object" /> is equal to this instance.
    /// </summary>
    public override bool Equals(object value)
    {
      return value is PxColor color && Equals(color);
    }

  }
}

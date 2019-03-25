using System;

namespace Peracto.Svg.Types
{
  public struct PxMatrix
  {
    public readonly float M11;
    public readonly float M12;
    public readonly float M21;
    public readonly float M22;
    public readonly float M31;
    public readonly float M32;

    public PxMatrix(float m11, float m12, float m21, float m22, float m31, float m32)
    {
      M11 = m11;
      M12 = m12;
      M21 = m21;
      M22 = m22;
      M31 = m31;
      M32 = m32;
    }

    public PxMatrix(float m11, float m12, float m21, float m22)
    {
      M11 = m11;
      M12 = m12;
      M21 = m21;
      M22 = m22;
      M31 = 0f;
      M32 = 0f;
    }

    public static readonly PxMatrix Identity = new PxMatrix(1, 0, 0, 1);

    public static PxMatrix Translate(float x, float y)
    {
      return new PxMatrix(1, 0, 0, 1, x, y);
    }

    public static PxMatrix Translate(PxPoint pt)
    {
      return new PxMatrix(1, 0, 0, 1, pt.X, pt.Y);
    }


    public static PxMatrix Scale(float scaleX, float scaleY)
    {
      return new PxMatrix(
        scaleX, //m11
        0, //m12
        0, //m21
        scaleY //m22
      );
    }

    public static PxMatrix Translation(float scaleX, float scaleY, float x,float y)
    {
      return new PxMatrix(
        scaleX, //m11
        0, //m12
        0, //m21
        scaleY, //m22
        x,
        y
      );
    }



    public bool IsIdentity
    {
      get
      {
        return
          MathEx.IsOne(M11) &&
          MathEx.IsZero(M12) &&
          MathEx.IsZero(M21) &&
          MathEx.IsOne(M22) &&
          MathEx.IsZero(M31) &&
          MathEx.IsZero(M32);
      }
    }

    public static PxMatrix Rotate(float angle)
    {
      var cos = (float) Math.Cos(angle);
      var sin = (float) Math.Sin(angle);
      return new PxMatrix(
        cos,
        sin,
        -sin,
        cos
      );
    }


    public static PxMatrix Skew(float angleX, float angleY)
    {
      return new PxMatrix(
        1, //m11
        (float) Math.Tan(angleY), //m12
        (float) Math.Tan(angleX), //m21
        1 //m22
      );
    }


    public static PxMatrix operator *(PxMatrix left, PxMatrix right)
    {
      return Multiply(ref left, ref right);
    }

    public static PxMatrix Multiply(ref PxMatrix left, ref PxMatrix right)
    {
      return new PxMatrix(
        (left.M11 * right.M11 + left.M12 * right.M21),
        (left.M11 * right.M12 + left.M12 * right.M22),
        (left.M21 * right.M11 + left.M22 * right.M21),
        (left.M21 * right.M12 + left.M22 * right.M22),
        (left.M31 * right.M11 + left.M32 * right.M21) + right.M31,
        (left.M31 * right.M12 + left.M32 * right.M22) + right.M32
      );
    }
  }
}
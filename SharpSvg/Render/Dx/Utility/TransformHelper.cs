using System;
using Peracto.Svg.Types;
using DX=SharpDX;
using D2D1=SharpDX.Direct2D1;
using DXM=SharpDX.Mathematics.Interop;

namespace Peracto.Svg.Render.Dx.Utility
{
  public class TransformHelper : IDisposable
  {
    public static IDisposable Create(D2D1.RenderTarget target, IElement element, IFrameContext context, bool setPosition)
    {
      var matrix = setPosition
        ? element.GetTransformMatrix() * PxMatrix.Translate(element.GetPosition(context))
        : element.GetTransformMatrix();

      return setPosition || !matrix.IsIdentity
        ? new TransformHelper(target, matrix)
        : null;
    }

    private readonly D2D1.RenderTarget _target;
    private readonly DXM.RawMatrix3x2 _matrix;
    public TransformHelper(D2D1.RenderTarget target, DX.Matrix3x2 matrix)
    {
      _target = target;
      _matrix = target.Transform;
      target.Transform = matrix * _matrix;
    }
    public TransformHelper(D2D1.RenderTarget target, PxMatrix matrix) : this(target, matrix.ToMatrix3x2())
    {
    }

    void IDisposable.Dispose()
    {
      _target.Transform = _matrix;
    }
  }
}
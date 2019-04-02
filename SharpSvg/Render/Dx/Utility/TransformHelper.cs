using Peracto.Svg.Render.Dx.Render;
using Peracto.Svg.Types;
using System;
using D2D1 = SharpDX.Direct2D1;
using DXM = SharpDX.Mathematics.Interop;

namespace Peracto.Svg.Render.Dx.Utility
{
  public static class TransformHelper
  {
    public static IDisposable Create(RendererDirect2D render, IElement element, IFrameContext context)
    {
      var transform = element.GetTransform();
      return transform != null
        ? new TransformHelperImpl(render.Target, transform.Matrix)
        : null;
    }

    public static IDisposable CreatePosition(RendererDirect2D render, IElement element, IFrameContext context)
    {
      var transform = element.GetTransform();
      return transform == null
        ? new TransformHelperImpl(render.Target, element.GetPositionMatrix(context))
        : new TransformHelperImpl(render.Target, transform.Matrix * element.GetPositionMatrix(context));
    }


    public static IDisposable Create(RendererDirect2D render, PxMatrix matrix)
    {
      return new TransformHelperImpl(render.Target, matrix);
    }

    private class TransformHelperImpl : IDisposable
    {
      private readonly D2D1.RenderTarget _target;
      private readonly DXM.RawMatrix3x2 _matrix;

      public TransformHelperImpl(D2D1.RenderTarget target, PxMatrix matrix) 
      {
        _target = target;
        _matrix = target.Transform;
        target.Transform = matrix.ToMatrix3x2() * _matrix;
      }

      void IDisposable.Dispose()
      {
        _target.Transform = _matrix;
      }
    }
  }
}
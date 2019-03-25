using Peracto.Svg.Types;

namespace Peracto.Svg.Transform
{
  public class MultiTransform2 : ITransform
  {
    public TransformType TransformType => TransformType.Multi;

    private readonly ITransform _t0;
    private readonly ITransform _t1;
    public PxMatrix Matrix { get; }

    public MultiTransform2(ITransform t0, ITransform t1)
    {
      _t0 = t0;
      _t1 = t1;
      Matrix = _t1.Matrix * _t0.Matrix;
    }

    public override string ToString()
    {
      return $"{_t0} {_t1}";
    }
  }
}
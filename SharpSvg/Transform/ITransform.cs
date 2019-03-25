using Peracto.Svg.Types;

namespace Peracto.Svg.Transform
{
    public interface ITransform
    {
        TransformType TransformType { get; }
        PxMatrix Matrix { get; }
    }
}